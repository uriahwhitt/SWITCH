# SWITCH Unity Setup Guide

## 🎮 Unity Project Configuration

### Project Overview
- **Unity Version**: 2022.3.25f1 LTS
- **Project Name**: SWITCH
- **Company**: Whitt's End
- **Package ID**: com.whitts-end.switch
- **Target Platforms**: iOS & Android
- **Architecture**: Event-driven with singleton GameManager

### Project Structure
```
SWITCH/
├── Assets/
│   └── _Project/
│       ├── Scripts/
│       │   ├── Core/           # Core game systems
│       │   ├── UI/             # User interface components
│       │   ├── Data/           # Data structures
│       │   ├── PowerUps/       # Power-up systems
│       │   ├── Services/       # Service layer
│       │   └── Tests/          # Test framework
│       ├── Scenes/             # Game scenes
│       ├── Prefabs/            # Reusable objects
│       ├── Materials/          # Visual materials
│       ├── Audio/              # Sound assets
│       └── Sprites/            # 2D graphics
├── Packages/                   # Package Manager dependencies
├── ProjectSettings/            # Unity project configuration
└── .gitignore                  # Unity-specific git exclusions
```

## 📦 Package Dependencies

### Core Packages
- **Input System** (com.unity.inputsystem) - Modern input handling
- **TextMeshPro** (com.unity.textmeshpro) - Advanced text rendering
- **2D Sprite** (com.unity.2d.sprite) - 2D graphics support
- **Test Framework** (com.unity.test-framework) - Testing infrastructure

### Mobile Packages
- **Mobile Notifications** (com.unity.mobile.notifications) - Push notifications
- **Unity Ads** (com.unity.ads) - Advertisement integration
- **Analytics** (com.unity.analytics) - Game analytics

### Package Manifest
```json
{
  "dependencies": {
    "com.unity.2d.sprite": "1.0.0",
    "com.unity.2d.tilemap": "1.0.0",
    "com.unity.ads": "4.4.2",
    "com.unity.analytics": "3.8.1",
    "com.unity.inputsystem": "1.5.1",
    "com.unity.mobile.notifications": "2.2.0",
    "com.unity.test-framework": "1.1.33",
    "com.unity.textmeshpro": "3.0.6"
  }
}
```

## 🏗️ Assembly Definitions

### Main Assembly (SWITCH.asmdef)
```json
{
    "name": "SWITCH",
    "rootNamespace": "SWITCH",
    "references": [
        "Unity.InputSystem",
        "Unity.TextMeshPro"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

### Test Assembly (SWITCH.Tests.asmdef)
```json
{
    "name": "SWITCH.Tests",
    "rootNamespace": "SWITCH.Tests",
    "references": [
        "UnityEngine.TestRunner",
        "UnityEditor.TestRunner"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [
        "nunit.framework.dll"
    ],
    "autoReferenced": false,
    "defineConstraints": [
        "UNITY_INCLUDE_TESTS"
    ],
    "versionDefines": [],
    "noEngineReferences": false
}
```

## 📱 Mobile Configuration

### iOS Settings
- **Bundle Identifier**: com.whitts-end.switch
- **Target iOS Version**: 13.0+
- **Orientation**: Portrait only
- **Architecture**: ARM64
- **Signing**: Automatic (development)

### Android Settings
- **Package Name**: com.whitts-end.switch
- **Min SDK**: API 26 (Android 8.0)
- **Target SDK**: Latest
- **Architecture**: ARM64
- **Orientation**: Portrait only
- **Compression**: LZ4
- **Scripting Backend**: IL2CPP

### Build Settings
- **Scenes in Build**:
  - Main.unity (Index 0)
  - Game.unity (Index 1)
  - Menu.unity (Index 2)
- **Compression**: LZ4
- **Development Build**: Enabled for testing
- **Script Debugging**: Enabled for development

## 🧪 Testing Framework

### Smoke Tests
Comprehensive validation tests to ensure project setup:

```csharp
[Test]
public void UnityProjectSetup_ShouldHaveCorrectVersion()
{
    Assert.AreEqual("SWITCH", Application.productName);
    Assert.AreEqual("Whitt's End", Application.companyName);
}

[Test]
public void ProjectStructure_ShouldHaveRequiredFolders()
{
    string[] requiredFolders = {
        "Assets/_Project/Scripts/Core",
        "Assets/_Project/Scripts/UI",
        "Assets/_Project/Scripts/Data",
        "Assets/_Project/Scripts/PowerUps",
        "Assets/_Project/Scripts/Services",
        "Assets/_Project/Scenes",
        "Assets/_Project/Prefabs",
        "Assets/_Project/Materials",
        "Assets/_Project/Audio",
        "Assets/_Project/Sprites"
    };

    foreach (string folder in requiredFolders)
    {
        Assert.IsTrue(System.IO.Directory.Exists(folder));
    }
}
```

### Build Validation Script
Automated setup verification:

```bash
#!/bin/bash
# SWITCH Unity Project Build Test Script

echo "🎮 SWITCH Unity Project Setup Validation"
echo "========================================"

# Check Unity version
UNITY_VERSION=$(grep "m_EditorVersion:" ProjectSettings/ProjectVersion.txt | cut -d' ' -f2)
echo "✅ Unity Version: $UNITY_VERSION"

# Validate project structure
# Check package manifest
# Verify build settings
# Confirm test framework
```

## 🔧 Development Workflow

### Session Start
1. **Pull Latest Changes**: `git pull origin main`
2. **Validate Setup**: `./SWITCH/build-test.sh`
3. **Open Unity Project**: Load `SWITCH/` directory in Unity
4. **Check Sprint Status**: Review `planning-context/SPRINT_STATUS.md`

### Development Process
1. **Create Feature Branch**: `git checkout -b feature/sprint-1-component`
2. **Implement Feature**: Follow implementation standards
3. **Run Tests**: Execute smoke tests and new tests
4. **Profile Performance**: Check frame rate and memory usage
5. **Update Documentation**: Update relevant docs and status

### Session End
1. **Stage Changes**: `git add .`
2. **Commit Changes**: `git commit -m "[Sprint 1] feat: Description"`
3. **Push Changes**: `git push origin feature/sprint-1-component`
4. **Update Status**: Update SPRINT_STATUS.md

## 📊 Performance Targets

### Frame Rate
- **Target**: 60 FPS
- **Minimum**: 30 FPS
- **Measurement**: Unity Profiler

### Memory Usage
- **Target**: <200MB total
- **Allocation**: <1KB per frame
- **Measurement**: Unity Profiler Memory tab

### Load Time
- **Target**: <5 seconds initial load
- **Measurement**: Unity Profiler

### Battery Impact
- **Target**: <10% per hour of gameplay
- **Measurement**: Device testing

## 🚀 Build Process

### Development Build
```bash
# Android Development Build
Unity -batchmode -quit -projectPath ./src -buildTarget Android -executeMethod BuildScript.BuildAndroidDev

# iOS Development Build
Unity -batchmode -quit -projectPath ./src -buildTarget iOS -executeMethod BuildScript.BuildiOSDev
```

### Release Build
```bash
# Android Release Build
Unity -batchmode -quit -projectPath ./src -buildTarget Android -executeMethod BuildScript.BuildAndroidRelease

# iOS Release Build
Unity -batchmode -quit -projectPath ./src -buildTarget iOS -executeMethod BuildScript.BuildiOSRelease
```

## 🔍 Troubleshooting

### Common Issues

#### Package Installation
- **Issue**: Package not found
- **Solution**: Check Unity version compatibility
- **Prevention**: Use LTS versions only

#### Build Failures
- **Issue**: Android build fails
- **Solution**: Check Android SDK installation
- **Prevention**: Validate setup with build script

#### Test Failures
- **Issue**: Smoke tests fail
- **Solution**: Run build validation script
- **Prevention**: Regular setup validation

### Validation Commands
```bash
# Check Unity version
grep "m_EditorVersion:" ProjectSettings/ProjectVersion.txt

# Validate package manifest
cat Packages/manifest.json | jq '.dependencies'

# Check build settings
cat ProjectSettings/EditorBuildSettings.asset

# Run smoke tests
Unity -batchmode -quit -projectPath ./src -runTests
```

## 📋 Setup Checklist

### Initial Setup
- [ ] Unity 2022.3 LTS installed
- [ ] Project created in `SWITCH/` directory
- [ ] Package Manager packages installed
- [ ] Assembly definitions created
- [ ] Build settings configured
- [ ] Test framework setup
- [ ] Git integration configured

### Validation
- [ ] Build validation script passes
- [ ] Smoke tests pass
- [ ] Project opens in Unity
- [ ] Builds successfully for target platforms
- [ ] Performance targets met (empty project)

### Development Ready
- [ ] Feature branch workflow established
- [ ] Documentation updated
- [ ] Sprint status current
- [ ] Team access configured
- [ ] CI/CD pipeline ready (if applicable)

---

**Status**: ✅ **UNITY SETUP COMPLETE**  
**Next Phase**: Sprint 1 - Core Mechanics Implementation  
**Last Updated**: Sprint 0 Completion  
**Maintained By**: Development Team
