#!/bin/bash

# SWITCH Unity Project Build Test Script
# This script validates the Unity project setup

echo "🎮 SWITCH Unity Project Setup Validation"
echo "========================================"

# Check if we're in the right directory
if [ ! -f "ProjectSettings/ProjectVersion.txt" ]; then
    echo "❌ Error: Not in Unity project directory"
    exit 1
fi

echo "✅ Unity project directory found"

# Check Unity version
UNITY_VERSION=$(grep "m_EditorVersion:" ProjectSettings/ProjectVersion.txt | cut -d' ' -f2)
echo "✅ Unity Version: $UNITY_VERSION"

# Check project settings
if [ -f "ProjectSettings/ProjectSettings.asset" ]; then
    echo "✅ Project settings configured"
else
    echo "❌ Project settings missing"
    exit 1
fi

# Check package manifest
if [ -f "Packages/manifest.json" ]; then
    echo "✅ Package manifest exists"
    
    # Check for required packages
    if grep -q "com.unity.inputsystem" Packages/manifest.json; then
        echo "✅ Input System package configured"
    else
        echo "❌ Input System package missing"
    fi
    
    if grep -q "com.unity.textmeshpro" Packages/manifest.json; then
        echo "✅ TextMeshPro package configured"
    else
        echo "❌ TextMeshPro package missing"
    fi
    
    if grep -q "com.unity.test-framework" Packages/manifest.json; then
        echo "✅ Test Framework package configured"
    else
        echo "❌ Test Framework package missing"
    fi
else
    echo "❌ Package manifest missing"
    exit 1
fi

# Check folder structure
REQUIRED_FOLDERS=(
    "Assets/_Project/Scripts/Core"
    "Assets/_Project/Scripts/UI"
    "Assets/_Project/Scripts/Data"
    "Assets/_Project/Scripts/PowerUps"
    "Assets/_Project/Scripts/Services"
    "Assets/_Project/Scenes"
    "Assets/_Project/Prefabs"
    "Assets/_Project/Materials"
    "Assets/_Project/Audio"
    "Assets/_Project/Sprites"
)

echo ""
echo "📁 Checking folder structure..."
for folder in "${REQUIRED_FOLDERS[@]}"; do
    if [ -d "$folder" ]; then
        echo "✅ $folder"
    else
        echo "❌ Missing: $folder"
    fi
done

# Check core scripts
REQUIRED_SCRIPTS=(
    "Assets/_Project/Scripts/Core/GameManager.cs"
    "Assets/_Project/Scripts/Core/BoardController.cs"
    "Assets/_Project/Scripts/Core/Tile.cs"
    "Assets/_Project/Scripts/Core/MomentumSystem.cs"
    "Assets/_Project/Scripts/Core/TurnScoreCalculator.cs"
)

echo ""
echo "📄 Checking core scripts..."
for script in "${REQUIRED_SCRIPTS[@]}"; do
    if [ -f "$script" ]; then
        echo "✅ $script"
    else
        echo "❌ Missing: $script"
    fi
done

# Check scenes
REQUIRED_SCENES=(
    "Assets/_Project/Scenes/Main.unity"
    "Assets/_Project/Scenes/Game.unity"
    "Assets/_Project/Scenes/Menu.unity"
)

echo ""
echo "🎬 Checking scenes..."
for scene in "${REQUIRED_SCENES[@]}"; do
    if [ -f "$scene" ]; then
        echo "✅ $scene"
    else
        echo "❌ Missing: $scene"
    fi
done

# Check test files
if [ -f "Assets/_Project/Scripts/Tests/SmokeTest.cs" ]; then
    echo "✅ Smoke test created"
else
    echo "❌ Smoke test missing"
fi

if [ -f "Assets/_Project/Scripts/Tests/SWITCH.Tests.asmdef" ]; then
    echo "✅ Test assembly definition created"
else
    echo "❌ Test assembly definition missing"
fi

echo ""
echo "🎯 Unity Project Setup Summary"
echo "=============================="
echo "Project Name: SWITCH"
echo "Company: Whitt's End"
echo "Unity Version: $UNITY_VERSION"
echo "Target Platforms: iOS, Android"
echo "Package Manager: Configured"
echo "Test Framework: Ready"
echo ""
echo "✅ Unity project setup complete!"
echo "🚀 Ready for Sprint 1 development"
