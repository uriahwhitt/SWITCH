# SWITCH Sequence Diagrams

This directory contains sequence diagrams showing the interaction flows and system communication patterns in SWITCH.

## Diagram Overview

### 01_turn_execution_flow.mmd
**Complete turn execution sequence from input to completion**

#### Scope
- **Input Processing**: Swipe/tap detection and validation
- **Gravity System**: Direction calculation and application
- **Queue Management**: Tile request and distribution
- **Board Operations**: Tile placement and gravity application
- **Match Detection**: Pattern recognition and validation
- **Cascade Resolution**: Chain reaction processing
- **Score Calculation**: Points and combo calculation
- **UI Updates**: Display updates and animations
- **Performance Monitoring**: Timing and optimization tracking

#### Key Interactions
1. **Player → InputManager**: Touch input processing
2. **InputManager → GravitySystem**: Direction calculation
3. **GameManager → QueueSystem**: Tile requests
4. **QueueSystem → BoardController**: Tile transfer
5. **BoardController → MatchDetector**: Match checking
6. **MatchDetector → CascadeResolver**: Cascade processing
7. **ScoreSystem → UIManager**: Score updates

#### Performance Checkpoints
- Input processing time logging
- Turn completion time tracking
- System performance monitoring

### 02_match_detection_cascade.mmd
**Match detection and cascade resolution sequence**

#### Scope
- **Pattern Scanning**: Board analysis for matches
- **Match Validation**: Pattern verification and scoring
- **Cascade Detection**: Chain reaction identification
- **Cascade Processing**: Multi-level cascade resolution
- **Score Calculation**: Match and cascade scoring
- **Animation Coordination**: Visual effect synchronization
- **Performance Monitoring**: Detection time tracking

#### Key Features
- **Loop Processing**: Continuous cascade detection
- **Level Tracking**: Cascade level incrementation
- **Score Accumulation**: Progressive score calculation
- **Animation Coordination**: Synchronized visual effects

#### Performance Considerations
- Match detection timer
- Cascade processing efficiency
- Animation synchronization

### 03_powerup_execution.mmd
**Power-up selection, validation, and execution sequence**

#### Scope
- **Power-Up Selection**: UI interaction and validation
- **Inventory Management**: Availability checking
- **Power-Up Creation**: Factory instantiation
- **Preview System**: Effect preview and validation
- **Execution Logic**: Power-up specific operations
- **Result Processing**: Effect application and cleanup
- **UI Updates**: Inventory and display updates

#### Power-Up Types Covered
- **QueueShuffle**: Queue randomization
- **ColorBomb**: Targeted tile destruction
- **QueuePeek**: Future tile revelation
- **GravityReverse**: Direction inversion
- **UndoMove**: State restoration

#### Key Features
- **Validation Chain**: Multi-level validation
- **Preview System**: Effect preview before execution
- **Error Handling**: Graceful failure management
- **Performance Tracking**: Execution time monitoring

### 04_simple_score_submission.mmd
**Simplified score submission and social sharing sequence**

#### Scope
- **Score Submission**: Basic server-side validation
- **Friend Code System**: Add/remove friends via codes
- **Social Media Sharing**: External sharing integration
- **Leaderboard Views**: Global and friends leaderboards
- **Ad Integration**: Watch ad to continue feature

#### Key Features
- **Simple Validation**: Basic score validation only
- **Friend Codes**: 6-character alphanumeric codes
- **External Sharing**: Social media integration
- **No P2P**: Removed complex peer validation
- **Firebase Backend**: Simple server-side storage

#### Social Features
- **Share to Twitter/Facebook/Instagram**: Pre-formatted messages
- **Copy Friend Code**: Easy friend addition
- **Friends Leaderboard**: Filtered view of friends' scores
- **Global Leaderboard**: Top 100 scores

## Design Patterns

### Sequence Diagram Patterns
- **Request-Response**: Clear request and response patterns
- **Validation Chain**: Multi-level validation sequences
- **Error Handling**: Graceful error recovery
- **Performance Monitoring**: Built-in performance tracking

### Interaction Patterns
- **Event-Driven**: Event-based communication
- **State Validation**: State checking before operations
- **Resource Management**: Proper resource allocation
- **Cleanup Operations**: Proper resource cleanup

## Performance Considerations

### Timing Critical Paths
- **Input Processing**: < 16ms for 60 FPS
- **Match Detection**: < 8ms for smooth gameplay
- **Cascade Resolution**: < 32ms for complex cascades
- **Power-Up Execution**: < 16ms for responsive feel

### Optimization Strategies
- **Parallel Processing**: Concurrent operations where possible
- **Caching**: Result caching for repeated operations
- **Early Exit**: Quick validation and early termination
- **Batch Operations**: Grouped operations for efficiency

## Error Handling

### Validation Points
- **Input Validation**: Input format and range checking
- **State Validation**: Game state consistency checking
- **Resource Validation**: Resource availability checking
- **Result Validation**: Operation result verification

### Recovery Strategies
- **Graceful Degradation**: Fallback to simpler operations
- **State Restoration**: Return to previous valid state
- **User Notification**: Clear error messages to user
- **Logging**: Comprehensive error logging

## Testing Strategy

### Sequence Testing
- **Happy Path**: Normal operation flow
- **Error Paths**: Error condition handling
- **Edge Cases**: Boundary condition testing
- **Performance**: Timing and resource usage

### Mock Objects
- **Input Simulators**: Simulated user input
- **System Mocks**: Mocked external systems
- **Timer Mocks**: Controlled timing for testing
- **State Mocks**: Controlled game states

## Future Enhancements

### Planned Sequences
- **Save/Load Operations**: Game state persistence
- **Analytics Integration**: Event tracking and reporting
- **Enhanced Social Features**: Additional sharing options
- **Daily Challenges**: Challenge system integration

### Performance Improvements
- **Async Operations**: Non-blocking operations
- **Predictive Processing**: Anticipatory operations
- **Smart Caching**: Intelligent result caching
- **Adaptive Timing**: Dynamic timing adjustments
