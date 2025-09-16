# SWITCH Documentation Standards

## Overview
This document defines the comprehensive documentation standards for the SWITCH project, ensuring all code is educational and well-documented for learning purposes.

## Documentation Philosophy
- **Educational First**: Every piece of code should teach something
- **Self-Documenting**: Code should be readable without external documentation
- **Performance Aware**: Document performance implications and optimizations
- **Unity-Specific**: Explain Unity patterns and best practices
- **Example-Driven**: Provide usage examples and patterns

## Documentation Requirements

### 1. File Headers
Every C# file must have a comprehensive header:

```csharp
/******************************************************************************
 * SWITCH - [Component Name]
 * Sprint: [X]
 * Author: [Your Name]
 * Date: [Creation Date]
 * 
 * Description: [Brief description of the component's purpose]
 * Dependencies: [List key dependencies and their purposes]
 * 
 * Educational Notes:
 * - [Key learning points for developers]
 * - [Architecture decisions explained]
 * - [Performance considerations]
 * - [Unity-specific patterns demonstrated]
 * 
 * Usage Example:
 * ```csharp
 * // Example of how to use this component
 * var gameManager = GameManager.Instance;
 * gameManager.StartGame();
 * ```
 *****************************************************************************/
```

### 2. Class Documentation
All classes must have XML documentation:

```csharp
/// <summary>
/// GameManager handles the overall game state and coordinates between systems.
/// Educational: This demonstrates the singleton pattern for global state management in Unity.
/// Performance: Ensures only one instance exists, reducing memory overhead.
/// Unity: Uses DontDestroyOnLoad to persist across scene changes.
/// </summary>
/// <example>
/// <code>
/// // Get the GameManager instance
/// var gameManager = GameManager.Instance;
/// 
/// // Start a new game
/// gameManager.StartGame();
/// 
/// // Check if game is active
/// if (gameManager.IsGameActive)
/// {
///     // Game is running
/// }
/// </code>
/// </example>
public class GameManager : MonoBehaviour
{
    // Implementation...
}
```

### 3. Method Documentation
All public methods must have comprehensive XML documentation:

```csharp
/// <summary>
/// Calculates the score for a match based on tile count and multipliers.
/// Educational: This demonstrates score calculation with performance optimization.
/// Performance: Uses cached values to avoid repeated calculations.
/// </summary>
/// <param name="matchCount">Number of tiles in the match (3, 4, 5, etc.)</param>
/// <param name="cascadeLevel">Current cascade level for multiplier calculation</param>
/// <param name="timeBonus">Time-based bonus multiplier (0.0 to 1.0)</param>
/// <returns>The calculated score value</returns>
/// <exception cref="ArgumentException">Thrown when matchCount is less than 3</exception>
/// <example>
/// <code>
/// // Calculate score for a 4-tile match in first cascade
/// int score = CalculateScore(4, 1, 0.5f);
/// // Returns: 300 * 1.5 * 1.5 = 675 points
/// </code>
/// </example>
public int CalculateScore(int matchCount, int cascadeLevel, float timeBonus)
{
    // Implementation...
}
```

### 4. Property Documentation
All public properties must have XML documentation:

```csharp
/// <summary>
/// Current game score. Updated automatically when matches are made.
/// Educational: This property demonstrates the backing field pattern for Unity serialization.
/// Performance: Direct field access avoids property call overhead.
/// Unity: SerializeField allows Inspector configuration while maintaining encapsulation.
/// </summary>
/// <value>The current score value</value>
/// <example>
/// <code>
/// // Access current score
/// int currentScore = GameManager.Instance.Score;
/// 
/// // Use in UI update
/// scoreText.text = $"Score: {currentScore}";
/// </code>
/// </example>
public int Score => score;
```

### 5. Event Documentation
All events must have XML documentation:

```csharp
/// <summary>
/// Fired when the player's score changes. Used for UI updates and analytics.
/// Educational: This demonstrates the observer pattern for loose coupling between systems.
/// Performance: Events avoid direct references, reducing memory coupling.
/// Unity: Static events allow global access without singleton references.
/// </summary>
/// <param name="newScore">The new score value</param>
/// <example>
/// <code>
/// // Subscribe to score changes
/// GameManager.OnScoreChanged += UpdateScoreUI;
/// 
/// // Unsubscribe when done
/// GameManager.OnScoreChanged -= UpdateScoreUI;
/// </code>
/// </example>
public static event Action<int> OnScoreChanged;
```

### 6. Inline Comments
Complex logic must have inline comments:

```csharp
/// <summary>
/// Finds all possible matches on the current board state.
/// Educational: This demonstrates efficient match detection algorithms.
/// Performance: O(n²) complexity optimized for mobile devices.
/// </summary>
public List<Match> FindMatches(Tile[,] board)
{
    var matches = new List<Match>();
    
    // Horizontal scan: Check each row for consecutive matches
    // Educational: This shows how to iterate through a 2D array efficiently
    for (int y = 0; y < boardHeight; y++)
    {
        // Check for horizontal matches of 3 or more tiles
        // Performance: Early exit when no more matches possible
        matches.AddRange(FindHorizontalMatches(board, y));
    }
    
    // Vertical scan: Check each column for consecutive matches
    // Educational: This demonstrates the same algorithm applied vertically
    for (int x = 0; x < boardWidth; x++)
    {
        matches.AddRange(FindVerticalMatches(board, x));
    }
    
    // Remove duplicate matches that were found in both scans
    // Educational: This shows how to handle overlapping match detection
    return RemoveDuplicateMatches(matches);
}
```

### 7. Performance Comments
Performance-critical code must have performance notes:

```csharp
/// <summary>
/// Initializes the object pool with pre-allocated GameObjects.
/// Educational: This method shows how to pre-allocate objects for performance.
/// Performance: Reduces garbage collection by reusing objects instead of creating/destroying.
/// </summary>
/// <param name="size">Number of objects to pre-allocate in the pool</param>
private void InitializePool(int size)
{
    // Performance: Pre-allocate all objects to avoid runtime Instantiate calls
    // Educational: This demonstrates the object pool pattern for performance optimization
    for (int i = 0; i < size; i++)
    {
        GameObject tile = Instantiate(tilePrefab);
        tile.SetActive(false); // Start inactive to avoid rendering overhead
        tilePool.Enqueue(tile); // Add to pool for reuse
    }
    
    // Performance: Pre-warm the pool to avoid first-frame hitches
    // Educational: This shows how to prepare resources before they're needed
}
```

### 8. Unity-Specific Comments
Unity-specific code must explain Unity patterns:

```csharp
/// <summary>
/// Unity lifecycle method for singleton initialization.
/// Educational: This demonstrates Unity's Awake() method and DontDestroyOnLoad pattern.
/// Unity: Awake() is called before Start() and is ideal for initialization.
/// Performance: Singleton pattern ensures only one instance exists.
/// </summary>
private void Awake()
{
    // Singleton pattern: ensure only one instance exists
    // Educational: This shows how to implement the singleton pattern in Unity
    if (instance != null && instance != this)
    {
        Destroy(gameObject); // Destroy duplicate instances
        return;
    }
    
    instance = this;
    // Unity: DontDestroyOnLoad keeps the object alive across scene changes
    // Educational: This demonstrates Unity's scene management system
    DontDestroyOnLoad(gameObject);
}
```

## Documentation Templates

### Method Template
```csharp
/// <summary>
/// [Brief description of what the method does]
/// Educational: [What developers can learn from this method]
/// Performance: [Performance implications and optimizations]
/// Unity: [Unity-specific patterns or considerations]
/// </summary>
/// <param name="paramName">[Parameter description]</param>
/// <returns>[Return value description]</returns>
/// <exception cref="ExceptionType">[When this exception is thrown]</exception>
/// <example>
/// <code>
/// // [Usage example]
/// var result = MethodName(parameter);
/// </code>
/// </example>
public ReturnType MethodName(ParameterType paramName)
{
    // Implementation with inline comments
}
```

### Class Template
```csharp
/// <summary>
/// [Brief description of the class purpose]
/// Educational: [What developers can learn from this class]
/// Performance: [Performance considerations]
/// Unity: [Unity-specific patterns demonstrated]
/// </summary>
/// <example>
/// <code>
/// // [Usage example]
/// var instance = new ClassName();
/// instance.DoSomething();
/// </code>
/// </example>
public class ClassName : MonoBehaviour
{
    // Implementation...
}
```

## Quality Checklist

### Documentation Review Checklist
- [ ] File header includes educational notes
- [ ] All public methods have XML documentation
- [ ] All public properties have XML documentation
- [ ] All events have XML documentation
- [ ] Complex algorithms have inline comments
- [ ] Performance-critical code has performance notes
- [ ] Unity-specific code explains Unity patterns
- [ ] Usage examples are provided
- [ ] Educational value is clearly stated
- [ ] Code is self-documenting

### Educational Value Checklist
- [ ] Explains the "why" behind design decisions
- [ ] Demonstrates common patterns and best practices
- [ ] Shows performance optimization techniques
- [ ] Illustrates Unity-specific concepts
- [ ] Provides learning opportunities for developers
- [ ] Includes practical usage examples
- [ ] Documents architectural decisions
- [ ] Explains trade-offs and alternatives

## Examples of Good Documentation

### Well-Documented Method
```csharp
/// <summary>
/// Applies directional gravity to tiles based on player input.
/// Educational: This demonstrates the core SWITCH mechanic and physics integration.
/// Performance: Uses object pooling to avoid garbage collection during tile movement.
/// Unity: Integrates with Unity's physics system for smooth animations.
/// </summary>
/// <param name="direction">The direction of gravity (Left, Right, Up, Down)</param>
/// <param name="intensity">Gravity strength multiplier (0.0 to 2.0)</param>
/// <returns>Number of tiles that moved</returns>
/// <example>
/// <code>
/// // Apply gravity to the left
/// int movedTiles = ApplyGravity(Direction.Left, 1.0f);
/// 
/// // Apply stronger gravity to the right
/// int movedTiles = ApplyGravity(Direction.Right, 1.5f);
/// </code>
/// </example>
public int ApplyGravity(Direction direction, float intensity)
{
    // Performance: Cache frequently used values to avoid repeated calculations
    var flowVector = GetFlowVector(direction);
    var movingTiles = GetMovingTiles(flowVector);
    
    // Educational: This loop demonstrates the physics update pattern
    // Performance: Batch processing reduces per-frame overhead
    while (movingTiles.Count > 0)
    {
        foreach (var tile in movingTiles)
        {
            // Unity: Use Unity's physics system for smooth movement
            // Educational: This shows how to integrate custom logic with Unity physics
            tile.Move(flowVector * intensity);
        }
        
        // Performance: Recalculate moving tiles to handle collisions
        movingTiles = GetMovingTiles(flowVector);
    }
    
    return movingTiles.Count;
}
```

## Conclusion
Comprehensive documentation is essential for the SWITCH project's educational value. Every piece of code should teach something, explain performance considerations, and demonstrate Unity best practices. This documentation standard ensures that the codebase serves as a learning resource for developers while maintaining high quality and performance.
