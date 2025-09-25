# SWITCH Game - Unity Project Setup

## Project Overview
This is the Unity project for SWITCH, a mobile puzzle game built with Unity 2022.3.25f1 LTS.

## Project Structure
```
Assets/_Project/
├── Scripts/
│   ├── Core/           # Core game systems
│   │   ├── GameManager.cs      # Main game state manager
│   │   ├── TouchInputHandler.cs # Touch/mouse input handling
│   │   ├── SmokeTest.cs        # Performance testing
│   │   ├── TestSceneSetup.cs   # Scene setup automation
│   │   └── BuildHelper.cs      # Build configuration helper
│   ├── Mechanics/      # Game mechanics
│   │   ├── Tile.cs            # Individual tile behavior
│   │   └── Grid.cs            # 8x8 grid management
│   ├── UI/             # User interface (future)
│   └── Data/           # Data management (future)
├── Prefabs/            # Game object prefabs
├── Materials/          # Materials and shaders
├── Sprites/            # 2D sprites and textures
└── Scenes/             # Game scenes
    └── TestScene.unity # Main test scene
```

## Core Components

### GameManager
- **Purpose**: Central game state management
- **Features**: 
  - Singleton pattern
  - State machine (Menu, Playing, Paused, GameOver)
  - Event system for game state changes
  - Application focus/pause handling

### Grid System
- **Purpose**: Manages the 8x8 game grid
- **Features**:
  - Dynamic tile creation
  - Tile selection and deselection
  - Color management
  - Grid validation and bounds checking

### Tile System
- **Purpose**: Individual tile behavior and visual representation
- **Features**:
  - Multiple tile colors (Red, Blue, Green, Yellow, Purple, Orange)
  - Selection visual feedback
  - Animation support
  - Event system for interactions

### TouchInputHandler
- **Purpose**: Handles touch and mouse input
- **Features**:
  - Unity Input System integration
  - Touch and mouse support
  - Tap and swipe gesture detection
  - Screen to world position conversion

### SmokeTest
- **Purpose**: Performance validation and testing
- **Features**:
  - FPS monitoring
  - Memory usage tracking
  - Basic Unity functionality testing
  - Performance threshold validation

## Setup Instructions

### 1. Package Dependencies
The following packages are required and should be automatically installed:
- `com.unity.inputsystem` (1.5.1) - Touch input handling
- `com.unity.ads` (4.4.2) - Monetization
- `com.unity.analytics` (3.8.1) - Analytics
- `com.unity.mobile.notifications` (2.2.0) - Push notifications
- `com.unity.2d.sprite` (1.0.0) - 2D sprite support
- `com.unity.2d.tilemap` (1.0.0) - 2D tilemap support

### 2. Project Settings
- **Company Name**: Whitt's End
- **Product Name**: SWITCH
- **Bundle Identifier**: com.whittsend.switch
- **Orientation**: Portrait only
- **Target Frame Rate**: 60 FPS

### 3. Scene Setup
1. Open `TestScene.unity`
2. The scene will automatically set up all required components
3. Use the `TestSceneSetup` component to validate the setup

## Testing

### Smoke Tests
Run the smoke tests to validate basic functionality:
1. Play the scene
2. Check the console for test results
3. Verify FPS stays above 30
4. Verify memory usage stays below 100MB

### Manual Testing
- **Tile Selection**: Click/tap tiles to select them
- **Visual Feedback**: Selected tiles should scale up
- **Performance**: Monitor FPS and memory usage
- **Input**: Test both touch (mobile) and mouse (editor) input

## Build Configuration

### Mobile Build Settings
- **Android**: API 26+ (Android 8.0+)
- **iOS**: iOS 13.0+
- **Architecture**: ARM64
- **Graphics**: Optimized for mobile 2D

### Build Validation
Use the `BuildHelper` component to validate build settings:
1. Check company name and bundle ID
2. Verify orientation settings
3. Validate required packages
4. Check performance requirements

## Performance Targets
- **FPS**: Minimum 30 FPS, Target 60 FPS
- **Memory**: Maximum 100MB for basic scene
- **Build Size**: Maximum 20MB for prototype
- **Touch Latency**: Maximum 100ms

## Development Guidelines

### Code Standards
- Use C# naming conventions
- Implement proper error handling
- Add comprehensive logging
- Use events for loose coupling

### Performance Guidelines
- Use object pooling for frequently created/destroyed objects
- Minimize garbage collection
- Optimize for mobile hardware
- Profile regularly

### Testing Guidelines
- Test on actual mobile devices
- Validate performance on low-end devices
- Test touch input responsiveness
- Verify memory usage patterns

## Troubleshooting

### Common Issues
1. **Scripts not compiling**: Check for missing packages
2. **Touch input not working**: Verify Input System package
3. **Low FPS**: Check for performance bottlenecks
4. **High memory usage**: Look for memory leaks

### Debug Tools
- Use `SmokeTest` for performance validation
- Use `BuildHelper` for build configuration
- Use `TestSceneSetup` for scene validation
- Check Unity Console for errors and warnings

## Next Steps
1. Implement game mechanics (matching, scoring)
2. Add UI system
3. Create tile sprites and animations
4. Implement sound system
5. Add analytics and monetization

## Support
For issues or questions, refer to the project documentation in the `docs/` directory.
