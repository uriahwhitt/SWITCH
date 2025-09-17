# SWITCH Core Algorithms

## Implementation Status
**Sprint 0**: Unity project setup complete ✅  
**Sprint 1**: Ready for algorithm implementation  
**Current State**: Foundation ready, algorithms to be implemented

## Match Detection Algorithm

### Overview
Efficiently detects matches of 3+ tiles in the 8x8 grid using optimized scanning techniques.

### Algorithm Steps
1. **Horizontal Scan**: Check each row for consecutive matches
2. **Vertical Scan**: Check each column for consecutive matches
3. **L-Shape Detection**: Find L-shaped and T-shaped matches
4. **Cross Detection**: Find cross-shaped matches
5. **Result Compilation**: Combine all matches, removing duplicates

### Implementation
```csharp
public class MatchDetector
{
    public List<Match> FindMatches(Tile[,] board)
    {
        var matches = new List<Match>();
        
        // Horizontal matches
        for (int y = 0; y < 8; y++)
        {
            matches.AddRange(FindHorizontalMatches(board, y));
        }
        
        // Vertical matches
        for (int x = 0; x < 8; x++)
        {
            matches.AddRange(FindVerticalMatches(board, x));
        }
        
        // L-shape and cross matches
        matches.AddRange(FindComplexMatches(board));
        
        return RemoveDuplicateMatches(matches);
    }
}
```

### Performance
- Time Complexity: O(n²) where n is grid size
- Space Complexity: O(m) where m is number of matches
- Optimized for mobile performance

## Directional Gravity Algorithm

### Overview
Implements player-controlled gravity where tiles flow from the chosen direction.

### Algorithm Steps
1. **Direction Input**: Player selects gravity direction
2. **Flow Calculation**: Calculate tile movement vectors
3. **Collision Detection**: Handle tile-to-tile collisions
4. **Movement Execution**: Apply smooth movement with physics
5. **Stability Check**: Verify all tiles are stable

### Implementation
```csharp
public class DirectionalGravity
{
    public void ApplyGravity(Direction direction, Tile[,] board)
    {
        var flowVector = GetFlowVector(direction);
        var movingTiles = GetMovingTiles(board, flowVector);
        
        while (movingTiles.Count > 0)
        {
            foreach (var tile in movingTiles)
            {
                tile.Move(flowVector);
            }
            
            movingTiles = GetMovingTiles(board, flowVector);
        }
    }
}
```

### Physics Integration
- Uses Unity's physics system for smooth movement
- Implements custom collision detection
- Handles edge cases and boundary conditions

## Smart Tile Distribution Algorithm

### Overview
Intelligent tile generation that ensures playability while maintaining challenge.

### Algorithm Steps
1. **Board Analysis**: Analyze current board state
2. **Match Potential**: Calculate available matches
3. **Distribution Rules**: Apply anti-frustration rules
4. **Color Selection**: Choose appropriate colors
5. **Validation**: Ensure at least one match is possible

### Implementation
```csharp
public class SmartTileDistribution
{
    public TileType[] GenerateTiles(BoardState boardState)
    {
        var analysis = AnalyzeBoard(boardState);
        var distribution = CalculateDistribution(analysis);
        var tiles = SelectTiles(distribution);
        
        return ValidateAndAdjust(tiles, boardState);
    }
    
    private DistributionAnalysis AnalyzeBoard(BoardState board)
    {
        return new DistributionAnalysis
        {
            MatchPotential = CalculateMatchPotential(board),
            ColorDistribution = GetColorDistribution(board),
            ClusterQuality = AnalyzeClusters(board),
            EdgePressure = CalculateEdgePressure(board)
        };
    }
}
```

### Anti-Frustration Rules
- Always ensure at least 1 valid match
- Prevent more than 4 consecutive identical colors
- Avoid isolated single tiles
- Provide clearing opportunities for edge pressure

## Cascade Resolution Algorithm

### Overview
Handles chain reactions when matches create new matches through tile falling.

### Algorithm Steps
1. **Initial Matches**: Process initial match results
2. **Gravity Application**: Apply gravity to remaining tiles
3. **New Match Detection**: Check for new matches after gravity
4. **Recursive Processing**: Repeat until no more matches
5. **Score Calculation**: Calculate total cascade score

### Implementation
```csharp
public class CascadeResolver
{
    public CascadeResult ResolveCascade(List<Match> initialMatches, Tile[,] board)
    {
        var result = new CascadeResult();
        var currentMatches = initialMatches;
        int cascadeLevel = 0;
        
        while (currentMatches.Count > 0)
        {
            cascadeLevel++;
            
            // Remove matched tiles
            RemoveMatchedTiles(currentMatches, board);
            
            // Apply gravity
            ApplyGravity(board);
            
            // Find new matches
            currentMatches = matchDetector.FindMatches(board);
            
            // Calculate score for this level
            result.AddCascadeLevel(cascadeLevel, currentMatches);
        }
        
        return result;
    }
}
```

### Performance Optimization
- Batch processing for multiple cascades
- Efficient tile removal and replacement
- Optimized gravity calculations
- Memory-efficient result storage

## Queue Management Algorithm

### Overview
Manages the 10-dot queue system with smooth animations and intelligent refilling.

### Algorithm Steps
1. **Queue Analysis**: Analyze current queue state
2. **Tile Selection**: Select tiles for deployment
3. **Animation Planning**: Plan smooth drop animations
4. **Refill Calculation**: Calculate new tiles for queue
5. **Animation Execution**: Execute drop and refill animations

### Implementation
```csharp
public class QueueSystem
{
    public void DeployTiles(int count, Direction direction)
    {
        var tilesToDeploy = GetTilesFromQueue(count);
        var deploymentPlan = PlanDeployment(tilesToDeploy, direction);
        
        StartCoroutine(ExecuteDeployment(deploymentPlan));
    }
    
    private IEnumerator ExecuteDeployment(DeploymentPlan plan)
    {
        // Drop animation
        yield return AnimateDrop(plan.Tiles);
        
        // Refill animation
        yield return AnimateRefill(plan.NewTiles);
        
        // Update queue state
        UpdateQueueState();
    }
}
```

### Animation System
- Smooth interpolation for tile movement
- Coordinated timing for multiple tiles
- Visual feedback for queue state
- Performance-optimized animation pooling

## Score Calculation Algorithm

### Overview
Calculates scores with multipliers for matches, cascades, and special combinations.

### Algorithm Steps
1. **Base Score**: Calculate base score for matches
2. **Multiplier Application**: Apply cascade and combo multipliers
3. **Special Bonuses**: Add bonuses for special combinations
4. **Time Bonuses**: Apply time-based bonuses
5. **Final Calculation**: Calculate total score with all modifiers

### Implementation
```csharp
public class ScoreCalculator
{
    public int CalculateScore(List<Match> matches, int cascadeLevel, float timeBonus)
    {
        int baseScore = 0;
        float multiplier = 1.0f;
        
        foreach (var match in matches)
        {
            baseScore += GetMatchScore(match);
            multiplier += GetMatchMultiplier(match);
        }
        
        // Apply cascade multiplier
        multiplier *= GetCascadeMultiplier(cascadeLevel);
        
        // Apply time bonus
        multiplier *= (1.0f + timeBonus);
        
        return Mathf.RoundToInt(baseScore * multiplier);
    }
}
```

### Scoring Rules
- 3-match: 100 points
- 4-match: 300 points
- 5-match: 500 points
- L-shape: 2x multiplier
- Cross: 3x multiplier
- Cascade: +0.5x per level

## Performance Considerations

### Optimization Strategies
- Efficient data structures for tile storage
- Cached calculations for repeated operations
- Object pooling for temporary objects
- Batch processing for multiple operations

### Memory Management
- Minimize allocations in hot paths
- Reuse data structures where possible
- Efficient garbage collection patterns
- Memory profiling integration

### Mobile Optimization
- Optimize for 60 FPS target
- Minimize CPU usage
- Efficient battery usage
- Thermal management considerations
