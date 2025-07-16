#!/usr/bin/env python3
"""
YOLOv8 to ONNX Converter for Unity Barracuda
This script converts YOLOv8 PyTorch models to ONNX format compatible with Unity Barracuda.
"""

import argparse
import os
import sys
from pathlib import Path

def check_dependencies():
    """Check if required packages are installed."""
    try:
        import torch
        import ultralytics
        from ultralytics import YOLO
        print(f"✓ PyTorch version: {torch.__version__}")
        print(f"✓ Ultralytics version: {ultralytics.__version__}")
        return True
    except ImportError as e:
        print(f"✗ Missing dependency: {e}")
        print("Please install required packages:")
        print("pip install torch ultralytics")
        return False

def convert_yolo_to_onnx(model_path, output_path=None, img_size=640, batch_size=1, dynamic=False, simplify=True):
    """
    Convert YOLOv8 model to ONNX format.
    
    Args:
        model_path (str): Path to the YOLOv8 .pt model file
        output_path (str): Output path for ONNX file
        img_size (int): Input image size (default: 640)
        batch_size (int): Batch size (default: 1)
        dynamic (bool): Enable dynamic input shapes
        simplify (bool): Simplify the ONNX model
    """
    try:
        from ultralytics import YOLO
        
        # Load the model
        print(f"Loading YOLOv8 model from: {model_path}")
        model = YOLO(model_path)
        
        # Set export parameters
        export_kwargs = {
            'format': 'onnx',
            'imgsz': img_size,
            'dynamic': dynamic,
            'simplify': simplify,
            'opset': 11  # Use opset 11 for better Barracuda compatibility
        }
        
        print(f"Converting to ONNX with parameters:")
        print(f"  Image size: {img_size}")
        print(f"  Dynamic shapes: {dynamic}")
        print(f"  Simplify: {simplify}")
        print(f"  ONNX opset: 11")
        
        # Export the model
        exported_files = model.export(**export_kwargs)
        
        # Get the exported ONNX file path
        if isinstance(exported_files, str):
            onnx_path = exported_files
        else:
            # Handle case where export returns a list
            onnx_path = str(exported_files)
        
        # Move to desired output path if specified
        if output_path and onnx_path != output_path:
            import shutil
            output_dir = os.path.dirname(output_path)
            if output_dir and not os.path.exists(output_dir):
                os.makedirs(output_dir)
            
            shutil.move(onnx_path, output_path)
            onnx_path = output_path
        
        print(f"✓ Conversion successful!")
        print(f"✓ ONNX model saved to: {onnx_path}")
        
        # Verify the exported model
        verify_onnx_model(onnx_path)
        
        return onnx_path
        
    except Exception as e:
        print(f"✗ Error during conversion: {str(e)}")
        return None

def verify_onnx_model(onnx_path):
    """Verify the exported ONNX model."""
    try:
        import onnx
        
        print("Verifying ONNX model...")
        model = onnx.load(onnx_path)
        onnx.checker.check_model(model)
        
        # Print model info
        print(f"✓ Model verification passed")
        print(f"  Input shape: {[d.dim_value for d in model.graph.input[0].type.tensor_type.shape.dim]}")
        print(f"  Output count: {len(model.graph.output)}")
        
        for i, output in enumerate(model.graph.output):
            output_shape = [d.dim_value for d in output.type.tensor_type.shape.dim]
            print(f"  Output {i} shape: {output_shape}")
            
    except ImportError:
        print("⚠ ONNX package not available for verification")
        print("Install with: pip install onnx")
    except Exception as e:
        print(f"⚠ Model verification failed: {e}")

def main():
    parser = argparse.ArgumentParser(
        description="Convert YOLOv8 model to ONNX format for Unity Barracuda",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  python convert_yolo.py yolov8n.pt
  python convert_yolo.py yolov8n.pt --output models/yolov8n.onnx
  python convert_yolo.py yolov8s.pt --img-size 416 --dynamic
        """
    )
    
    parser.add_argument('model_path', help='Path to YOLOv8 .pt model file')
    parser.add_argument('--output', '-o', help='Output path for ONNX file')
    parser.add_argument('--img-size', type=int, default=640, 
                       help='Input image size (default: 640)')
    parser.add_argument('--batch-size', type=int, default=1,
                       help='Batch size (default: 1)')
    parser.add_argument('--dynamic', action='store_true',
                       help='Enable dynamic input shapes')
    parser.add_argument('--no-simplify', action='store_true',
                       help='Disable ONNX model simplification')
    
    args = parser.parse_args()
    
    # Check if model file exists
    if not os.path.exists(args.model_path):
        print(f"✗ Model file not found: {args.model_path}")
        sys.exit(1)
    
    # Check dependencies
    if not check_dependencies():
        sys.exit(1)
    
    # Set output path if not specified
    if not args.output:
        model_name = Path(args.model_path).stem
        args.output = f"{model_name}.onnx"
    
    print(f"Converting {args.model_path} to {args.output}")
    print("-" * 50)
    
    # Convert the model
    result = convert_yolo_to_onnx(
        model_path=args.model_path,
        output_path=args.output,
        img_size=args.img_size,
        batch_size=args.batch_size,
        dynamic=args.dynamic,
        simplify=not args.no_simplify
    )
    
    if result:
        print("-" * 50)
        print("🎉 Conversion completed successfully!")
        print(f"Import {args.output} into Unity and assign it to ObjectDetectionManager.modelAsset")
    else:
        print("❌ Conversion failed!")
        sys.exit(1)

if __name__ == "__main__":
    main()
