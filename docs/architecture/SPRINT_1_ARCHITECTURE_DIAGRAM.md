# SWITCH Sprint 1 Architecture Diagram

## System Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        SWITCH GAME ARCHITECTURE                │
│                           Sprint 1 Implementation              │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   INPUT LAYER   │    │   CORE LAYER    │    │  DATA LAYER     │
│                 │    │                 │    │                 │
│ DoubleTapDetector│    │   GameManager   │    │   TileData      │
│                 │    │   (Singleton)   │    │   (Scriptable)  │
│                 │    │                 │    │                 │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  PHYSICS LAYER  │    │  GAME LOGIC     │    │  PERFORMANCE    │
│                 │    │                 │    │                 │
│ DirectionalGravity│   │  MatchDetector  │    │   SwapCache     │
│                 │    │                 │    │   (Caching)     │
│                 │    │                 │    │                 │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  BOARD LAYER    │    │  QUEUE LAYER    │    │  TESTING LAYER  │
│                 │    │                 │    │                 │
│ BoardController │    │ExtendedQueueSys │    │  Unit Tests     │
│ (Object Pool)   │    │ (15-tile + AI)  │    │  (NUnit)        │
│                 │    │                 │    │                 │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   TILE LAYER    │    │  SCORING LAYER  │    │  INTEGRATION    │
│                 │    │                 │    │                 │
│      Tile       │    │ MomentumSystem  │    │ Event System    │
│   (Individual)  │    │ TurnScoreCalc   │    │ (Loose Coupling)│
│                 │    │                 │    │                 │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## Event Flow Architecture

```
USER INPUT
    │
    ▼
┌─────────────────┐
│ DoubleTapDetector│
│   (<100ms)      │
└─────────────────┘
    │ OnDoubleTapDetected
    ▼
┌─────────────────┐
│   GameManager   │
│   (Singleton)   │
└─────────────────┘
    │ HandleDoubleTap
    ▼
┌─────────────────┐
│ DirectionalGravity│
│   (Physics)     │
└─────────────────┘
    │ OnGravityComplete
    ▼
┌─────────────────┐
│  MatchDetector  │
│   (Pattern)     │
└─────────────────┘
    │ OnMatchesFound
    ▼
┌─────────────────┐
│   GameManager   │
│   (Scoring)     │
└─────────────────┘
    │ HandleTurnComplete
    ▼
┌─────────────────┐
│ MomentumSystem  │
│   (Heat/Score)  │
└─────────────────┘
```

## Object Pooling Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    OBJECT POOLING SYSTEM                       │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   POOL STORAGE  │    │   ACTIVE TILES  │    │   POOL MANAGER  │
│                 │    │                 │    │                 │
│ Queue<GameObject>│    │ List<GameObject>│    │ BoardController │
│                 │    │                 │    │                 │
│ [Inactive Tiles]│    │ [Active Tiles]  │    │ GetPooledTile() │
│                 │    │                 │    │ ReturnToPool()  │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   PERFORMANCE   │    │   MEMORY MGMT   │    │   STATISTICS    │
│                 │    │                 │    │                 │
│ Zero Allocation │    │ Pre-allocated   │    │ Pool Count      │
│ O(1) Operations │    │ 100 Objects     │    │ Active Count    │
│ 60 FPS Target   │    │ No GC Pressure  │    │ Expansion Flag  │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## Queue System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    EXTENDED QUEUE SYSTEM                       │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  VISIBLE TILES  │    │  BUFFER TILES   │    │  ANTI-FRUSTRATION│
│                 │    │                 │    │                 │
│   10 Tiles      │    │    5 Tiles      │    │ Smart Algorithm │
│   (Player)      │    │   (Hidden)      │    │                 │
│                 │    │                 │    │ Color Tracking  │
│                 │    │                 │    │ Weighted Select │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   QUEUE CORE    │    │   DISTRIBUTION  │    │   PERFORMANCE   │
│                 │    │                 │    │                 │
│ Queue<TileData> │    │ Statistical     │    │ O(1) Draw       │
│                 │    │ Analysis        │    │ O(1) Refill     │
│ DrawTile()      │    │ Max Same Color  │    │ <1ms Operations │
│ RefillQueue()   │    │ Color Balance   │    │ Memory Efficient│
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## Match Detection Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    MATCH DETECTION SYSTEM                      │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  HORIZONTAL     │    │   VERTICAL      │    │  SPECIAL SHAPES │
│                 │    │                 │    │                 │
│ Line Detection  │    │ Line Detection  │    │ L-Shape         │
│ 3+ Tiles        │    │ 3+ Tiles        │    │ Cross           │
│                 │    │                 │    │ T-Shape         │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   DETECTION     │    │   OPTIMIZATION  │    │   RESULTS       │
│                 │    │                 │    │                 │
│ Grid Scanning   │    │ Early Termination│    │ MatchData[]     │
│ Pattern Matching│    │ Duplicate Remove│    │ Score Calculation│
│ Color Validation│    │ Visited Tracking│    │ Event Triggering│
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## Performance Characteristics

```
┌─────────────────────────────────────────────────────────────────┐
│                    PERFORMANCE METRICS                         │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   INPUT LAG     │    │   MEMORY USE    │    │   CPU EFFICIENCY│
│                 │    │                 │    │                 │
│ <100ms Latency  │    │ <1KB/Frame      │    │ O(1) Operations │
│ Double-Tap      │    │ Object Pooling  │    │ Cached Results  │
│ Touch/Mouse     │    │ No GC Pressure  │    │ Early Exit      │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   CACHING       │    │   POOLING       │    │   ALGORITHMS    │
│                 │    │                 │    │                 │
│ <0.1ms Swap     │    │ 100 Pre-alloc   │    │ Optimized Match │
│ Gravity Cache   │    │ Zero Runtime    │    │ Efficient Queue  │
│ Time-based      │    │ Instantiate     │    │ Smart Distribution│
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## Testing Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    TESTING INFRASTRUCTURE                      │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  UNIT TESTS     │    │  INTEGRATION    │    │  PERFORMANCE    │
│                 │    │                 │    │                 │
│ GameManager     │    │ System Events   │    │ Memory Profiling│
│ BoardController │    │ Component Comm  │    │ Frame Rate      │
│ SwapCache       │    │ Data Flow       │    │ Input Latency   │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   COVERAGE      │    │   VALIDATION    │    │   BENCHMARKS    │
│                 │    │                 │    │                 │
│ 45 Test Cases   │    │ Edge Cases      │    │ 60 FPS Target   │
│ All Core Systems│    │ Boundary Tests  │    │ <200MB Memory   │
│ Mock Objects    │    │ Error Handling  │    │ <50MB Build     │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## Key Architectural Decisions

### 1. Event-Driven Architecture
- **Loose Coupling**: Systems communicate through events
- **Maintainability**: Easy to add new systems
- **Testability**: Systems can be tested in isolation
- **Performance**: No unnecessary updates

### 2. Object Pooling
- **Memory Efficiency**: Pre-allocated objects
- **Performance**: Zero runtime allocation
- **GC Pressure**: Minimal garbage collection
- **Scalability**: Configurable pool size

### 3. Caching System
- **Performance**: <0.1ms operations
- **Data Integrity**: Time-based expiration
- **Memory Efficient**: Minimal storage overhead
- **Extensible**: Easy to add new cache types

### 4. Anti-Frustration Algorithm
- **Player Experience**: Prevents impossible situations
- **Statistical Analysis**: Color distribution tracking
- **Weighted Selection**: Balanced randomness
- **Configurable**: Adjustable parameters

### 5. Comprehensive Testing
- **Quality Assurance**: 45 unit test cases
- **Coverage**: All core systems tested
- **Edge Cases**: Boundary conditions covered
- **Performance**: Time-based functionality validated

This architecture provides a solid foundation for the remaining sprints while maintaining high performance and code quality standards.
