# Unity Setup Checklist

Follow this step-by-step checklist to get your object detection system running in Unity.

## ✅ Pre-Setup Verification

- [x] All C# scripts are present in Assets/Scripts/
- [x] YOLOv8n.pt model is available
- [x] YOLOv8n.onnx model has been generated
- [x] Python virtual environment is configured
- [x] All dependencies are installed

## 🎯 Unity Setup Steps

### Step 1: Create New Unity Project
1. Open Unity Hub
2. Create new 3D project (URP recommended)
3. Choose location: `d:\Unity\Projects\Object Detection\`
4. Wait for project creation to complete

### Step 2: Install Required Packages
1. Open **Window → Package Manager**
2. Install **Unity Barracuda**:
   - Search for "Barracuda"
   - Click "Install"
3. Install **TextMeshPro**:
   - Search for "TextMeshPro"
   - Click "Install"
   - Accept TMP Essentials import

### Step 3: Import Scripts
1. Copy all `.cs` files from `Assets/Scripts/` to your Unity project's `Assets/Scripts/` folder
2. Copy the `Editor/` folder with all its contents
3. Refresh Unity (Ctrl+R) or let it auto-refresh

### Step 4: Import ONNX Model
1. Copy `yolov8n.onnx` to `Assets/Models/` (create folder if needed)
2. Select the ONNX file in Unity
3. In Inspector, set **Model Type** to "Neural Network"
4. Click "Apply"

### Step 5: Run Automated Setup
1. Go to **Tools → Object Detection → Setup Wizard**
2. Follow the guided setup process:
   - Click "Install Required Packages" (if not done)
   - Click "Create Scene Objects"
   - Click "Setup Animal Prefabs"
   - Click "Configure Detection Manager"
3. The wizard will create everything automatically

### Step 6: Manual Configuration (if needed)
If automated setup doesn't work, manually:

1. **Create Detection Manager**:
   - Create empty GameObject named "DetectionManager"
   - Add `ObjectDetectionManager` script
   - Assign the ONNX model to `Model Asset`

2. **Create Bounding Box Prefab**:
   - Create UI → Image
   - Add `BoundingBoxUI` script
   - Configure as prefab
   - Assign to Detection Manager

3. **Create Statistics UI**:
   - Create UI → Panel
   - Add `DetectionStatsUI` script
   - Set up UI layout

4. **Create Animal System**:
   - Create empty GameObject named "AnimalSpawner"
   - Add `AnimalSpawner` script
   - Create animal prefabs with `AnimalController` script

5. **Setup Camera**:
   - Add `CameraController` script to Main Camera
   - Position camera appropriately

### Step 7: Test the System
1. Press **Play** in Unity
2. Verify:
   - [ ] Camera controls work (WASD, mouse)
   - [ ] Animals spawn and move around
   - [ ] Detection UI appears
   - [ ] Bounding boxes show around detected objects
   - [ ] Statistics update in real-time

## 🔧 Troubleshooting

### Package Issues
- **Barracuda not found**: Install via Package Manager or check Unity version compatibility
- **TextMeshPro errors**: Import TMP Essentials via TextMeshPro menu

### Model Issues
- **ONNX model not loading**: Check model format and Unity Barracuda compatibility
- **No detections**: Lower confidence threshold or check target categories

### Script Errors
- **Missing references**: Ensure all prefabs and UI elements are properly assigned
- **Compilation errors**: Check Unity version compatibility and namespace issues

### Performance Issues
- **Low FPS**: Reduce input resolution or limit animal count
- **Memory issues**: Use GPU acceleration if available

## 🎮 Controls

Once everything is working:

**Camera Movement**:
- W/A/S/D: Move forward/left/backward/right
- Q/E: Move up/down
- Right mouse + drag: Look around
- Mouse wheel: Zoom in/out

**Detection System**:
- Automatically detects objects in camera view
- Shows colored bounding boxes:
  - 🟢 Green: Humans
  - 🔵 Blue: Cows  
  - ⚪ White: Sheep
  - 🟡 Yellow: Chickens
- Real-time statistics in UI panel

## 📁 Final Project Structure

```
Assets/
├── Scripts/
│   ├── ObjectDetectionManager.cs
│   ├── BoundingBoxUI.cs
│   ├── DetectionStatsUI.cs
│   ├── AnimalController.cs
│   ├── AnimalSpawner.cs
│   ├── CameraController.cs
│   ├── ObjectDetectionSetup.cs
│   └── Editor/
│       ├── ObjectDetectionSetupEditor.cs
│       ├── ObjectDetectionWizard.cs
│       └── YOLOModelConverter.cs
├── Models/
│   └── yolov8n.onnx
├── Prefabs/
│   ├── BoundingBox.prefab
│   ├── Human_Prefab.prefab
│   ├── Cow_Prefab.prefab
│   ├── Sheep_Prefab.prefab
│   └── Chicken_Prefab.prefab
└── Scenes/
    └── ObjectDetectionScene.unity
```

## 🚀 You're Ready!

Once you complete this checklist, your Unity object detection system will be fully operational. The system will automatically detect and track animals and humans in your 3D scene with real-time visual feedback and statistics.

**Happy detecting!** 🎯
