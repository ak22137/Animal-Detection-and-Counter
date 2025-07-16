using UnityEngine;
using UnityEngine.UI;

public class PerformanceMonitor : MonoBehaviour
{
    [Header("UI Elements")]
    public Text fpsText;
    public Text performanceText;
    public Button togglePerformanceButton;
    
    [Header("Settings")]
    public bool showDetailedStats = true;
    
    private float deltaTime = 0.0f;
    private ObjectDetectionManager detectionManager;
    private float lastFpsUpdate = 0f;
    private float fpsUpdateInterval = 0.5f; // Update FPS display twice per second
    
    void Start()
    {
        detectionManager = FindObjectOfType<ObjectDetectionManager>();
        
        if (togglePerformanceButton != null)
        {
            togglePerformanceButton.onClick.AddListener(TogglePerformanceMode);
        }
        
        // Create UI elements if not assigned
        if (fpsText == null)
        {
            CreateFPSDisplay();
        }
    }
    
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        
        // Update FPS display periodically for better performance
        if (Time.time - lastFpsUpdate > fpsUpdateInterval)
        {
            UpdateFPSDisplay();
            UpdatePerformanceDisplay();
            lastFpsUpdate = Time.time;
        }
    }
    
    void UpdateFPSDisplay()
    {
        if (fpsText != null)
        {
            float fps = 1.0f / deltaTime;
            string fpsColor = GetFPSColor(fps);
            fpsText.text = $"<color={fpsColor}>FPS: {fps:F1}</color>";
        }
    }
      void UpdatePerformanceDisplay()
    {
        if (performanceText != null && detectionManager != null && showDetailedStats)
        {
            int frameSkip = detectionManager.frameSkip;
            float confidence = detectionManager.confidenceThreshold;
            bool enhancement = detectionManager.enhanceDistantObjects;
            
            string mode = frameSkip <= 2 ? "Quality" : "Performance";
            string enhanceMode = enhancement ? "ON" : "OFF";
            
            performanceText.text = $"Mode: {mode} (640x640)\n" +
                                 $"Frame Skip: {frameSkip}\n" +
                                 $"Confidence: {confidence:F2}\n" +
                                 $"Enhancement: {enhanceMode}";
        }
    }
    
    string GetFPSColor(float fps)
    {
        if (fps >= 30f) return "green";
        else if (fps >= 15f) return "yellow";
        else return "red";
    }
      void TogglePerformanceMode()
    {
        if (detectionManager != null)
        {
            // Since low-resolution mode is disabled for YOLO compatibility,
            // we'll toggle between different frame skip rates for performance
            if (detectionManager.frameSkip == 2)
            {
                // Switch to performance mode
                detectionManager.frameSkip = 4;
                detectionManager.enhanceDistantObjects = false;
                Debug.Log("Switched to Performance Mode (640x640, Frame Skip 4)");
            }
            else
            {
                // Switch to quality mode
                detectionManager.frameSkip = 2;
                detectionManager.enhanceDistantObjects = false; // Keep enhancement off for performance
                Debug.Log("Switched to Quality Mode (640x640, Frame Skip 2)");
            }
        }
    }
    
    void CreateFPSDisplay()
    {
        // Create a simple FPS display if none exists
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("PerformanceCanvas");
            Canvas canvasComponent = canvasObj.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasComponent.sortingOrder = 1000; // High priority
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas = canvasObj;
        }
        
        // Create FPS text
        GameObject fpsObj = new GameObject("FPS Display");
        fpsObj.transform.SetParent(canvas.transform, false);
        
        RectTransform fpsRect = fpsObj.AddComponent<RectTransform>();
        fpsRect.anchorMin = new Vector2(0, 1);
        fpsRect.anchorMax = new Vector2(0, 1);
        fpsRect.pivot = new Vector2(0, 1);
        fpsRect.anchoredPosition = new Vector2(10, -10);
        fpsRect.sizeDelta = new Vector2(200, 50);
        
        fpsText = fpsObj.AddComponent<Text>();
        fpsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        fpsText.fontSize = 24;
        fpsText.color = Color.white;
        fpsText.text = "FPS: --";
        
        // Create performance text
        if (showDetailedStats)
        {
            GameObject perfObj = new GameObject("Performance Display");
            perfObj.transform.SetParent(canvas.transform, false);
            
            RectTransform perfRect = perfObj.AddComponent<RectTransform>();
            perfRect.anchorMin = new Vector2(0, 1);
            perfRect.anchorMax = new Vector2(0, 1);
            perfRect.pivot = new Vector2(0, 1);
            perfRect.anchoredPosition = new Vector2(10, -70);
            perfRect.sizeDelta = new Vector2(300, 120);
            
            performanceText = perfObj.AddComponent<Text>();
            performanceText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            performanceText.fontSize = 16;
            performanceText.color = Color.yellow;
            performanceText.text = "Performance Stats";
        }
        
        // Create toggle button
        GameObject buttonObj = new GameObject("Toggle Performance Button");
        buttonObj.transform.SetParent(canvas.transform, false);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(1, 1);
        buttonRect.anchorMax = new Vector2(1, 1);
        buttonRect.pivot = new Vector2(1, 1);
        buttonRect.anchoredPosition = new Vector2(-10, -10);
        buttonRect.sizeDelta = new Vector2(150, 40);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.8f, 0.8f);
        
        togglePerformanceButton = buttonObj.AddComponent<Button>();
        togglePerformanceButton.targetGraphic = buttonImage;
        togglePerformanceButton.onClick.AddListener(TogglePerformanceMode);
        
        // Button text
        GameObject buttonTextObj = new GameObject("Button Text");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;
        buttonTextRect.anchoredPosition = Vector2.zero;
        
        Text buttonText = buttonTextObj.AddComponent<Text>();
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 14;
        buttonText.color = Color.white;
        buttonText.text = "Toggle Mode";
        buttonText.alignment = TextAnchor.MiddleCenter;
    }
    
    // Public method to get current FPS for external monitoring
    public float GetCurrentFPS()
    {
        return 1.0f / deltaTime;
    }
      // Method to log performance statistics
    public void LogPerformanceStats()
    {
        float fps = GetCurrentFPS();
        Debug.Log($"[Performance Monitor] FPS: {fps:F1}, Frame Skip: {detectionManager?.frameSkip}");
    }
}
