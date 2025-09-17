# SWITCH Development Requirements

## Development Environment

### Essential Tools
- **Unity 2022.3 LTS** - Game engine and development environment
- **Git** - Version control system
- **Visual Studio Code** (recommended) - Code editor with Unity integration
- **Mermaid** - For viewing architecture diagrams

### Mermaid Setup Options
1. **VS Code Extension**: Install "Mermaid Preview" extension
2. **GitHub**: Native support for Mermaid diagrams in markdown
3. **Online Editor**: [Mermaid Live Editor](https://mermaid.live/)
4. **CLI Tool**: `npm install -g @mermaid-js/mermaid-cli`

### Mobile Testing
- **iOS Device**: iPhone 8 or newer for testing
- **Android Device**: Samsung Galaxy S10 or equivalent
- **Unity Remote**: For real-time testing during development

## System Requirements

### Development Machine
- **OS**: Windows 10/11, macOS 10.15+, or Ubuntu 18.04+
- **RAM**: 8GB minimum, 16GB recommended
- **Storage**: 10GB free space for Unity and project files
- **Graphics**: DirectX 11 compatible GPU

### Unity Requirements
- **Unity Hub**: Latest version
- **Unity 2022.3 LTS**: Specific version required
- **Android Build Support**: For Android development
- **iOS Build Support**: For iOS development (macOS only)

## Documentation Requirements

### Architecture Diagrams
All system architecture diagrams use Mermaid syntax and require:
- Mermaid viewer for proper rendering
- VS Code with Mermaid Preview extension (recommended)
- GitHub for online viewing

### File Structure
- All documentation must be accessible without additional tools
- Mermaid diagrams must render correctly in GitHub
- Architecture documents located in `docs/architecture/`

## Performance Targets

### Mobile Performance
- **Frame Rate**: 60 FPS target, 30 FPS minimum
- **Memory**: <200MB total usage
- **Battery**: <10% per hour of gameplay
- **Load Time**: <5 seconds initial load

### Development Performance
- **Build Time**: <2 minutes for development builds
- **Test Execution**: <30 seconds for full test suite
- **Profiling**: Real-time performance monitoring required

## Core Game System Requirements

### Turn Execution Flow
- **Double-Tap Selection**: Player selects two adjacent tiles with double-tap
- **Swap Caching**: System caches selection data (positions + direction)
- **Match Validation**: Verify match exists BEFORE gravity calculation
- **Gravity Extraction**: Extract gravity direction from cached swap data
- **Performance**: <100ms input latency for double-tap recognition

### Extended Queue System (15-Tile)
- **Visible Queue**: 10 tiles shown to player
- **Buffer Queue**: 5 hidden tiles for anti-frustration algorithm
- **Look-Ahead Analysis**: System analyzes 15 tiles for strategic distribution
- **Clockwise Fill Priority**: Top→Right→Bottom→Left edge filling
- **Memory**: <1KB allocation per queue operation

## Scoring System Requirements

### Momentum-Based Heat System
- **Heat Generation**: Match-4 (1.0 heat), Match-5 (2.0 heat), Cascades (0.5 per level)
- **Heat Decay**: Automatic -1.0 heat per turn (prevents coasting)
- **Score Multiplier**: 1.0x to 10.0x based on heat level (1 + heat × 0.9)
- **Heat Levels**: Cold (0-2), Warm (3-4), Hot (5-7), Blazing (8-9), Inferno (10)

### Special Tile Systems

#### Blocking Blocks
- **Progressive Introduction**: 0% → 2% → 10% spawn rate over time
- **Spawn Conditions**: Only when regular tiles are placed
- **Removal**: Must reach board edge to be removed
- **Performance**: <0.1ms per blocking block operation
- **Visual**: Clearly distinct stone/concrete texture

#### Power Orbs
- **Spawn Location**: Center cells (3,3), (4,3), (3,4), (4,4)
- **Spawn Trigger**: When center cell is cleared by match
- **Spawn Chance**: 5% base, increasing to 15% over time
- **Instant Max Heat**: Power orbs provide instant 10.0 heat when collected
- **Strategic Timing**: Risk/reward decisions about when to collect orbs
- **Age-Based Scoring**: Base 5,000 points + 500 per turn survived
- **Edge Targeting**: Orbs move toward specific colored edges
- **Performance**: <0.5ms per power orb operation

### Position-Based Scoring
- **Edge Multiplier**: 1x (outer ring)
- **Transition Multiplier**: 2x (middle ring)  
- **Center Multiplier**: 3x (center 4 cells)
- **Pattern Bonuses**: L-shape (+50), Cross (+100)

### Dynamic Audio System
- **Layered Music**: Base, Rhythm, Melody, Climax layers
- **Tempo Changes**: 120-180 BPM based on heat level
- **Heartbeat Effect**: Accelerates with heat level
- **Sound Effects**: Heat up, cool down, power orb explosion

### Visual Heat System
- **Heat Meter**: Color-coded bar with multiplier display
- **Particle Effects**: Heat, flame, and inferno particles
- **Screen Effects**: Edge glow and pulsing effects
- **Color Transitions**: Smooth color changes between heat levels

## Quality Standards

### Code Quality
- **Test Coverage**: >90% for logic, 100% for algorithms
- **Documentation**: 100% public API coverage
- **Performance**: Profiling after each feature
- **Mobile Testing**: Daily testing on real devices
- **Scoring System**: 100% test coverage for momentum and heat calculations

### Documentation Quality
- **Architecture**: Complete system diagrams with Mermaid
- **API Documentation**: XML documentation for all public methods
- **Performance Notes**: Optimization strategies documented
- **Educational Value**: Code examples and best practices
- **Scoring Documentation**: Complete scoring system documentation with examples
- **Special Tiles Documentation**: Complete blocking blocks and power orbs documentation
- **Turn Execution Documentation**: Complete swap caching and validation documentation
- **Queue System Documentation**: Complete 15-tile system documentation

## Workflow Requirements

### File Management
- **Immediate Cleanup**: Delete unnecessary files immediately
- **Archive Strategy**: Move outdated files to `archive/` directory
- **No Orphaned Files**: Maintain clean repository structure

### Git Workflow
- **Branch Strategy**: Feature branches for all development
- **Commit Format**: Conventional commits with sprint information
- **PR Requirements**: Tests pass, documented, reviewed

### Development Process
- **Sprint-Based**: 2-week sprints with clear deliverables
- **Performance First**: 60 FPS requirement for all features
- **Mobile Testing**: Real device testing mandatory
- **Documentation**: Update docs with each feature

## External Dependencies

### Unity Packages
- **VContainer**: Dependency injection framework
- **DOTween**: Animation system
- **Unity Input System**: Modern input handling
- **Unity Test Framework**: Testing infrastructure

### Backend Services
- **Firebase**: Authentication, database, analytics
- **Unity Analytics**: Game analytics and metrics
- **Unity Ads**: Advertisement integration

### Development Tools
- **Unity Profiler**: Performance monitoring
- **Unity Test Runner**: Automated testing
- **Git LFS**: Large file version control
- **Mermaid CLI**: Diagram generation (optional)

---

**Last Updated**: January 2025  
**Version**: 1.0  
**Status**: Active Requirements
