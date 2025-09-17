using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SWITCH.PowerUps
{
    /// <summary>
    /// Manages power orb spawning, movement, and collection.
    /// Integrates with the momentum scoring system for instant heat boosts.
    /// </summary>
    public class PowerOrbManager : MonoBehaviour
    {
        [Header("Power Orb Data")]
        [SerializeField] private PowerOrbData[] powerOrbTypes;
        
        [Header("Spawn Configuration")]
        [SerializeField] private GameObject powerOrbPrefab;
        [SerializeField] private Transform orbParent;
        [SerializeField] private float spawnCheckInterval = 1f;
        
        [Header("Board Configuration")]
        [SerializeField] private Vector2Int boardSize = new Vector2Int(8, 8);
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        
        // References
        private MomentumSystem momentumSystem;
        private TurnScoreCalculator scoreCalculator;
        
        // Active orbs
        private List<PowerOrb> activeOrbs = new List<PowerOrb>();
        private Dictionary<Vector2Int, PowerOrb> orbPositions = new Dictionary<Vector2Int, PowerOrb>();
        
        // Spawn tracking
        private float gameStartTime;
        private Coroutine spawnCheckCoroutine;
        
        // Events
        public System.Action<PowerOrb> OnOrbSpawned;
        public System.Action<PowerOrb> OnOrbCollected;
        public System.Action<PowerOrb> OnOrbLost;
        
        private void Awake()
        {
            // Get references
            momentumSystem = FindObjectOfType<MomentumSystem>();
            scoreCalculator = FindObjectOfType<TurnScoreCalculator>();
            
            if (momentumSystem == null)
            {
                Debug.LogError("[PowerOrbManager] MomentumSystem not found!");
            }
            
            if (scoreCalculator == null)
            {
                Debug.LogError("[PowerOrbManager] TurnScoreCalculator not found!");
            }
        }
        
        private void Start()
        {
            gameStartTime = Time.time;
            StartSpawnChecking();
        }
        
        private void OnDestroy()
        {
            StopSpawnChecking();
        }
        
        /// <summary>
        /// Starts the spawn checking coroutine
        /// </summary>
        private void StartSpawnChecking()
        {
            if (spawnCheckCoroutine != null)
            {
                StopCoroutine(spawnCheckCoroutine);
            }
            
            spawnCheckCoroutine = StartCoroutine(SpawnCheckCoroutine());
        }
        
        /// <summary>
        /// Stops the spawn checking coroutine
        /// </summary>
        private void StopSpawnChecking()
        {
            if (spawnCheckCoroutine != null)
            {
                StopCoroutine(spawnCheckCoroutine);
                spawnCheckCoroutine = null;
            }
        }
        
        /// <summary>
        /// Spawn check coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private System.Collections.IEnumerator SpawnCheckCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnCheckInterval);
                CheckForSpawnOpportunities();
            }
        }
        
        /// <summary>
        /// Checks for opportunities to spawn power orbs
        /// </summary>
        private void CheckForSpawnOpportunities()
        {
            if (powerOrbTypes == null || powerOrbTypes.Length == 0) return;
            
            float gameTime = Time.time - gameStartTime;
            
            foreach (var orbData in powerOrbTypes)
            {
                float spawnChance = orbData.GetSpawnChance(gameTime);
                
                if (Random.Range(0f, 1f) < spawnChance)
                {
                    TrySpawnPowerOrb(orbData);
                }
            }
        }
        
        /// <summary>
        /// Attempts to spawn a power orb of the specified type
        /// </summary>
        /// <param name="orbData">Power orb data to spawn</param>
        /// <returns>True if successfully spawned</returns>
        public bool TrySpawnPowerOrb(PowerOrbData orbData)
        {
            // Check if we can spawn (not too many orbs active)
            if (activeOrbs.Count >= 4) // Max 4 orbs at once
            {
                LogDebug("Cannot spawn power orb - too many active orbs");
                return false;
            }
            
            // Get available spawn position
            Vector2Int spawnPos = GetAvailableSpawnPosition(orbData);
            if (spawnPos == Vector2Int.one * -1) // Invalid position
            {
                LogDebug("Cannot spawn power orb - no available spawn positions");
                return false;
            }
            
            // Create and initialize orb
            PowerOrb newOrb = CreatePowerOrb(orbData, spawnPos);
            if (newOrb == null)
            {
                LogDebug("Failed to create power orb");
                return false;
            }
            
            // Add to tracking
            activeOrbs.Add(newOrb);
            orbPositions[spawnPos] = newOrb;
            
            // Subscribe to events
            newOrb.OnOrbCollected += HandleOrbCollected;
            newOrb.OnOrbLost += HandleOrbLost;
            
            // Notify spawn
            OnOrbSpawned?.Invoke(newOrb);
            
            LogDebug($"Power orb spawned at {spawnPos} targeting {orbData.GetEdgeName()}");
            return true;
        }
        
        /// <summary>
        /// Gets an available spawn position for the orb
        /// </summary>
        /// <param name="orbData">Orb data containing spawn positions</param>
        /// <returns>Available spawn position or (-1,-1) if none available</returns>
        private Vector2Int GetAvailableSpawnPosition(PowerOrbData orbData)
        {
            // Get all possible spawn positions
            var possiblePositions = orbData.centerSpawnPositions.ToList();
            
            // Shuffle to randomize selection
            for (int i = 0; i < possiblePositions.Count; i++)
            {
                Vector2Int temp = possiblePositions[i];
                int randomIndex = Random.Range(i, possiblePositions.Count);
                possiblePositions[i] = possiblePositions[randomIndex];
                possiblePositions[randomIndex] = temp;
            }
            
            // Find first available position
            foreach (var pos in possiblePositions)
            {
                if (!orbPositions.ContainsKey(pos))
                {
                    return pos;
                }
            }
            
            return Vector2Int.one * -1; // No available positions
        }
        
        /// <summary>
        /// Creates a new power orb GameObject
        /// </summary>
        /// <param name="orbData">Orb data</param>
        /// <param name="position">Spawn position</param>
        /// <returns>Created power orb or null if failed</returns>
        private PowerOrb CreatePowerOrb(PowerOrbData orbData, Vector2Int position)
        {
            if (powerOrbPrefab == null)
            {
                Debug.LogError("[PowerOrbManager] Power orb prefab not assigned!");
                return null;
            }
            
            // Create GameObject
            GameObject orbGO = Instantiate(powerOrbPrefab, orbParent);
            orbGO.name = $"PowerOrb_{orbData.color}_{position.x}_{position.y}";
            
            // Get PowerOrb component
            PowerOrb powerOrb = orbGO.GetComponent<PowerOrb>();
            if (powerOrb == null)
            {
                Debug.LogError("[PowerOrbManager] Power orb prefab missing PowerOrb component!");
                Destroy(orbGO);
                return null;
            }
            
            // Initialize orb
            powerOrb.Initialize(orbData, position);
            
            return powerOrb;
        }
        
        /// <summary>
        /// Handles power orb collection
        /// </summary>
        /// <param name="orb">Collected orb</param>
        private void HandleOrbCollected(PowerOrb orb)
        {
            // Remove from tracking
            RemoveOrbFromTracking(orb);
            
            // Trigger momentum boost
            if (momentumSystem != null)
            {
                momentumSystem.TriggerPowerOrbBoost();
            }
            
            // Notify collection
            OnOrbCollected?.Invoke(orb);
            
            LogDebug($"Power orb collected! Heat boosted to maximum!");
        }
        
        /// <summary>
        /// Handles power orb loss
        /// </summary>
        /// <param name="orb">Lost orb</param>
        private void HandleOrbLost(PowerOrb orb)
        {
            // Remove from tracking
            RemoveOrbFromTracking(orb);
            
            // Notify loss
            OnOrbLost?.Invoke(orb);
            
            LogDebug("Power orb lost!");
        }
        
        /// <summary>
        /// Removes orb from tracking lists
        /// </summary>
        /// <param name="orb">Orb to remove</param>
        private void RemoveOrbFromTracking(PowerOrb orb)
        {
            if (activeOrbs.Contains(orb))
            {
                activeOrbs.Remove(orb);
            }
            
            // Remove from position tracking
            var positionEntry = orbPositions.FirstOrDefault(x => x.Value == orb);
            if (positionEntry.Key != Vector2Int.zero || positionEntry.Value != null)
            {
                orbPositions.Remove(positionEntry.Key);
            }
            
            // Unsubscribe from events
            orb.OnOrbCollected -= HandleOrbCollected;
            orb.OnOrbLost -= HandleOrbLost;
        }
        
        /// <summary>
        /// Gets all active power orbs
        /// </summary>
        /// <returns>List of active orbs</returns>
        public List<PowerOrb> GetActiveOrbs()
        {
            return new List<PowerOrb>(activeOrbs);
        }
        
        /// <summary>
        /// Gets power orb at specific position
        /// </summary>
        /// <param name="position">Board position</param>
        /// <returns>Power orb at position or null</returns>
        public PowerOrb GetOrbAtPosition(Vector2Int position)
        {
            orbPositions.TryGetValue(position, out PowerOrb orb);
            return orb;
        }
        
        /// <summary>
        /// Checks if position has a power orb
        /// </summary>
        /// <param name="position">Board position</param>
        /// <returns>True if position has orb</returns>
        public bool HasOrbAtPosition(Vector2Int position)
        {
            return orbPositions.ContainsKey(position);
        }
        
        /// <summary>
        /// Clears all active power orbs
        /// </summary>
        public void ClearAllOrbs()
        {
            var orbsToRemove = new List<PowerOrb>(activeOrbs);
            
            foreach (var orb in orbsToRemove)
            {
                orb.LoseOrb();
            }
            
            activeOrbs.Clear();
            orbPositions.Clear();
            
            LogDebug("All power orbs cleared");
        }
        
        /// <summary>
        /// Forces spawn of a specific power orb type
        /// </summary>
        /// <param name="orbData">Orb data to spawn</param>
        /// <returns>True if successfully spawned</returns>
        public bool ForceSpawnOrb(PowerOrbData orbData)
        {
            return TrySpawnPowerOrb(orbData);
        }
        
        /// <summary>
        /// Gets spawn statistics for debugging
        /// </summary>
        /// <returns>Spawn statistics</returns>
        public PowerOrbSpawnStats GetSpawnStats()
        {
            float gameTime = Time.time - gameStartTime;
            
            return new PowerOrbSpawnStats
            {
                gameTime = gameTime,
                activeOrbCount = activeOrbs.Count,
                totalSpawnChance = powerOrbTypes?.Sum(x => x.GetSpawnChance(gameTime)) ?? 0f,
                orbPositions = new Dictionary<Vector2Int, PowerOrb>(orbPositions)
            };
        }
        
        /// <summary>
        /// Debug logging helper
        /// </summary>
        /// <param name="message">Message to log</param>
        private void LogDebug(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[PowerOrbManager] {message}");
            }
        }
        
        #region Editor Debugging
        #if UNITY_EDITOR
        [Header("Editor Debug Controls")]
        [SerializeField] private bool showDebugControls = false;
        
        private void OnGUI()
        {
            if (!showDebugControls) return;
            
            GUILayout.BeginArea(new Rect(10, 400, 300, 200));
            GUILayout.Label("Power Orb Manager Debug");
            GUILayout.Label($"Active Orbs: {activeOrbs.Count}");
            GUILayout.Label($"Game Time: {Time.time - gameStartTime:F1}s");
            
            GUILayout.Space(10);
            
            if (powerOrbTypes != null && powerOrbTypes.Length > 0)
            {
                foreach (var orbData in powerOrbTypes)
                {
                    if (GUILayout.Button($"Spawn {orbData.color} Orb"))
                        ForceSpawnOrb(orbData);
                }
            }
            
            if (GUILayout.Button("Clear All Orbs"))
                ClearAllOrbs();
            
            GUILayout.EndArea();
        }
        #endif
        #endregion
    }
    
    /// <summary>
    /// Statistics for power orb spawning
    /// </summary>
    [System.Serializable]
    public class PowerOrbSpawnStats
    {
        public float gameTime;
        public int activeOrbCount;
        public float totalSpawnChance;
        public Dictionary<Vector2Int, PowerOrb> orbPositions;
    }
}
