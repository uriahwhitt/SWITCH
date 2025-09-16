#!/bin/bash

echo "Running SWITCH Tests..."

# Check if Unity is available
if ! command -v unity &> /dev/null; then
    echo "Unity not found in PATH. Please ensure Unity is installed and accessible."
    exit 1
fi

# Set test parameters
TEST_MODE=${1:-"All"}
TEST_RESULTS_PATH="tests/results"

echo "Test Mode: $TEST_MODE"
echo "Results Path: $TEST_RESULTS_PATH"

# Create results directory
mkdir -p "$TEST_RESULTS_PATH"

# Run Unity tests
case $TEST_MODE in
    "EditMode")
        echo "Running Edit Mode tests..."
        unity -batchmode -quit -projectPath "src" -runTests -testPlatform EditMode -testResults "$TEST_RESULTS_PATH/EditMode_Results.xml"
        ;;
    "PlayMode")
        echo "Running Play Mode tests..."
        unity -batchmode -quit -projectPath "src" -runTests -testPlatform PlayMode -testResults "$TEST_RESULTS_PATH/PlayMode_Results.xml"
        ;;
    "All")
        echo "Running all tests..."
        unity -batchmode -quit -projectPath "src" -runTests -testPlatform EditMode -testResults "$TEST_RESULTS_PATH/EditMode_Results.xml"
        unity -batchmode -quit -projectPath "src" -runTests -testPlatform PlayMode -testResults "$TEST_RESULTS_PATH/PlayMode_Results.xml"
        ;;
    *)
        echo "Invalid test mode. Use: EditMode, PlayMode, or All"
        exit 1
        ;;
esac

if [ $? -eq 0 ]; then
    echo "Tests completed successfully!"
    echo "Results location: $TEST_RESULTS_PATH"
else
    echo "Tests failed!"
    exit 1
fi
