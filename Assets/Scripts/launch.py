#!/usr/bin/env python3
"""
Unity Object Detection System - Quick Launcher
Opens the project setup checklist and provides easy access to key files
"""

import os
import sys
import webbrowser
from pathlib import Path

def main():
    print("🎯 Unity Object Detection System - Quick Launcher")
    print("=" * 50)
    print()
    
    base_dir = Path(__file__).parent
    
    print("📁 Project Location:")
    print(f"   {base_dir}")
    print()
    
    print("📋 Key Files:")
    print(f"✅ Setup Guide: {base_dir / 'UNITY_SETUP_CHECKLIST.md'}")
    print(f"✅ Documentation: {base_dir / 'README.md'}")
    print(f"✅ Status Report: {base_dir / 'FINAL_STATUS_REPORT.md'}")
    print(f"✅ ONNX Model: {base_dir / 'yolov8n.onnx'}")
    print()
    
    print("🚀 Next Steps:")
    print("1. Open Unity Hub")
    print("2. Create new 3D project at: d:\\Unity\\Projects\\Object Detection\\")
    print("3. Install packages: Barracuda, TextMeshPro")
    print("4. Import scripts to Assets/Scripts/")
    print("5. Import yolov8n.onnx to Assets/Models/")
    print("6. Run Tools → Object Detection → Setup Wizard")
    print("7. Press Play and start detecting!")
    print()
    
    # Ask if user wants to open documentation
    try:
        response = input("📖 Open setup checklist in browser? (y/n): ").lower().strip()
        if response in ['y', 'yes']:
            checklist_path = base_dir / 'UNITY_SETUP_CHECKLIST.md'
            if checklist_path.exists():
                # Convert to file:// URL for browser
                file_url = f"file:///{checklist_path.as_posix()}"
                webbrowser.open(file_url)
                print("✅ Opening setup checklist in browser...")
            else:
                print("❌ Checklist file not found")
    except KeyboardInterrupt:
        print("\n👋 Goodbye!")
    except:
        pass
    
    print("\n🎉 Happy detecting! Your system is ready to go!")

if __name__ == "__main__":
    main()
