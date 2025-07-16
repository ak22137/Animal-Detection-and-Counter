using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

public class YOLOModelConverter : EditorWindow
{
    private string pythonPath = "python";
    private string modelPath = "";
    private string outputPath = "";
    private bool useVirtualEnv = true;
    private string virtualEnvPath = "";
    
    [MenuItem("Tools/YOLO Model Converter")]
    public static void ShowWindow()
    {
        GetWindow<YOLOModelConverter>("YOLO Model Converter");
    }
    
    void OnGUI()
    {
        EditorGUILayout.LabelField("YOLO Model Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("This tool converts YOLOv8 PyTorch models to ONNX format for Unity Barracuda", EditorStyles.helpBox);
        EditorGUILayout.Space();
        
        // Python settings
        EditorGUILayout.LabelField("Python Settings", EditorStyles.boldLabel);
        useVirtualEnv = EditorGUILayout.Toggle("Use Virtual Environment", useVirtualEnv);
        
        if (useVirtualEnv)
        {
            EditorGUILayout.BeginHorizontal();
            virtualEnvPath = EditorGUILayout.TextField("Virtual Env Path", virtualEnvPath);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Virtual Environment", "", "");
                if (!string.IsNullOrEmpty(path))
                {
                    virtualEnvPath = path;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            pythonPath = EditorGUILayout.TextField("Python Executable", pythonPath);
        }
        
        EditorGUILayout.Space();
        
        // Model conversion settings
        EditorGUILayout.LabelField("Model Conversion", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        modelPath = EditorGUILayout.TextField("YOLOv8 Model (.pt)", modelPath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFilePanel("Select YOLO Model", "", "pt");
            if (!string.IsNullOrEmpty(path))
            {
                modelPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        outputPath = EditorGUILayout.TextField("Output ONNX Path", outputPath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string path = EditorUtility.SaveFilePanel("Save ONNX Model", "", "yolov8n", "onnx");
            if (!string.IsNullOrEmpty(path))
            {
                outputPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Auto-fill paths if empty
        if (string.IsNullOrEmpty(modelPath))
        {
            string autoPath = Path.Combine(Application.dataPath, "Scripts", "yolov8n.pt");
            if (File.Exists(autoPath))
            {
                modelPath = autoPath;
            }
        }
        
        if (string.IsNullOrEmpty(outputPath))
        {
            outputPath = Path.Combine(Application.dataPath, "Models", "yolov8n.onnx");
        }
        
        if (string.IsNullOrEmpty(virtualEnvPath))
        {
            string autoVenvPath = Path.Combine(Application.dataPath, "Scripts", "object_detection_env");
            if (Directory.Exists(autoVenvPath))
            {
                virtualEnvPath = autoVenvPath;
            }
        }
        
        EditorGUILayout.Space();
        
        // Convert button
        GUI.enabled = !string.IsNullOrEmpty(modelPath) && !string.IsNullOrEmpty(outputPath);
        if (GUILayout.Button("Convert Model to ONNX", GUILayout.Height(30)))
        {
            ConvertModel();
        }
        GUI.enabled = true;
        
        EditorGUILayout.Space();
        
        // Instructions
        EditorGUILayout.LabelField("Instructions:", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(
            "1. Ensure you have ultralytics installed: pip install ultralytics\n" +
            "2. Select your YOLOv8 .pt model file\n" +
            "3. Choose output location for ONNX file\n" +
            "4. Click Convert Model to ONNX\n" +
            "5. Import the generated ONNX file into Unity\n" +
            "6. Assign it to ObjectDetectionManager.modelAsset",
            GUILayout.Height(100));
    }
    
    void ConvertModel()
    {
        if (!File.Exists(modelPath))
        {
            EditorUtility.DisplayDialog("Error", "Model file does not exist!", "OK");
            return;
        }
        
        // Create output directory if it doesn't exist
        string outputDir = Path.GetDirectoryName(outputPath);
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }
        
        // Create Python script for conversion
        string conversionScript = CreateConversionScript();
        string scriptPath = Path.Combine(Application.temporaryCachePath, "convert_yolo.py");
        File.WriteAllText(scriptPath, conversionScript);
        
        // Prepare command
        string command;
        string arguments;
        
        if (useVirtualEnv && !string.IsNullOrEmpty(virtualEnvPath))
        {
            // Use virtual environment
            string activateScript = Path.Combine(virtualEnvPath, "Scripts", "activate.bat");
            command = "cmd.exe";
            arguments = $"/c \"{activateScript} && python \"{scriptPath}\" \"{modelPath}\" \"{outputPath}\"\"";
        }
        else
        {
            command = pythonPath;
            arguments = $"\"{scriptPath}\" \"{modelPath}\" \"{outputPath}\"";
        }
        
        // Execute conversion
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            
            UnityEngine.Debug.Log($"Executing: {command} {arguments}");
            
            Process process = Process.Start(startInfo);
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            
            if (process.ExitCode == 0)
            {
                UnityEngine.Debug.Log("Model conversion successful!");
                UnityEngine.Debug.Log(output);
                
                // Refresh assets to show the new ONNX file
                AssetDatabase.Refresh();
                
                EditorUtility.DisplayDialog("Success", "Model converted successfully!\nImport the ONNX file into Unity and assign it to ObjectDetectionManager.", "OK");
            }
            else
            {
                UnityEngine.Debug.LogError("Model conversion failed!");
                UnityEngine.Debug.LogError(error);
                EditorUtility.DisplayDialog("Error", $"Conversion failed:\n{error}", "OK");
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Exception during conversion: {e.Message}");
            EditorUtility.DisplayDialog("Error", $"Exception during conversion:\n{e.Message}", "OK");
        }
        finally
        {
            // Clean up temporary script
            if (File.Exists(scriptPath))
            {
                File.Delete(scriptPath);
            }
        }
    }
    
    string CreateConversionScript()
    {
        return @"
import sys
import os
from ultralytics import YOLO

def convert_yolo_to_onnx(model_path, output_path):
    try:
        print(f'Loading YOLO model from: {model_path}')
        model = YOLO(model_path)
        
        print(f'Converting to ONNX format...')
        model.export(format='onnx', dynamic=False, simplify=True)
        
        # The export function creates the ONNX file with the same name as input
        default_onnx_path = model_path.replace('.pt', '.onnx')
        
        if os.path.exists(default_onnx_path):
            if default_onnx_path != output_path:
                print(f'Moving ONNX file to: {output_path}')
                os.rename(default_onnx_path, output_path)
            print(f'Conversion successful! ONNX model saved to: {output_path}')
        else:
            print('Error: ONNX file was not created')
            sys.exit(1)
            
    except Exception as e:
        print(f'Error during conversion: {str(e)}')
        sys.exit(1)

if __name__ == '__main__':
    if len(sys.argv) != 3:
        print('Usage: python convert_yolo.py <input_model.pt> <output_model.onnx>')
        sys.exit(1)
    
    model_path = sys.argv[1]
    output_path = sys.argv[2]
    
    convert_yolo_to_onnx(model_path, output_path)
";
    }
}
