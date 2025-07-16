using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class ObjectDetectionSetupEditor : EditorWindow
{
    [MenuItem("Tools/Object Detection/Setup Scene")]
    public static void ShowWindow()
    {
        GetWindow<ObjectDetectionSetupEditor>("Object Detection Setup");
    }
    
    private bool createCanvas = true;
    private bool createCamera = true;
    private bool createDetectionManager = true;
    private bool createStatsUI = true;
    private bool createAnimalSpawner = true;
    private bool setupLighting = true;
    private bool createTerrain = true;
    
    void OnGUI()
    {
        EditorGUILayout.LabelField("Unity Object Detection System Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("This tool will set up a complete object detection scene in Unity", EditorStyles.helpBox);
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Setup Options:", EditorStyles.boldLabel);
        createCanvas = EditorGUILayout.Toggle("Create UI Canvas", createCanvas);
        createCamera = EditorGUILayout.Toggle("Setup Detection Camera", createCamera);
        createDetectionManager = EditorGUILayout.Toggle("Create Detection Manager", createDetectionManager);
        createStatsUI = EditorGUILayout.Toggle("Create Statistics UI", createStatsUI);
        createAnimalSpawner = EditorGUILayout.Toggle("Create Animal Spawner", createAnimalSpawner);
        createTerrain = EditorGUILayout.Toggle("Create Simple Terrain", createTerrain);
        setupLighting = EditorGUILayout.Toggle("Setup Basic Lighting", setupLighting);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Setup Complete Scene", GUILayout.Height(30)))
        {
            SetupObjectDetectionScene();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Create Bounding Box Prefab", GUILayout.Height(25)))
        {
            CreateBoundingBoxPrefab();
        }
        
        if (GUILayout.Button("Create Animal Prefabs", GUILayout.Height(25)))
        {
            CreateAnimalPrefabs();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Required Assets:", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(
            "• YOLOv8 ONNX model (convert using Tools/YOLO Model Converter)\n" +
            "• Unity Barracuda package\n" +
            "• TextMeshPro package\n" +
            "• Basic 3D models for animals (primitives will be used if not available)",
            GUILayout.Height(80));
    }
    
    void SetupObjectDetectionScene()
    {
        // Create canvas if needed
        Canvas canvas = null;
        if (createCanvas)
        {
            canvas = SetupUICanvas();
        }
        
        // Setup camera
        Camera detectionCamera = null;
        if (createCamera)
        {
            detectionCamera = SetupDetectionCamera();
        }
        
        // Create detection manager
        ObjectDetectionManager detectionManager = null;
        if (createDetectionManager)
        {
            detectionManager = SetupDetectionManager(canvas, detectionCamera);
        }
        
        // Create stats UI
        if (createStatsUI && canvas != null)
        {
            SetupStatsUI(canvas);
        }
        
        // Create animal spawner
        if (createAnimalSpawner)
        {
            SetupAnimalSpawner();
        }
        
        // Create terrain
        if (createTerrain)
        {
            SetupTerrain();
        }
        
        // Setup lighting
        if (setupLighting)
        {
            SetupLighting();
        }
        
        Debug.Log("Object Detection Scene setup completed!");
        EditorUtility.DisplayDialog("Setup Complete", 
            "Object Detection scene has been set up!\n\n" +
            "Next steps:\n" +
            "1. Convert your YOLOv8 model using Tools/YOLO Model Converter\n" +
            "2. Assign the ONNX model to ObjectDetectionManager\n" +
            "3. Create and assign the BoundingBox prefab\n" +
            "4. Create animal prefabs and assign to AnimalSpawner", "OK");
    }
    
    Canvas SetupUICanvas()
    {
        GameObject canvasObj = new GameObject("Object Detection Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        return canvas;
    }
    
    Camera SetupDetectionCamera()
    {
        GameObject cameraObj = new GameObject("Detection Camera");
        Camera camera = cameraObj.AddComponent<Camera>();
        
        // Position camera for good view of the scene
        cameraObj.transform.position = new Vector3(0, 10, -20);
        cameraObj.transform.rotation = Quaternion.Euler(15, 0, 0);
        
        // Add camera controller
        cameraObj.AddComponent<CameraController>();
        
        // Set camera as main camera
        camera.tag = "MainCamera";
        
        return camera;
    }
    
    ObjectDetectionManager SetupDetectionManager(Canvas canvas, Camera camera)
    {
        GameObject managerObj = new GameObject("Object Detection Manager");
        ObjectDetectionManager manager = managerObj.AddComponent<ObjectDetectionManager>();
        
        manager.uiCanvas = canvas;
        manager.detectionCamera = camera;
        manager.confidenceThreshold = 0.5f;
        manager.iouThreshold = 0.4f;
        
        return manager;
    }
    
    void SetupStatsUI(Canvas canvas)
    {
        // Create stats panel
        GameObject statsPanel = new GameObject("Detection Stats Panel");
        statsPanel.transform.SetParent(canvas.transform, false);
        
        RectTransform statsRect = statsPanel.AddComponent<RectTransform>();
        statsRect.anchorMin = new Vector2(0, 1);
        statsRect.anchorMax = new Vector2(0, 1);
        statsRect.pivot = new Vector2(0, 1);
        statsRect.anchoredPosition = new Vector2(20, -20);
        statsRect.sizeDelta = new Vector2(250, 180);
        
        Image background = statsPanel.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.8f);
        
        // Add DetectionStatsUI component
        DetectionStatsUI statsUI = statsPanel.AddComponent<DetectionStatsUI>();
        
        // Create title
        CreateUIText(statsPanel, "OBJECT DETECTION", new Vector2(10, -10), 16, Color.yellow, out TextMeshProUGUI titleText);
        
        // Create stat texts
        CreateUIText(statsPanel, "Humans: 0", new Vector2(10, -35), 14, Color.white, out statsUI.personCountText);
        CreateUIText(statsPanel, "Cows: 0", new Vector2(10, -55), 14, Color.white, out statsUI.cowCountText);
        CreateUIText(statsPanel, "Sheep: 0", new Vector2(10, -75), 14, Color.white, out statsUI.sheepCountText);
        CreateUIText(statsPanel, "Chickens: 0", new Vector2(10, -95), 14, Color.white, out statsUI.chickenCountText);
        CreateUIText(statsPanel, "Total: 0", new Vector2(10, -120), 14, Color.cyan, out statsUI.totalDetectionsText);
        CreateUIText(statsPanel, "FPS: 0", new Vector2(10, -145), 14, Color.green, out statsUI.fpsText);
    }
    
    void CreateUIText(GameObject parent, string text, Vector2 position, float fontSize, Color color, out TextMeshProUGUI textComponent)
    {
        GameObject textObj = new GameObject("UI Text");
        textObj.transform.SetParent(parent.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 1);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.pivot = new Vector2(0, 1);
        textRect.anchoredPosition = position;
        textRect.sizeDelta = new Vector2(-20, 20);
        
        textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = color;
        textComponent.fontStyle = FontStyles.Bold;
    }
    
    void SetupAnimalSpawner()
    {
        GameObject spawnerObj = new GameObject("Animal Spawner");
        AnimalSpawner spawner = spawnerObj.AddComponent<AnimalSpawner>();
        
        spawner.maxAnimals = 15;
        spawner.spawnRadius = 30f;
        spawner.spawnCenter = Vector3.zero;
        spawner.moveSpeed = 2f;
        spawner.rotationSpeed = 30f;
        spawner.changeDirectionInterval = 4f;
    }
    
    void SetupTerrain()
    {
        // Create simple ground plane
        GameObject groundObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        groundObj.name = "Ground";
        groundObj.transform.localScale = new Vector3(10, 1, 10);
        groundObj.transform.position = Vector3.zero;
        
        // Create material for ground
        Material groundMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        groundMaterial.color = new Color(0.3f, 0.7f, 0.3f); // Green color
        groundObj.GetComponent<Renderer>().material = groundMaterial;
        
        // Set layer for ground detection
        groundObj.layer = LayerMask.NameToLayer("Default");
    }
    
    void SetupLighting()
    {
        // Create directional light (sun)
        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = Color.white;
        light.intensity = 1.5f;
        lightObj.transform.rotation = Quaternion.Euler(45f, 45f, 0f);
        
        // Set ambient lighting
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.5f, 0.7f, 1f);
        RenderSettings.ambientEquatorColor = new Color(0.4f, 0.4f, 0.4f);
        RenderSettings.ambientGroundColor = new Color(0.2f, 0.2f, 0.2f);
    }
    
    void CreateBoundingBoxPrefab()
    {
        // Create bounding box prefab
        GameObject boundingBoxPrefab = new GameObject("BoundingBox");
        
        // Add RectTransform
        RectTransform rect = boundingBoxPrefab.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 100);
        
        // Add Image for border
        Image image = boundingBoxPrefab.AddComponent<Image>();
        image.color = new Color(1, 0, 0, 0); // Transparent fill
        
        // Add outline for border effect
        Outline outline = boundingBoxPrefab.AddComponent<Outline>();
        outline.effectColor = Color.red;
        outline.effectDistance = new Vector2(2, 2);
        
        // Create label background
        GameObject labelBg = new GameObject("Label Background");
        labelBg.transform.SetParent(boundingBoxPrefab.transform, false);
        
        RectTransform labelBgRect = labelBg.AddComponent<RectTransform>();
        labelBgRect.anchorMin = new Vector2(0, 1);
        labelBgRect.anchorMax = new Vector2(1, 1);
        labelBgRect.pivot = new Vector2(0, 1);
        labelBgRect.anchoredPosition = new Vector2(0, 2);
        labelBgRect.sizeDelta = new Vector2(0, 25);
        
        Image labelBgImage = labelBg.AddComponent<Image>();
        labelBgImage.color = new Color(0, 0, 0, 0.8f);
        
        // Create label text
        GameObject labelObj = new GameObject("Label Text");
        labelObj.transform.SetParent(labelBg.transform, false);
        
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(5, 0);
        labelRect.offsetMax = new Vector2(-5, 0);
        
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();        labelText.text = "OBJECT #1";
        labelText.fontSize = 12;
        labelText.color = Color.white;
        labelText.fontStyle = FontStyles.Bold;
        labelText.alignment = TextAlignmentOptions.MidlineLeft;
        
        // Create confidence text
        GameObject confidenceObj = new GameObject("Confidence Text");
        confidenceObj.transform.SetParent(labelBg.transform, false);
        
        RectTransform confidenceRect = confidenceObj.AddComponent<RectTransform>();
        confidenceRect.anchorMin = Vector2.zero;
        confidenceRect.anchorMax = Vector2.one;
        confidenceRect.offsetMin = new Vector2(5, 0);
        confidenceRect.offsetMax = new Vector2(-5, 0);
        
        TextMeshProUGUI confidenceText = confidenceObj.AddComponent<TextMeshProUGUI>();
        confidenceText.text = "95%";
        confidenceText.fontSize = 10;
        confidenceText.color = Color.green;
        confidenceText.alignment = TextAlignmentOptions.MidlineRight;
        
        // Add BoundingBoxUI component
        BoundingBoxUI boundingBoxUI = boundingBoxPrefab.AddComponent<BoundingBoxUI>();
        boundingBoxUI.boundingBoxRect = rect;
        boundingBoxUI.boundingBoxImage = image;
        boundingBoxUI.labelText = labelText;
        boundingBoxUI.confidenceText = confidenceText;
        
        // Save as prefab
        string prefabPath = "Assets/Prefabs/BoundingBox.prefab";
        string prefabDir = Path.GetDirectoryName(prefabPath);
        
        if (!Directory.Exists(prefabDir))
        {
            Directory.CreateDirectory(prefabDir);
        }
        
        PrefabUtility.SaveAsPrefabAsset(boundingBoxPrefab, prefabPath);
        DestroyImmediate(boundingBoxPrefab);
        
        Debug.Log($"BoundingBox prefab created at: {prefabPath}");
        
        // Assign to detection manager if it exists
        ObjectDetectionManager manager = FindObjectOfType<ObjectDetectionManager>();
        if (manager != null)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            manager.boundingBoxPrefab = prefab;
            EditorUtility.SetDirty(manager);
        }
    }
    
    void CreateAnimalPrefabs()
    {
        string prefabDir = "Assets/Prefabs/Animals";
        if (!Directory.Exists(prefabDir))
        {
            Directory.CreateDirectory(prefabDir);
        }
        
        // Create animal prefabs using Unity primitives
        CreateAnimalPrefab("Human", PrimitiveType.Capsule, new Vector3(1, 2, 1), Color.blue, prefabDir);
        CreateAnimalPrefab("Cow", PrimitiveType.Cube, new Vector3(2, 1.5f, 3), Color.black, prefabDir);
        CreateAnimalPrefab("Sheep", PrimitiveType.Cube, new Vector3(1.5f, 1.2f, 2), Color.white, prefabDir);
        CreateAnimalPrefab("Chicken", PrimitiveType.Sphere, new Vector3(0.8f, 0.8f, 1), Color.yellow, prefabDir);
        
        Debug.Log($"Animal prefabs created in: {prefabDir}");
    }
    
    void CreateAnimalPrefab(string name, PrimitiveType primitive, Vector3 scale, Color color, string directory)
    {
        GameObject animalObj = GameObject.CreatePrimitive(primitive);
        animalObj.name = name;
        animalObj.transform.localScale = scale;
        
        // Set color
        Renderer renderer = animalObj.GetComponent<Renderer>();
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = color;
        renderer.material = material;
        
        // Add Rigidbody
        Rigidbody rb = animalObj.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.linearDamping = 2f;
        
        // Add AnimalController
        AnimalController controller = animalObj.AddComponent<AnimalController>();
        controller.animalType = name.ToLower();
        
        // Set appropriate tag
        animalObj.tag = "Untagged"; // Will be set by AnimalController
        
        // Save as prefab
        string prefabPath = Path.Combine(directory, $"{name}.prefab");
        PrefabUtility.SaveAsPrefabAsset(animalObj, prefabPath);
        DestroyImmediate(animalObj);
        
        // Assign to animal spawner if it exists
        AnimalSpawner spawner = FindObjectOfType<AnimalSpawner>();
        if (spawner != null)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            switch (name.ToLower())
            {
                case "human":
                    spawner.humanPrefab = prefab;
                    break;
                case "cow":
                    spawner.cowPrefab = prefab;
                    break;
                case "sheep":
                    spawner.sheepPrefab = prefab;
                    break;
                case "chicken":
                    spawner.chickenPrefab = prefab;
                    break;
            }
            
            EditorUtility.SetDirty(spawner);
        }
    }
}
