#!/bin/bash

# SWITCH Mobile Build Script
# Builds the Unity project for Android and iOS

echo "=== SWITCH Mobile Build Script ==="
echo "Building for mobile platforms..."

# Check if Unity is available
UNITY_PATH="/Applications/Unity/Hub/Editor/2022.3.25f1/Unity.app/Contents/MacOS/Unity"
if [ ! -f "$UNITY_PATH" ]; then
    echo "Unity not found at $UNITY_PATH"
    echo "Please update the UNITY_PATH variable in this script"
    exit 1
fi

# Create build directory
BUILD_DIR="Builds"
mkdir -p "$BUILD_DIR"

echo "Build directory: $BUILD_DIR"

# Build for Android
echo "Building for Android..."
"$UNITY_PATH" \
    -batchmode \
    -quit \
    -projectPath "$(pwd)/SWITCH" \
    -buildTarget Android \
    -executeMethod BuildHelper.BuildAndroid \
    -logFile "$BUILD_DIR/android_build.log"

if [ $? -eq 0 ]; then
    echo "✅ Android build successful"
else
    echo "❌ Android build failed"
    echo "Check $BUILD_DIR/android_build.log for details"
fi

# Build for iOS
echo "Building for iOS..."
"$UNITY_PATH" \
    -batchmode \
    -quit \
    -projectPath "$(pwd)/SWITCH" \
    -buildTarget iOS \
    -executeMethod BuildHelper.BuildiOS \
    -logFile "$BUILD_DIR/ios_build.log"

if [ $? -eq 0 ]; then
    echo "✅ iOS build successful"
else
    echo "❌ iOS build failed"
    echo "Check $BUILD_DIR/ios_build.log for details"
fi

echo "=== Build Complete ==="
echo "Check the $BUILD_DIR directory for build artifacts"
echo "Logs are available in $BUILD_DIR/*_build.log"
