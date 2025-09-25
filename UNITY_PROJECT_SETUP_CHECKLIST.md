# Unity Project Setup Checklist

## New Unity Project: SWITCH/
**Location**: `SWITCH/` directory  
**Unity Version**: 2022.3.25f1 LTS  
**Status**: Basic project created, needs configuration

## Required Configuration

### 1. Package Dependencies (Missing from new project)
The following packages need to be added to `SWITCH/Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.unity.inputsystem": "1.5.1",
    "com.unity.ads": "4.4.2", 
    "com.unity.analytics": "3.8.1",
    "com.unity.mobile.notifications": "2.2.0",
    "com.unity.2d.sprite": "1.0.0",
    "com.unity.2d.tilemap": "1.0.0"
  }
}
```

### 2. Project Settings Configuration
- **Company Name**: Set to "Whitt's End"
- **Product Name**: Already set to "SWITCH"
- **Bundle Identifier**: Set to "com.whitts-end.switch"
- **Package Name**: Set to "com.whitts-end.switch"

### 3. Build Settings
- **Target Platforms**: iOS and Android
- **Android Min SDK**: API 26 (Android 8.0)
- **iOS Target**: 13.0+
- **Orientation**: Portrait only
- **Architecture**: ARM64

### 4. Test Framework
- **Unity Test Framework**: Already included
- **Test Assembly**: Needs to be created
- **Smoke Tests**: Needs to be implemented

## Archived Configuration (from src/ directory)

### Package Configuration
The `src/Packages/manifest.json` contains the complete package configuration that was previously set up:

- Input System (1.5.1)
- Unity Ads (4.4.2)
- Unity Analytics (3.8.1)
- Mobile Notifications (2.2.0)
- 2D Sprite (1.0.0)
- 2D Tilemap (1.0.0)
- Test Framework (1.1.33)
- TextMeshPro (3.0.6)

### Project Settings
The `src/ProjectSettings/` directory contains:
- Company name: "Whitt's End"
- Product name: "SWITCH"
- Unity version: 2022.3.25f1
- All Unity project settings configured

## Next Steps

1. **Add Missing Packages**: Update `SWITCH/Packages/manifest.json` with required dependencies
2. **Configure Project Settings**: Set company name, bundle identifier, and build settings
3. **Set Up Test Framework**: Create test assembly and implement smoke tests
4. **Configure Build Settings**: Set up iOS and Android build targets
5. **Archive src/ Directory**: Move to archive once configuration is complete

## Files to Reference
- `src/Packages/manifest.json` - Complete package configuration
- `src/ProjectSettings/ProjectSettings.asset` - Project settings reference
- `docs/technical/UNITY_SETUP_GUIDE.md` - Detailed setup guide
