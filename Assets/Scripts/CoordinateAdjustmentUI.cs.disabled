using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CoordinateAdjustmentUI : MonoBehaviour
{
    [Header("UI References")]
    public Canvas adjustmentCanvas;
    public GameObject adjustmentPanel;
    
    [Header("Target")]
    public ObjectDetectionManager detectionManager;
    
    // UI Sliders
    private Slider offsetXSlider;
    private Slider offsetYSlider;
    private Slider scaleXSlider;
    private Slider scaleYSlider;
    private Toggle flipXToggle;
    private Toggle flipYToggle;
    
    // UI Text displays
    private TextMeshProUGUI offsetXText;
    private TextMeshProUGUI offsetYText;
    private TextMeshProUGUI scaleXText;
    private TextMeshProUGUI scaleYText;
    
    private bool isUIVisible = false;
      void Start()
    {
        // DISABLED: Bounding box fix UI removed as requested
        // Auto-find components if not assigned
        // if (detectionManager == null)
        //     detectionManager = FindObjectOfType<ObjectDetectionManager>();
            
        // // Create UI if it doesn't exist
        // if (adjustmentCanvas == null)
        //     CreateAdjustmentUI();
        // else
        //     SetupExistingUI();
            
        // // Start with UI hidden
        // ToggleUI(false);
    }
      void Update()
    {
        // DISABLED: Bounding box fix UI removed as requested
        // Toggle UI with Ctrl+C
        // if (Input.GetKeyDown(KeyCode.C) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        // {
        //     ToggleUI(!isUIVisible);
        // }
        
        // // Quick reset with Ctrl+R
        // if (Input.GetKeyDown(KeyCode.R) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        // {
        //     ResetToDefaults();
        // }
    }
    
    void CreateAdjustmentUI()
    {
        // Create canvas if needed
        if (adjustmentCanvas == null)
        {
            GameObject canvasObj = new GameObject("Coordinate Adjustment Canvas");
            adjustmentCanvas = canvasObj.AddComponent<Canvas>();
            adjustmentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            adjustmentCanvas.sortingOrder = 100; // Above other UI
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Create main panel
        GameObject panelObj = new GameObject("Coordinate Adjustment Panel");
        panelObj.transform.SetParent(adjustmentCanvas.transform, false);
        adjustmentPanel = panelObj;
        
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.02f, 0.02f);
        panelRect.anchorMax = new Vector2(0.45f, 0.8f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image panelBg = panelObj.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.85f);
        
        // Create scroll view for better organization
        CreateScrollableContent(panelObj);
    }
    
    void CreateScrollableContent(GameObject parent)
    {
        // Create title
        CreateTitle(parent, "🎯 BOUNDING BOX POSITION FIX", new Vector2(10, -10));
        
        float yPos = -60;
        const float spacing = 90;
        
        // Instructions
        CreateInstructions(parent, yPos);
        yPos -= 100;
        
        // Offset controls
        CreateSliderControl(parent, "X Offset", -200f, 200f, 0f, yPos, 
                          (value) => {
                              if (detectionManager != null) 
                                  detectionManager.coordinateOffsetX = value;
                          }, 
                          out offsetXSlider, out offsetXText);
        yPos -= spacing;
        
        CreateSliderControl(parent, "Y Offset", -200f, 200f, 0f, yPos, 
                          (value) => {
                              if (detectionManager != null) 
                                  detectionManager.coordinateOffsetY = value;
                          }, 
                          out offsetYSlider, out offsetYText);
        yPos -= spacing;
        
        // Scale controls
        CreateSliderControl(parent, "X Scale", 0.5f, 2.0f, 1.0f, yPos, 
                          (value) => {
                              if (detectionManager != null) 
                                  detectionManager.coordinateScaleX = value;
                          }, 
                          out scaleXSlider, out scaleXText);
        yPos -= spacing;
        
        CreateSliderControl(parent, "Y Scale", 0.5f, 2.0f, 1.0f, yPos, 
                          (value) => {
                              if (detectionManager != null) 
                                  detectionManager.coordinateScaleY = value;
                          }, 
                          out scaleYSlider, out scaleYText);
        yPos -= spacing;
        
        // Flip toggles
        CreateToggleControl(parent, "Flip X", false, yPos, 
                          (value) => {
                              if (detectionManager != null) 
                                  detectionManager.flipX = value;
                          }, 
                          out flipXToggle);
        yPos -= 50;
        
        CreateToggleControl(parent, "Flip Y", false, yPos, 
                          (value) => {
                              if (detectionManager != null) 
                                  detectionManager.flipY = value;
                          }, 
                          out flipYToggle);
        yPos -= 80;
        
        // Quick adjustment buttons
        CreateQuickAdjustButtons(parent, yPos);
    }
    
    void CreateTitle(GameObject parent, string text, Vector2 position)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent.transform, false);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = text;
        titleText.fontSize = 16;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = Color.cyan;
        titleText.alignment = TextAlignmentOptions.Top;
        
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0, 1);
        titleRect.anchoredPosition = position;
        titleRect.sizeDelta = new Vector2(-20, 30);
    }
    
    void CreateInstructions(GameObject parent, float yPos)
    {
        GameObject instructObj = new GameObject("Instructions");
        instructObj.transform.SetParent(parent.transform, false);
        
        TextMeshProUGUI instructText = instructObj.AddComponent<TextMeshProUGUI>();
        instructText.text = "🔧 INSTRUCTIONS:\n" +
                           "• Adjust X/Y Offset to move boxes left/right/up/down\n" +
                           "• Use Scale to resize coordinate mapping\n" +
                           "• Try Flip X/Y if boxes are mirrored\n" +
                           "• Press Ctrl+C to toggle this panel\n" +
                           "• Press Ctrl+R to reset all values";
        instructText.fontSize = 11;
        instructText.color = Color.yellow;
        instructText.alignment = TextAlignmentOptions.TopLeft;
        
        RectTransform instructRect = instructText.GetComponent<RectTransform>();
        instructRect.anchorMin = new Vector2(0, 1);
        instructRect.anchorMax = new Vector2(1, 1);
        instructRect.pivot = new Vector2(0, 1);
        instructRect.anchoredPosition = new Vector2(10, yPos);
        instructRect.sizeDelta = new Vector2(-20, 90);
    }
    
    void CreateSliderControl(GameObject parent, string label, float min, float max, float defaultValue, float yPos, 
                           System.Action<float> onValueChanged, out Slider slider, out TextMeshProUGUI valueText)
    {
        // Label
        GameObject labelObj = new GameObject($"{label}_Label");
        labelObj.transform.SetParent(parent.transform, false);
        
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 13;
        labelText.color = Color.white;
        labelText.fontStyle = FontStyles.Bold;
        
        RectTransform labelRect = labelText.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 1);
        labelRect.anchorMax = new Vector2(0.4f, 1);
        labelRect.pivot = new Vector2(0, 1);
        labelRect.anchoredPosition = new Vector2(10, yPos);
        labelRect.sizeDelta = new Vector2(0, 20);
        
        // Slider
        GameObject sliderObj = new GameObject($"{label}_Slider");
        sliderObj.transform.SetParent(parent.transform, false);
        
        slider = sliderObj.AddComponent<Slider>();
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = defaultValue;
        slider.wholeNumbers = false;
        
        // Slider visual setup
        Image sliderBg = sliderObj.AddComponent<Image>();
        sliderBg.color = new Color(0.2f, 0.2f, 0.2f);
        
        // Handle
        GameObject handleObj = new GameObject("Handle");
        handleObj.transform.SetParent(sliderObj.transform, false);
        Image handleImg = handleObj.AddComponent<Image>();
        handleImg.color = Color.cyan;
        
        RectTransform handleRect = handleImg.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(15, 15);
        
        slider.targetGraphic = handleImg;
        slider.handleRect = handleRect;
        
        RectTransform sliderRect = slider.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.4f, 1);
        sliderRect.anchorMax = new Vector2(0.75f, 1);
        sliderRect.pivot = new Vector2(0, 1);
        sliderRect.anchoredPosition = new Vector2(0, yPos - 5);
        sliderRect.sizeDelta = new Vector2(0, 20);
        
        // Value text
        GameObject valueObj = new GameObject($"{label}_Value");
        valueObj.transform.SetParent(parent.transform, false);
        
        valueText = valueObj.AddComponent<TextMeshProUGUI>();
        valueText.text = defaultValue.ToString("F1");
        valueText.fontSize = 12;
        valueText.color = Color.green;
        valueText.alignment = TextAlignmentOptions.Center;
        
        RectTransform valueRect = valueText.GetComponent<RectTransform>();
        valueRect.anchorMin = new Vector2(0.75f, 1);
        valueRect.anchorMax = new Vector2(1, 1);
        valueRect.pivot = new Vector2(0, 1);        valueRect.anchoredPosition = new Vector2(0, yPos);
        valueRect.sizeDelta = new Vector2(0, 20);
        
        // Wire up event - capture local reference to avoid ref/out parameter issue
        TextMeshProUGUI localValueText = valueText;
        slider.onValueChanged.AddListener((value) => {
            localValueText.text = value.ToString("F1");
            onValueChanged?.Invoke(value);
        });
    }
    
    void CreateToggleControl(GameObject parent, string label, bool defaultValue, float yPos, 
                           System.Action<bool> onValueChanged, out Toggle toggle)
    {
        GameObject toggleObj = new GameObject($"{label}_Toggle");
        toggleObj.transform.SetParent(parent.transform, false);
        
        toggle = toggleObj.AddComponent<Toggle>();
        toggle.isOn = defaultValue;
        
        // Background
        Image toggleBg = toggleObj.AddComponent<Image>();
        toggleBg.color = new Color(0.3f, 0.3f, 0.3f);
        
        // Checkmark
        GameObject checkObj = new GameObject("Checkmark");
        checkObj.transform.SetParent(toggleObj.transform, false);
        
        Image checkImg = checkObj.AddComponent<Image>();
        checkImg.color = Color.green;
        checkImg.sprite = null; // Will show as colored box
        
        RectTransform checkRect = checkImg.GetComponent<RectTransform>();
        checkRect.anchorMin = Vector2.zero;
        checkRect.anchorMax = Vector2.one;
        checkRect.offsetMin = Vector2.zero;
        checkRect.offsetMax = Vector2.zero;
        
        toggle.graphic = checkImg;
        
        // Label
        GameObject labelObj = new GameObject($"{label}_Label");
        labelObj.transform.SetParent(toggleObj.transform, false);
        
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 13;
        labelText.color = Color.white;
        
        RectTransform labelRect = labelText.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.pivot = new Vector2(0, 0.5f);
        labelRect.anchoredPosition = new Vector2(30, 0);
        
        RectTransform toggleRect = toggle.GetComponent<RectTransform>();        toggleRect.anchorMin = new Vector2(0, 1);
        toggleRect.anchorMax = new Vector2(0, 1);
        toggleRect.pivot = new Vector2(0, 1);
        toggleRect.anchoredPosition = new Vector2(10, yPos);
        toggleRect.sizeDelta = new Vector2(150, 25);
        
        // Wire up event - convert System.Action to UnityAction
        toggle.onValueChanged.AddListener(new UnityAction<bool>(onValueChanged));
    }
    
    void CreateQuickAdjustButtons(GameObject parent, float yPos)
    {
        string[] buttonTexts = { "Reset All", "Move Left (-50)", "Move Right (+50)", "Move Up (-50)", "Move Down (+50)" };
        System.Action[] buttonActions = {
            () => ResetToDefaults(),
            () => AdjustOffset(-50, 0),
            () => AdjustOffset(50, 0),
            () => AdjustOffset(0, -50),
            () => AdjustOffset(0, 50)
        };
        
        for (int i = 0; i < buttonTexts.Length; i++)
        {
            CreateButton(parent, buttonTexts[i], new Vector2(10, yPos - i * 35), buttonActions[i]);
        }
    }
    
    void CreateButton(GameObject parent, string text, Vector2 position, System.Action onClick)
    {
        GameObject buttonObj = new GameObject($"Button_{text}");
        buttonObj.transform.SetParent(parent.transform, false);
        
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImg = buttonObj.AddComponent<Image>();
        buttonImg.color = new Color(0.2f, 0.5f, 0.8f);
        
        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 11;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        button.targetGraphic = buttonImg;
        
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0, 1);
        buttonRect.anchorMax = new Vector2(1, 1);
        buttonRect.pivot = new Vector2(0, 1);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(-20, 30);
        
        button.onClick.AddListener(() => onClick?.Invoke());
    }
    
    void SetupExistingUI()
    {
        // If UI already exists, find and setup references
        // This is for when the UI is created manually in the scene
    }
    
    void ToggleUI(bool visible)
    {
        isUIVisible = visible;
        if (adjustmentPanel != null)
            adjustmentPanel.SetActive(visible);
            
        if (visible)
        {
            Debug.Log("🎯 Coordinate Adjustment UI opened! Use the sliders to fix bounding box positioning.");
            Debug.Log("💡 TIP: If boxes are to the right, try negative X offset. If too high, try negative Y offset.");
        }
    }
    
    void ResetToDefaults()
    {
        if (detectionManager != null)
        {
            detectionManager.coordinateOffsetX = 0f;
            detectionManager.coordinateOffsetY = 0f;
            detectionManager.coordinateScaleX = 1.0f;
            detectionManager.coordinateScaleY = 1.0f;
            detectionManager.flipX = false;
            detectionManager.flipY = false;
        }
        
        // Update UI sliders
        if (offsetXSlider != null) offsetXSlider.value = 0f;
        if (offsetYSlider != null) offsetYSlider.value = 0f;
        if (scaleXSlider != null) scaleXSlider.value = 1.0f;
        if (scaleYSlider != null) scaleYSlider.value = 1.0f;
        if (flipXToggle != null) flipXToggle.isOn = false;
        if (flipYToggle != null) flipYToggle.isOn = false;
        
        Debug.Log("🔄 Reset all coordinate adjustments to defaults");
    }
    
    void AdjustOffset(float deltaX, float deltaY)
    {
        if (detectionManager != null)
        {
            detectionManager.coordinateOffsetX += deltaX;
            detectionManager.coordinateOffsetY += deltaY;
            
            // Update sliders
            if (offsetXSlider != null) offsetXSlider.value = detectionManager.coordinateOffsetX;
            if (offsetYSlider != null) offsetYSlider.value = detectionManager.coordinateOffsetY;
        }
        
        Debug.Log($"🎯 Adjusted offset by ({deltaX}, {deltaY})");
    }
    
    void OnGUI()
    {
        // Show help text when UI is hidden
        if (!isUIVisible)
        {
            GUILayout.BeginArea(new Rect(10, Screen.height - 80, 400, 70));
            GUILayout.Box("🎯 BOUNDING BOX POSITION FIX");
            GUILayout.Label("Press Ctrl+C to open coordinate adjustment panel");
            GUILayout.Label("Use it to fix bounding box positioning issues");
            GUILayout.EndArea();
        }
    }
}
