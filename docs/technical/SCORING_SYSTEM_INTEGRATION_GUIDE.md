# SWITCH Scoring System Integration Guide

## Overview

This guide provides step-by-step instructions for integrating the momentum-based scoring system into your SWITCH Unity project. The scoring system includes heat management, power orbs, dynamic audio, and visual effects.

## Prerequisites

- Unity 2022.3 LTS
- All scoring system scripts implemented
- Basic game structure (GameManager, BoardController, etc.)

## Integration Steps

### 1. Core System Setup

#### 1.1 Add Scoring Components to GameManager

1. Open `GameManager.cs`
2. Add the following fields to the GameManager class:

```csharp
[Header("Scoring System")]
[SerializeField] private MomentumSystem momentumSystem;
[SerializeField] private TurnScoreCalculator scoreCalculator;
[SerializeField] private PowerOrbManager powerOrbManager;
```

3. Add scoring properties:

```csharp
// Scoring system
public int CurrentScore { get; private set; } = 0;
public int HighScore { get; private set; } = 0;
```

4. Initialize components in `InitializeGame()`:

```csharp
// Initialize scoring system components
if (momentumSystem == null)
    momentumSystem = GetComponent<MomentumSystem>();
if (scoreCalculator == null)
    scoreCalculator = GetComponent<TurnScoreCalculator>();
if (powerOrbManager == null)
    powerOrbManager = FindObjectOfType<PowerOrbManager>();
```

#### 1.2 Add Scoring Methods to GameManager

Add these methods to handle scoring:

```csharp
// Scoring System Integration
public void HandleTurnComplete(TurnResult result)
{
    if (scoreCalculator == null) return;
    
    var scoreResult = scoreCalculator.CalculateTurnScore(result);
    UpdateScore(scoreResult.FinalScore);
    
    Log($"Turn complete: +{scoreResult.FinalScore} points (Heat: {scoreResult.FinalHeat:F1}, Multiplier: {scoreResult.Multiplier:F1}x)");
}

public void UpdateScore(int points)
{
    CurrentScore += points;
    
    if (CurrentScore > HighScore)
    {
        HighScore = CurrentScore;
        Log($"New high score: {HighScore}");
    }
}

public void ResetScore()
{
    CurrentScore = 0;
    if (momentumSystem != null)
        momentumSystem.ResetMomentum();
}

public float GetCurrentHeat()
{
    return momentumSystem != null ? momentumSystem.CurrentMomentum : 0f;
}

public float GetScoreMultiplier()
{
    return momentumSystem != null ? momentumSystem.GetScoreMultiplier() : 1f;
}
```

### 2. BoardController Integration

#### 2.1 Add Power Orb Manager Reference

1. Open `BoardController.cs`
2. Add power orb manager reference:

```csharp
[Header("References")]
[SerializeField] private GameObject tilePrefab;
[SerializeField] private Transform boardParent;
[SerializeField] private PowerOrbManager powerOrbManager;
```

#### 2.2 Add Power Orb-Aware Match Clearing

Add this method to handle power orb destruction during matches:

```csharp
public void ClearMatches(List<Vector2Int> positions)
{
    foreach (var position in positions)
    {
        // Check for power orb at position
        if (powerOrbManager != null)
        {
            var orb = powerOrbManager.GetOrbAtPosition(position);
            if (orb != null)
            {
                orb.LoseOrb(); // Orb destroyed by match
            }
        }
        
        // Clear tile at position
        if (IsValidPosition(position) && board[position.x, position.y] != null)
        {
            board[position.x, position.y].Clear();
        }
    }
}
```

### 3. MatchDetector Integration

#### 3.1 Create TurnResult Objects

Update your MatchDetector to create TurnResult objects:

```csharp
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
```

#### 3.2 Connect to GameManager

In your match detection logic, call:

```csharp
var turnResult = CreateTurnResult(matches);
GameManager.Instance.HandleTurnComplete(turnResult);
```

### 4. Unity Scene Setup

#### 4.1 Create GameManager GameObject

1. Create empty GameObject named "GameManager"
2. Add `GameManager` component
3. Add `MomentumSystem` component
4. Add `TurnScoreCalculator` component
5. Add `HeatAudioManager` component

#### 4.2 Create Power Orb Manager

1. Create empty GameObject named "PowerOrbManager"
2. Add `PowerOrbManager` component
3. Create power orb prefab with `PowerOrb` component
4. Assign prefab to PowerOrbManager

#### 4.3 Create Heat UI Manager

1. Create Canvas for heat UI
2. Add heat meter UI elements:
   - Slider for heat meter
   - Text for multiplier display
   - Text for heat level display
3. Add `HeatUIManager` component
4. Assign UI elements to HeatUIManager

#### 4.4 Configure Audio Sources

1. Create AudioSource GameObjects for each layer:
   - BaseLayer (always playing)
   - RhythmLayer (fades in at 3+ heat)
   - MelodyLayer (fades in at 6+ heat)
   - ClimaxLayer (fades in at 9+ heat)
2. Assign to HeatAudioManager

### 5. Power Orb Configuration

#### 5.1 Create Power Orb Data Assets

1. Right-click in Project window
2. Create > SWITCH > Power Orb
3. Configure each orb type:
   - Set color and target edge
   - Configure spawn chances
   - Set visual properties
   - Define center spawn positions

#### 5.2 Assign to Power Orb Manager

1. Select PowerOrbManager GameObject
2. Drag PowerOrbData assets to PowerOrbTypes array
3. Assign power orb prefab
4. Set spawn check interval

### 6. Testing Integration

#### 6.1 Run Unit Tests

```bash
# Run scoring system tests
./run-tests.sh EditMode

# Specific test categories
# - MomentumSystemTests
# - TurnScoreCalculatorTests
# - PowerOrbTests
# - PowerOrbManagerTests
```

#### 6.2 Manual Testing Checklist

- [ ] Heat builds with Match-4 and Match-5
- [ ] Heat decays by 1.0 each turn
- [ ] Score multiplier increases with heat
- [ ] Power orbs spawn in center positions
- [ ] Power orbs move toward target edges
- [ ] Power orb collection triggers max heat
- [ ] Audio layers fade in/out with heat
- [ ] Visual effects change with heat level
- [ ] Heat meter updates correctly
- [ ] Score calculation includes all modifiers

### 7. Performance Optimization

#### 7.1 Audio Optimization

- Use compressed audio formats
- Implement audio pooling for sound effects
- Limit simultaneous audio sources

#### 7.2 Visual Optimization

- Use object pooling for particle effects
- Implement LOD system for particles
- Use UI Canvas for heat meter

#### 7.3 Memory Management

- Limit active power orbs to 4 maximum
- Properly clean up coroutines
- Use events for loose coupling

### 8. Troubleshooting

#### Common Issues

**Heat not building:**
- Check MomentumSystem is properly initialized
- Verify TurnScoreCalculator is calling momentum methods
- Ensure events are properly subscribed

**Power orbs not spawning:**
- Check PowerOrbData configuration
- Verify spawn chances are > 0
- Ensure center positions are valid

**Audio not responding:**
- Check HeatAudioManager is subscribed to MomentumSystem events
- Verify AudioSource components are assigned
- Check audio clips are assigned

**Visual effects not working:**
- Check HeatUIManager is subscribed to events
- Verify UI elements are assigned
- Ensure particle systems are configured

#### Debug Tools

Enable debug logging in components:
- MomentumSystem: `enableDebugLogs = true`
- TurnScoreCalculator: `enableDebugLogs = true`
- PowerOrbManager: `enableDebugLogs = true`

### 9. Advanced Configuration

#### 9.1 Customizing Heat Values

Modify heat generation values in MomentumSystem:
- `match4Heat`: Heat from Match-4 (default: 1.0)
- `match5Heat`: Heat from Match-5 (default: 2.0)
- `cascadeHeat`: Heat per cascade level (default: 0.5)
- `turnEndDecay`: Heat lost per turn (default: 1.0)

#### 9.2 Customizing Score Multipliers

Modify position multipliers in TurnScoreCalculator:
- `edgeMultiplier`: Edge position multiplier (default: 1)
- `transitionMultiplier`: Transition position multiplier (default: 2)
- `centerMultiplier`: Center position multiplier (default: 3)

#### 9.3 Customizing Power Orb Behavior

Modify PowerOrbData assets:
- `baseScore`: Base points for orb (default: 5000)
- `ageBonus`: Points per turn survived (default: 500)
- `baseSpawnChance`: Initial spawn chance (default: 0.05)
- `maxSpawnChance`: Maximum spawn chance (default: 0.15)

## Conclusion

The scoring system is now fully integrated into your SWITCH project. The momentum-based heat system provides engaging risk/reward gameplay, while power orbs add strategic depth. The dynamic audio and visual systems provide clear feedback to players about their current state and progress.

For additional support, refer to:
- `docs/technical/SWITCH_SCORING_SYSTEM.md` - Complete system documentation
- `docs/architecture/SWITCH_system_architecture.md` - System architecture
- Unit tests in `tests/EditMode/Core/` and `tests/EditMode/PowerUps/`
