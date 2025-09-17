# SWITCH - Strategic Wisdom In Tile Clearing Heuristics

**Revolutionary Match-3 Game with Directional Gravity**

## Project Overview

SWITCH revolutionizes the endless puzzle genre by combining match-3 mechanics with Tetris-style strategic planning through player-controlled directional gravity, creating the first truly strategic endless match-3 experience.

### Core Innovation
- **Directional Gravity**: Player controls where new blocks enter through double-tap selection
- **Extended Queue System**: 15-tile system (10 visible + 5 buffer) with anti-frustration
- **Special Tile Systems**: Blocking blocks and power orbs for strategic depth
- **Momentum-Based Scoring**: Heat system with 1.0x to 10.0x multipliers
- **Strategic Depth**: Every move affects future board state
- **Skill-Based**: True high-score competition based on mastery
- **Endless Gameplay**: No artificial difficulty walls

## Technology Stack

- **Engine**: Unity 2022.3 LTS
- **Language**: C#
- **Platform**: iOS & Android
- **Backend**: Firebase
- **Performance**: 60 FPS target on mobile
- **Documentation**: Mermaid diagrams for architecture visualization

## Project Structure

```
SWITCH/
├── planning-context/          # AI planning context files
├── docs/                     # Comprehensive documentation
│   ├── architecture/         # System architecture docs
│   ├── diagrams/            # UML diagrams and flow charts
│   ├── technical/           # Technical specifications
│   └── business/            # Business and monetization docs
├── project-knowledge/        # Organized knowledge base
├── src/                     # Unity project root
├── tests/                   # Test suite
├── data/                    # Game data and configurations
├── builds/                  # Build outputs
└── archive/                 # Previous versions
```

## Quick Start

### Prerequisites
- Unity 2022.3 LTS
- Git
- Mobile device for testing
- **Mermaid** (for viewing architecture diagrams)
  - VS Code: Install "Mermaid Preview" extension
  - GitHub: Native support for Mermaid diagrams
  - Online: [Mermaid Live Editor](https://mermaid.live/)

### Setup
1. Clone the repository
2. Open Unity and load the project from `src/` directory
3. Run `./update-planning-context.sh` to sync planning files
4. Check `planning-context/SPRINT_STATUS.md` for current tasks

### Development Workflow
1. Check current sprint status
2. Review relevant UML diagrams
3. Implement according to specifications
4. Run tests and profile performance
5. Update sprint status

## Key Features

### Core Mechanics
- **8x8 Grid**: Strategic tile placement
- **Extended Queue System**: 15-tile system (10 visible + 5 buffer)
- **Double-Tap Selection**: Select two adjacent tiles to swap
- **Swap Caching**: Gravity direction extracted from cached selection
- **Match Validation**: Verify matches before gravity calculation
- **Directional Gravity**: Player-controlled tile flow
- **Smart Distribution**: Anti-frustration algorithm with 15-tile look-ahead
- **Cascade System**: Chain reaction matches

### Momentum-Based Scoring System
- **Heat System**: Build momentum through skillful play (0-10 heat levels)
- **Score Multipliers**: 1.0x to 10.0x based on heat level
- **Position Scoring**: Edge (1x), Transition (2x), Center (3x) multipliers
- **Pattern Bonuses**: L-shape (+50), Cross (+100) points
- **Power Orbs**: Instant max heat + massive score boosts
- **Automatic Decay**: -1.0 heat per turn prevents coasting

### Special Tile Systems
- **Blocking Blocks**: Stone obstacles with progressive introduction (0% → 10%)
- **Power Orbs**: Center-spawn system with edge targeting and instant max heat
- **Strategic Interactions**: Special tiles create multi-turn planning opportunities

### Power-Up System
- **Queue Manipulation**: Shuffle, delete, delay tiles
- **Board Powers**: Clear rows, columns, colors
- **Emergency Powers**: Undo, hints, safety nets
- **Power Orbs**: Strategic scoring opportunities with edge targeting

### Dynamic Audio & Visual System
- **Layered Music**: Base, Rhythm, Melody, Climax layers
- **Tempo Changes**: 120-180 BPM based on heat level
- **Visual Heat Meter**: Color-coded with particle effects
- **Screen Effects**: Edge glow and pulsing at high heat

### Accessibility
- **Shape System**: Colors paired with shapes
- **Multiple Modes**: Standard, accessible, shape-only
- **Progressive Hints**: Learn through playing

## Performance Targets

- **Frame Rate**: 60 FPS sustained
- **Memory**: <200MB peak usage
- **Load Time**: <5 seconds
- **Battery**: <10% per hour
- **Input Latency**: <100ms for double-tap detection
- **Special Tiles**: <0.5ms per operation
- **Swap Caching**: <0.1ms per operation

## Development Phases

### Sprint 1: Foundation (Weeks 1-2)
- Unity project architecture
- Grid and matching system
- Directional gravity core with swap caching
- Extended 15-tile queue implementation
- Double-tap selection system
- Basic tile graphics

### Sprint 2: Special Tiles & Intelligence (Weeks 3-4)
- Blocking blocks system with progressive introduction
- Power orbs system with center spawning
- Smart distribution algorithm with 15-tile look-ahead
- Anti-frustration system
- Cascade detection

### Sprint 3: Power & Polish (Weeks 5-6)
- 5 basic power-ups
- Power-up inventory system
- Shape accessibility
- Animation system

### Sprint 4: Business Ready (Weeks 7-8)
- Ad integration
- Analytics setup
- Hint system
- Performance optimization

## Testing

### Test Types
- **Unit Tests**: Individual component testing
- **Integration Tests**: System interaction testing
- **Play Mode Tests**: Gameplay scenario testing
- **Performance Tests**: FPS and memory validation

### Running Tests
```bash
# Run all tests
./run-tests.sh

# Run specific test types
./run-tests.sh EditMode
./run-tests.sh PlayMode
```

## Building

### Build Targets
- **iOS**: iPhone 6s and newer
- **Android**: API 26+ (Android 8.0+)

### Build Commands
```bash
# Build for Android
./build-game.sh Android

# Build for iOS
./build-game.sh iOS
```

## Documentation

### Essential Documents
- `SWITCH_PRD_Final.md`: Complete product requirements
- `planning-context/SPRINT_PLAN.md`: Development roadmap
- `planning-context/SPRINT_STATUS.md`: Current progress
- `docs/architecture/`: System architecture
- `docs/diagrams/`: UML diagrams and flow charts

### UML Diagrams
- **Phase 1**: Core architecture and game flow
- **Phase 2**: Mechanics and power-up system
- **Phase 3**: Systems integration and UI
- **Phase 4**: Backend services and analytics

## Contributing

### Development Rules
- Follow SWITCH_Cursor_rules.md for implementation
- Maintain 60 FPS performance target
- Test on mobile devices regularly
- Update documentation with changes

### Code Standards
- Use C# naming conventions
- Implement object pooling for performance
- Follow event-driven architecture
- Write comprehensive tests

## License

Copyright © 2025 Whitt's End, LLC. All rights reserved.

## Support

For questions or issues:
1. Check documentation in `docs/` directory
2. Review planning context in `planning-context/`
3. Check current sprint status
4. Contact development team
