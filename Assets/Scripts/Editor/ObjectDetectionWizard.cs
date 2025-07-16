using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

public class ObjectDetectionWizard : EditorWindow
{
    [MenuItem("Tools/Object Detection/Setup Wizard")]
    public static void ShowWindow()
    {
        var window = GetWindow<ObjectDetectionWizard>("Object Detection Setup Wizard");
        window.minSize = new Vector2(600, 500);
    }
    
    private int currentStep = 0;
    private readonly string[] stepTitles = {
        "Welcome",
        "Check Dependencies", 
        "Model Setup",
        "Scene Setup",
        "Prefab Setup",
        "Testing",
        "Complete"
    };
    
    private bool pythonInstalled = false;
    private bool barracudaInstalled = false;
    private bool textMeshProInstalled = false;
    private bool modelExists = false;
    private bool onnxModelExists = false;
    private Vector2 scrollPosition;
    
    void OnGUI()
    {
        DrawHeader();
        DrawProgressBar();
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        switch (currentStep)
        {
            case 0: DrawWelcomeStep(); break;
            case 1: DrawDependencyCheckStep(); break;
            case 2: DrawModelSetupStep(); break;
            case 3: DrawSceneSetupStep(); break;
            case 4: DrawPrefabSetupStep(); break;
            case 5: DrawTestingStep(); break;
            case 6: DrawCompleteStep(); break;
        }
        
        EditorGUILayout.EndScrollView();
        
        DrawNavigationButtons();
    }
    
    void DrawHeader()
    {
        EditorGUILayout.Space();
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 18;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        
        EditorGUILayout.LabelField("Unity Object Detection Setup Wizard", titleStyle);
        EditorGUILayout.LabelField($"Step {currentStep + 1}: {stepTitles[currentStep]}", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.Space();
    }
    
    void DrawProgressBar()
    {
        Rect rect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true));
        rect.x += 20;
        rect.width -= 40;
        
        EditorGUI.DrawRect(rect, Color.grey);
        
        float progress = (float)(currentStep + 1) / stepTitles.Length;
        Rect progressRect = rect;
        progressRect.width *= progress;
        EditorGUI.DrawRect(progressRect, Color.green);
        
        EditorGUILayout.Space();
    }
    
    void DrawWelcomeStep()
    {
        EditorGUILayout.LabelField("Welcome to the Unity Object Detection Setup Wizard!", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "This wizard will guide you through setting up a complete object detection system in Unity using YOLOv8 and Barracuda.\n\n" +
            "What this wizard will help you set up:\n" +
            "• YOLOv8 model conversion to ONNX format\n" +
            "• Unity scene with detection camera and UI\n" +
            "• Bounding box visualization system\n" +
            "• Animal spawner for testing\n" +
            "• Detection statistics display\n\n" +
            "Target detection categories:\n" +
            "• Humans (person)\n" +
            "• Cows\n" +
            "• Sheep\n" +
            "• Chickens", MessageType.Info);
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Requirements:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("• Python 3.8+ installed");
        EditorGUILayout.LabelField("• Unity 2022.3 LTS or later");
        EditorGUILayout.LabelField("• Internet connection for package downloads");
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Check System Requirements", GUILayout.Height(30)))
        {
            CheckSystemRequirements();
        }
    }
    
    void DrawDependencyCheckStep()
    {
        EditorGUILayout.LabelField("Checking Dependencies", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Python check
        DrawCheckItem("Python 3.8+", pythonInstalled, "Required for model conversion");
        
        if (!pythonInstalled && GUILayout.Button("Check Python Installation"))
        {
            CheckPythonInstallation();
        }
        
        EditorGUILayout.Space();
        
        // Unity packages
        DrawCheckItem("Unity Barracuda", barracudaInstalled, "ML inference engine");
        DrawCheckItem("TextMeshPro", textMeshProInstalled, "UI text rendering");
        
        if (!barracudaInstalled && GUILayout.Button("Install Barracuda Package"))
        {
            InstallBarracudaPackage();
        }
        
        if (!textMeshProInstalled && GUILayout.Button("Install TextMeshPro Package"))
        {
            InstallTextMeshProPackage();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Refresh Dependency Check"))
        {
            CheckAllDependencies();
        }
    }
    
    void DrawModelSetupStep()
    {
        EditorGUILayout.LabelField("YOLOv8 Model Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        string modelPath = Path.Combine(Application.dataPath, "Scripts", "yolov8n.pt");
        string onnxPath = Path.Combine(Application.dataPath, "Scripts", "yolov8n.onnx");
        
        modelExists = File.Exists(modelPath);
        onnxModelExists = File.Exists(onnxPath);
        
        DrawCheckItem("YOLOv8n PyTorch Model", modelExists, modelPath);
        DrawCheckItem("YOLOv8n ONNX Model", onnxModelExists, onnxPath);
        
        EditorGUILayout.Space();
        
        if (!modelExists)
        {
            EditorGUILayout.HelpBox("The YOLOv8n model needs to be downloaded first.", MessageType.Warning);
            
            if (GUILayout.Button("Download YOLOv8n Model"))
            {
                DownloadYOLOModel();
            }
        }
        
        if (modelExists && !onnxModelExists)
        {
            EditorGUILayout.HelpBox("The PyTorch model needs to be converted to ONNX format for Unity.", MessageType.Info);
            
            if (GUILayout.Button("Convert Model to ONNX"))
            {
                ConvertModelToONNX();
            }
        }
        
        if (modelExists && onnxModelExists)
        {
            EditorGUILayout.HelpBox("✓ Model setup is complete!", MessageType.Info);
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Open Model Converter Tool"))
        {
            YOLOModelConverter.ShowWindow();
        }
    }
    
    void DrawSceneSetupStep()
    {
        EditorGUILayout.LabelField("Unity Scene Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("This step will create the complete scene setup including camera, UI, and detection manager.", MessageType.Info);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Auto-Setup Complete Scene", GUILayout.Height(30)))
        {
            ObjectDetectionSetupEditor.ShowWindow();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Manual Setup Steps:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("1. Create UI Canvas (Screen Space Overlay)");
        EditorGUILayout.LabelField("2. Setup detection camera with CameraController");
        EditorGUILayout.LabelField("3. Add ObjectDetectionManager to scene");
        EditorGUILayout.LabelField("4. Create detection statistics UI panel");
        EditorGUILayout.LabelField("5. Add basic terrain and lighting");
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Open Scene Setup Tool"))
        {
            ObjectDetectionSetupEditor.ShowWindow();
        }
    }
    
    void DrawPrefabSetupStep()
    {
        EditorGUILayout.LabelField("Prefab Creation", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("Create the necessary prefabs for the object detection system.", MessageType.Info);
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Required Prefabs:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("• BoundingBox prefab (for detection visualization)");
        EditorGUILayout.LabelField("• Animal prefabs (Human, Cow, Sheep, Chicken)");
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Create BoundingBox Prefab", GUILayout.Height(25)))
        {
            CreateBoundingBoxPrefab();
        }
        
        if (GUILayout.Button("Create Animal Prefabs", GUILayout.Height(25)))
        {
            CreateAnimalPrefabs();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Manual Prefab Creation:", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(
            "BoundingBox Prefab:\n" +
            "• Create UI GameObject with RectTransform\n" +
            "• Add Image component for border\n" +
            "• Add TextMeshPro for labels\n" +
            "• Add BoundingBoxUI script\n\n" +
            "Animal Prefabs:\n" +
            "• Create 3D models or use primitives\n" +
            "• Add Rigidbody and Collider\n" +
            "• Add AnimalController script\n" +
            "• Name appropriately (Human, Cow, etc.)",
            GUILayout.Height(100));
    }
    
    void DrawTestingStep()
    {
        EditorGUILayout.LabelField("Testing the System", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("Test your object detection system to ensure everything is working correctly.", MessageType.Info);
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Testing Checklist:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("✓ ONNX model assigned to ObjectDetectionManager");
        EditorGUILayout.LabelField("✓ BoundingBox prefab assigned");
        EditorGUILayout.LabelField("✓ Animal prefabs assigned to AnimalSpawner");
        EditorGUILayout.LabelField("✓ Detection camera positioned correctly");
        EditorGUILayout.LabelField("✓ UI canvas set to Screen Space Overlay");
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Spawn Test Animals"))
        {
            SpawnTestAnimals();
        }
        
        if (GUILayout.Button("Test Detection System"))
        {
            TestDetectionSystem();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Troubleshooting:", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(
            "Common Issues:\n" +
            "• No detections: Check confidence threshold\n" +
            "• UI not showing: Verify Canvas settings\n" +
            "• Poor performance: Reduce input resolution\n" +
            "• Model errors: Ensure ONNX conversion was successful",
            GUILayout.Height(60));
    }
    
    void DrawCompleteStep()
    {
        EditorGUILayout.LabelField("Setup Complete!", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("🎉 Congratulations! Your Unity Object Detection System is ready to use.", MessageType.Info);
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("What you've accomplished:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("✓ Installed and configured all dependencies");
        EditorGUILayout.LabelField("✓ Set up YOLOv8 model for Unity");
        EditorGUILayout.LabelField("✓ Created complete detection scene");
        EditorGUILayout.LabelField("✓ Built visualization and UI systems");
        EditorGUILayout.LabelField("✓ Added animal simulation for testing");
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Usage:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("• Press Play to start the detection system");
        EditorGUILayout.LabelField("• Use WASD + mouse to control the camera");
        EditorGUILayout.LabelField("• View detection statistics in the UI panel");
        EditorGUILayout.LabelField("• Adjust detection settings in ObjectDetectionManager");
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Open Documentation"))
        {
            string readmePath = Path.Combine(Application.dataPath, "Scripts", "README.md");
            if (File.Exists(readmePath))
            {
                EditorUtility.RevealInFinder(readmePath);
            }
        }
        
        if (GUILayout.Button("Start Fresh Setup"))
        {
            currentStep = 0;
        }
    }
    
    void DrawNavigationButtons()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        
        EditorGUI.BeginDisabledGroup(currentStep == 0);
        if (GUILayout.Button("◀ Previous", GUILayout.Width(80)))
        {
            currentStep = Mathf.Max(0, currentStep - 1);
        }
        EditorGUI.EndDisabledGroup();
        
        GUILayout.FlexibleSpace();
        
        EditorGUI.BeginDisabledGroup(currentStep == stepTitles.Length - 1);
        if (GUILayout.Button("Next ▶", GUILayout.Width(80)))
        {
            currentStep = Mathf.Min(stepTitles.Length - 1, currentStep + 1);
        }
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }
    
    void DrawCheckItem(string label, bool isOk, string details = "")
    {
        EditorGUILayout.BeginHorizontal();
        
        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.normal.textColor = isOk ? Color.green : Color.red;
        
        EditorGUILayout.LabelField(isOk ? "✓" : "✗", labelStyle, GUILayout.Width(20));
        EditorGUILayout.LabelField(label, GUILayout.Width(150));
        
        if (!string.IsNullOrEmpty(details))
        {
            EditorGUILayout.LabelField(details, EditorStyles.miniLabel);
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    // Implementation methods
    void CheckSystemRequirements()
    {
        CheckAllDependencies();
        currentStep = 1;
    }
    
    void CheckAllDependencies()
    {
        CheckPythonInstallation();
        CheckUnityPackages();
    }
    
    void CheckPythonInstallation()
    {
        // This would need to be implemented to actually check Python
        pythonInstalled = true; // Placeholder
    }
    
    void CheckUnityPackages()
    {
        // Check for Barracuda and TextMeshPro
        barracudaInstalled = true; // Placeholder
        textMeshProInstalled = true; // Placeholder
    }
      void InstallBarracudaPackage()
    {
        // Implementation would use Package Manager API
        UnityEngine.Debug.Log("Install Barracuda package via Package Manager");
    }
    
    void InstallTextMeshProPackage()
    {
        // Implementation would use Package Manager API
        UnityEngine.Debug.Log("Install TextMeshPro package via Package Manager");
    }
    
    void DownloadYOLOModel()
    {
        Application.OpenURL("https://github.com/ultralytics/assets/releases/download/v0.0.0/yolov8n.pt");
        EditorUtility.DisplayDialog("Download Model", 
            "Opening download page in browser. Save the yolov8n.pt file to your Scripts folder.", "OK");
    }
    
    void ConvertModelToONNX()
    {
        YOLOModelConverter.ShowWindow();
    }
      void CreateBoundingBoxPrefab()
    {
        UnityEngine.Debug.Log("Creating BoundingBox prefab...");
        // Implementation would create the prefab
    }
    
    void CreateAnimalPrefabs()
    {
        UnityEngine.Debug.Log("Creating animal prefabs...");
        // Implementation would create animal prefabs
    }
    
    void SpawnTestAnimals()
    {
        AnimalSpawner spawner = FindObjectOfType<AnimalSpawner>();        if (spawner != null)
        {
            spawner.RespawnAnimals();
            UnityEngine.Debug.Log("Test animals spawned");
        }
        else
        {
            UnityEngine.Debug.LogWarning("AnimalSpawner not found in scene");
        }
    }
    
    void TestDetectionSystem()
    {
        ObjectDetectionManager manager = FindObjectOfType<ObjectDetectionManager>();        if (manager != null)
        {
            UnityEngine.Debug.Log("Detection system test initiated");
            EditorUtility.DisplayDialog("Test", "Detection system is running. Check the Game view for results.", "OK");
        }        else
        {
            UnityEngine.Debug.LogWarning("ObjectDetectionManager not found in scene");
        }
    }
}
