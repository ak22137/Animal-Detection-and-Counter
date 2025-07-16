"""
YOLO to Barracuda-Compatible ONNX Converter
Converts YOLOv8 model to ONNX format compatible with Unity Barracuda
"""

import sys
import torch
from ultralytics import YOLO
import onnx
from pathlib import Path

def convert_yolo_for_barracuda(model_path="yolov8n.pt", output_path="yolov8n_barracuda.onnx"):
    """
    Convert YOLOv8 model to Barracuda-compatible ONNX format
    Uses opset_version=9 which is supported by Unity Barracuda
    """
    try:
        print(f"Loading YOLOv8 model: {model_path}")
        
        # Load the model
        model = YOLO(model_path)
        
        # Export to ONNX with Barracuda-compatible settings
        print("Converting to ONNX format (Barracuda compatible)...")        # Export with specific settings for Barracuda compatibility
        # Note: opset=9 is REQUIRED for Unity Barracuda compatibility
        model.export(
            format="onnx",
            imgsz=640,
            opset=9,   # Unity Barracuda only supports up to opset 9
            simplify=False,  # Don't simplify to preserve tensor structure
            dynamic=False,
            half=False,  # Use full precision for better compatibility
            batch=1  # Ensure batch size is 1
        )
        
        # The model exports to the same directory with .onnx extension
        default_output = str(Path(model_path).with_suffix('.onnx'))
        
        # Rename to our desired output name if different
        if output_path != default_output:
            import shutil
            shutil.move(default_output, output_path)
        
        print(f"✅ Model successfully converted to: {output_path}")
        
        # Verify the ONNX model
        try:
            onnx_model = onnx.load(output_path)
            onnx.checker.check_model(onnx_model)
            print("✅ ONNX model validation passed")
            
            # Print model info
            print(f"📊 Model Info:")
            print(f"   - Input shape: {onnx_model.graph.input[0].type.tensor_type.shape}")
            print(f"   - Output shape: {onnx_model.graph.output[0].type.tensor_type.shape}")
            print(f"   - Opset version: {onnx_model.opset_import[0].version}")
            
        except Exception as e:
            print(f"⚠️  ONNX validation warning: {e}")
        
        return True
        
    except Exception as e:
        print(f"❌ Error during conversion: {e}")
        return False

def main():
    print("🎯 YOLOv8 to Barracuda-Compatible ONNX Converter")
    print("=" * 50)
    
    # Check if model file exists
    model_path = "yolov8n.pt"
    if not Path(model_path).exists():
        print(f"❌ Model file not found: {model_path}")
        print("Please ensure yolov8n.pt is in the current directory")
        return False
    
    # Convert the model
    output_path = "yolov8n_barracuda.onnx"
    success = convert_yolo_for_barracuda(model_path, output_path)
    
    if success:
        print("\n🎉 Conversion completed successfully!")
        print("\n📋 Next Steps:")
        print("1. Copy the ONNX file to Unity's StreamingAssets folder")
        print("2. Update ObjectDetectionManager fallbackModelPath to 'yolov8n_barracuda.onnx'")
        print("3. Test in Unity - should work without format errors")
        print("\n💡 This ONNX model uses opset 9 for Unity Barracuda compatibility")
    else:
        print("\n❌ Conversion failed. Check error messages above.")
    
    return success

if __name__ == "__main__":
    main()
