# 🎯 Unity ONNX Model Setup Guide

## Problem: ONNX Model Not Accepting Assignment

The issue you're experiencing is common when working with ONNX models in Unity. Here are the solutions:

## ✅ **SOLUTION 1: Direct Model Assignment (Recommended)**

### Step 1: Import ONNX as NNModel
1. **Copy your ONNX file to Unity Assets**
   ```
   Copy: d:\Unity\Projects\Object Detection\Assets\Scripts\yolov8n.onnx
   To: d:\Unity\Projects\Object Detection\Assets\Models\yolov8n.onnx
   ```

2. **Create Models folder if it doesn't exist**
   - Right-click in Project window → Create → Folder
   - Name it "Models"

3. **Import the ONNX file**
   - Drag `yolov8n.onnx` into the `Assets/Models/` folder
   - Unity should automatically import it as an `NNModel` asset
   - You'll see it with a brain icon 🧠

4. **Assign to ObjectDetectionManager**
   - Select your ObjectDetectionManager GameObject in the scene
   - In the Inspector, find "Model Asset" field
   - Drag the `yolov8n.onnx` from Project window to this field

## ✅ **SOLUTION 2: StreamingAssets Method (Fallback)**

If direct assignment doesn't work, use StreamingAssets:

### Step 1: Create StreamingAssets Folder
```
Assets/
└── StreamingAssets/
    └── yolov8n.onnx
```

### Step 2: Copy ONNX File
1. Create `StreamingAssets` folder in your `Assets` directory
2. Copy `yolov8n.onnx` into `Assets/StreamingAssets/`
3. Leave "Model Asset" field empty in ObjectDetectionManager
4. Set "Fallback Model Path" to: `yolov8n.onnx`

## 🔧 **Updated ObjectDetectionManager Features**

The script now supports:
- ✅ Direct NNModel assignment (preferred)
- ✅ Fallback loading from StreamingAssets
- ✅ Better error messages and debugging
- ✅ Automatic model detection

## 🚀 **Quick Setup Steps**

1. **Create the Models folder:**
   ```
   Assets/Models/
   ```

2. **Move your ONNX file:**
   ```
   From: Assets/Scripts/yolov8n.onnx
   To: Assets/Models/yolov8n.onnx
   ```

3. **Assign in Inspector:**
   - ObjectDetectionManager → Model Asset → yolov8n.onnx

## 🐛 **Troubleshooting**

### If the ONNX file shows as "unsupported":
1. **Check Unity Barracuda version**
   - Window → Package Manager → Barracuda
   - Ensure version 3.0.0 or later

2. **Reimport the ONNX file**
   - Right-click the ONNX file → Reimport
   - Check if import errors appear in Console

3. **Verify ONNX file integrity**
   - File size should be ~12.2 MB
   - Try re-converting with the conversion script

### If assignment still doesn't work:
1. **Use StreamingAssets method**
2. **Check Console for error messages**
3. **Ensure Unity 2022.3+ and Barracuda package installed**

## 📁 **Recommended Project Structure**

```
Assets/
├── Models/
│   └── yolov8n.onnx (NNModel asset)
├── Scripts/
│   └── [all your scripts]
├── Prefabs/
│   ├── BoundingBox.prefab
│   └── Animals/
├── StreamingAssets/ (fallback)
│   └── yolov8n.onnx (backup copy)
└── Scenes/
    └── ObjectDetection.unity
```

## 🎯 **Verification Steps**

1. **Check Model Assignment:**
   - ObjectDetectionManager Inspector shows model assigned ✅
   - No errors in Console when entering Play mode ✅

2. **Test Loading:**
   - Press Play
   - Console should show: "YOLO model loaded successfully" ✅

3. **If using StreamingAssets:**
   - Console shows: "YOLO model loaded successfully from StreamingAssets" ✅

## 💡 **Pro Tips**

- **Always use the Models folder** for organized asset management
- **Keep a backup in StreamingAssets** for deployment flexibility
- **Check the Inspector tooltip** for field descriptions
- **Monitor Console messages** during initialization

Your model should now be properly assignable to the ObjectDetectionManager! 🎉
