using UnityEngine;
using Unity.Barracuda;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class ObjectDetectionManager : MonoBehaviour
{    [Header("Model Settings")]
    public NNModel modelAsset;    [Tooltip("Fallback: ONNX filename in StreamingAssets folder")]
    public string fallbackModelPath = "yolov8n_unity_20250607_002751.onnx"; // New model with timestamp
    public Camera detectionCamera;
      [Header("Detection Settings")]
    [Tooltip("Lower values detect more distant/small objects but may increase false positives")]
    public float confidenceThreshold = 0.25f; // Lowered from 0.5f for better distant object detection
    public float iouThreshold = 0.4f;
    public int inputWidth = 640;
    public int inputHeight = 640;
      [Header("Distance Detection Settings")]
    [Tooltip("Minimum detection size to filter out very small noise")]
    public float minDetectionSize = 0.01f; // 1% of screen
    [Tooltip("Enable enhanced preprocessing for distant objects (reduces FPS)")]
    public bool enhanceDistantObjects = false; // Disabled by default for performance    [Header("Coordinate Adjustment (Fix Positioning Issues)")]
    [Tooltip("Manual X offset to correct bounding box horizontal positioning")]
    [Range(-1000, 200)]
    public float coordinateOffsetX = 0f;
    [Tooltip("Manual Y offset to correct bounding box vertical positioning")]
    [Range(-200, 200)]
    public float coordinateOffsetY = 0f;
    [Tooltip("Scale factor for X coordinates (1.0 = no scaling)")]
    [Range(0.5f, 2.0f)]
    public float coordinateScaleX = 1.0f;
    [Tooltip("Scale factor for Y coordinates (1.0 = no scaling)")]
    [Range(0.5f, 2.0f)]
    public float coordinateScaleY = 1.0f;
    [Tooltip("Flip X coordinates (useful if bounding boxes are mirrored)")]
    public bool flipX = false;
    [Tooltip("Flip Y coordinates (useful if bounding boxes are upside down)")]
    public bool flipY = false;[Header("Performance Settings")]
    [Tooltip("Skip frames between detections (1 = every frame, 2 = every other frame)")]
    public int frameSkip = 3; // Process every 3rd frame for better FPS
    [Tooltip("Reduce debug logging frequency")]
    public bool reducedLogging = true;
    
    [Header("UI Elements")]
    public Canvas uiCanvas;
    public GameObject boundingBoxPrefab;    [Header("Target Categories")]
    public string[] targetCategories = { "person", "cow", "sheep", "chicken" };    private Model model;
    private IWorker worker;
    private RenderTexture renderTexture;
    private Texture2D inputTexture;
    private List<BoundingBoxUI> activeBoundingBoxes = new List<BoundingBoxUI>();
    private Dictionary<string, int> categoryCounters = new Dictionary<string, int>();
    
    // Performance optimization variables
    private int frameCounter = 0;
    
    // YOLO class names (COCO dataset)
    private string[] yoloClasses = {
        "person", "bicycle", "car", "motorcycle", "airplane", "bus", "train", "truck", "boat",
        "traffic light", "fire hydrant", "stop sign", "parking meter", "bench", "bird", "cat",
        "dog", "horse", "sheep", "cow", "elephant", "bear", "zebra", "giraffe", "backpack",
        "umbrella", "handbag", "tie", "suitcase", "frisbee", "skis", "snowboard", "sports ball",
        "kite", "baseball bat", "baseball glove", "skateboard", "surfboard", "tennis racket",
        "bottle", "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple",
        "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake",
        "chair", "couch", "potted plant", "bed", "dining table", "toilet", "tv", "laptop",
        "mouse", "remote", "keyboard", "cell phone", "microwave", "oven", "toaster", "sink",
        "refrigerator", "book", "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush"
    };    void Start()
    {
        InitializeModel();
        InitializeRenderTexture();
        InitializeCategoryCounters();
    }void InitializeModel()
    {
        // Try to load from assigned model asset first
        if (modelAsset != null)
        {            try
            {
                model = ModelLoader.Load(modelAsset);
                worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
                Debug.Log("YOLO model loaded successfully from assigned asset");
                DebugModelInfo("Assigned Asset");
                return;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to load assigned model asset: {e.Message}");
            }
        }
        
        // Fallback: try to load from StreamingAssets with better error handling
        if (!string.IsNullOrEmpty(fallbackModelPath))
        {
            string modelPath = System.IO.Path.Combine(Application.streamingAssetsPath, fallbackModelPath);
            if (System.IO.File.Exists(modelPath))
            {                try
                {
                    byte[] modelData = System.IO.File.ReadAllBytes(modelPath);
                    model = ModelLoader.Load(modelData);
                    worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
                    Debug.Log($"YOLO model loaded successfully from StreamingAssets: {fallbackModelPath}");
                    DebugModelInfo("StreamingAssets");
                    return;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load model from StreamingAssets: {e.Message}");
                    Debug.LogError("The ONNX model format may be incompatible with Unity Barracuda. Try converting with opset_version=9");
                }
            }
            else
            {
                Debug.LogError($"Model file not found at: {modelPath}");
            }
        }
        
        Debug.LogError("No model found! Please assign a model asset or place ONNX file in StreamingAssets folder.");
        Debug.LogError("If using ONNX, ensure it's converted with opset_version=9 for Barracuda compatibility.");
    }    void InitializeRenderTexture()
    {
        renderTexture = new RenderTexture(inputWidth, inputHeight, 24);
        inputTexture = new Texture2D(inputWidth, inputHeight, TextureFormat.RGB24, false);
        
        if (detectionCamera == null)
            detectionCamera = Camera.main;
    }
    
    void InitializeCategoryCounters()
    {
        foreach (string category in targetCategories)
        {
            categoryCounters[category] = 0;
        }
    }
      void DebugModelInfo(string source)
    {
        if (model != null)
        {
            Debug.Log($"✅ YOLO model loaded successfully from {source}");
            Debug.Log($"Inputs: {model.inputs.Count}, Outputs: {model.outputs.Count}");
            
            // Test inference to verify tensor shapes
            if (worker != null)
            {
                try
                {
                    Tensor testInput = new Tensor(1, 3, 640, 640);
                    worker.Execute(testInput);
                    Tensor testOutput = worker.PeekOutput(model.outputs[0]);
                    
                    if (testOutput.length == 705600) // 84 * 8400
                    {
                        Debug.Log("✅ Model output has correct tensor shape");
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ Unexpected output tensor length: {testOutput.length}");
                    }
                    
                    testInput.Dispose();
                    testOutput.Dispose();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Model test failed: {e.Message}");
                }
            }
        }
        else
        {
            Debug.LogError("Model is null!");
        }
    }void Update()
    {
        if (worker != null && detectionCamera != null)
        {
            frameCounter++;
            
            // Skip frames for better performance
            if (frameCounter >= frameSkip)
            {
                frameCounter = 0;
                PerformDetection();
            }
        }
        else
        {
            // Debug missing components (reduced frequency)
            if (worker == null && Time.frameCount % 300 == 0) // Every 5 seconds at 60fps
            {
                Debug.LogWarning("⚠️ Worker is null - model not loaded properly");
            }
            if (detectionCamera == null && Time.frameCount % 300 == 0)
            {
                Debug.LogWarning("⚠️ Detection camera is null - assign Camera.main or set manually");
                detectionCamera = Camera.main; // Try to auto-assign
            }
        }
    }    void PerformDetection()
    {
        // Always use the model's expected input resolution (640x640)
        RenderTexture targetRenderTexture = renderTexture;
        Texture2D targetInputTexture = inputTexture;
        int currentWidth = inputWidth;  // Always 640
        int currentHeight = inputHeight; // Always 640
        
        // Capture camera image
        RenderTexture previousTarget = detectionCamera.targetTexture;
        detectionCamera.targetTexture = targetRenderTexture;
        detectionCamera.Render();
        detectionCamera.targetTexture = previousTarget;
        
        // Convert to texture
        RenderTexture.active = targetRenderTexture;
        targetInputTexture.ReadPixels(new Rect(0, 0, currentWidth, currentHeight), 0, 0);
        targetInputTexture.Apply();
        RenderTexture.active = null;        // Enhanced preprocessing ONLY if enabled and needed (performance optimization)
        if (enhanceDistantObjects && Time.frameCount % 10 == 0) // Only every 10th frame when enabled
        {
            ApplyImageEnhancement(targetInputTexture);
        }
        
        // Prepare input tensor - MUST use exact model input size (640x640)
        Tensor inputTensor = new Tensor(targetInputTexture, 3);
        
        // Run inference
        worker.Execute(inputTensor);
        
        // Get output tensor by name (more reliable than PeekOutput())
        string outputName = model.outputs[0];
        Tensor outputTensor = worker.PeekOutput(outputName);
        
        // Process results with standard resolution
        ProcessDetectionResults(outputTensor, currentWidth, currentHeight);
        
        // Cleanup
        inputTensor.Dispose();
        outputTensor.Dispose();
    }
      void ApplyImageEnhancement(Texture2D texture)
    {
        // Fast contrast enhancement for better distant object detection
        // Only process every 4th pixel for performance (640x640 -> 160x160 effective processing)
        Color[] pixels = texture.GetPixels();
        int width = texture.width;
        int height = texture.height;
        
        for (int y = 0; y < height; y += 4) // Skip rows for performance
        {
            for (int x = 0; x < width; x += 4) // Skip columns for performance
            {
                int index = y * width + x;
                if (index < pixels.Length)
                {
                    // Fast contrast boost without expensive pow operations
                    Color pixel = pixels[index];
                    pixel.r = Mathf.Clamp01(pixel.r * 1.15f);
                    pixel.g = Mathf.Clamp01(pixel.g * 1.15f);
                    pixel.b = Mathf.Clamp01(pixel.b * 1.15f);
                    
                    // Apply to neighboring pixels for coverage
                    for (int dy = 0; dy < 4 && y + dy < height; dy++)
                    {
                        for (int dx = 0; dx < 4 && x + dx < width; dx++)
                        {
                            int neighborIndex = (y + dy) * width + (x + dx);
                            if (neighborIndex < pixels.Length)
                            {
                                pixels[neighborIndex] = pixel;
                            }
                        }
                    }
                }
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }    void ProcessDetectionResults(Tensor output, int processingWidth, int processingHeight)
    {
        ClearBoundingBoxes();
        
        // Reduced debug logging - only log once every 5 seconds
        if (Time.frameCount % 300 == 0) // Every 5 seconds at 60fps
        {
            Debug.Log($"Detection Info - Confidence Threshold: {confidenceThreshold}, Enhanced Mode: {enhanceDistantObjects}");
        }
        
        // YOLOv8 output format: [batch, 84, 8400] where 84 = 4 (bbox) + 80 (classes)
        var detections = new List<Detection>();
        
        // Get tensor dimensions with robust shape handling
        var shapeArray = output.shape.ToArray();
        
        // Reduced debug logging
        if (Time.frameCount % 300 == 0)
        {
            Debug.Log($"Processing detection results - Tensor shape: [{string.Join(", ", shapeArray)}]");
        }
          // Handle different tensor shape formats
        int batchSize, numFeatures, numDetections;
        
        if (shapeArray.Length == 3)
        {
            // Expected format: [batch, features, detections]
            batchSize = shapeArray[0];
            numFeatures = shapeArray[1]; 
            numDetections = shapeArray[2];
        }
        else if (shapeArray.Length == 4)
        {
            // Some formats might be [batch, channels, height, width]
            batchSize = shapeArray[0];
            if (shapeArray[1] == 84 && shapeArray[2] * shapeArray[3] == 8400)
            {
                numFeatures = shapeArray[1];
                numDetections = shapeArray[2] * shapeArray[3];
            }
            else
            {
                Debug.LogError($"Unexpected 4D tensor shape: [{string.Join(", ", shapeArray)}]");
                return;
            }
        }        else if (shapeArray.Length == 8)
        {
            // Unity Barracuda sometimes creates 8D tensors: [1, 1, 1, 1, 1, 1, detections, features]
            if (Time.frameCount % 300 == 0)
            {
                Debug.Log("Detected 8D tensor format from Unity Barracuda");
            }
            batchSize = 1; // Always 1 for our use case
            
            // For 8D format [1, 1, 1, 1, 1, 1, 8400, 84], the last two dimensions are what we need
            numDetections = shapeArray[6]; // Should be 8400
            numFeatures = shapeArray[7];   // Should be 84
            
            if (Time.frameCount % 300 == 0)
            {
                Debug.Log($"8D tensor parsed as - Detections: {numDetections}, Features: {numFeatures}");
            }
        }
        else
        {
            Debug.LogError($"Unexpected tensor dimensions: {shapeArray.Length}D, shape: [{string.Join(", ", shapeArray)}]");
            return;
        }
        
        if (Time.frameCount % 300 == 0)
        {
            Debug.Log($"Parsed dimensions - Batch: {batchSize}, Features: {numFeatures}, Detections: {numDetections}");
        }
        
        // Safety check
        if (batchSize != 1 || numFeatures != 84)
        {
            Debug.LogError($"Unexpected tensor shape: [{batchSize}, {numFeatures}, {numDetections}]. Expected [1, 84, 8400]");
            return;
        }
          for (int i = 0; i < numDetections; i++)
        {
            // Handle different tensor access patterns based on dimensions
            float centerX, centerY, width, height;
            
            if (shapeArray.Length == 8)
            {
                // For 8D tensor [1, 1, 1, 1, 1, 1, detections, features]
                // Access pattern: tensor[0, 0, 0, 0, 0, 0, detection_idx, feature_idx]
                centerX = output[0, 0, 0, 0, 0, 0, i, 0];
                centerY = output[0, 0, 0, 0, 0, 0, i, 1];
                width = output[0, 0, 0, 0, 0, 0, i, 2];
                height = output[0, 0, 0, 0, 0, 0, i, 3];
            }
            else
            {
                // For 3D tensor [batch, features, detections] - original format
                int baseIndex = 0 * numFeatures * numDetections; // batch offset (always 0)
                centerX = output[baseIndex + 0 * numDetections + i];
                centerY = output[baseIndex + 1 * numDetections + i];
                width = output[baseIndex + 2 * numDetections + i];
                height = output[baseIndex + 3 * numDetections + i];
            }
              // Get class scores
            float maxScore = 0f;
            int maxClass = -1;
            
            for (int c = 0; c < 80; c++) // 80 COCO classes
            {
                float score;
                
                if (shapeArray.Length == 8)
                {
                    // For 8D tensor access: tensor[0, 0, 0, 0, 0, 0, detection_idx, feature_idx]
                    score = output[0, 0, 0, 0, 0, 0, i, 4 + c];
                }
                else
                {
                    // For 3D tensor access - original format
                    int baseIndex = 0 * numFeatures * numDetections;
                    score = output[baseIndex + (4 + c) * numDetections + i];
                }
                
                if (score > maxScore)
                {
                    maxScore = score;
                    maxClass = c;
                }
            }
              // Check if detection meets confidence threshold and is a target category
            if (maxScore > confidenceThreshold && maxClass < yoloClasses.Length)
            {
                string className = yoloClasses[maxClass];
                
                // Map some class names to our target categories
                if (className == "bird") className = "chicken"; // Approximate mapping
                  if (targetCategories.Contains(className))
                {
                    // Convert YOLO coordinates to screen coordinates
                    float normCenterX = centerX / processingWidth;
                    float normCenterY = centerY / processingHeight;
                    float normWidth = width / processingWidth;
                    float normHeight = height / processingHeight;
                    
                    // Filter out very small detections
                    if (normWidth < minDetectionSize && normHeight < minDetectionSize)
                    {
                        continue;
                    }
                    
                    // Convert to screen pixel coordinates
                    float screenCenterX = normCenterX * Screen.width;
                    float screenCenterY = normCenterY * Screen.height;
                    float screenWidth = normWidth * Screen.width;
                    float screenHeight = normHeight * Screen.height;
                    
                    // Apply coordinate adjustments for fixing positioning issues
                    if (flipX) screenCenterX = Screen.width - screenCenterX;
                    if (flipY) screenCenterY = Screen.height - screenCenterY;
                    
                    screenCenterX *= coordinateScaleX;
                    screenCenterY *= coordinateScaleY;
                    screenWidth *= coordinateScaleX;
                    screenHeight *= coordinateScaleY;
                    
                    screenCenterX += coordinateOffsetX;
                    screenCenterY += coordinateOffsetY;
                    
                    // Calculate bounding box corners
                    float x1 = screenCenterX - screenWidth / 2f;
                    float y1 = screenCenterY - screenHeight / 2f;
                    float x2 = screenCenterX + screenWidth / 2f;
                    float y2 = screenCenterY + screenHeight / 2f;
                    
                    // Clamp to screen bounds
                    x1 = Mathf.Clamp(x1, 0, Screen.width);
                    y1 = Mathf.Clamp(y1, 0, Screen.height);
                    x2 = Mathf.Clamp(x2, 0, Screen.width);
                    y2 = Mathf.Clamp(y2, 0, Screen.height);
                      // Calculate detection area for additional validation
                    float detectionArea = (x2 - x1) * (y2 - y1);
                    
                    detections.Add(new Detection
                    {
                        x1 = x1,
                        y1 = y1,
                        x2 = x2,
                        y2 = y2,
                        confidence = maxScore,
                        className = className
                    });
                }
            }
        }        // Apply Non-Maximum Suppression - optimized for performance
        var nmsDetections = ApplyNMS(detections, iouThreshold);
        
        // Create bounding boxes for final detections (limit max boxes for performance)
        int maxBoxes = 20; // Fixed limit regardless of resolution
        int boxCount = 0;
        foreach (var detection in nmsDetections)
        {
            if (boxCount >= maxBoxes) break; // Limit number of boxes for performance
            CreateBoundingBox(detection);
            boxCount++;
        }        // Log detection summary every few seconds for monitoring
        if (Time.frameCount % 240 == 0 && nmsDetections.Count > 0) // Every 4 seconds
        {
            Debug.Log($"Detection Summary (640x640): {nmsDetections.Count} objects detected. " +
                     $"Categories: {string.Join(", ", nmsDetections.Take(5).Select(d => $"{d.className}({d.confidence:F2})"))}");
        }
    }    List<Detection> ApplyNMS(List<Detection> detections, float iouThreshold)
    {
        var result = new List<Detection>();
        var sortedDetections = detections.OrderByDescending(d => d.confidence).ToList();
        
        // Limit processing for performance - fixed limit for all modes
        int maxDetectionsToProcess = 80; // Reasonable limit for 640x640 processing
        if (sortedDetections.Count > maxDetectionsToProcess)
        {
            sortedDetections = sortedDetections.Take(maxDetectionsToProcess).ToList();
        }
        
        while (sortedDetections.Count > 0)
        {
            var best = sortedDetections[0];
            result.Add(best);
            sortedDetections.RemoveAt(0);
            
            for (int i = sortedDetections.Count - 1; i >= 0; i--)
            {
                if (CalculateIoU(best, sortedDetections[i]) > iouThreshold)
                {
                    sortedDetections.RemoveAt(i);
                }
            }
        }
        
        return result;
    }
    
    float CalculateIoU(Detection a, Detection b)
    {
        float intersectionArea = Mathf.Max(0, Mathf.Min(a.x2, b.x2) - Mathf.Max(a.x1, b.x1)) *
                                Mathf.Max(0, Mathf.Min(a.y2, b.y2) - Mathf.Max(a.y1, b.y1));
        
        float areaA = (a.x2 - a.x1) * (a.y2 - a.y1);
        float areaB = (b.x2 - b.x1) * (b.y2 - b.y1);
        
        return intersectionArea / (areaA + areaB - intersectionArea);
    }
      void CreateBoundingBox(Detection detection)
    {
        if (uiCanvas == null)
        {
            Debug.LogError("UI Canvas is not assigned! Cannot create bounding boxes.");
            return;
        }
        
        // Increment category counter and assign ID
        categoryCounters[detection.className]++;
        int objectId = categoryCounters[detection.className];
        
        GameObject boundingBoxObj;
        
        // Create bounding box GameObject (either from prefab or manually)
        if (boundingBoxPrefab != null)
        {
            boundingBoxObj = Instantiate(boundingBoxPrefab, uiCanvas.transform);
        }
        else
        {
            // Create bounding box manually if no prefab is assigned
            boundingBoxObj = CreateBoundingBoxManually();
            boundingBoxObj.transform.SetParent(uiCanvas.transform, false);
        }
        
        BoundingBoxUI boundingBoxUI = boundingBoxObj.GetComponent<BoundingBoxUI>();
        
        if (boundingBoxUI == null)
        {
            // Add BoundingBoxUI component if it doesn't exist
            boundingBoxUI = boundingBoxObj.AddComponent<BoundingBoxUI>();
        }        if (boundingBoxUI != null)
        {
            boundingBoxUI.SetupBoundingBox(detection, objectId);
            activeBoundingBoxes.Add(boundingBoxUI);
        }
        else
        {
            Debug.LogError("Failed to create or find BoundingBoxUI component!");
        }
    }
    
    GameObject CreateBoundingBoxManually()
    {
        // Create the main bounding box GameObject
        GameObject boundingBoxObj = new GameObject("BoundingBox");
        
        // Add RectTransform component
        RectTransform rectTransform = boundingBoxObj.AddComponent<RectTransform>();
        
        // Configure RectTransform for screen space overlay
        rectTransform.anchorMin = new Vector2(0, 1); // Top-left anchor
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        
        // Add Image component for the outline
        Image image = boundingBoxObj.AddComponent<Image>();
        
        // Create a simple white texture for the outline
        Texture2D whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
        Sprite whiteSprite = Sprite.Create(whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
        image.sprite = whiteSprite;
        image.color = new Color(1, 1, 1, 0.8f); // Semi-transparent white
        
        return boundingBoxObj;
    }
    
    void ClearBoundingBoxes()
    {
        foreach (var bbox in activeBoundingBoxes)
        {
            if (bbox != null && bbox.gameObject != null)
                Destroy(bbox.gameObject);
        }
        activeBoundingBoxes.Clear();
    }
    
    void ResetCategoryCounters()
    {
        foreach (string category in targetCategories)
        {
            categoryCounters[category] = 0;
        }
    }
      public List<BoundingBoxUI> GetActiveBoundingBoxes()
    {
        return activeBoundingBoxes;
    }
    
    public Dictionary<string, int> GetCategoryCounters()
    {
        return new Dictionary<string, int>(categoryCounters);
    }    void OnDestroy()
    {
        worker?.Dispose();
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
        if (inputTexture != null)
            Destroy(inputTexture);
    }
}

[System.Serializable]
public class Detection
{
    public float x1, y1, x2, y2;
    public float confidence;
    public string className;
}
