# SWITCH Cursor Implementation Rules

## Project Context
- **Game**: SWITCH - Strategic endless match-3 puzzle (Single-Player MVP)
- **Engine**: Unity 2022.3 LTS
- **Language**: C#
- **Target**: iOS/Android mobile (60 FPS)
- **Architecture**: Event-driven, singleton GameManager
- **Timeline**: 6-week MVP development
- **Focus**: 90% single-player experience with basic social sharing

## Before Starting Work
1. Run `./update-planning-context.sh`
2. Check SPRINT_STATUS.md for current task
3. Review relevant UML diagrams
4. Check DECISIONS.md for technical decisions

## Implementation Standards

### Code Structure
```csharp
// File header for every script
/******************************************************************************
 * SWITCH - [Component Name]
 * Sprint: [X]
 * Author: [Your Name]
 * Date: [Creation Date]
 * 
 * Description: [Brief description]
 * Dependencies: [List key dependencies]
 * 
 * Educational Notes:
 * - [Key learning points for developers]
 * - [Architecture decisions explained]
 * - [Performance considerations]
 *****************************************************************************/
```

### Unity-Specific Rules
- Prefabs: Create prefab immediately after GameObject setup
- Serialization: Use [SerializeField] for private fields needing Inspector access
- Coroutines: Prefer async/await except for animation
- Object Pooling: Always pool frequently created/destroyed objects
- Mobile: Test on device every 3 features

### Performance Requirements
- Maintain 60 FPS at all times
- Memory allocation <1KB per frame
- Draw calls <100
- Texture memory <100MB

### Scoring System Implementation
- **MomentumSystem**: Core heat management with event-driven updates
- **TurnScoreCalculator**: Complete turn score calculation with position multipliers
- **HeatAudioManager**: Dynamic audio layers with tempo changes
- **HeatUIManager**: Visual heat meter with particle effects
- **PowerOrbManager**: Power orb spawning and collection management
- **PowerOrb**: Individual orb behavior and movement
- **PowerOrbData**: ScriptableObject configuration for orb types
- Audio memory <20MB

### C# Standards
```csharp
// Naming conventions
public class GameManager     // PascalCase for classes
private float moveSpeed;      // camelCase for fields
public void CalculateScore()  // PascalCase for methods
const int MAX_TILES = 64;     // UPPER_CASE for constants

// Property pattern with documentation
[SerializeField] private int score;
/// <summary>
/// Current game score. Updated automatically when matches are made.
/// Educational: This property demonstrates the backing field pattern for Unity serialization.
/// </summary>
public int Score => score;

// Event pattern with documentation
/// <summary>
/// Fired when the player's score changes. Used for UI updates and analytics.
/// Educational: This demonstrates the observer pattern for loose coupling between systems.
/// </summary>
public static event Action<int> OnScoreChanged;
```

### Testing Requirements
- Write Play Mode test for gameplay features
- Write Edit Mode test for algorithms
- Test on minimum spec device daily
- Profile performance after each feature

### Implementation Order
1. Core functionality (make it work)
2. Error handling (make it safe)
3. Optimization (make it fast)
4. Polish (make it beautiful)

### After Completing Task
1. Run all tests
2. Profile performance
3. Update SPRINT_STATUS.md
4. Run ./update-planning-context.sh
5. **Document all changes** with comprehensive comments and XML documentation
6. Stage and commit changes with conventional format
7. Push changes to remote repository

### Simplified Social Features (MVP Scope)
- **Friend Codes**: 6-character alphanumeric codes only
- **Social Sharing**: External social media integration only
- **Leaderboards**: Simple server-side validation only
- **No P2P**: Removed complex peer validation systems
- **No Real-time**: No multiplayer or real-time features
- **Firebase Backend**: Simple server-side storage only

### Forbidden Practices
- No deep inheritance (max 2 levels)
- No public fields (use properties)
- No Find() methods in Update loops
- No instantiate/destroy in gameplay (use pooling)
- No synchronous resource loading
- No hard-coded values (use ScriptableObjects)
- No P2P networking (use simple HTTP calls)
- No complex multiplayer features

### Documentation Requirements
- **XML Documentation**: All public methods, properties, and classes must have XML documentation
- **Inline Comments**: Explain complex algorithms, business logic, and performance optimizations
- **Educational Comments**: Include learning points and architectural decisions
- **Code Examples**: Provide usage examples in documentation
- **Performance Notes**: Document performance implications and optimizations
- **Unity-Specific Notes**: Explain Unity patterns and best practices

### Common Patterns with Documentation
```csharp
/// <summary>
/// Singleton pattern implementation for GameManager.
/// Educational: This demonstrates the singleton pattern for global state management in Unity.
/// Performance: Ensures only one instance exists, reducing memory overhead.
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    /// <summary>
    /// Global access point to the GameManager instance.
    /// Educational: This property demonstrates lazy initialization with error checking.
    /// </summary>
    public static GameManager Instance 
    { 
        get 
        {
            if (instance == null)
                Debug.LogError("GameManager not initialized!");
            return instance;
        }
    }
    
    /// <summary>
    /// Unity lifecycle method for singleton initialization.
    /// Educational: This demonstrates Unity's Awake() method and DontDestroyOnLoad pattern.
    /// </summary>
    private void Awake()
    {
        // Singleton pattern: ensure only one instance exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene loads
    }
}

/// <summary>
/// Object pooling implementation for performance optimization.
/// Educational: This demonstrates the object pool pattern to reduce garbage collection.
/// Performance: Pre-allocates objects to avoid runtime Instantiate/Destroy calls.
/// </summary>
private Queue<GameObject> tilePool = new Queue<GameObject>();

/// <summary>
/// Initializes the object pool with pre-allocated GameObjects.
/// Educational: This method shows how to pre-allocate objects for performance.
/// Performance: Reduces garbage collection by reusing objects instead of creating/destroying.
/// </summary>
/// <param name="size">Number of objects to pre-allocate in the pool</param>
private void InitializePool(int size)
{
    for (int i = 0; i < size; i++)
    {
        GameObject tile = Instantiate(tilePrefab);
        tile.SetActive(false); // Start inactive
        tilePool.Enqueue(tile); // Add to pool
    }
}
```

### Git Workflow Requirements
- Use conventional commit format: `[Sprint X] Type: Description`
- Types: feat, fix, docs, style, refactor, test, perf, chore
- Always stage and commit changes after completing tasks
- Push changes to remote repository
- Use feature branches for new development
- Create pull requests for code review

### Branch Management Strategy
- **main**: Production-ready code, always stable
- **develop**: Integration branch for features, sprint completion point
- **feature/sprint-x-component**: Individual feature development
- **hotfix/issue-description**: Critical bug fixes
- **docs/update-type**: Documentation updates
- **refactor/component-name**: Code refactoring

### Branch Workflow
1. **Start Feature**: Create feature branch from develop
2. **Develop**: Implement feature with regular commits
3. **Test**: Run tests and performance checks
4. **Merge**: Merge feature into develop via PR
5. **Release**: Merge develop into main for releases
6. **Hotfix**: Create hotfix branch from main if needed

### Git Commands for Implementation
```bash
# Create feature branch from develop
git checkout develop
git pull origin develop
git checkout -b feature/sprint-1-gamemanager

# Create hotfix branch from main
git checkout main
git pull origin main
git checkout -b hotfix/critical-crash-fix

# Create documentation branch
git checkout develop
git checkout -b docs/uml-diagrams-update

# Commit changes with conventional format
git add .
git commit -m "[Sprint 1] feat: Implement GameManager singleton pattern"

# Push feature branch and create PR
git push origin feature/sprint-1-gamemanager
# Then create PR via GitHub UI or CLI

# Merge feature into develop (after PR approval)
git checkout develop
git pull origin develop
git merge feature/sprint-1-gamemanager
git push origin develop

# Run pre-commit checks
./scripts/git-workflow.sh pre-commit
```

### Questions During Implementation
If you encounter decisions not covered in DECISIONS.md:
1. Check if it affects architecture (stop and ask Planning AI)
2. Check if it affects performance (implement simplest solution)
3. Check if it affects gameplay (refer to PRD)
4. Document decision in SPRINT_STATUS.md notes
5. Commit decision documentation

## Scoring System Implementation Guide

### Core Components Integration
The momentum-based scoring system integrates with existing game systems through event-driven architecture:

```csharp
// GameManager integration
public class GameManager : MonoBehaviour
{
    [SerializeField] private MomentumSystem momentumSystem;
    [SerializeField] private TurnScoreCalculator scoreCalculator;
    [SerializeField] private PowerOrbManager powerOrbManager;
    
    private void HandleTurnComplete(TurnResult result)
    {
        var scoreResult = scoreCalculator.CalculateTurnScore(result);
        UpdateTotalScore(scoreResult.FinalScore);
        // Momentum and heat updates handled automatically
    }
}
```

### MatchDetector Integration
Update MatchDetector to create TurnResult objects:

```csharp
public class MatchDetector : MonoBehaviour
{
    public TurnResult CreateTurnResult(MatchData[] matches)
    {
        var result = new TurnResult();
        result.ClearedTiles = GetClearedTiles(matches);
        result.MatchSizes = GetMatchSizes(matches);
        result.CascadeLevel = CalculateCascadeLevel(matches);
        result.HasLShape = DetectLShapePattern(matches);
        result.HasCross = DetectCrossPattern(matches);
        result.PowerOrbCollected = CheckPowerOrbCollection();
        return result;
    }
}
```

### BoardController Integration
Update BoardController to track power orbs:

```csharp
public class BoardController : MonoBehaviour
{
    [SerializeField] private PowerOrbManager powerOrbManager;
    
    private void ClearMatches(MatchData[] matches)
    {
        foreach (var match in matches)
        {
            foreach (var position in match.positions)
            {
                // Check for power orb at position
                var orb = powerOrbManager.GetOrbAtPosition(position);
                if (orb != null)
                {
                    orb.LoseOrb(); // Orb destroyed by match
                }
                
                // Clear tile at position
                ClearTileAt(position);
            }
        }
    }
}
```

### Audio System Integration
HeatAudioManager automatically responds to momentum changes:

```csharp
// Automatic integration - no manual setup required
// HeatAudioManager subscribes to MomentumSystem events
// Audio layers fade in/out based on heat level
// Tempo changes automatically with heat
```

### UI System Integration
HeatUIManager provides visual feedback:

```csharp
// Automatic integration - no manual setup required
// HeatUIManager subscribes to MomentumSystem events
// Heat meter updates automatically
// Particle effects trigger based on heat level
// Screen effects activate at high heat
```

### Power Orb System Setup
Configure power orb types in Unity Inspector:

1. Create PowerOrbData ScriptableObjects for each orb color
2. Set target edges for each color
3. Configure spawn chances and visual properties
4. Assign to PowerOrbManager's powerOrbTypes array

### Testing Integration
All scoring system components have comprehensive unit tests:

```csharp
// Run scoring system tests
// Tests cover heat generation, decay, multipliers
// Tests cover power orb lifecycle and collection
// Tests cover score calculation with all modifiers
// Tests cover audio and visual system integration
```

### Performance Considerations
- MomentumSystem uses events for loose coupling
- TurnScoreCalculator caches calculations
- HeatAudioManager uses object pooling for audio sources
- HeatUIManager uses coroutines for smooth transitions
- PowerOrbManager limits active orbs to 4 maximum

### Mobile Optimization
- Audio layers use compressed formats
- Particle effects have LOD system
- Heat meter uses UI Canvas for performance
- Power orb movement uses simple transforms
- All coroutines have proper cleanup
