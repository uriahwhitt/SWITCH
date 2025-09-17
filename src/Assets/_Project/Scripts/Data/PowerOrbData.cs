using UnityEngine;

namespace SWITCH.PowerUps
{
    /// <summary>
    /// Configuration data for power orbs with edge targeting.
    /// Provides instant maximum heat when collected at correct edge.
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
        
        [Header("Movement Properties")]
        [Tooltip("Speed of movement toward edge")]
        [Range(0.5f, 5f)]
        public float moveSpeed = 2f;
        
        [Tooltip("Delay between moves (in seconds)")]
        [Range(0.5f, 3f)]
        public float moveDelay = 1f;
        
        /// <summary>
        /// Calculates score based on orb age.
        /// This score is then multiplied by the current heat multiplier.
        /// </summary>
        /// <param name="age">Age of the orb in turns</param>
        /// <returns>Base score for this orb</returns>
        public int CalculateScore(int age)
        {
            return baseScore + (age * ageBonus);
        }
        
        /// <summary>
        /// Gets the spawn chance based on game time.
        /// Spawn chance increases over time to maintain pressure.
        /// </summary>
        /// <param name="gameTime">Current game time in seconds</param>
        /// <returns>Spawn chance (0-1)</returns>
        public float GetSpawnChance(float gameTime)
        {
            var increaseCount = Mathf.Floor(gameTime / increaseInterval);
            return Mathf.Min(
                baseSpawnChance + (increaseCount * 0.01f), // +1% per interval
                maxSpawnChance
            );
        }
        
        /// <summary>
        /// Gets a random center spawn position
        /// </summary>
        /// <returns>Random center position</returns>
        public Vector2Int GetRandomSpawnPosition()
        {
            if (centerSpawnPositions.Length == 0)
            {
                return new Vector2Int(3, 3); // Default center
            }
            
            int randomIndex = Random.Range(0, centerSpawnPositions.Length);
            return centerSpawnPositions[randomIndex];
        }
        
        /// <summary>
        /// Validates the power orb data configuration
        /// </summary>
        /// <returns>True if configuration is valid</returns>
        public bool ValidateConfiguration()
        {
            // Check target edge is valid
            if (targetEdge.x < 0 || targetEdge.x > 7 || targetEdge.y < 0 || targetEdge.y > 7)
            {
                Debug.LogError($"Invalid target edge {targetEdge} for PowerOrbData {name}");
                return false;
            }
            
            // Check target edge is actually on an edge
            bool isOnEdge = targetEdge.x == 0 || targetEdge.x == 7 || targetEdge.y == 0 || targetEdge.y == 7;
            if (!isOnEdge)
            {
                Debug.LogError($"Target edge {targetEdge} is not on board edge for PowerOrbData {name}");
                return false;
            }
            
            // Check spawn positions are valid
            foreach (var pos in centerSpawnPositions)
            {
                if (pos.x < 0 || pos.x > 7 || pos.y < 0 || pos.y > 7)
                {
                    Debug.LogError($"Invalid spawn position {pos} for PowerOrbData {name}");
                    return false;
                }
            }
            
            // Check score values are reasonable
            if (baseScore <= 0 || ageBonus < 0)
            {
                Debug.LogError($"Invalid score values for PowerOrbData {name}");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Gets the edge name for display purposes
        /// </summary>
        /// <returns>Human-readable edge name</returns>
        public string GetEdgeName()
        {
            if (targetEdge.x == 0) return "Left Edge";
            if (targetEdge.x == 7) return "Right Edge";
            if (targetEdge.y == 0) return "Bottom Edge";
            if (targetEdge.y == 7) return "Top Edge";
            return "Unknown Edge";
        }
        
        /// <summary>
        /// Gets the color name for display purposes
        /// </summary>
        /// <returns>Human-readable color name</returns>
        public string GetColorName()
        {
            return color.ToString();
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            // Auto-validate when values change in editor
            ValidateConfiguration();
        }
        #endif
    }
}
