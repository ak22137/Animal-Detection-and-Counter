using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DetectionStatsUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI personCountText;
    public TextMeshProUGUI cowCountText;
    public TextMeshProUGUI sheepCountText;
    public TextMeshProUGUI chickenCountText;
    public TextMeshProUGUI totalDetectionsText;
    public TextMeshProUGUI fpsText;
    
    [Header("Settings")]
    public float updateInterval = 0.5f;
    
    private ObjectDetectionManager detectionManager;
    private float lastUpdateTime;
    private int frameCount;
    private float fps;
    
    void Start()
    {
        detectionManager = FindObjectOfType<ObjectDetectionManager>();
        lastUpdateTime = Time.time;
    }
    
    void Update()
    {
        frameCount++;
        
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateStats();
            UpdateFPS();
            lastUpdateTime = Time.time;
            frameCount = 0;
        }
    }
    
    void UpdateStats()
    {
        if (detectionManager == null) return;
        
        // Get current detection counts from the detection manager
        var stats = GetDetectionCounts();
        
        // Update UI texts
        if (personCountText != null)
            personCountText.text = $"Humans: {stats["person"]}";
        
        if (cowCountText != null)
            cowCountText.text = $"Cows: {stats["cow"]}";
        
        if (sheepCountText != null)
            sheepCountText.text = $"Sheep: {stats["sheep"]}";
        
        if (chickenCountText != null)
            chickenCountText.text = $"Chickens: {stats["chicken"]}";
        
        if (totalDetectionsText != null)
        {
            int total = stats["person"] + stats["cow"] + stats["sheep"] + stats["chicken"];
            totalDetectionsText.text = $"Total Detections: {total}";
        }
    }
    
    void UpdateFPS()
    {
        fps = frameCount / updateInterval;
        
        if (fpsText != null)
            fpsText.text = $"FPS: {fps:F1}";
    }
    
    Dictionary<string, int> GetDetectionCounts()
    {
        var counts = new Dictionary<string, int>
        {
            ["person"] = 0,
            ["cow"] = 0,
            ["sheep"] = 0,
            ["chicken"] = 0
        };
        
        if (detectionManager != null)
        {
            // Count active bounding boxes by category
            var activeBoundingBoxes = detectionManager.GetActiveBoundingBoxes();
            
            foreach (var bbox in activeBoundingBoxes)
            {
                if (bbox != null)
                {
                    string category = bbox.GetDetectionCategory();
                    if (counts.ContainsKey(category))
                    {
                        counts[category]++;
                    }
                }
            }
        }
        
        return counts;
    }
}
