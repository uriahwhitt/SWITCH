# Blocking Blocks System - Technical Documentation

## Overview
Blocking Blocks are stone/concrete obstacles that add strategic complexity to SWITCH by creating board obstacles that must be managed over multiple turns. They force players to plan ahead and create multi-turn strategies.

## Design Philosophy

### Strategic Purpose
- **Multi-turn Planning**: Forces players to think beyond immediate matches
- **Board Obstacles**: Creates dynamic challenges that evolve over time
- **Progressive Difficulty**: Gradually increases complexity without overwhelming new players
- **Non-Frustrating**: Can be swapped and removed, never permanently blocks gameplay

### Visual Design
- **Clear Differentiation**: Stone/concrete texture clearly different from regular tiles
- **Consistent Appearance**: Always recognizable as obstacles
- **Subtle Animation**: Minimal visual effects to avoid distraction

## Technical Implementation

### 1. Blocking Block Data Structure

```csharp
/// <summary>
/// Configuration data for blocking blocks (obstacles).
/// Educational: This demonstrates special tile behavior configuration.
/// </summary>
[CreateAssetMenu(fileName = "BlockingTileData", menuName = "SWITCH/Blocking Tile")]
public class BlockingTileData : ScriptableObject
{
    [Header("Visual Properties")]
    [Tooltip("Stone/concrete texture sprite")]
    public Sprite stoneTexture;
    
    [Tooltip("Concrete color for visual consistency")]
    public Color concreteColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    
    [Header("Behavior Properties")]
    [Tooltip("Can this blocking block be swapped with regular tiles?")]
    public bool canBeSwapped = true;
    
    [Tooltip("Can this blocking block be matched? (Always false)")]
    public bool canBeMatched = false;
    
    [Header("Spawn Configuration")]
    [Tooltip("Base spawn rate (2% = 0.02f)")]
    [Range(0f, 0.1f)]
    public float baseSpawnRate = 0.02f;
    
    [Tooltip("Maximum spawn rate (10% = 0.10f)")]
    [Range(0f, 0.2f)]
    public float maxSpawnRate = 0.10f;
    
    [Tooltip("Maximum number of blocking blocks on board simultaneously")]
    [Range(1, 10)]
    public int maxOnBoard = 3;
    
    [Header("Progressive Spawn Settings")]
    [Tooltip("Spawn rate increase per interval (1% = 0.01f)")]
    [Range(0f, 0.05f)]
    public float spawnRateIncrease = 0.01f;
    
    [Tooltip("Time interval between spawn rate increases (seconds)")]
    [Range(60f, 300f)]
    public float increaseInterval = 120f; // 2 minutes
    
    [Tooltip("Time before progressive spawning begins (seconds)")]
    [Range(0f, 600f)]
    public float initialDelay = 300f; // 5 minutes
}
```

### 2. Blocking Block Implementation

```csharp
/// <summary>
/// Blocking block tile that acts as an obstacle on the board.
/// Educational: This demonstrates special tile behavior and conditional interactions.
/// </summary>
public class BlockingTile : MonoBehaviour, ITile
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D tileCollider;
    
    [Header("Data")]
    [SerializeField] private BlockingTileData data;
    
    [Header("State")]
    [SerializeField] private Vector2Int boardPosition;
    [SerializeField] private bool isActive = false;
    [SerializeField] private bool canBeSwapped = true;
    
    /// <summary>
    /// Initializes the blocking block with data and position.
    /// Educational: This shows how special tiles are configured.
    /// </summary>
    public void Initialize(BlockingTileData tileData, Vector2Int position)
    {
        data = tileData;
        boardPosition = position;
        canBeSwapped = data.canBeSwapped;
        
        // Set visual properties
        spriteRenderer.sprite = data.stoneTexture;
        spriteRenderer.color = data.concreteColor;
        
        // Set up collider for input detection
        tileCollider.enabled = true;
        
        isActive = true;
        
        // Play spawn animation
        PlaySpawnAnimation();
    }
    
    /// <summary>
    /// Validates if this blocking block can be swapped with a regular tile.
    /// Educational: This demonstrates conditional swap logic for special tiles.
    /// </summary>
    public bool CanSwapWith(TileData regularTile)
    {
        if (!canBeSwapped || !isActive) return false;
        
        // Only allow swap if the regular tile would create a match
        var matchDetector = GameManager.Instance.MatchDetector;
        return matchDetector.WouldCreateMatch(regularTile, boardPosition);
    }
    
    /// <summary>
    /// Checks if this blocking block can be matched (always false).
    /// Educational: This demonstrates immutable tile properties.
    /// </summary>
    public bool CanCreateMatch()
    {
        return false; // Blocking blocks cannot be matched
    }
    
    /// <summary>
    /// Handles the blocking block reaching the board edge.
    /// Educational: This shows how special tiles are removed from play.
    /// </summary>
    public void OnReachEdge()
    {
        PlayRemovalAnimation();
        StartCoroutine(RemoveFromBoard());
    }
    
    /// <summary>
    /// Plays animation when blocking block is removed.
    /// Educational: This demonstrates visual feedback for special tile removal.
    /// </summary>
    private void PlayRemovalAnimation()
    {
        animator.SetTrigger("Remove");
    }
    
    /// <summary>
    /// Coroutine to remove blocking block after animation.
    /// Educational: This shows how to handle delayed object destruction.
    /// </summary>
    private IEnumerator RemoveFromBoard()
    {
        yield return new WaitForSeconds(0.5f); // Wait for animation
        
        // Return to object pool
        BlockingBlockPooler.Instance.ReturnBlockingBlock(this);
    }
    
    #region ITile Implementation
    
    public TileType Type => TileType.Blocking;
    public Vector2Int Position => boardPosition;
    public bool IsActive => isActive;
    
    public void SetPosition(Vector2Int newPosition)
    {
        boardPosition = newPosition;
        transform.position = BoardController.Instance.GetWorldPosition(newPosition);
    }
    
    public void AnimateToPosition(Vector3 worldPosition)
    {
        StartCoroutine(AnimateMovement(worldPosition));
    }
    
    private IEnumerator AnimateMovement(Vector3 targetPosition)
    {
        var startPosition = transform.position;
        var duration = 0.3f;
        var elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        transform.position = targetPosition;
    }
    
    #endregion
}
```

### 3. Progressive Spawn System

```csharp
/// <summary>
/// Manages progressive spawning of blocking blocks based on game time.
/// Educational: This demonstrates time-based difficulty scaling.
/// </summary>
public class BlockingBlockSpawner : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private BlockingTileData spawnData;
    [SerializeField] private BoardController boardController;
    [SerializeField] private GameManager gameManager;
    
    [Header("State")]
    [SerializeField] private float currentSpawnRate;
    [SerializeField] private float lastSpawnTime;
    [SerializeField] private int blocksOnBoard;
    [SerializeField] private float gameStartTime;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogging = false;
    
    private void Start()
    {
        gameStartTime = Time.time;
        currentSpawnRate = 0f; // Start with 0% spawn rate
        blocksOnBoard = 0;
    }
    
    private void Update()
    {
        UpdateSpawnRate();
        TrySpawnBlock();
    }
    
    /// <summary>
    /// Updates spawn rate based on game time and progressive settings.
    /// Educational: This shows how to implement progressive difficulty scaling.
    /// </summary>
    private void UpdateSpawnRate()
    {
        var gameTime = Time.time - gameStartTime;
        
        // First 5 minutes: 0% spawn rate
        if (gameTime < spawnData.initialDelay)
        {
            currentSpawnRate = 0f;
            return;
        }
        
        // After initial delay: start at base rate, increase over time
        var timeAfterStart = gameTime - spawnData.initialDelay;
        var increaseCount = Mathf.FloorToInt(timeAfterStart / spawnData.increaseInterval);
        
        currentSpawnRate = Mathf.Min(
            spawnData.baseSpawnRate + (increaseCount * spawnData.spawnRateIncrease),
            spawnData.maxSpawnRate
        );
        
        if (enableDebugLogging && Time.time - lastSpawnTime > 10f)
        {
            Debug.Log($"Blocking Block Spawn Rate: {currentSpawnRate:P1} (Game Time: {gameTime:F1}s)");
            lastSpawnTime = Time.time;
        }
    }
    
    /// <summary>
    /// Attempts to spawn a blocking block based on current spawn rate.
    /// Educational: This demonstrates probabilistic spawning with constraints.
    /// </summary>
    private void TrySpawnBlock()
    {
        // Check constraints
        if (blocksOnBoard >= spawnData.maxOnBoard) return;
        if (currentSpawnRate <= 0f) return;
        
        // Probabilistic spawn check
        if (Random.Range(0f, 1f) > currentSpawnRate * Time.deltaTime) return;
        
        // Find valid spawn position
        var spawnPosition = FindValidSpawnPosition();
        if (spawnPosition == Vector2Int.one * -1) return; // No valid position found
        
        // Spawn blocking block
        SpawnBlockingBlock(spawnPosition);
    }
    
    /// <summary>
    /// Finds a valid position to spawn a blocking block.
    /// Educational: This shows how to validate spawn positions for special tiles.
    /// </summary>
    private Vector2Int FindValidSpawnPosition()
    {
        var validPositions = new List<Vector2Int>();
        var boardSize = boardController.BoardSize;
        
        // Check all board positions
        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                var position = new Vector2Int(x, y);
                
                // Skip if position is occupied
                if (boardController.GetTileAt(position) != null) continue;
                
                // Skip center positions (reserved for power orbs)
                if (IsCenterPosition(position)) continue;
                
                // Skip if would create immediate match
                if (WouldCreateMatch(position)) continue;
                
                validPositions.Add(position);
            }
        }
        
        // Return random valid position or invalid if none found
        return validPositions.Count > 0 ? validPositions[Random.Range(0, validPositions.Count)] : Vector2Int.one * -1;
    }
    
    /// <summary>
    /// Checks if position is in center area (reserved for power orbs).
    /// Educational: This demonstrates position-based constraints.
    /// </summary>
    private bool IsCenterPosition(Vector2Int position)
    {
        var centerPositions = new Vector2Int[]
        {
            new Vector2Int(3, 3), new Vector2Int(4, 3),
            new Vector2Int(3, 4), new Vector2Int(4, 4)
        };
        
        return centerPositions.Contains(position);
    }
    
    /// <summary>
    /// Checks if spawning at position would create an immediate match.
    /// Educational: This shows how to prevent disruptive spawning.
    /// </summary>
    private bool WouldCreateMatch(Vector2Int position)
    {
        // Create temporary blocking tile data for match check
        var tempTile = new TileData
        {
            type = TileType.Blocking,
            position = position
        };
        
        // Check if this would create a match (it shouldn't, but safety check)
        var matchDetector = gameManager.MatchDetector;
        return matchDetector.WouldCreateMatch(tempTile, position);
    }
    
    /// <summary>
    /// Spawns a blocking block at the specified position.
    /// Educational: This demonstrates object pooling for special tiles.
    /// </summary>
    private void SpawnBlockingBlock(Vector2Int position)
    {
        var blockingBlock = BlockingBlockPooler.Instance.GetBlockingBlock();
        blockingBlock.Initialize(spawnData, position);
        
        // Place on board
        boardController.PlaceTile(blockingBlock, position);
        
        // Update count
        blocksOnBoard++;
        
        // Subscribe to removal event
        blockingBlock.OnReachEdge += OnBlockingBlockRemoved;
        
        if (enableDebugLogging)
        {
            Debug.Log($"Spawned Blocking Block at {position} (Total: {blocksOnBoard})");
        }
    }
    
    /// <summary>
    /// Handles blocking block removal from board.
    /// Educational: This shows event handling for special tile lifecycle.
    /// </summary>
    private void OnBlockingBlockRemoved(BlockingTile blockingBlock)
    {
        blocksOnBoard--;
        blockingBlock.OnReachEdge -= OnBlockingBlockRemoved;
        
        if (enableDebugLogging)
        {
            Debug.Log($"Blocking Block removed (Total: {blocksOnBoard})");
        }
    }
}
```

### 4. Object Pooling for Performance

```csharp
/// <summary>
/// Object pooler for blocking blocks to maintain 60 FPS.
/// Educational: This demonstrates performance optimization for special objects.
/// Performance: Prevents garbage collection spikes from blocking block creation.
/// </summary>
public class BlockingBlockPooler : MonoBehaviour
{
    [Header("Pool Configuration")]
    [SerializeField] private GameObject blockingBlockPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private Transform poolParent;
    
    private Queue<BlockingTile> availableBlocks;
    private List<BlockingTile> allBlocks;
    
    public static BlockingBlockPooler Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Initializes the blocking block pool.
    /// Educational: This shows how to pre-allocate special objects for performance.
    /// </summary>
    private void InitializePool()
    {
        availableBlocks = new Queue<BlockingTile>();
        allBlocks = new List<BlockingTile>();
        
        for (int i = 0; i < poolSize; i++)
        {
            var blockingBlock = CreateBlockingBlock();
            blockingBlock.gameObject.SetActive(false);
            availableBlocks.Enqueue(blockingBlock);
            allBlocks.Add(blockingBlock);
        }
        
        Debug.Log($"Initialized Blocking Block Pool with {poolSize} objects");
    }
    
    /// <summary>
    /// Gets a blocking block from the pool.
    /// Educational: This demonstrates object pool retrieval pattern.
    /// </summary>
    public BlockingTile GetBlockingBlock()
    {
        if (availableBlocks.Count > 0)
        {
            var blockingBlock = availableBlocks.Dequeue();
            blockingBlock.gameObject.SetActive(true);
            return blockingBlock;
        }
        
        // Pool exhausted, create new one
        Debug.LogWarning("Blocking Block Pool exhausted, creating new object");
        return CreateBlockingBlock();
    }
    
    /// <summary>
    /// Returns a blocking block to the pool.
    /// Educational: This shows object pool return pattern.
    /// </summary>
    public void ReturnBlockingBlock(BlockingTile blockingBlock)
    {
        if (blockingBlock == null) return;
        
        // Reset state
        blockingBlock.gameObject.SetActive(false);
        blockingBlock.transform.SetParent(poolParent);
        blockingBlock.transform.localPosition = Vector3.zero;
        
        // Return to pool
        availableBlocks.Enqueue(blockingBlock);
    }
    
    /// <summary>
    /// Creates a new blocking block GameObject.
    /// Educational: This demonstrates prefab instantiation for pooling.
    /// </summary>
    private BlockingTile CreateBlockingBlock()
    {
        var go = Instantiate(blockingBlockPrefab, poolParent);
        var blockingBlock = go.GetComponent<BlockingTile>();
        
        if (blockingBlock == null)
        {
            Debug.LogError("Blocking Block Prefab missing BlockingTile component!");
            Destroy(go);
            return null;
        }
        
        return blockingBlock;
    }
    
    /// <summary>
    /// Gets pool statistics for debugging.
    /// Educational: This shows how to monitor pool performance.
    /// </summary>
    public PoolStatistics GetPoolStatistics()
    {
        return new PoolStatistics
        {
            totalObjects = allBlocks.Count,
            availableObjects = availableBlocks.Count,
            inUseObjects = allBlocks.Count - availableBlocks.Count,
            poolUtilization = (float)(allBlocks.Count - availableBlocks.Count) / allBlocks.Count
        };
    }
}

/// <summary>
/// Statistics for object pool monitoring.
/// Educational: This demonstrates performance monitoring data structures.
/// </summary>
[System.Serializable]
public struct PoolStatistics
{
    public int totalObjects;
    public int availableObjects;
    public int inUseObjects;
    public float poolUtilization;
}
```

### 5. Integration with Match Detection

```csharp
/// <summary>
/// Extended match detector that handles blocking blocks.
/// Educational: This shows how to extend existing systems for special tiles.
/// </summary>
public class ExtendedMatchDetector : MatchDetector
{
    /// <summary>
    /// Checks if a swap would create a match, considering blocking blocks.
    /// Educational: This demonstrates conditional match validation.
    /// </summary>
    public override bool WouldCreateMatch(TileData tile, Vector2Int position)
    {
        // Get current tile at position
        var currentTile = boardController.GetTileAt(position);
        
        // If position has a blocking block, check if it can be swapped
        if (currentTile is BlockingTile blockingTile)
        {
            return blockingTile.CanSwapWith(tile);
        }
        
        // Standard match check for regular tiles
        return base.WouldCreateMatch(tile, position);
    }
    
    /// <summary>
    /// Validates matches considering blocking blocks.
    /// Educational: This shows how special tiles affect match validation.
    /// </summary>
    public override MatchResult ValidateMatches(MatchData[] matches)
    {
        var validMatches = new List<MatchData>();
        
        foreach (var match in matches)
        {
            // Check if match contains any blocking blocks
            bool containsBlockingBlocks = false;
            foreach (var position in match.positions)
            {
                var tile = boardController.GetTileAt(position);
                if (tile is BlockingTile)
                {
                    containsBlockingBlocks = true;
                    break;
                }
            }
            
            // Blocking blocks cannot be part of matches
            if (!containsBlockingBlocks)
            {
                validMatches.Add(match);
            }
        }
        
        return new MatchResult
        {
            matches = validMatches.ToArray(),
            totalScore = validMatches.Sum(m => m.score),
            hasCascades = validMatches.Any(m => m.isCascade)
        };
    }
}
```

## Spawn Rate Progression

### Timeline
- **0-5 minutes**: 0% spawn rate (tutorial phase)
- **5-7 minutes**: 2% spawn rate (introduction)
- **7-9 minutes**: 3% spawn rate (gradual increase)
- **9-11 minutes**: 4% spawn rate (building complexity)
- **11+ minutes**: 5% spawn rate (maximum complexity)

### Spawn Rate Formula
```csharp
spawnRate = Mathf.Min(
    baseSpawnRate + (Mathf.Floor((gameTime - initialDelay) / increaseInterval) * spawnRateIncrease),
    maxSpawnRate
);
```

## Visual Design Guidelines

### Appearance
- **Texture**: Stone/concrete material with visible grain
- **Color**: Neutral gray (#808080) with slight variation
- **Shape**: Same as regular tiles but clearly different material
- **Size**: Identical to regular tiles for consistent grid

### Animation
- **Spawn**: Subtle scale-in animation (0.3s duration)
- **Removal**: Fade-out with slight scale-down (0.5s duration)
- **Movement**: Same as regular tiles for consistency
- **No Special Effects**: Avoid particle effects to maintain clarity

## Testing Requirements

### Unit Tests
```csharp
[TestFixture]
public class BlockingBlockTests
{
    [Test]
    public void BlockingBlock_CannotBeMatched()
    {
        // Arrange
        var blockingBlock = new BlockingTile();
        var blockingData = ScriptableObject.CreateInstance<BlockingTileData>();
        blockingData.canBeMatched = false;
        
        // Act
        blockingBlock.Initialize(blockingData, Vector2Int.zero);
        
        // Assert
        Assert.IsFalse(blockingBlock.CanCreateMatch());
    }
    
    [Test]
    public void BlockingBlock_CanBeSwapped_WhenCreatesMatch()
    {
        // Arrange
        var blockingBlock = new BlockingTile();
        var regularTile = new TileData { type = TileType.Basic };
        
        // Mock match detector to return true
        var mockMatchDetector = new Mock<IMatchDetector>();
        mockMatchDetector.Setup(x => x.WouldCreateMatch(It.IsAny<TileData>(), It.IsAny<Vector2Int>()))
                        .Returns(true);
        
        // Act
        var canSwap = blockingBlock.CanSwapWith(regularTile);
        
        // Assert
        Assert.IsTrue(canSwap);
    }
    
    [Test]
    public void SpawnRate_ProgressiveIncrease()
    {
        // Arrange
        var spawner = new BlockingBlockSpawner();
        var spawnData = ScriptableObject.CreateInstance<BlockingTileData>();
        spawnData.baseSpawnRate = 0.02f;
        spawnData.spawnRateIncrease = 0.01f;
        spawnData.increaseInterval = 120f;
        spawnData.initialDelay = 300f;
        
        // Act & Assert
        // At 5 minutes (300s): 0% spawn rate
        Assert.AreEqual(0f, spawner.CalculateSpawnRate(300f));
        
        // At 7 minutes (420s): 2% spawn rate
        Assert.AreEqual(0.02f, spawner.CalculateSpawnRate(420f));
        
        // At 9 minutes (540s): 3% spawn rate
        Assert.AreEqual(0.03f, spawner.CalculateSpawnRate(540f));
    }
}
```

### Integration Tests
- Test blocking block spawning during gameplay
- Test swap validation with blocking blocks
- Test removal when reaching board edges
- Test progressive spawn rate increases
- Test maximum blocking block limits

### Performance Tests
- Verify 60 FPS with maximum blocking blocks
- Test object pooling efficiency
- Monitor memory allocation during spawning
- Test spawn rate calculation performance

## Configuration Examples

### Conservative Settings (Easy Mode)
```csharp
baseSpawnRate = 0.01f;        // 1%
maxSpawnRate = 0.05f;         // 5%
spawnRateIncrease = 0.005f;   // 0.5%
increaseInterval = 180f;      // 3 minutes
maxOnBoard = 2;
```

### Standard Settings (Normal Mode)
```csharp
baseSpawnRate = 0.02f;        // 2%
maxSpawnRate = 0.10f;         // 10%
spawnRateIncrease = 0.01f;    // 1%
increaseInterval = 120f;      // 2 minutes
maxOnBoard = 3;
```

### Aggressive Settings (Hard Mode)
```csharp
baseSpawnRate = 0.03f;        // 3%
maxSpawnRate = 0.15f;         // 15%
spawnRateIncrease = 0.015f;   // 1.5%
increaseInterval = 90f;       // 1.5 minutes
maxOnBoard = 4;
```

## Conclusion

The Blocking Blocks system adds strategic depth to SWITCH by creating obstacles that require multi-turn planning. The progressive spawn system ensures the difficulty scales appropriately, while the object pooling maintains performance. The system is designed to be non-frustrating while still providing meaningful strategic challenges.

The implementation prioritizes clarity in both code and gameplay, with comprehensive testing and configuration options to support different difficulty levels and player preferences.
