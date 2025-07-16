# Unity Object Detection Project Setup Script
# Run this script to initialize the Unity project and prepare for testing

Write-Host "🚀 Unity Object Detection Project Setup" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green

# Check if Unity Hub is installed
$unityHubPath = Get-Command "Unity Hub.exe" -ErrorAction SilentlyContinue
if (-not $unityHubPath) {
    Write-Host "❌ Unity Hub not found in PATH" -ForegroundColor Red
    Write-Host "Please install Unity Hub and add it to your PATH" -ForegroundColor Yellow
    exit 1
}

# Project directory
$projectDir = "d:\Unity\Projects\Object Detection"
Write-Host "📁 Project Directory: $projectDir" -ForegroundColor Cyan

# Check if this looks like a Unity project
$assetsDir = Join-Path $projectDir "Assets"
if (Test-Path $assetsDir) {
    Write-Host "✅ Assets directory found" -ForegroundColor Green
} else {
    Write-Host "❌ Assets directory not found - creating Unity project structure" -ForegroundColor Red
    # Create basic Unity project structure
    New-Item -ItemType Directory -Path $assetsDir -Force
    New-Item -ItemType Directory -Path (Join-Path $projectDir "ProjectSettings") -Force
    New-Item -ItemType Directory -Path (Join-Path $projectDir "Library") -Force
}

# Check for critical scripts
$criticalScripts = @(
    "ObjectDetectionManager.cs",
    "BoundingBoxUI.cs", 
    "DetectionStatsUI.cs",
    "QuickSceneSetup.cs",
    "TestObjectDetection.cs"
)

Write-Host "`n🔍 Verifying Critical Scripts:" -ForegroundColor Cyan
$scriptsDir = Join-Path $assetsDir "Scripts"
$allScriptsFound = $true

foreach ($script in $criticalScripts) {
    $scriptPath = Join-Path $scriptsDir $script
    if (Test-Path $scriptPath) {
        Write-Host "✅ $script" -ForegroundColor Green
    } else {
        Write-Host "❌ $script - MISSING" -ForegroundColor Red
        $allScriptsFound = $false
    }
}

# Check for ONNX models
Write-Host "`n🧠 Verifying ONNX Models:" -ForegroundColor Cyan
$modelFiles = Get-ChildItem -Path $scriptsDir -Filter "*.onnx" -ErrorAction SilentlyContinue
if ($modelFiles.Count -gt 0) {
    foreach ($model in $modelFiles) {
        $sizeKB = [math]::Round($model.Length / 1KB, 1)
        Write-Host "✅ $($model.Name) ($sizeKB KB)" -ForegroundColor Green
    }
} else {
    Write-Host "❌ No ONNX models found" -ForegroundColor Red
    $allScriptsFound = $false
}

if ($allScriptsFound) {
    Write-Host "`n🎯 READY FOR UNITY TESTING!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor Yellow
    Write-Host "1. Open Unity Hub" -ForegroundColor White
    Write-Host "2. Click 'Open' and select: $projectDir" -ForegroundColor White
    Write-Host "3. Install required packages:" -ForegroundColor White
    Write-Host "   - ML Agents (Barracuda)" -ForegroundColor Cyan
    Write-Host "   - TextMeshPro" -ForegroundColor Cyan
    Write-Host "4. Create a new scene and attach QuickSceneSetup.cs" -ForegroundColor White
    Write-Host "5. Click 'Setup Detection Scene' to auto-configure" -ForegroundColor White
    Write-Host ""
    Write-Host "📖 See COMPREHENSIVE_TESTING_GUIDE.md for detailed instructions" -ForegroundColor Cyan
} else {
    Write-Host "`n❌ SETUP INCOMPLETE" -ForegroundColor Red
    Write-Host "Please ensure all critical scripts and models are present" -ForegroundColor Yellow
}

Write-Host "`nPress any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
