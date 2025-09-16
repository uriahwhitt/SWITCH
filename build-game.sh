#!/bin/bash

echo "Building SWITCH Game..."

# Check if Unity is available
if ! command -v unity &> /dev/null; then
    echo "Unity not found in PATH. Please ensure Unity is installed and accessible."
    exit 1
fi

# Set build parameters
BUILD_TARGET=${1:-"Android"}
BUILD_PATH="builds/${BUILD_TARGET}"
BUILD_NAME="SWITCH_${BUILD_TARGET}_$(date +%Y%m%d_%H%M%S)"

echo "Build Target: $BUILD_TARGET"
echo "Build Path: $BUILD_PATH"
echo "Build Name: $BUILD_NAME"

# Create build directory
mkdir -p "$BUILD_PATH"

# Run Unity build
unity -batchmode -quit -projectPath "src" -buildTarget "$BUILD_TARGET" -executeMethod BuildScript.BuildGame -buildPath "$BUILD_PATH/$BUILD_NAME"

if [ $? -eq 0 ]; then
    echo "Build completed successfully!"
    echo "Build location: $BUILD_PATH/$BUILD_NAME"
else
    echo "Build failed!"
    exit 1
fi
