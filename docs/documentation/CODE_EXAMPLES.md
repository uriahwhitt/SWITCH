# SWITCH Code Examples and Templates

## Overview
This document provides comprehensive code examples and templates for the SWITCH project, demonstrating proper documentation standards and educational value.

## File Header Template

```csharp
/******************************************************************************
 * SWITCH - GameManager
 * Sprint: 1
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Central game state management and coordination system
 * Dependencies: BoardController, UIManager, AudioManager, AnalyticsManager
 * 
 * Educational Notes:
 * - Demonstrates the singleton pattern for global state management
 * - Shows Unity's DontDestroyOnLoad pattern for persistent objects
 * - Illustrates event-driven architecture for loose coupling
 * - Examples of Unity lifecycle methods (Awake, Start, Update)
 * 
 * Performance Considerations:
 * - Singleton pattern reduces memory overhead
 * - Event system avoids direct references
 * - Cached component references improve performance
 * 
 * Usage Example:
 * ```csharp
 * // Get the GameManager instance
 * var gameManager = GameManager.Instance;
 * 
 * // Start a new game
 * gameManager.StartGame();
 * 
 * // Subscribe to events
 * GameManager.OnGameStateChanged += HandleGameStateChange;
 * ```
 *****************************************************************************/
```

## Class Documentation Template

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

## Method Documentation Template

```csharp
/// <summary>
/// Calculates the score for a match based on tile count and multipliers.
/// Educational: This demonstrates score calculation with performance optimization.
/// Performance: Uses cached values to avoid repeated calculations.
/// Unity: Integrates with Unity's scoring system for UI updates.
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
/// 
/// // Calculate score for a 3-tile match with time bonus
/// int score = CalculateScore(3, 0, 0.8f);
/// // Returns: 100 * 1.0 * 1.8 = 180 points
/// </code>
/// </example>
public int CalculateScore(int matchCount, int cascadeLevel, float timeBonus)
{
    // Validate input parameters
    if (matchCount < 3)
        throw new ArgumentException("Match count must be at least 3", nameof(matchCount));
    
    // Performance: Use cached base scores to avoid repeated calculations
    // Educational: This shows how to optimize frequently called methods
    int baseScore = GetBaseScore(matchCount);
    
    // Calculate cascade multiplier
    // Educational: This demonstrates how to implement game mechanics
    float cascadeMultiplier = 1.0f + (cascadeLevel * 0.5f);
    
    // Apply time bonus
    // Educational: This shows how to implement dynamic scoring
    float totalMultiplier = cascadeMultiplier * (1.0f + timeBonus);
    
    return Mathf.RoundToInt(baseScore * totalMultiplier);
}
```

## Property Documentation Template

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
/// 
/// // Check for high score
/// if (currentScore > highScore)
/// {
///     highScore = currentScore;
/// }
/// </code>
/// </example>
[SerializeField] private int score;
public int Score => score;
```

## Event Documentation Template

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
/// GameManager.OnScoreChanged += UpdateAnalytics;
/// 
/// // Unsubscribe when done
/// GameManager.OnScoreChanged -= UpdateScoreUI;
/// GameManager.OnScoreChanged -= UpdateAnalytics;
/// 
/// // Event handler example
/// private void UpdateScoreUI(int newScore)
/// {
///     scoreText.text = $"Score: {newScore}";
/// }
/// </code>
/// </example>
public static event Action<int> OnScoreChanged;
```

## Complex Algorithm Documentation

```csharp
/// <summary>
/// Finds all possible matches on the current board state using optimized scanning.
/// Educational: This demonstrates efficient match detection algorithms for mobile games.
/// Performance: O(n²) complexity optimized for mobile devices with early exit conditions.
/// Unity: Uses Unity's 2D array system for efficient memory access.
/// </summary>
/// <param name="board">2D array representing the game board</param>
/// <returns>List of all found matches</returns>
/// <example>
/// <code>
/// // Find matches on current board
/// var matches = FindMatches(currentBoard);
/// 
/// // Process each match
/// foreach (var match in matches)
/// {
///     ProcessMatch(match);
/// }
/// </code>
/// </example>
public List<Match> FindMatches(Tile[,] board)
{
    var matches = new List<Match>();
    
    // Horizontal scan: Check each row for consecutive matches
    // Educational: This shows how to iterate through a 2D array efficiently
    // Performance: Early exit when no more matches possible
    for (int y = 0; y < boardHeight; y++)
    {
        // Check for horizontal matches of 3 or more tiles
        // Educational: This demonstrates the core match detection algorithm
        var horizontalMatches = FindHorizontalMatches(board, y);
        matches.AddRange(horizontalMatches);
        
        // Performance: Early exit if board is too full for more matches
        if (matches.Count > MAX_POSSIBLE_MATCHES)
            break;
    }
    
    // Vertical scan: Check each column for consecutive matches
    // Educational: This demonstrates the same algorithm applied vertically
    // Performance: Reuse the same logic for different orientations
    for (int x = 0; x < boardWidth; x++)
    {
        var verticalMatches = FindVerticalMatches(board, x);
        matches.AddRange(verticalMatches);
    }
    
    // Remove duplicate matches that were found in both scans
    // Educational: This shows how to handle overlapping match detection
    // Performance: Use HashSet for O(1) duplicate removal
    return RemoveDuplicateMatches(matches);
}
```

## Performance-Critical Code Documentation

```csharp
/// <summary>
/// Initializes the object pool with pre-allocated GameObjects for performance.
/// Educational: This method shows how to pre-allocate objects for performance optimization.
/// Performance: Reduces garbage collection by reusing objects instead of creating/destroying.
/// Unity: Uses Unity's Instantiate system with proper lifecycle management.
/// </summary>
/// <param name="size">Number of objects to pre-allocate in the pool</param>
/// <example>
/// <code>
/// // Initialize pool with 100 tiles
/// InitializePool(100);
/// 
/// // Get a tile from the pool
/// var tile = GetTileFromPool();
/// 
/// // Return tile to pool when done
/// ReturnTileToPool(tile);
/// </code>
/// </example>
private void InitializePool(int size)
{
    // Performance: Pre-allocate all objects to avoid runtime Instantiate calls
    // Educational: This demonstrates the object pool pattern for performance optimization
    for (int i = 0; i < size; i++)
    {
        // Unity: Instantiate creates a new GameObject instance
        // Educational: This shows how to create objects programmatically
        GameObject tile = Instantiate(tilePrefab);
        
        // Performance: Start inactive to avoid rendering overhead
        // Educational: This demonstrates how to control object visibility
        tile.SetActive(false);
        
        // Performance: Add to pool for reuse
        // Educational: This shows how to implement the pool data structure
        tilePool.Enqueue(tile);
    }
    
    // Performance: Pre-warm the pool to avoid first-frame hitches
    // Educational: This shows how to prepare resources before they're needed
    Log($"Initialized object pool with {size} tiles");
}
```

## Unity-Specific Code Documentation

```csharp
/// <summary>
/// Unity lifecycle method for singleton initialization and component setup.
/// Educational: This demonstrates Unity's Awake() method and DontDestroyOnLoad pattern.
/// Unity: Awake() is called before Start() and is ideal for initialization.
/// Performance: Singleton pattern ensures only one instance exists.
/// </summary>
/// <example>
/// <code>
/// // GameManager will automatically initialize when the scene loads
/// // No manual initialization required due to singleton pattern
/// 
/// // Access the instance from anywhere
/// var gameManager = GameManager.Instance;
/// </code>
/// </example>
private void Awake()
{
    // Singleton pattern: ensure only one instance exists
    // Educational: This shows how to implement the singleton pattern in Unity
    if (instance != null && instance != this)
    {
        // Unity: Destroy removes the GameObject from the scene
        // Educational: This demonstrates Unity's object lifecycle
        Destroy(gameObject);
        return;
    }
    
    // Set this as the singleton instance
    instance = this;
    
    // Unity: DontDestroyOnLoad keeps the object alive across scene changes
    // Educational: This demonstrates Unity's scene management system
    // Performance: Avoids recreating the GameManager on scene transitions
    DontDestroyOnLoad(gameObject);
    
    // Initialize core systems
    // Educational: This shows how to set up dependencies in Unity
    InitializeCoreSystems();
}
```

## Error Handling Documentation

```csharp
/// <summary>
/// Validates and processes player input with comprehensive error handling.
/// Educational: This demonstrates proper error handling and validation patterns.
/// Performance: Early validation prevents unnecessary processing.
/// Unity: Integrates with Unity's input system for cross-platform compatibility.
/// </summary>
/// <param name="input">Raw input data from Unity's input system</param>
/// <returns>Processed input data or null if invalid</returns>
/// <exception cref="ArgumentNullException">Thrown when input is null</exception>
/// <exception cref="ArgumentException">Thrown when input format is invalid</exception>
/// <example>
/// <code>
/// // Process valid input
/// var processedInput = ProcessInput(validInput);
/// if (processedInput != null)
/// {
///     ExecuteAction(processedInput);
/// }
/// 
/// // Handle invalid input gracefully
/// try
/// {
///     var result = ProcessInput(invalidInput);
/// }
/// catch (ArgumentException ex)
/// {
///     LogError($"Invalid input: {ex.Message}");
/// }
/// </code>
/// </example>
public ProcessedInput ProcessInput(RawInput input)
{
    // Validate input parameters
    if (input == null)
        throw new ArgumentNullException(nameof(input), "Input cannot be null");
    
    // Educational: This shows how to validate input data
    if (!IsValidInputFormat(input))
        throw new ArgumentException("Invalid input format", nameof(input));
    
    // Performance: Early validation prevents unnecessary processing
    // Educational: This demonstrates the fail-fast principle
    if (input.Timestamp < lastProcessedTimestamp)
    {
        LogWarning("Received out-of-order input, ignoring");
        return null;
    }
    
    // Process the input
    // Educational: This shows how to transform raw data into usable format
    var processedInput = new ProcessedInput
    {
        Action = input.Action,
        Direction = input.Direction,
        Timestamp = input.Timestamp,
        IsValid = true
    };
    
    // Update tracking variables
    lastProcessedTimestamp = input.Timestamp;
    
    return processedInput;
}
```

## Testing Documentation

```csharp
/// <summary>
/// Unit test for the CalculateScore method with comprehensive test cases.
/// Educational: This demonstrates how to write effective unit tests for game logic.
/// Performance: Tests validate performance requirements and edge cases.
/// Unity: Uses Unity's Test Framework for integrated testing.
/// </summary>
/// <example>
/// <code>
/// // Run this test to validate score calculation
/// [Test]
/// public void CalculateScore_ValidInput_ReturnsCorrectScore()
/// {
///     // Arrange
///     var gameManager = new GameManager();
///     
///     // Act
///     int score = gameManager.CalculateScore(4, 1, 0.5f);
///     
///     // Assert
///     Assert.AreEqual(675, score);
/// }
/// </code>
/// </example>
[Test]
public void CalculateScore_ValidInput_ReturnsCorrectScore()
{
    // Arrange: Set up test data
    // Educational: This shows how to structure unit tests
    var gameManager = new GameManager();
    int matchCount = 4;
    int cascadeLevel = 1;
    float timeBonus = 0.5f;
    
    // Act: Execute the method under test
    // Educational: This demonstrates the Act phase of unit testing
    int result = gameManager.CalculateScore(matchCount, cascadeLevel, timeBonus);
    
    // Assert: Verify the results
    // Educational: This shows how to validate test results
    int expectedScore = 300 * 1.5f * 1.5f; // 675
    Assert.AreEqual(expectedScore, result, "Score calculation should match expected value");
    
    // Performance: Validate that calculation is fast enough
    // Educational: This shows how to test performance requirements
    var stopwatch = Stopwatch.StartNew();
    gameManager.CalculateScore(matchCount, cascadeLevel, timeBonus);
    stopwatch.Stop();
    
    Assert.Less(stopwatch.ElapsedMilliseconds, 1, "Score calculation should complete in under 1ms");
}
```

## Conclusion
These examples demonstrate the comprehensive documentation standards for the SWITCH project. Every piece of code should be educational, well-documented, and provide clear examples of usage. This approach ensures that the codebase serves as a learning resource while maintaining high quality and performance standards.
