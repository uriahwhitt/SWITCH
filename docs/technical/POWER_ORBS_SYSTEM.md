# Power Orbs System - Technical Documentation

## Overview
Power Orbs are glowing colored orbs that spawn in the center of the board when center cells are cleared by matches. They provide high-value scoring opportunities by moving toward specific edges based on their color. This system adds strategic depth by creating objectives that players must work toward over multiple turns.

## Design Philosophy

### Strategic Purpose
- **Center Pressure**: Encourages players to focus on center board management
- **Objective-Based Scoring**: Creates clear goals with high reward potential
- **Risk vs Reward**: High scores for correct edge, no points for wrong edge
- **Age-Based Scoring**: Rewards players for keeping orbs alive longer

### Visual Design
- **Glowing Effect**: Pulsing glow to indicate special nature
- **Color-Coded**: Each color corresponds to a specific edge
- **Edge Indicators**: Matching colored borders show target edges
- **Clear Feedback**: Success/failure animations at edges

## Technical Implementation

### 1. Power Orb Data Structure

```csharp
/// <summary>
/// Configuration data for power orbs with edge targeting.
/// Educational: This demonstrates color-coded objective systems.
/// </summary>
[CreateAssetMenu(fileName = "PowerOrbData", menuName = "SWITCH/Power Orb")]
public class PowerOrbData : ScriptableObject
{
    [Header("Orb Properties")]
    [Tooltip("Color of the power orb")]
    public OrbColor color;
    
    [Tooltip("Target edge for this orb color")]
    public Vector2Int targetEdge;
    
    [Tooltip("Base score value")]
    [Range(1000, 10000)]
    public int baseScore = 5000;
    
    [Tooltip("Score bonus per turn survived")]
    [Range(100, 1000)]
    public int ageBonus = 500;
    
    [Header("Spawn Configuration")]
    [Tooltip("Base spawn chance when center cell is cleared")]
    [Range(0f, 0.5f)]
    public float baseSpawnChance = 0.05f; // 5%
    
    [Tooltip("Maximum spawn chance (increases over time)")]
    [Range(0f, 0.5f)]
    public float maxSpawnChance = 0.15f; // 15%
    
    [Tooltip("Time interval for spawn chance increases")]
    [Range(60f, 300f)]
    public float increaseInterval = 120f; // 2 minutes
    
    [Header("Center Spawn Positions")]
    [Tooltip("Positions in center where orbs can spawn")]
    public Vector2Int[] centerSpawnPositions = {
        new Vector2Int(3, 3), new Vector2Int(4, 3),
        new Vector2Int(3, 4), new Vector2Int(4, 4)
    };
    
    [Header("Visual Properties")]
    [Tooltip("Glow color for this orb")]
    public Color glowColor;
    
    [Tooltip("Pulse speed for glow effect")]
    [Range(0.5f, 3f)]
    public float pulseSpeed = 1.5f;
    
    [Tooltip("Glow intensity")]
    [Range(0.5f, 2f)]
    public float glowIntensity = 1.2f;
    
    /// <summary>
    /// Calculates score based on orb age.
    /// Educational: This demonstrates time-based scoring mechanics.
    /// </summary>
    public int CalculateScore(int age)
    {
        return baseScore + (age * ageBonus);
    }
    
    /// <summary>
    /// Gets the spawn chance based on game time.
    /// Educational: This shows progressive spawn rate calculation.
    /// </summary>
    public float GetSpawnChance(float gameTime)
    {
        var increaseCount = Mathf.Floor(gameTime / increaseInterval);
        return Mathf.Min(
            baseSpawnChance + (increaseCount * 0.01f), // +1% per interval
            maxSpawnChance
        );
    }
}
```

### 2. Power Orb Implementation

```csharp
/// <summary>
/// Power orb that spawns in center and moves toward target edge.
/// Educational: This demonstrates objective-based gameplay mechanics.
/// </summary>
public class PowerOrb : MonoBehaviour, ITile
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer glowRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D orbCollider;
    
    [Header("Data")]
    [SerializeField] private PowerOrbData data;
    
    [Header("State")]
    [SerializeField] private Vector2Int boardPosition;
    [SerializeField] private Vector2Int targetEdge;
    [SerializeField] private int age = 0;
    [SerializeField] private bool isActive = false;
    [SerializeField] private bool hasReachedEdge = false;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem glowParticles;
    [SerializeField] private Light orbLight;
    
    private Coroutine glowCoroutine;
    private Coroutine movementCoroutine;
    
    /// <summary>
    /// Initializes the power orb with data and position.
    /// Educational: This shows how special tiles are configured with objectives.
    /// </summary>
    public void Initialize(PowerOrbData orbData, Vector2Int position)
    {
        data = orbData;
        boardPosition = position;
        targetEdge = data.targetEdge;
        age = 0;
        hasReachedEdge = false;
        
        // Set visual properties
        spriteRenderer.color = data.glowColor;
        glowRenderer.color = data.glowColor;
        orbLight.color = data.glowColor;
        
        // Set up collider for input detection
        orbCollider.enabled = true;
        
        isActive = true;
        
        // Start visual effects
        StartGlowEffect();
        PlaySpawnAnimation();
        
        // Subscribe to game events
        GameManager.Instance.OnTurnComplete += OnTurnComplete;
    }
    
    /// <summary>
    /// Handles turn completion to age the orb.
    /// Educational: This demonstrates event-driven aging mechanics.
    /// </summary>
    private void OnTurnComplete()
    {
        if (isActive && !hasReachedEdge)
        {
            age++;
            UpdateAgeVisuals();
        }
    }
    
    /// <summary>
    /// Moves orb toward its target edge based on gravity direction.
    /// Educational: This shows directional movement toward objectives.
    /// </summary>
    public void MoveTowardEdge(Direction gravityDirection)
    {
        if (!isActive || hasReachedEdge) return;
        
        var targetDirection = GetOptimalDirection(gravityDirection);
        var newPosition = CalculateNewPosition(targetDirection);
        
        if (IsValidPosition(newPosition))
        {
            SetPosition(newPosition);
            CheckEdgeReached();
        }
    }
    
    /// <summary>
    /// Calculates the optimal direction to move toward target edge.
    /// Educational: This demonstrates pathfinding toward objectives.
    /// </summary>
    private Direction GetOptimalDirection(Direction gravityDirection)
    {
        var currentPos = boardPosition;
        var targetPos = targetEdge;
        
        // Calculate direction toward target edge
        var deltaX = targetPos.x - currentPos.x;
        var deltaY = targetPos.y - currentPos.y;
        
        // Determine primary direction toward target
        Direction targetDirection;
        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            targetDirection = deltaX > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            targetDirection = deltaY > 0 ? Direction.Down : Direction.Up;
        }
        
        // Prefer gravity direction if it helps reach target
        if (IsDirectionTowardTarget(gravityDirection))
        {
            return gravityDirection;
        }
        
        // Otherwise use target direction
        return targetDirection;
    }
    
    /// <summary>
    /// Checks if a direction helps move toward the target edge.
    /// Educational: This shows how to evaluate movement options.
    /// </summary>
    private bool IsDirectionTowardTarget(Direction direction)
    {
        var currentPos = boardPosition;
        var targetPos = targetEdge;
        
        switch (direction)
        {
            case Direction.Up:
                return currentPos.y > targetPos.y;
            case Direction.Down:
                return currentPos.y < targetPos.y;
            case Direction.Left:
                return currentPos.x > targetPos.x;
            case Direction.Right:
                return currentPos.x < targetPos.x;
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Calculates new position based on movement direction.
    /// Educational: This demonstrates position calculation for movement.
    /// </summary>
    private Vector2Int CalculateNewPosition(Direction direction)
    {
        var newPosition = boardPosition;
        
        switch (direction)
        {
            case Direction.Up:
                newPosition.y--;
                break;
            case Direction.Down:
                newPosition.y++;
                break;
            case Direction.Left:
                newPosition.x--;
                break;
            case Direction.Right:
                newPosition.x++;
                break;
        }
        
        return newPosition;
    }
    
    /// <summary>
    /// Checks if the orb has reached its target edge.
    /// Educational: This shows objective completion detection.
    /// </summary>
    private void CheckEdgeReached()
    {
        if (IsAtTargetEdge())
        {
            OnReachCorrectEdge();
        }
        else if (IsAtAnyEdge())
        {
            OnReachWrongEdge();
        }
    }
    
    /// <summary>
    /// Checks if orb is at its target edge.
    /// Educational: This demonstrates objective validation.
    /// </summary>
    private bool IsAtTargetEdge()
    {
        var boardSize = BoardController.Instance.BoardSize;
        
        switch (data.color)
        {
            case OrbColor.Blue: // Top edge
                return boardPosition.y == 0;
            case OrbColor.Green: // Right edge
                return boardPosition.x == boardSize.x - 1;
            case OrbColor.Yellow: // Bottom edge
                return boardPosition.y == boardSize.y - 1;
            case OrbColor.Purple: // Left edge
                return boardPosition.x == 0;
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Checks if orb is at any edge.
    /// Educational: This shows edge detection logic.
    /// </summary>
    private bool IsAtAnyEdge()
    {
        var boardSize = BoardController.Instance.BoardSize;
        return boardPosition.x == 0 || boardPosition.x == boardSize.x - 1 ||
               boardPosition.y == 0 || boardPosition.y == boardSize.y - 1;
    }
    
    /// <summary>
    /// Handles orb reaching the correct edge.
    /// Educational: This demonstrates success state handling.
    /// </summary>
    private void OnReachCorrectEdge()
    {
        hasReachedEdge = true;
        var score = data.CalculateScore(age);
        
        // Award score
        GameManager.Instance.AddScore(score);
        
        // Play success animation
        PlaySuccessAnimation();
        
        // Show score popup
        UIManager.Instance.ShowScorePopup(score, transform.position);
        
        // Remove from board
        StartCoroutine(RemoveFromBoard());
        
        Debug.Log($"Power Orb reached correct edge! Score: {score} (Age: {age})");
    }
    
    /// <summary>
    /// Handles orb reaching the wrong edge.
    /// Educational: This demonstrates failure state handling.
    /// </summary>
    private void OnReachWrongEdge()
    {
        hasReachedEdge = true;
        
        // No score awarded
        GameManager.Instance.AddScore(0);
        
        // Play failure animation
        PlayFailureAnimation();
        
        // Remove from board
        StartCoroutine(RemoveFromBoard());
        
        Debug.Log($"Power Orb reached wrong edge! No score awarded (Age: {age})");
    }
    
    /// <summary>
    /// Starts the glow effect for the orb.
    /// Educational: This demonstrates visual effect management.
    /// </summary>
    private void StartGlowEffect()
    {
        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
        }
        
        glowCoroutine = StartCoroutine(PulseGlow());
    }
    
    /// <summary>
    /// Coroutine for pulsing glow effect.
    /// Educational: This shows how to create smooth visual effects.
    /// </summary>
    private IEnumerator PulseGlow()
    {
        var baseIntensity = data.glowIntensity;
        var time = 0f;
        
        while (isActive && !hasReachedEdge)
        {
            time += Time.deltaTime * data.pulseSpeed;
            var intensity = baseIntensity + Mathf.Sin(time) * 0.3f;
            
            glowRenderer.color = new Color(data.glowColor.r, data.glowColor.g, data.glowColor.b, intensity);
            orbLight.intensity = intensity;
            
            yield return null;
        }
    }
    
    /// <summary>
    /// Updates visual effects based on orb age.
    /// Educational: This demonstrates age-based visual feedback.
    /// </summary>
    private void UpdateAgeVisuals()
    {
        // Increase glow intensity with age
        var ageMultiplier = 1f + (age * 0.1f);
        data.glowIntensity = Mathf.Min(1.2f * ageMultiplier, 2f);
        
        // Update particle system
        if (glowParticles != null)
        {
            var emission = glowParticles.emission;
            emission.rateOverTime = 10f + (age * 2f);
        }
    }
    
    /// <summary>
    /// Plays spawn animation for the orb.
    /// Educational: This shows how to create engaging spawn effects.
    /// </summary>
    private void PlaySpawnAnimation()
    {
        animator.SetTrigger("Spawn");
        
        // Play spawn sound
        AudioManager.Instance.PlaySound("PowerOrbSpawn");
        
        // Show edge indicator
        UIManager.Instance.ShowEdgeIndicator(data.color, targetEdge);
    }
    
    /// <summary>
    /// Plays success animation when orb reaches correct edge.
    /// Educational: This demonstrates success feedback.
    /// </summary>
    private void PlaySuccessAnimation()
    {
        animator.SetTrigger("Success");
        
        // Play success sound
        AudioManager.Instance.PlaySound("PowerOrbSuccess");
        
        // Show celebration particles
        if (glowParticles != null)
        {
            glowParticles.Play();
        }
    }
    
    /// <summary>
    /// Plays failure animation when orb reaches wrong edge.
    /// Educational: This demonstrates failure feedback.
    /// </summary>
    private void PlayFailureAnimation()
    {
        animator.SetTrigger("Failure");
        
        // Play failure sound
        AudioManager.Instance.PlaySound("PowerOrbFailure");
        
        // Show dissolve effect
        StartCoroutine(DissolveEffect());
    }
    
    /// <summary>
    /// Coroutine for dissolve effect on failure.
    /// Educational: This shows how to create failure visual feedback.
    /// </summary>
    private IEnumerator DissolveEffect()
    {
        var duration = 1f;
        var elapsed = 0f;
        var startAlpha = spriteRenderer.color.a;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            
            var color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
            
            yield return null;
        }
    }
    
    /// <summary>
    /// Coroutine to remove orb from board after animation.
    /// Educational: This shows how to handle delayed object destruction.
    /// </summary>
    private IEnumerator RemoveFromBoard()
    {
        yield return new WaitForSeconds(1f); // Wait for animation
        
        // Unsubscribe from events
        GameManager.Instance.OnTurnComplete -= OnTurnComplete;
        
        // Return to object pool
        PowerOrbPooler.Instance.ReturnPowerOrb(this);
    }
    
    #region ITile Implementation
    
    public TileType Type => TileType.PowerOrb;
    public Vector2Int Position => boardPosition;
    public bool IsActive => isActive;
    
    public void SetPosition(Vector2Int newPosition)
    {
        boardPosition = newPosition;
        transform.position = BoardController.Instance.GetWorldPosition(newPosition);
    }
    
    public void AnimateToPosition(Vector3 worldPosition)
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
        
        movementCoroutine = StartCoroutine(AnimateMovement(worldPosition));
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

### 3. Center Spawn System

```csharp
/// <summary>
/// Manages power orb spawning in center cells.
/// Educational: This demonstrates location-based spawning mechanics.
/// </summary>
public class PowerOrbSpawner : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private PowerOrbData[] orbDataArray;
    [SerializeField] private BoardController boardController;
    [SerializeField] private GameManager gameManager;
    
    [Header("Center Positions")]
    [SerializeField] private Vector2Int[] centerPositions = {
        new Vector2Int(3, 3), new Vector2Int(4, 3),
        new Vector2Int(3, 4), new Vector2Int(4, 4)
    };
    
    [Header("State")]
    [SerializeField] private float gameStartTime;
    [SerializeField] private int orbsSpawned = 0;
    [SerializeField] private int orbsCollected = 0;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogging = false;
    
    private void Start()
    {
        gameStartTime = Time.time;
    }
    
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
    /// Checks if a center position was cleared by the match.
    /// Educational: This demonstrates position-based event handling.
    /// </summary>
    private bool IsPositionCleared(Vector2Int position, Vector2Int[] clearedPositions)
    {
        return clearedPositions.Contains(position);
    }
    
    /// <summary>
    /// Attempts to spawn power orb at center position.
    /// Educational: This demonstrates probabilistic spawning with increasing rates.
    /// </summary>
    private void TrySpawnPowerOrb(Vector2Int position)
    {
        var gameTime = Time.time - gameStartTime;
        var spawnChance = CalculateSpawnChance(gameTime);
        
        if (Random.Range(0f, 1f) <= spawnChance)
        {
            var orbColor = GetRandomOrbColor();
            SpawnPowerOrb(position, orbColor);
        }
    }
    
    /// <summary>
    /// Calculates spawn chance based on game time.
    /// Educational: This shows progressive spawn rate calculation.
    /// </summary>
    private float CalculateSpawnChance(float gameTime)
    {
        var baseChance = 0.05f; // 5% base
        var increaseCount = Mathf.Floor(gameTime / 120f); // Every 2 minutes
        var increaseAmount = increaseCount * 0.01f; // +1% per interval
        
        return Mathf.Min(baseChance + increaseAmount, 0.15f); // Max 15%
    }
    
    /// <summary>
    /// Gets a random orb color for spawning.
    /// Educational: This demonstrates weighted random selection.
    /// </summary>
    private OrbColor GetRandomOrbColor()
    {
        var colors = new OrbColor[] { OrbColor.Blue, OrbColor.Green, OrbColor.Yellow, OrbColor.Purple };
        return colors[Random.Range(0, colors.Length)];
    }
    
    /// <summary>
    /// Spawns a power orb at the specified position.
    /// Educational: This demonstrates object pooling for special tiles.
    /// </summary>
    private void SpawnPowerOrb(Vector2Int position, OrbColor color)
    {
        var orbData = GetOrbDataForColor(color);
        if (orbData == null)
        {
            Debug.LogError($"No orb data found for color: {color}");
            return;
        }
        
        var powerOrb = PowerOrbPooler.Instance.GetPowerOrb();
        powerOrb.Initialize(orbData, position);
        
        // Place on board
        boardController.PlaceTile(powerOrb, position);
        
        // Update statistics
        orbsSpawned++;
        
        if (enableDebugLogging)
        {
            Debug.Log($"Spawned {color} Power Orb at {position} (Total spawned: {orbsSpawned})");
        }
    }
    
    /// <summary>
    /// Gets orb data for the specified color.
    /// Educational: This shows how to retrieve configuration data.
    /// </summary>
    private PowerOrbData GetOrbDataForColor(OrbColor color)
    {
        return orbDataArray.FirstOrDefault(data => data.color == color);
    }
    
    /// <summary>
    /// Handles power orb collection (success or failure).
    /// Educational: This demonstrates event handling for special tile completion.
    /// </summary>
    public void OnPowerOrbCollected(bool success, int score)
    {
        orbsCollected++;
        
        if (enableDebugLogging)
        {
            var result = success ? "Success" : "Failure";
            Debug.Log($"Power Orb {result}! Score: {score} (Total collected: {orbsCollected})");
        }
    }
    
    /// <summary>
    /// Gets spawn statistics for debugging.
    /// Educational: This shows how to monitor system performance.
    /// </summary>
    public PowerOrbStatistics GetStatistics()
    {
        return new PowerOrbStatistics
        {
            orbsSpawned = orbsSpawned,
            orbsCollected = orbsCollected,
            successRate = orbsCollected > 0 ? (float)orbsCollected / orbsSpawned : 0f,
            gameTime = Time.time - gameStartTime
        };
    }
}

/// <summary>
/// Statistics for power orb system monitoring.
/// Educational: This demonstrates performance monitoring data structures.
/// </summary>
[System.Serializable]
public struct PowerOrbStatistics
{
    public int orbsSpawned;
    public int orbsCollected;
    public float successRate;
    public float gameTime;
}
```

### 4. Edge Indicator System

```csharp
/// <summary>
/// Manages edge indicators for power orb targets.
/// Educational: This demonstrates visual feedback for objectives.
/// </summary>
public class EdgeIndicatorManager : MonoBehaviour
{
    [Header("Edge Indicators")]
    [SerializeField] private GameObject topEdgeIndicator;
    [SerializeField] private GameObject rightEdgeIndicator;
    [SerializeField] private GameObject bottomEdgeIndicator;
    [SerializeField] private GameObject leftEdgeIndicator;
    
    [Header("Colors")]
    [SerializeField] private Color blueColor = Color.blue;
    [SerializeField] private Color greenColor = Color.green;
    [SerializeField] private Color yellowColor = Color.yellow;
    [SerializeField] private Color purpleColor = Color.magenta;
    
    private Dictionary<OrbColor, Color> colorMap;
    private Dictionary<OrbColor, GameObject> indicatorMap;
    
    private void Start()
    {
        InitializeColorMap();
        InitializeIndicatorMap();
        HideAllIndicators();
    }
    
    /// <summary>
    /// Initializes the color mapping for orb colors.
    /// Educational: This shows how to create lookup tables for efficiency.
    /// </summary>
    private void InitializeColorMap()
    {
        colorMap = new Dictionary<OrbColor, Color>
        {
            { OrbColor.Blue, blueColor },
            { OrbColor.Green, greenColor },
            { OrbColor.Yellow, yellowColor },
            { OrbColor.Purple, purpleColor }
        };
    }
    
    /// <summary>
    /// Initializes the indicator mapping for edges.
    /// Educational: This demonstrates how to organize UI elements.
    /// </summary>
    private void InitializeIndicatorMap()
    {
        indicatorMap = new Dictionary<OrbColor, GameObject>
        {
            { OrbColor.Blue, topEdgeIndicator },
            { OrbColor.Green, rightEdgeIndicator },
            { OrbColor.Yellow, bottomEdgeIndicator },
            { OrbColor.Purple, leftEdgeIndicator }
        };
    }
    
    /// <summary>
    /// Shows edge indicator for the specified orb color.
    /// Educational: This demonstrates visual feedback for objectives.
    /// </summary>
    public void ShowEdgeIndicator(OrbColor orbColor, Vector2Int targetEdge)
    {
        if (indicatorMap.TryGetValue(orbColor, out var indicator))
        {
            indicator.SetActive(true);
            
            // Set color
            var color = colorMap[orbColor];
            var renderer = indicator.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = color;
            }
            
            // Start pulse animation
            StartCoroutine(PulseIndicator(indicator));
        }
    }
    
    /// <summary>
    /// Hides edge indicator for the specified orb color.
    /// Educational: This shows how to manage visual feedback lifecycle.
    /// </summary>
    public void HideEdgeIndicator(OrbColor orbColor)
    {
        if (indicatorMap.TryGetValue(orbColor, out var indicator))
        {
            indicator.SetActive(false);
        }
    }
    
    /// <summary>
    /// Hides all edge indicators.
    /// Educational: This demonstrates bulk UI management.
    /// </summary>
    public void HideAllIndicators()
    {
        foreach (var indicator in indicatorMap.Values)
        {
            indicator.SetActive(false);
        }
    }
    
    /// <summary>
    /// Coroutine for pulsing edge indicator.
    /// Educational: This shows how to create smooth visual effects.
    /// </summary>
    private IEnumerator PulseIndicator(GameObject indicator)
    {
        var renderer = indicator.GetComponent<SpriteRenderer>();
        if (renderer == null) yield break;
        
        var baseColor = renderer.color;
        var time = 0f;
        var pulseSpeed = 2f;
        
        while (indicator.activeInHierarchy)
        {
            time += Time.deltaTime * pulseSpeed;
            var alpha = 0.5f + Mathf.Sin(time) * 0.3f;
            
            var color = baseColor;
            color.a = alpha;
            renderer.color = color;
            
            yield return null;
        }
    }
}
```

### 5. Object Pooling for Performance

```csharp
/// <summary>
/// Object pooler for power orbs to maintain 60 FPS.
/// Educational: This demonstrates performance optimization for special objects.
/// Performance: Prevents garbage collection spikes from power orb creation.
/// </summary>
public class PowerOrbPooler : MonoBehaviour
{
    [Header("Pool Configuration")]
    [SerializeField] private GameObject powerOrbPrefab;
    [SerializeField] private int poolSize = 8;
    [SerializeField] private Transform poolParent;
    
    private Queue<PowerOrb> availableOrbs;
    private List<PowerOrb> allOrbs;
    
    public static PowerOrbPooler Instance { get; private set; }
    
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
    /// Initializes the power orb pool.
    /// Educational: This shows how to pre-allocate special objects for performance.
    /// </summary>
    private void InitializePool()
    {
        availableOrbs = new Queue<PowerOrb>();
        allOrbs = new List<PowerOrb>();
        
        for (int i = 0; i < poolSize; i++)
        {
            var powerOrb = CreatePowerOrb();
            powerOrb.gameObject.SetActive(false);
            availableOrbs.Enqueue(powerOrb);
            allOrbs.Add(powerOrb);
        }
        
        Debug.Log($"Initialized Power Orb Pool with {poolSize} objects");
    }
    
    /// <summary>
    /// Gets a power orb from the pool.
    /// Educational: This demonstrates object pool retrieval pattern.
    /// </summary>
    public PowerOrb GetPowerOrb()
    {
        if (availableOrbs.Count > 0)
        {
            var powerOrb = availableOrbs.Dequeue();
            powerOrb.gameObject.SetActive(true);
            return powerOrb;
        }
        
        // Pool exhausted, create new one
        Debug.LogWarning("Power Orb Pool exhausted, creating new object");
        return CreatePowerOrb();
    }
    
    /// <summary>
    /// Returns a power orb to the pool.
    /// Educational: This shows object pool return pattern.
    /// </summary>
    public void ReturnPowerOrb(PowerOrb powerOrb)
    {
        if (powerOrb == null) return;
        
        // Reset state
        powerOrb.gameObject.SetActive(false);
        powerOrb.transform.SetParent(poolParent);
        powerOrb.transform.localPosition = Vector3.zero;
        
        // Return to pool
        availableOrbs.Enqueue(powerOrb);
    }
    
    /// <summary>
    /// Creates a new power orb GameObject.
    /// Educational: This demonstrates prefab instantiation for pooling.
    /// </summary>
    private PowerOrb CreatePowerOrb()
    {
        var go = Instantiate(powerOrbPrefab, poolParent);
        var powerOrb = go.GetComponent<PowerOrb>();
        
        if (powerOrb == null)
        {
            Debug.LogError("Power Orb Prefab missing PowerOrb component!");
            Destroy(go);
            return null;
        }
        
        return powerOrb;
    }
    
    /// <summary>
    /// Gets pool statistics for debugging.
    /// Educational: This shows how to monitor pool performance.
    /// </summary>
    public PoolStatistics GetPoolStatistics()
    {
        return new PoolStatistics
        {
            totalObjects = allOrbs.Count,
            availableObjects = availableOrbs.Count,
            inUseObjects = allOrbs.Count - availableOrbs.Count,
            poolUtilization = (float)(allOrbs.Count - availableOrbs.Count) / allOrbs.Count
        };
    }
}
```

## Scoring System

### Base Scoring
- **Base Score**: 5,000 points
- **Age Bonus**: +500 points per turn survived
- **Cascade Multiplier**: Applied if collected during cascade
- **Wrong Edge**: 0 points (orb is lost)

### Score Calculation Formula
```csharp
score = (baseScore + (age * ageBonus)) * cascadeMultiplier
```

### Example Scores
- **Turn 1**: 5,000 points
- **Turn 5**: 7,500 points
- **Turn 10**: 10,000 points
- **Turn 15**: 12,500 points

## Spawn Rate Progression

### Timeline
- **0-2 minutes**: 5% spawn chance
- **2-4 minutes**: 6% spawn chance
- **4-6 minutes**: 7% spawn chance
- **6+ minutes**: 8% spawn chance (max 15%)

### Spawn Rate Formula
```csharp
spawnChance = Mathf.Min(
    baseSpawnChance + (Mathf.Floor(gameTime / increaseInterval) * 0.01f),
    maxSpawnChance
);
```

## Visual Design Guidelines

### Orb Appearance
- **Shape**: Circular with glowing effect
- **Size**: Slightly larger than regular tiles
- **Glow**: Pulsing effect with color matching target edge
- **Particles**: Subtle particle system for visual appeal

### Edge Indicators
- **Color**: Matching orb color
- **Effect**: Pulsing border around target edge
- **Duration**: Visible while orb is active
- **Intensity**: Subtle but noticeable

### Animations
- **Spawn**: Scale-in with glow effect (0.5s duration)
- **Success**: Celebration particles and score popup (1s duration)
- **Failure**: Dissolve effect with fade-out (1s duration)
- **Movement**: Smooth interpolation between positions

## Testing Requirements

### Unit Tests
```csharp
[TestFixture]
public class PowerOrbTests
{
    [Test]
    public void PowerOrb_CalculatesScoreCorrectly()
    {
        // Arrange
        var orbData = ScriptableObject.CreateInstance<PowerOrbData>();
        orbData.baseScore = 5000;
        orbData.ageBonus = 500;
        
        // Act & Assert
        Assert.AreEqual(5000, orbData.CalculateScore(0));
        Assert.AreEqual(7500, orbData.CalculateScore(5));
        Assert.AreEqual(10000, orbData.CalculateScore(10));
    }
    
    [Test]
    public void PowerOrb_MovesTowardTargetEdge()
    {
        // Arrange
        var powerOrb = new PowerOrb();
        var orbData = ScriptableObject.CreateInstance<PowerOrbData>();
        orbData.color = OrbColor.Blue; // Target: Top edge
        powerOrb.Initialize(orbData, new Vector2Int(3, 3));
        
        // Act
        powerOrb.MoveTowardEdge(Direction.Up);
        
        // Assert
        Assert.AreEqual(new Vector2Int(3, 2), powerOrb.Position);
    }
    
    [Test]
    public void SpawnChance_ProgressiveIncrease()
    {
        // Arrange
        var spawner = new PowerOrbSpawner();
        
        // Act & Assert
        Assert.AreEqual(0.05f, spawner.CalculateSpawnChance(0f));   // 5%
        Assert.AreEqual(0.06f, spawner.CalculateSpawnChance(120f)); // 6%
        Assert.AreEqual(0.07f, spawner.CalculateSpawnChance(240f)); // 7%
        Assert.AreEqual(0.15f, spawner.CalculateSpawnChance(1200f)); // Max 15%
    }
}
```

### Integration Tests
- Test power orb spawning after center cell clears
- Test movement toward target edges
- Test scoring on correct edge reach
- Test no scoring on wrong edge reach
- Test age-based scoring progression
- Test edge indicator display

### Performance Tests
- Verify 60 FPS with multiple power orbs
- Test object pooling efficiency
- Monitor memory allocation during spawning
- Test glow effect performance

## Configuration Examples

### Conservative Settings (Easy Mode)
```csharp
baseSpawnChance = 0.03f;      // 3%
maxSpawnChance = 0.08f;       // 8%
baseScore = 3000;             // 3,000 points
ageBonus = 300;               // +300 per turn
increaseInterval = 180f;      // 3 minutes
```

### Standard Settings (Normal Mode)
```csharp
baseSpawnChance = 0.05f;      // 5%
maxSpawnChance = 0.15f;       // 15%
baseScore = 5000;             // 5,000 points
ageBonus = 500;               // +500 per turn
increaseInterval = 120f;      // 2 minutes
```

### Aggressive Settings (Hard Mode)
```csharp
baseSpawnChance = 0.08f;      // 8%
maxSpawnChance = 0.20f;       // 20%
baseScore = 7500;             // 7,500 points
ageBonus = 750;               // +750 per turn
increaseInterval = 90f;       // 1.5 minutes
```

## Conclusion

The Power Orbs system adds strategic depth to SWITCH by creating high-value objectives that require multi-turn planning. The center-spawn mechanic encourages players to focus on board management, while the age-based scoring rewards patience and strategic thinking.

The system is designed to be visually appealing with clear feedback, while the object pooling ensures performance remains optimal. The progressive spawn rates and configurable scoring provide flexibility for different difficulty levels and player preferences.

The implementation prioritizes clarity in both code and gameplay, with comprehensive testing and configuration options to support the development team and ensure a polished player experience.
