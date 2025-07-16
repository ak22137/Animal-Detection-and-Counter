using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoundingBoxUI : MonoBehaviour
{
    [Header("UI Components")]
    public RectTransform boundingBoxRect;
    public Image boundingBoxImage;
    public TextMeshProUGUI labelText;
    public TextMeshProUGUI confidenceText;
    
    [Header("Category Colors")]
    public Color personColor = Color.green;
    public Color cowColor = Color.blue;
    public Color sheepColor = Color.white;
    public Color chickenColor = Color.yellow;
    public Color defaultColor = Color.red;
    
    private Detection currentDetection;
    private int objectId;
      void Awake()
    {
        // If components are not assigned, try to find them automatically
        if (boundingBoxRect == null)
            boundingBoxRect = GetComponent<RectTransform>();
        
        if (boundingBoxImage == null)
            boundingBoxImage = GetComponent<Image>();
        
        if (labelText == null)
            labelText = GetComponentInChildren<TextMeshProUGUI>();
        
        // Create UI components if they don't exist
        SetupUIComponents();
        
        // Ensure the bounding box is just an outline
        if (boundingBoxImage != null)
        {
            // Set up as outline with transparent fill
            boundingBoxImage.color = new Color(1, 1, 1, 0.8f); // Semi-transparent outline
            boundingBoxImage.type = Image.Type.Sliced;
        }
    }
    
    void SetupUIComponents()
    {
        // Ensure we have a RectTransform
        if (boundingBoxRect == null)
            boundingBoxRect = GetComponent<RectTransform>();
        
        // Create Image component if missing
        if (boundingBoxImage == null)
        {
            boundingBoxImage = gameObject.AddComponent<Image>();
            // Create a simple white pixel texture for the outline
            Texture2D whiteTexture = new Texture2D(1, 1);
            whiteTexture.SetPixel(0, 0, Color.white);
            whiteTexture.Apply();
            Sprite whiteSprite = Sprite.Create(whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
            boundingBoxImage.sprite = whiteSprite;
        }
        
        // Create label text if missing
        if (labelText == null)
        {
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(transform);
            labelText = labelObj.AddComponent<TextMeshProUGUI>();
            
            // Configure text component
            labelText.text = "OBJECT #1";
            labelText.fontSize = 14;
            labelText.color = Color.white;
            labelText.alignment = TextAlignmentOptions.TopLeft;
            
            // Position at top-left of bounding box
            RectTransform labelRect = labelText.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 1);
            labelRect.anchorMax = new Vector2(1, 1);
            labelRect.pivot = new Vector2(0, 1);
            labelRect.anchoredPosition = new Vector2(5, -5);
            labelRect.sizeDelta = new Vector2(-10, 20);
        }
        
        // Create confidence text if missing
        if (confidenceText == null)
        {
            GameObject confObj = new GameObject("Confidence");
            confObj.transform.SetParent(transform);
            confidenceText = confObj.AddComponent<TextMeshProUGUI>();
            
            // Configure confidence text
            confidenceText.text = "100%";
            confidenceText.fontSize = 12;
            confidenceText.color = Color.white;
            confidenceText.alignment = TextAlignmentOptions.TopRight;
            
            // Position at top-right of bounding box
            RectTransform confRect = confidenceText.GetComponent<RectTransform>();
            confRect.anchorMin = new Vector2(1, 1);
            confRect.anchorMax = new Vector2(1, 1);
            confRect.pivot = new Vector2(1, 1);
            confRect.anchoredPosition = new Vector2(-5, -5);
            confRect.sizeDelta = new Vector2(60, 20);
        }
    }
    
    public void SetupBoundingBox(Detection detection, int id)
    {
        currentDetection = detection;
        objectId = id;
        
        UpdateBoundingBoxVisuals();
        UpdatePosition();
    }
    
    void UpdateBoundingBoxVisuals()
    {
        // Set color based on category
        Color categoryColor = GetCategoryColor(currentDetection.className);
        
        if (boundingBoxImage != null)
        {
            // Create outline effect by using a border
            boundingBoxImage.color = categoryColor;
        }
        
        // Update label text
        if (labelText != null)
        {
            string label = $"{currentDetection.className.ToUpper()} #{objectId}";
            labelText.text = label;
            labelText.color = categoryColor;
        }
        
        // Update confidence text if available
        if (confidenceText != null)
        {
            confidenceText.text = $"{(currentDetection.confidence * 100):F1}%";
            confidenceText.color = categoryColor;
        }
    }    void UpdatePosition()
    {
        if (boundingBoxRect == null) return;
        
        // Calculate bounding box dimensions
        float width = currentDetection.x2 - currentDetection.x1;
        float height = currentDetection.y2 - currentDetection.y1;
        
        // Set size
        boundingBoxRect.sizeDelta = new Vector2(width, height);
        
        // Simple direct positioning - use detection coordinates as-is
        boundingBoxRect.anchorMin = Vector2.zero;
        boundingBoxRect.anchorMax = Vector2.zero;
        boundingBoxRect.pivot = Vector2.zero;
        boundingBoxRect.anchoredPosition = new Vector2(currentDetection.x1, currentDetection.y1);
    }
    
    Color GetCategoryColor(string category)
    {
        switch (category.ToLower())
        {
            case "person":
                return personColor;
            case "cow":
                return cowColor;
            case "sheep":
                return sheepColor;
            case "chicken":
                return chickenColor;
            default:
                return defaultColor;
        }
    }
      public string GetDetectionCategory()
    {
        return currentDetection?.className ?? "";
    }
    
    public int GetObjectId()
    {
        return objectId;
    }
    
    public float GetConfidence()
    {
        return currentDetection?.confidence ?? 0f;
    }
    
    void Update()
    {
        // Optional: Add pulsing or animation effects
        AnimateBoundingBox();
    }
    
    void AnimateBoundingBox()
    {
        // Simple pulsing effect for the outline
        if (boundingBoxImage != null)
        {
            float pulse = Mathf.Sin(Time.time * 2f) * 0.1f + 0.9f;
            Color currentColor = boundingBoxImage.color;
            currentColor.a = pulse;
            boundingBoxImage.color = currentColor;
        }
    }
}
