# Unity YOLOv8 Object Detection System
<img src="https://raw.githubusercontent.com/ak22137/Animal-Detection-and-Counter/main/Gif.gif" width="500"/>
A complete Unity-based object detection system using YOLOv8 and Unity's Barracuda inference engine for real-time detection of humans, cows, sheep, and chickens in simulated environments.



 **All Scripts**: Core detection, UI, animal simulation, and camera controls  
 **ONNX Model**: YOLOv8n converted from PyTorch  
 **Python Environment**: Virtual environment configured with all dependencies  
 **Documentation**: Complete setup instructions and troubleshooting guide  

**Steps**: Import into Unity, install packages (Barracuda, TextMeshPro), and run the setup wizard!

## Features

- **Real-time Object Detection**: Uses YOLOv8 model for detecting people, cows, sheep, and chickens
- **Visual Feedback**: 2D bounding boxes with category labels and confidence scores
- **Object Tracking**: Each detected object gets a unique ID within its category
- **Statistics UI**: Real-time display of detection counts and FPS
- **Simulated Environment**: No physical camera required - works with Unity's virtual cameras
- **Animal Simulation**: Includes scripts for spawning and controlling simulated animals

## Setup Instructions

### 1. Prerequisites

Install the following Unity packages:
- **Unity Barracuda** (for ML inference)
- **TextMeshPro** (for UI text)


## Usage

### Camera Controls
- **WASD**: Move camera
- **Q/E**: Move up/down
- **Right Mouse + Drag**: Look around
- **Mouse Wheel**: Zoom

### Detection System
- The system automatically detects objects in the camera view
- Each detection shows:
  - Colored bounding box (different color per category)
  - Category name and object ID
  - Confidence percentage
- Statistics panel shows real-time counts and FPS

### Animal Spawner
- Automatically spawns animals in the scene
- Animals move randomly within boundaries
- Configurable spawn count, movement speed, and behavior

## Script Overview

### Core Scripts
- **ObjectDetectionManager.cs**: Main detection system controller
- **BoundingBoxUI.cs**: Manages visual bounding box representation
- **DetectionStatsUI.cs**: Handles statistics display

### Animal System
- **AnimalSpawner.cs**: Spawns and manages simulated animals
- **AnimalController.cs**: Controls individual animal movement and behavior

### Utilities
- **CameraController.cs**: First-person camera controls
- **ObjectDetectionSetup.cs**: Runtime setup utilities
- **YOLOModelConverter.cs**: Editor tool for model conversion

## Configuration

### Detection Categories
The system is configured to detect:
- **person** → Humans (green bounding box)
- **cow** → Cows (blue bounding box)
- **sheep** → Sheep (white bounding box)
- **chicken** → Chickens (yellow bounding box)


### Performance Tuning
- Adjust `confidenceThreshold` to filter low-confidence detections
- Modify `iouThreshold` for non-maximum suppression sensitivity
- Change detection frequency by modifying the Update loop

## Troubleshooting

### Common Issues

1. **Model not loading**
   - Ensure the ONNX model is properly converted from PyTorch
   - Check that Unity Barracuda package is installed
   - Verify model asset is assigned in ObjectDetectionManager

2. **No detections appearing**
   - Lower the confidence threshold
   - Check that target categories match YOLO class names
   - Ensure camera is pointing at objects in the scene

3. **UI not displaying**
   - Verify Canvas is set to Screen Space - Overlay
   - Check that BoundingBox prefab is properly configured
   - Ensure TextMeshPro package is installed

4. **Poor performance**
   - Reduce input resolution (inputWidth/inputHeight)
   - Limit number of animals in the scene
   - Use GPU compute shaders if available

### Model Requirements
- Input size: 640x640 (default YOLO input)
- Format: ONNX
- Classes: Must include COCO dataset classes (person, cow, sheep, bird)

## Extensions

### Adding New Categories
1. Add the category to `targetCategories` array
2. Update `GetCategoryColor()` in BoundingBoxUI
3. Add corresponding UI elements in DetectionStatsUI

### Custom Animal Models
1. Replace primitive prefabs with detailed 3D models
2. Ensure models have appropriate colliders
3. Add AnimalController script for movement

### Advanced Features
- Add object persistence across frames
- Implement trajectory tracking
- Add detection history and analytics
- Integrate with external data logging

## Dependencies

- Unity 2022.3 LTS or later
- Unity Barracuda 3.0.0+
- TextMeshPro 3.0.6+
- Python 3.8+ (for model conversion)
- ultralytics package (pip install ultralytics)

## Below is the working video of the system model

