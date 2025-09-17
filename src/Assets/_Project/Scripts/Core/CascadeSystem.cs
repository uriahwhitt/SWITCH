/******************************************************************************
 * SWITCH - CascadeSystem
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Cascade detection and resolution system for chain reactions
 * Dependencies: BoardController, MatchDetector, Tile
 * 
 * Educational Notes:
 * - Demonstrates cascade chain reaction systems
 * - Shows how to implement complex match resolution
 * - Performance: Efficient cascade detection with minimal overhead
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SWITCH.Data;

namespace SWITCH.Core
{
    /// <summary>
    /// Cascade detection and resolution system for chain reactions.
    /// Educational: This demonstrates cascade chain reaction systems.
    /// Performance: Efficient cascade detection with minimal overhead.
    /// </summary>
    public class CascadeSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private bool enableCascades = true;
        [SerializeField] private int maxCascadeDepth = 10;
        [SerializeField] private float cascadeDelay = 0.5f;
        
        [Header("Cascade Settings")]
        [SerializeField] private float cascadeMultiplier = 1.5f;
        [SerializeField] private int minCascadeLength = 2;
        [SerializeField] private float cascadeScoreBonus = 100f;
        [SerializeField] private bool enableCascadeAnimations = true;
        
        [Header("References")]
        [SerializeField] private BoardController boardController;
        [SerializeField] private MatchDetector matchDetector;
        [SerializeField] private GameManager gameManager;
        
        // Cascade state
        private List<CascadeData> activeCascades = new List<CascadeData>();
        private List<CascadeData> completedCascades = new List<CascadeData>();
        private bool isProcessingCascade = false;
        private int currentCascadeLevel = 0;
        private int totalCascadesProcessed = 0;
        
        // Performance tracking
        private int totalMatchesInCascades = 0;
        private int totalTilesClearedInCascades = 0;
        private float totalCascadeScore = 0f;
        
        // Events
        public System.Action<CascadeData> OnCascadeStarted;
        public System.Action<CascadeData> OnCascadeCompleted;
        public System.Action<int> OnCascadeLevelChanged;
        public System.Action<float> OnCascadeScoreAdded;
        
        // Properties
        public bool IsProcessingCascade => isProcessingCascade;
        public int CurrentCascadeLevel => currentCascadeLevel;
        public int ActiveCascadeCount => activeCascades.Count;
        public int TotalCascadesProcessed => totalCascadesProcessed;
        
        private void Awake()
        {
            // Get references if not assigned
            if (boardController == null)
                boardController = FindObjectOfType<BoardController>();
            if (matchDetector == null)
                matchDetector = FindObjectOfType<MatchDetector>();
            if (gameManager == null)
                gameManager = FindObjectOfType<GameManager>();
        }
        
        private void Start()
        {
            InitializeCascadeSystem();
        }
        
        /// <summary>
        /// Initializes the cascade system.
        /// Educational: Shows how to set up cascade systems.
        /// </summary>
        private void InitializeCascadeSystem()
        {
            Log("Initializing Cascade System");
            
            // Reset system state
            ResetSystemState();
            
            Log("Cascade System initialized");
        }
        
        /// <summary>
        /// Resets the system state.
        /// Educational: Shows how to reset tracking systems.
        /// </summary>
        private void ResetSystemState()
        {
            activeCascades.Clear();
            completedCascades.Clear();
            isProcessingCascade = false;
            currentCascadeLevel = 0;
            totalCascadesProcessed = 0;
            totalMatchesInCascades = 0;
            totalTilesClearedInCascades = 0;
            totalCascadeScore = 0f;
        }
        
        /// <summary>
        /// Processes a cascade starting from initial matches.
        /// Educational: Shows how to implement cascade processing.
        /// </summary>
        /// <param name="initialMatches">Initial matches that triggered the cascade</param>
        /// <returns>Coroutine for cascade processing</returns>
        public IEnumerator ProcessCascade(List<MatchData> initialMatches)
        {
            if (!enableCascades || isProcessingCascade)
                yield break;
            
            isProcessingCascade = true;
            currentCascadeLevel = 0;
            
            Log($"Starting cascade with {initialMatches.Count} initial matches");
            
            try
            {
                // Create initial cascade data
                CascadeData cascade = new CascadeData
                {
                    cascadeId = System.Guid.NewGuid().ToString(),
                    startTime = Time.time,
                    initialMatches = new List<MatchData>(initialMatches),
                    cascadeLevels = new List<CascadeLevel>(),
                    totalScore = 0f,
                    totalMatches = 0,
                    totalTilesCleared = 0
                };
                
                activeCascades.Add(cascade);
                OnCascadeStarted?.Invoke(cascade);
                
                // Process cascade levels
                yield return StartCoroutine(ProcessCascadeLevels(cascade));
                
                // Complete cascade
                CompleteCascade(cascade);
            }
            finally
            {
                isProcessingCascade = false;
            }
        }
        
        /// <summary>
        /// Processes all levels of a cascade.
        /// Educational: Shows how to implement cascade level processing.
        /// </summary>
        /// <param name="cascade">Cascade data to process</param>
        /// <returns>Coroutine for level processing</returns>
        private IEnumerator ProcessCascadeLevels(CascadeData cascade)
        {
            List<MatchData> currentMatches = new List<MatchData>(cascade.initialMatches);
            
            while (currentMatches.Count > 0 && currentCascadeLevel < maxCascadeDepth)
            {
                // Process current level
                CascadeLevel level = ProcessCascadeLevel(currentMatches, currentCascadeLevel);
                cascade.cascadeLevels.Add(level);
                
                // Update cascade data
                cascade.totalMatches += level.matches.Count;
                cascade.totalTilesCleared += level.tilesCleared;
                cascade.totalScore += level.score;
                
                // Update global tracking
                totalMatchesInCascades += level.matches.Count;
                totalTilesClearedInCascades += level.tilesCleared;
                totalCascadeScore += level.score;
                
                // Fire events
                OnCascadeLevelChanged?.Invoke(currentCascadeLevel);
                OnCascadeScoreAdded?.Invoke(level.score);
                
                Log($"Cascade level {currentCascadeLevel}: {level.matches.Count} matches, {level.tilesCleared} tiles cleared, {level.score} score");
                
                // Wait for cascade delay
                if (enableCascadeAnimations && cascadeDelay > 0f)
                {
                    yield return new WaitForSeconds(cascadeDelay);
                }
                
                // Find next level matches
                currentMatches = FindNextLevelMatches(level);
                currentCascadeLevel++;
            }
        }
        
        /// <summary>
        /// Processes a single cascade level.
        /// Educational: Shows how to implement cascade level processing.
        /// </summary>
        /// <param name="matches">Matches to process</param>
        /// <param name="level">Cascade level</param>
        /// <returns>Cascade level data</returns>
        private CascadeLevel ProcessCascadeLevel(List<MatchData> matches, int level)
        {
            CascadeLevel cascadeLevel = new CascadeLevel
            {
                level = level,
                matches = new List<MatchData>(matches),
                tilesCleared = 0,
                score = 0f,
                startTime = Time.time
            };
            
            // Process each match in the level
            foreach (var match in matches)
            {
                // Clear tiles in the match
                int tilesCleared = ClearMatchTiles(match);
                cascadeLevel.tilesCleared += tilesCleared;
                
                // Calculate score for this match
                float matchScore = CalculateMatchScore(match, level);
                cascadeLevel.score += matchScore;
            }
            
            cascadeLevel.endTime = Time.time;
            cascadeLevel.duration = cascadeLevel.endTime - cascadeLevel.startTime;
            
            return cascadeLevel;
        }
        
        /// <summary>
        /// Clears tiles in a match.
        /// Educational: Shows how to implement tile clearing.
        /// </summary>
        /// <param name="match">Match to clear</param>
        /// <returns>Number of tiles cleared</returns>
        private int ClearMatchTiles(MatchData match)
        {
            int tilesCleared = 0;
            
            // TODO: Implement actual tile clearing
            // This would clear the tiles at the match positions
            // and handle any special effects
            
            foreach (var position in match.positions)
            {
                // Clear tile at position
                // boardController.ClearTileAt(position);
                tilesCleared++;
            }
            
            return tilesCleared;
        }
        
        /// <summary>
        /// Calculates score for a match in a cascade.
        /// Educational: Shows how to implement cascade scoring.
        /// </summary>
        /// <param name="match">Match to score</param>
        /// <param name="cascadeLevel">Cascade level</param>
        /// <returns>Score for the match</returns>
        private float CalculateMatchScore(MatchData match, int cascadeLevel)
        {
            float baseScore = match.positions.Count * 100f;
            float cascadeMultiplier = 1f + (cascadeLevel * this.cascadeMultiplier);
            float finalScore = baseScore * cascadeMultiplier;
            
            return finalScore;
        }
        
        /// <summary>
        /// Finds matches for the next cascade level.
        /// Educational: Shows how to implement cascade continuation.
        /// </summary>
        /// <param name="previousLevel">Previous cascade level</param>
        /// <returns>Matches for next level</returns>
        private List<MatchData> FindNextLevelMatches(CascadeLevel previousLevel)
        {
            List<MatchData> nextLevelMatches = new List<MatchData>();
            
            // TODO: Implement actual next level match detection
            // This would analyze the board after the previous level
            // and find new matches that were created by gravity
            
            // For now, generate some placeholder matches
            int matchCount = UnityEngine.Random.Range(0, 3);
            for (int i = 0; i < matchCount; i++)
            {
                MatchData match = GeneratePlaceholderMatch();
                nextLevelMatches.Add(match);
            }
            
            return nextLevelMatches;
        }
        
        /// <summary>
        /// Generates a placeholder match for testing.
        /// Educational: Shows how to create test data.
        /// </summary>
        /// <returns>Placeholder match data</returns>
        private MatchData GeneratePlaceholderMatch()
        {
            return new MatchData
            {
                positions = new List<Vector2Int>
                {
                    new Vector2Int(UnityEngine.Random.Range(0, 8), UnityEngine.Random.Range(0, 8)),
                    new Vector2Int(UnityEngine.Random.Range(0, 8), UnityEngine.Random.Range(0, 8)),
                    new Vector2Int(UnityEngine.Random.Range(0, 8), UnityEngine.Random.Range(0, 8))
                },
                color = (ColorType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(ColorType)).Length),
                matchType = MatchType.Horizontal
            };
        }
        
        /// <summary>
        /// Completes a cascade.
        /// Educational: Shows how to complete cascade processing.
        /// </summary>
        /// <param name="cascade">Cascade to complete</param>
        private void CompleteCascade(CascadeData cascade)
        {
            cascade.endTime = Time.time;
            cascade.duration = cascade.endTime - cascade.startTime;
            
            // Move to completed cascades
            activeCascades.Remove(cascade);
            completedCascades.Add(cascade);
            
            // Update global tracking
            totalCascadesProcessed++;
            
            // Fire events
            OnCascadeCompleted?.Invoke(cascade);
            
            Log($"Cascade completed: {cascade.cascadeLevels.Count} levels, {cascade.totalMatches} matches, {cascade.totalScore} score");
        }
        
        /// <summary>
        /// Gets all active cascades.
        /// Educational: Shows how to expose system state.
        /// </summary>
        /// <returns>List of active cascades</returns>
        public List<CascadeData> GetActiveCascades()
        {
            return new List<CascadeData>(activeCascades);
        }
        
        /// <summary>
        /// Gets all completed cascades.
        /// Educational: Shows how to expose system state.
        /// </summary>
        /// <returns>List of completed cascades</returns>
        public List<CascadeData> GetCompletedCascades()
        {
            return new List<CascadeData>(completedCascades);
        }
        
        /// <summary>
        /// Gets cascade statistics.
        /// Educational: Shows how to provide system analytics.
        /// </summary>
        /// <returns>Cascade statistics</returns>
        public CascadeStats GetCascadeStats()
        {
            return new CascadeStats
            {
                TotalCascadesProcessed = totalCascadesProcessed,
                TotalMatchesInCascades = totalMatchesInCascades,
                TotalTilesClearedInCascades = totalTilesClearedInCascades,
                TotalCascadeScore = totalCascadeScore,
                CurrentCascadeLevel = currentCascadeLevel,
                ActiveCascadeCount = activeCascades.Count
            };
        }
        
        /// <summary>
        /// Resets the system for a new game.
        /// Educational: Shows how to reset systems for new games.
        /// </summary>
        public void ResetForNewGame()
        {
            ResetSystemState();
            Log("Cascade system reset for new game");
        }
        
        /// <summary>
        /// Logs a message if debug logging is enabled.
        /// Educational: Shows how to implement conditional logging.
        /// </summary>
        /// <param name="message">Message to log</param>
        private void Log(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[CascadeSystem] {message}");
        }
    }
    
    /// <summary>
    /// Cascade data structure.
    /// Educational: Shows how to define cascade data structures.
    /// </summary>
    [System.Serializable]
    public struct CascadeData
    {
        public string cascadeId;
        public float startTime;
        public float endTime;
        public float duration;
        public List<MatchData> initialMatches;
        public List<CascadeLevel> cascadeLevels;
        public float totalScore;
        public int totalMatches;
        public int totalTilesCleared;
    }
    
    /// <summary>
    /// Cascade level data structure.
    /// Educational: Shows how to define cascade level data.
    /// </summary>
    [System.Serializable]
    public struct CascadeLevel
    {
        public int level;
        public List<MatchData> matches;
        public int tilesCleared;
        public float score;
        public float startTime;
        public float endTime;
        public float duration;
    }
    
    /// <summary>
    /// Cascade statistics.
    /// Educational: Shows how to create analytics data structures.
    /// </summary>
    [System.Serializable]
    public struct CascadeStats
    {
        public int TotalCascadesProcessed;
        public int TotalMatchesInCascades;
        public int TotalTilesClearedInCascades;
        public float TotalCascadeScore;
        public int CurrentCascadeLevel;
        public int ActiveCascadeCount;
    }
}
