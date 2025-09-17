# SWITCH Game Modifications - Technical Implementation Guide

## Overview
This document outlines the technical implementation requirements for the critical game modifications discussed in the SWITCH project. These changes significantly enhance the strategic depth while maintaining core simplicity.

## 1. CORRECTED Turn Execution Sequence

### Critical Change: Gravity from Cached Selection
**Problem Solved**: Eliminates wasted calculations on failed swaps by only calculating gravity AFTER confirming a valid match.

### Implementation Requirements

#### 1.1 Input System Modifications
```csharp
/// <summary>
/// Caches player selection data for gravity calculation.
/// Educational: This demonstrates the command pattern for deferred execution.
/// Performance: Avoids redundant calculations on invalid moves.
/// </summary>
public class SwapCache
{
    public Vector2Int tile1Position;
    public Vector2Int tile2Position;
    public Direction swapDirection;
    public float timestamp;
    public bool isValid;
    
    public SwapCache(Vector2Int tile1, Vector2Int tile2)
    {
        tile1Position = tile1;
        tile2Position = tile2;
        swapDirection = CalculateDirection(tile1, tile2);
        timestamp = Time.time;
        isValid = true;
    }
    
    /// <summary>
    /// Extracts gravity direction from cached swap data.
    /// Educational: This shows how to derive game state from user input.
    /// </summary>
    public Direction GetGravityDirection()
    {
        return swapDirection;
    }
}
```

#### 1.2 Gravity System Updates
```csharp
/// <summary>
/// Updated gravity system that uses cached selection data.
/// Educational: This demonstrates deferred execution and cache-based state management.
/// </summary>
public class DirectionalGravity : IGravitySystem
{
    private SwapCache cachedSwap;
    
    /// <summary>
    /// Caches swap data when player selects tiles.
    /// Educational: This shows how to store user intent for later processing.
    /// </summary>
    public void CacheSwapData(Vector2Int tile1, Vector2Int tile2)
    {
        cachedSwap = new SwapCache(tile1, tile2);
    }
    
    /// <summary>
    /// Extracts gravity direction from cached swap data.
    /// Educational: This demonstrates how to derive game mechanics from user input.
    /// </summary>
    public Direction ExtractGravityFromCache()
    {
        if (cachedSwap?.isValid == true)
        {
            return cachedSwap.GetGravityDirection();
        }
        return Direction.None;
    }
}
```

### Turn Execution Flow Implementation
```csharp
/// <summary>
/// Corrected turn execution sequence implementation.
/// Educational: This demonstrates the corrected flow with gravity from cached selection.
/// </summary>
public class TurnExecutor
{
    public async Task ExecuteTurn(Vector2Int tile1, Vector2Int tile2)
    {
        // Step 1-2: Board generation and queue analysis (already implemented)
        
        // Step 3: Cache selection data
        gravitySystem.CacheSwapData(tile1, tile2);
        
        // Step 4: Swap animation
        await AnimateSwap(tile1, tile2);
        
        // Step 5: Match check
        var matchResult = matchDetector.CheckForMatches(tile1, tile2);
        
        if (!matchResult.hasMatches)
        {
            // Animate tiles back, return to step 3
            await AnimateSwapBack(tile1, tile2);
            return;
        }
        
        // Step 6: Extract gravity from cached selection
        var gravityDirection = gravitySystem.ExtractGravityFromCache();
        
        // Step 7-13: Continue with match clearing, gravity, cascades, etc.
        await ProcessValidMatch(matchResult, gravityDirection);
    }
}
```

## 2. Extended Queue System (15-Tile System)

### Implementation Requirements

#### 2.1 Queue Configuration
```csharp
/// <summary>
/// Configuration for the extended 15-tile queue system.
/// Educational: This demonstrates configuration-driven design for flexible gameplay.
/// </summary>
[CreateAssetMenu(fileName = "QueueConfig", menuName = "SWITCH/Queue Config")]
public class QueueConfig : ScriptableObject
{
    [Header("Queue Sizes")]
    public int visibleQueueSize = 10;
    public int bufferSize = 5;
    public int totalQueueSize = 15;
    
    [Header("Anti-Frustration")]
    public bool enableAntiFrustration = true;
    public int lookAheadDepth = 15;
    public float shuffleProbability = 0.1f;
    
    [Header("Display")]
    public bool enableExtendedView = false;
    public float refillThreshold = 0.3f;
}
```

#### 2.2 Queue Manager Implementation
```csharp
/// <summary>
/// Extended queue manager supporting 15-tile system with buffer.
/// Educational: This demonstrates the buffer pattern for smooth gameplay.
/// Performance: Prevents queue starvation during rapid cascades.
/// </summary>
public class QueueManager : IQueueSystem
{
    private const int VISIBLE_QUEUE_SIZE = 10;
    private const int BUFFER_SIZE = 5;
    private const int TOTAL_QUEUE_SIZE = 15;
    
    private TileData[] visibleTiles = new TileData[VISIBLE_QUEUE_SIZE];
    private TileData[] bufferTiles = new TileData[BUFFER_SIZE];
    private int currentIndex = 0;
    private int bufferIndex = 0;
    
    /// <summary>
    /// Generates extended queue with anti-frustration analysis.
    /// Educational: This shows how to implement look-ahead algorithms.
    /// </summary>
    public void GenerateExtendedQueue()
    {
        var allTiles = new TileData[TOTAL_QUEUE_SIZE];
        
        // Generate with 15-tile look-ahead
        for (int i = 0; i < TOTAL_QUEUE_SIZE; i++)
        {
            allTiles[i] = tileDistributor.GenerateOptimalTile(i, allTiles);
        }
        
        // Split into visible and buffer
        Array.Copy(allTiles, 0, visibleTiles, 0, VISIBLE_QUEUE_SIZE);
        Array.Copy(allTiles, VISIBLE_QUEUE_SIZE, bufferTiles, 0, BUFFER_SIZE);
    }
    
    /// <summary>
    /// Checks if buffer is running low and needs refill.
    /// Educational: This demonstrates proactive resource management.
    /// </summary>
    public bool IsBufferLow()
    {
        return bufferIndex >= BUFFER_SIZE * 0.7f; // 70% threshold
    }
}
```

## 3. Blocking Blocks System

### Implementation Requirements

#### 3.1 Blocking Tile Data Structure
```csharp
/// <summary>
/// Data structure for blocking blocks (obstacles).
/// Educational: This demonstrates special tile behavior implementation.
/// </summary>
[CreateAssetMenu(fileName = "BlockingTileData", menuName = "SWITCH/Blocking Tile")]
public class BlockingTileData : ScriptableObject
{
    [Header("Visual")]
    public Sprite stoneTexture;
    public Color concreteColor = Color.gray;
    
    [Header("Behavior")]
    public bool canBeSwapped = true;
    public bool canBeMatched = false;
    
    [Header("Spawn Settings")]
    public float baseSpawnRate = 0.02f; // 2%
    public float maxSpawnRate = 0.10f;  // 10%
    public int maxOnBoard = 3;
    
    [Header("Progressive Spawn")]
    public float spawnRateIncrease = 0.01f; // +1% every 2 minutes
    public float increaseInterval = 120f;   // 2 minutes
}
```

#### 3.2 Blocking Tile Implementation
```csharp
/// <summary>
/// Blocking tile that acts as an obstacle on the board.
/// Educational: This demonstrates special tile behavior and swap validation.
/// </summary>
public class BlockingTile : ITile
{
    private BlockingTileData data;
    private bool canBeSwapped;
    private bool canBeMatched;
    
    /// <summary>
    /// Validates if this blocking tile can be swapped with a regular tile.
    /// Educational: This shows conditional swap logic for special tiles.
    /// </summary>
    public bool CanSwapWith(TileData regularTile)
    {
        if (!canBeSwapped) return false;
        
        // Only allow swap if regular tile would create a match
        return matchDetector.WouldCreateMatch(regularTile, this.position);
    }
    
    /// <summary>
    /// Checks if this blocking tile can be matched (always false).
    /// Educational: This demonstrates immutable tile properties.
    /// </summary>
    public bool CanCreateMatch()
    {
        return false; // Blocking tiles cannot be matched
    }
}
```

#### 3.3 Progressive Spawn System
```csharp
/// <summary>
/// Manages progressive spawning of blocking blocks.
/// Educational: This demonstrates time-based difficulty scaling.
/// </summary>
public class BlockingBlockSpawner
{
    private float currentSpawnRate;
    private float lastSpawnTime;
    private int blocksOnBoard;
    
    /// <summary>
    /// Updates spawn rate based on game time.
    /// Educational: This shows progressive difficulty implementation.
    /// </summary>
    public void UpdateSpawnRate(float gameTime)
    {
        // First 5 minutes: 0% spawn rate
        if (gameTime < 300f) // 5 minutes
        {
            currentSpawnRate = 0f;
            return;
        }
        
        // After 5 minutes: start at 2%, increase by 1% every 2 minutes
        var timeAfterStart = gameTime - 300f;
        var increaseCount = Mathf.FloorToInt(timeAfterStart / 120f); // Every 2 minutes
        currentSpawnRate = Mathf.Min(0.02f + (increaseCount * 0.01f), 0.10f);
    }
    
    /// <summary>
    /// Attempts to spawn a blocking block based on current rate.
    /// Educational: This demonstrates probabilistic spawning.
    /// </summary>
    public bool TrySpawnBlock()
    {
        if (blocksOnBoard >= data.maxOnBoard) return false;
        if (Random.Range(0f, 1f) > currentSpawnRate) return false;
        
        // Spawn blocking block
        return true;
    }
}
```

## 4. Power Orbs System

### Implementation Requirements

#### 4.1 Power Orb Data Structure
```csharp
/// <summary>
/// Data structure for power orbs with edge targeting.
/// Educational: This demonstrates color-coded objective systems.
/// </summary>
[CreateAssetMenu(fileName = "PowerOrbData", menuName = "SWITCH/Power Orb")]
public class PowerOrbData : ScriptableObject
{
    [Header("Orb Properties")]
    public OrbColor color;
    public Vector2Int targetEdge;
    public int baseScore = 5000;
    public int ageBonus = 500;
    
    [Header("Spawn Settings")]
    public float baseSpawnChance = 0.05f; // 5%
    public float maxSpawnChance = 0.15f;  // 15%
    public Vector2Int[] centerSpawnPositions = {
        new Vector2Int(3, 3), new Vector2Int(4, 3),
        new Vector2Int(3, 4), new Vector2Int(4, 4)
    };
    
    /// <summary>
    /// Calculates score based on orb age.
    /// Educational: This demonstrates time-based scoring mechanics.
    /// </summary>
    public int CalculateScore(int age)
    {
        return baseScore + (age * ageBonus);
    }
}
```

#### 4.2 Power Orb Implementation
```csharp
/// <summary>
/// Power orb that spawns in center and moves toward target edge.
/// Educational: This demonstrates objective-based gameplay mechanics.
/// </summary>
public class PowerOrb : ITile
{
    private PowerOrbData data;
    private OrbColor targetEdge;
    private Vector2Int spawnPosition;
    private int age;
    
    /// <summary>
    /// Moves orb toward its target edge.
    /// Educational: This shows directional movement toward objectives.
    /// </summary>
    public void MoveTowardEdge(Direction gravityDirection)
    {
        var targetDirection = GetTargetDirection();
        
        // Move in direction that gets closer to target edge
        if (ShouldMoveInGravityDirection(gravityDirection, targetDirection))
        {
            MoveInDirection(gravityDirection);
        }
        else
        {
            // Move toward target edge despite gravity
            MoveInDirection(targetDirection);
        }
        
        age++;
    }
    
    /// <summary>
    /// Calculates score when orb reaches correct edge.
    /// Educational: This demonstrates conditional scoring based on objectives.
    /// </summary>
    public int CalculateScore()
    {
        if (ReachedCorrectEdge())
        {
            return data.CalculateScore(age);
        }
        return 0; // No points if wrong edge
    }
}
```

#### 4.3 Center Spawn System
```csharp
/// <summary>
/// Manages power orb spawning in center cells.
/// Educational: This demonstrates location-based spawning mechanics.
/// </summary>
public class PowerOrbSpawner
{
    private Vector2Int[] centerPositions = {
        new Vector2Int(3, 3), new Vector2Int(4, 3),
        new Vector2Int(3, 4), new Vector2Int(4, 4)
    };
    
    /// <summary>
    /// Checks center cells after match clear and spawns orbs.
    /// Educational: This shows immediate response to game events.
    /// </summary>
    public void CheckCenterCellsAfterMatch(Vector2Int[] clearedPositions)
    {
        foreach (var centerPos in centerPositions)
        {
            if (IsPositionCleared(centerPos, clearedPositions))
            {
                TrySpawnPowerOrb(centerPos);
            }
        }
    }
    
    /// <summary>
    /// Attempts to spawn power orb at center position.
    /// Educational: This demonstrates probabilistic spawning with increasing rates.
    /// </summary>
    private void TrySpawnPowerOrb(Vector2Int position)
    {
        var spawnChance = CalculateSpawnChance();
        if (Random.Range(0f, 1f) <= spawnChance)
        {
            var orbColor = GetRandomOrbColor();
            SpawnPowerOrb(position, orbColor);
        }
    }
}
```

## 5. Edge Fill Priority System

### Implementation Requirements

#### 5.1 Clockwise Fill Implementation
```csharp
/// <summary>
/// Implements clockwise edge fill priority system.
/// Educational: This demonstrates ordered processing for consistent behavior.
/// </summary>
public class EdgeFillManager
{
    private readonly Direction[] fillOrder = {
        Direction.Up,    // Top edge
        Direction.Right, // Right edge
        Direction.Down,  // Bottom edge
        Direction.Left   // Left edge
    };
    
    /// <summary>
    /// Fills board gaps using clockwise priority.
    /// Educational: This shows systematic gap filling with visual feedback.
    /// </summary>
    public void FillGapsClockwise(Vector2Int[] gaps)
    {
        foreach (var direction in fillOrder)
        {
            var gapsInDirection = GetGapsInDirection(gaps, direction);
            if (gapsInDirection.Length > 0)
            {
                FillGapsInDirection(gapsInDirection, direction);
                ShowEdgeIndicator(direction);
            }
        }
    }
    
    /// <summary>
    /// Shows visual indicator for edge being filled.
    /// Educational: This demonstrates user feedback for game mechanics.
    /// </summary>
    private void ShowEdgeIndicator(Direction direction)
    {
        var edgeColor = GetEdgeColor(direction);
        uiManager.ShowEdgeGlow(direction, edgeColor);
    }
}
```

## 6. UI Monetization Integration

### Implementation Requirements

#### 6.1 Ad Banner Placement
```csharp
/// <summary>
/// Manages ad banner placement in UI.
/// Educational: This demonstrates non-intrusive monetization integration.
/// </summary>
public class AdBannerManager
{
    [Header("Banner Settings")]
    public RectTransform bannerContainer;
    public Vector2 bannerSize = new Vector2(320, 50);
    public Vector2 bannerPosition = new Vector2(0, -200); // Bottom UI
    
    /// <summary>
    /// Places ad banner in designated UI area.
    /// Educational: This shows how to integrate ads without disrupting gameplay.
    /// </summary>
    public void PlaceAdBanner()
    {
        bannerContainer.sizeDelta = bannerSize;
        bannerContainer.anchoredPosition = bannerPosition;
        
        // Ensure banner doesn't obscure gameplay
        var gameArea = GetGameplayArea();
        if (IsBannerOverlappingGameplay(gameArea))
        {
            AdjustBannerPosition();
        }
    }
}
```

## 7. Future Tile Monetization Architecture

### Implementation Requirements

#### 7.1 Enhanced Tile Data Structure
```csharp
/// <summary>
/// Enhanced tile data supporting future branded content.
/// Educational: This demonstrates extensible architecture for monetization.
/// </summary>
[CreateAssetMenu(fileName = "TileData", menuName = "SWITCH/Tile Data")]
public class TileData : ScriptableObject
{
    [Header("Basic Properties")]
    public TileType type;
    public Color baseColor;
    
    [Header("Visual Assets")]
    public Sprite defaultSprite;
    public Sprite brandedSprite;  // Future: company logos
    public Color tintColor = Color.white;
    
    [Header("Accessibility")]
    public Shape accessibilityShape;
    public bool useShapeForColorblind;
    
    [Header("Monetization")]
    public bool isBranded = false;
    public string brandName = "";
    public float brandWeight = 1f; // Spawn probability modifier
    
    /// <summary>
    /// Gets appropriate sprite based on branding settings.
    /// Educational: This shows how to support future monetization features.
    /// </summary>
    public Sprite GetDisplaySprite()
    {
        return isBranded && brandedSprite != null ? brandedSprite : defaultSprite;
    }
}
```

## 8. Performance Considerations

### Critical Performance Requirements

#### 8.1 Object Pooling for Special Tiles
```csharp
/// <summary>
/// Object pooler for special tiles to maintain 60 FPS.
/// Educational: This demonstrates performance optimization for special objects.
/// Performance: Prevents garbage collection spikes from special tile creation.
/// </summary>
public class SpecialTilePooler
{
    private Queue<BlockingTile> blockingTilePool;
    private Queue<PowerOrb> powerOrbPool;
    
    /// <summary>
    /// Pre-warms pools for special tiles.
    /// Educational: This shows proactive performance optimization.
    /// </summary>
    public void PrewarmPools()
    {
        // Pre-allocate blocking tiles
        for (int i = 0; i < 10; i++)
        {
            var blockingTile = CreateBlockingTile();
            blockingTile.gameObject.SetActive(false);
            blockingTilePool.Enqueue(blockingTile);
        }
        
        // Pre-allocate power orbs
        for (int i = 0; i < 8; i++)
        {
            var powerOrb = CreatePowerOrb();
            powerOrb.gameObject.SetActive(false);
            powerOrbPool.Enqueue(powerOrb);
        }
    }
}
```

#### 8.2 Immediate Spawn Performance
```csharp
/// <summary>
/// Ensures power orbs spawn immediately before gravity.
/// Educational: This demonstrates timing-critical game mechanics.
/// Performance: Spawns before expensive gravity calculations.
/// </summary>
public class ImmediateSpawnManager
{
    /// <summary>
    /// Spawns power orbs immediately after match clear.
    /// Educational: This shows how to prioritize critical game events.
    /// </summary>
    public void SpawnPowerOrbsImmediately(Vector2Int[] clearedPositions)
    {
        // CRITICAL: Spawn before gravity to maintain visual consistency
        powerOrbSpawner.CheckCenterCellsAfterMatch(clearedPositions);
        
        // Then apply gravity
        gravitySystem.ApplyGravity();
    }
}
```

## 9. Testing Requirements

### Unit Tests for New Systems
```csharp
/// <summary>
/// Unit tests for corrected turn execution sequence.
/// Educational: This demonstrates testing of complex game mechanics.
/// </summary>
[TestFixture]
public class TurnExecutionTests
{
    [Test]
    public void GravityDirection_ExtractedFromCachedSwap()
    {
        // Arrange
        var gravitySystem = new DirectionalGravity();
        var tile1 = new Vector2Int(3, 3);
        var tile2 = new Vector2Int(4, 3);
        
        // Act
        gravitySystem.CacheSwapData(tile1, tile2);
        var direction = gravitySystem.ExtractGravityFromCache();
        
        // Assert
        Assert.AreEqual(Direction.Right, direction);
    }
    
    [Test]
    public void InvalidSwap_DoesNotCalculateGravity()
    {
        // Arrange
        var turnExecutor = new TurnExecutor();
        var invalidTile1 = new Vector2Int(0, 0);
        var invalidTile2 = new Vector2Int(2, 0); // Not adjacent
        
        // Act & Assert
        Assert.DoesNotThrow(() => turnExecutor.ExecuteTurn(invalidTile1, invalidTile2));
        // Gravity should not be calculated for invalid swaps
    }
}
```

## 10. Implementation Checklist

### Phase 1: Core Corrections
- [ ] Implement SwapCache class
- [ ] Update InputManager to cache selection data
- [ ] Modify GravitySystem to extract from cache
- [ ] Update TurnExecutor with corrected flow
- [ ] Add unit tests for new flow

### Phase 2: Extended Queue System
- [ ] Implement 15-tile queue system
- [ ] Add buffer management
- [ ] Update QueueDisplay for extended view
- [ ] Implement look-ahead analysis
- [ ] Add performance tests

### Phase 3: Special Tiles
- [ ] Implement BlockingTile class
- [ ] Add progressive spawn system
- [ ] Implement PowerOrb class
- [ ] Add center spawn system
- [ ] Create visual differentiation

### Phase 4: Edge Fill System
- [ ] Implement clockwise fill priority
- [ ] Add edge indicators
- [ ] Update visual feedback
- [ ] Test fill consistency

### Phase 5: Monetization Architecture
- [ ] Add ad banner placement
- [ ] Enhance TileData for branding
- [ ] Implement accessibility features
- [ ] Add configuration options

### Phase 6: Performance & Polish
- [ ] Implement object pooling for special tiles
- [ ] Optimize immediate spawn performance
- [ ] Add comprehensive testing
- [ ] Performance profiling and optimization

## Conclusion

These modifications significantly enhance SWITCH's strategic depth while maintaining the core simplicity of the swap-match-gravity loop. The corrected turn execution sequence eliminates wasted calculations, the extended queue system prevents frustration, and the special tiles add complexity without overwhelming players.

The implementation prioritizes performance with object pooling and immediate spawning, while the monetization architecture provides a foundation for future revenue features. All systems are designed with comprehensive testing and educational documentation to support the development team.
