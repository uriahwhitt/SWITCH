/******************************************************************************
 * SWITCH - PowerUpEarningSystem
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: System for earning power-ups through gameplay achievements
 * Dependencies: PowerUpInventory, PowerUpManager, GameManager
 * 
 * Educational Notes:
 * - Demonstrates achievement-based reward systems
 * - Shows how to integrate with game events
 * - Performance: Efficient achievement tracking with minimal overhead
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SWITCH.Core;
using SWITCH.Data;

namespace SWITCH.PowerUps
{
    /// <summary>
    /// System for earning power-ups through gameplay achievements.
    /// Educational: This demonstrates achievement-based reward systems.
    /// Performance: Efficient achievement tracking with minimal overhead.
    /// </summary>
    public class PowerUpEarningSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private float achievementCheckInterval = 1f;
        
        [Header("Earning Rules")]
        [SerializeField] private int matchesForColorBomb = 5;
        [SerializeField] private int matchesForLineClear = 10;
        [SerializeField] private int matchesForAreaClear = 15;
        [SerializeField] private int matchesForTimeFreeze = 20;
        [SerializeField] private int matchesForScoreMultiplier = 25;
        
        [Header("Score Thresholds")]
        [SerializeField] private int scoreThreshold1 = 1000;
        [SerializeField] private int scoreThreshold2 = 5000;
        [SerializeField] private int scoreThreshold3 = 10000;
        
        [Header("References")]
        [SerializeField] private PowerUpInventory powerUpInventory;
        [SerializeField] private PowerUpManager powerUpManager;
        [SerializeField] private GameManager gameManager;
        
        // Achievement tracking
        private Dictionary<string, AchievementData> achievements = new Dictionary<string, AchievementData>();
        private Dictionary<string, int> currentProgress = new Dictionary<string, int>();
        private Dictionary<string, bool> earnedPowerUps = new Dictionary<string, bool>();
        
        // Game state tracking
        private int totalMatches = 0;
        private int currentScore = 0;
        private int highestCombo = 0;
        private int consecutiveTurns = 0;
        private float gameStartTime = 0f;
        
        // Events
        public event Action<string> OnAchievementUnlocked;
        public event Action<string, int> OnProgressUpdated;
        public event Action<string> OnPowerUpEarned;
        
        // Properties
        public int TotalMatches => totalMatches;
        public int CurrentScore => currentScore;
        public int HighestCombo => highestCombo;
        public int ConsecutiveTurns => consecutiveTurns;
        
        private void Awake()
        {
            // Get references if not assigned
            if (powerUpInventory == null)
                powerUpInventory = FindObjectOfType<PowerUpInventory>();
            if (powerUpManager == null)
                powerUpManager = FindObjectOfType<PowerUpManager>();
            if (gameManager == null)
                gameManager = FindObjectOfType<GameManager>();
        }
        
        private void Start()
        {
            InitializeEarningSystem();
        }
        
        private void OnEnable()
        {
            SubscribeToGameEvents();
        }
        
        private void OnDisable()
        {
            UnsubscribeFromGameEvents();
        }
        
        /// <summary>
        /// Initializes the power-up earning system.
        /// Educational: Shows how to set up achievement systems.
        /// </summary>
        private void InitializeEarningSystem()
        {
            Log("Initializing Power-Up Earning System");
            
            // Initialize achievements
            InitializeAchievements();
            
            // Initialize progress tracking
            InitializeProgressTracking();
            
            // Reset game state
            ResetGameState();
            
            Log("Power-Up Earning System initialized");
        }
        
        /// <summary>
        /// Initializes achievement definitions.
        /// Educational: Shows how to define achievement systems.
        /// </summary>
        private void InitializeAchievements()
        {
            achievements.Clear();
            
            // Match-based achievements
            achievements["color_bomb_earner"] = new AchievementData
            {
                Id = "color_bomb_earner",
                Name = "Color Master",
                Description = $"Make {matchesForColorBomb} matches to earn Color Bomb",
                TargetValue = matchesForColorBomb,
                PowerUpReward = "color_bomb",
                AchievementType = AchievementType.MatchCount
            };
            
            achievements["line_clear_earner"] = new AchievementData
            {
                Id = "line_clear_earner",
                Name = "Line Breaker",
                Description = $"Make {matchesForLineClear} matches to earn Line Clear",
                TargetValue = matchesForLineClear,
                PowerUpReward = "line_clear",
                AchievementType = AchievementType.MatchCount
            };
            
            achievements["area_clear_earner"] = new AchievementData
            {
                Id = "area_clear_earner",
                Name = "Area Destroyer",
                Description = $"Make {matchesForAreaClear} matches to earn Area Clear",
                TargetValue = matchesForAreaClear,
                PowerUpReward = "area_clear",
                AchievementType = AchievementType.MatchCount
            };
            
            achievements["time_freeze_earner"] = new AchievementData
            {
                Id = "time_freeze_earner",
                Name = "Time Master",
                Description = $"Make {matchesForTimeFreeze} matches to earn Time Freeze",
                TargetValue = matchesForTimeFreeze,
                PowerUpReward = "time_freeze",
                AchievementType = AchievementType.MatchCount
            };
            
            achievements["score_multiplier_earner"] = new AchievementData
            {
                Id = "score_multiplier_earner",
                Name = "Score Master",
                Description = $"Make {matchesForScoreMultiplier} matches to earn Score Multiplier",
                TargetValue = matchesForScoreMultiplier,
                PowerUpReward = "score_multiplier",
                AchievementType = AchievementType.MatchCount
            };
            
            // Score-based achievements
            achievements["score_threshold_1"] = new AchievementData
            {
                Id = "score_threshold_1",
                Name = "Score Starter",
                Description = $"Reach {scoreThreshold1} points to earn a random power-up",
                TargetValue = scoreThreshold1,
                PowerUpReward = GetRandomPowerUpId(),
                AchievementType = AchievementType.ScoreThreshold
            };
            
            achievements["score_threshold_2"] = new AchievementData
            {
                Id = "score_threshold_2",
                Name = "Score Achiever",
                Description = $"Reach {scoreThreshold2} points to earn a random power-up",
                TargetValue = scoreThreshold2,
                PowerUpReward = GetRandomPowerUpId(),
                AchievementType = AchievementType.ScoreThreshold
            };
            
            achievements["score_threshold_3"] = new AchievementData
            {
                Id = "score_threshold_3",
                Name = "Score Master",
                Description = $"Reach {scoreThreshold3} points to earn a random power-up",
                TargetValue = scoreThreshold3,
                PowerUpReward = GetRandomPowerUpId(),
                AchievementType = AchievementType.ScoreThreshold
            };
        }
        
        /// <summary>
        /// Initializes progress tracking for all achievements.
        /// Educational: Shows how to set up progress tracking.
        /// </summary>
        private void InitializeProgressTracking()
        {
            currentProgress.Clear();
            earnedPowerUps.Clear();
            
            foreach (var achievement in achievements.Values)
            {
                currentProgress[achievement.Id] = 0;
                earnedPowerUps[achievement.PowerUpReward] = false;
            }
        }
        
        /// <summary>
        /// Resets game state tracking.
        /// Educational: Shows how to reset tracking systems.
        /// </summary>
        private void ResetGameState()
        {
            totalMatches = 0;
            currentScore = 0;
            highestCombo = 0;
            consecutiveTurns = 0;
            gameStartTime = Time.time;
        }
        
        /// <summary>
        /// Subscribes to game events for achievement tracking.
        /// Educational: Shows how to integrate with game event systems.
        /// </summary>
        private void SubscribeToGameEvents()
        {
            // TODO: Subscribe to actual game events
            // GameManager.OnMatchMade += OnMatchMade;
            // GameManager.OnScoreChanged += OnScoreChanged;
            // GameManager.OnComboChanged += OnComboChanged;
            // GameManager.OnTurnCompleted += OnTurnCompleted;
        }
        
        /// <summary>
        /// Unsubscribes from game events.
        /// Educational: Shows how to properly clean up event subscriptions.
        /// </summary>
        private void UnsubscribeFromGameEvents()
        {
            // TODO: Unsubscribe from actual game events
            // GameManager.OnMatchMade -= OnMatchMade;
            // GameManager.OnScoreChanged -= OnScoreChanged;
            // GameManager.OnComboChanged -= OnComboChanged;
            // GameManager.OnTurnCompleted -= OnTurnCompleted;
        }
        
        /// <summary>
        /// Handles match made events for achievement tracking.
        /// Educational: Shows how to process game events for achievements.
        /// </summary>
        /// <param name="matchCount">Number of matches made</param>
        private void OnMatchMade(int matchCount)
        {
            totalMatches += matchCount;
            consecutiveTurns++;
            
            // Update match-based achievements
            UpdateMatchBasedAchievements();
            
            Log($"Match made: {matchCount} (total: {totalMatches})");
        }
        
        /// <summary>
        /// Handles score changed events for achievement tracking.
        /// Educational: Shows how to process score events for achievements.
        /// </summary>
        /// <param name="newScore">New score value</param>
        private void OnScoreChanged(int newScore)
        {
            currentScore = newScore;
            
            // Update score-based achievements
            UpdateScoreBasedAchievements();
            
            Log($"Score changed: {currentScore}");
        }
        
        /// <summary>
        /// Handles combo changed events for achievement tracking.
        /// Educational: Shows how to process combo events for achievements.
        /// </summary>
        /// <param name="newCombo">New combo value</param>
        private void OnComboChanged(int newCombo)
        {
            if (newCombo > highestCombo)
            {
                highestCombo = newCombo;
                Log($"New highest combo: {highestCombo}");
            }
        }
        
        /// <summary>
        /// Handles turn completed events for achievement tracking.
        /// Educational: Shows how to process turn events for achievements.
        /// </summary>
        private void OnTurnCompleted()
        {
            // Check for turn-based achievements
            CheckTurnBasedAchievements();
        }
        
        /// <summary>
        /// Updates match-based achievements.
        /// Educational: Shows how to update achievement progress.
        /// </summary>
        private void UpdateMatchBasedAchievements()
        {
            foreach (var achievement in achievements.Values)
            {
                if (achievement.AchievementType == AchievementType.MatchCount)
                {
                    int newProgress = Mathf.Min(totalMatches, achievement.TargetValue);
                    UpdateAchievementProgress(achievement.Id, newProgress);
                }
            }
        }
        
        /// <summary>
        /// Updates score-based achievements.
        /// Educational: Shows how to update achievement progress.
        /// </summary>
        private void UpdateScoreBasedAchievements()
        {
            foreach (var achievement in achievements.Values)
            {
                if (achievement.AchievementType == AchievementType.ScoreThreshold)
                {
                    int newProgress = Mathf.Min(currentScore, achievement.TargetValue);
                    UpdateAchievementProgress(achievement.Id, newProgress);
                }
            }
        }
        
        /// <summary>
        /// Checks for turn-based achievements.
        /// Educational: Shows how to check for achievement completion.
        /// </summary>
        private void CheckTurnBasedAchievements()
        {
            // TODO: Implement turn-based achievements
            // - Consecutive turns without power-ups
            // - Perfect turns (no wasted moves)
            // - Speed achievements
        }
        
        /// <summary>
        /// Updates achievement progress and checks for completion.
        /// Educational: Shows how to manage achievement progress.
        /// </summary>
        /// <param name="achievementId">ID of achievement to update</param>
        /// <param name="newProgress">New progress value</param>
        private void UpdateAchievementProgress(string achievementId, int newProgress)
        {
            if (!currentProgress.ContainsKey(achievementId))
                return;
            
            int oldProgress = currentProgress[achievementId];
            currentProgress[achievementId] = newProgress;
            
            // Fire progress update event
            OnProgressUpdated?.Invoke(achievementId, newProgress);
            
            // Check for achievement completion
            if (newProgress >= achievements[achievementId].TargetValue && oldProgress < achievements[achievementId].TargetValue)
            {
                CompleteAchievement(achievementId);
            }
        }
        
        /// <summary>
        /// Completes an achievement and awards the power-up.
        /// Educational: Shows how to complete achievements and award rewards.
        /// </summary>
        /// <param name="achievementId">ID of achievement to complete</param>
        private void CompleteAchievement(string achievementId)
        {
            if (!achievements.ContainsKey(achievementId))
                return;
            
            var achievement = achievements[achievementId];
            string powerUpId = achievement.PowerUpReward;
            
            // Check if power-up already earned
            if (earnedPowerUps.ContainsKey(powerUpId) && earnedPowerUps[powerUpId])
                return;
            
            // Award power-up
            if (powerUpInventory != null)
            {
                powerUpInventory.AddPowerUp(powerUpId, 1);
                earnedPowerUps[powerUpId] = true;
                
                Log($"Achievement completed: {achievement.Name} - Awarded {powerUpId}");
                
                // Fire events
                OnAchievementUnlocked?.Invoke(achievementId);
                OnPowerUpEarned?.Invoke(powerUpId);
            }
        }
        
        /// <summary>
        /// Gets a random power-up ID for score-based achievements.
        /// Educational: Shows how to implement random rewards.
        /// </summary>
        /// <returns>Random power-up ID</returns>
        private string GetRandomPowerUpId()
        {
            string[] powerUpIds = { "color_bomb", "line_clear", "area_clear", "time_freeze", "score_multiplier" };
            return powerUpIds[UnityEngine.Random.Range(0, powerUpIds.Length)];
        }
        
        /// <summary>
        /// Gets achievement progress for a specific achievement.
        /// Educational: Shows how to query achievement progress.
        /// </summary>
        /// <param name="achievementId">ID of achievement to check</param>
        /// <returns>Current progress</returns>
        public int GetAchievementProgress(string achievementId)
        {
            return currentProgress.TryGetValue(achievementId, out var progress) ? progress : 0;
        }
        
        /// <summary>
        /// Gets all achievement data.
        /// Educational: Shows how to expose achievement data.
        /// </summary>
        /// <returns>Dictionary of all achievements</returns>
        public Dictionary<string, AchievementData> GetAllAchievements()
        {
            return new Dictionary<string, AchievementData>(achievements);
        }
        
        /// <summary>
        /// Gets all current progress data.
        /// Educational: Shows how to expose progress data.
        /// </summary>
        /// <returns>Dictionary of all progress</returns>
        public Dictionary<string, int> GetAllProgress()
        {
            return new Dictionary<string, int>(currentProgress);
        }
        
        /// <summary>
        /// Resets all achievement progress.
        /// Educational: Shows how to reset achievement systems.
        /// </summary>
        public void ResetAllAchievements()
        {
            InitializeProgressTracking();
            ResetGameState();
            Log("All achievements reset");
        }
        
        /// <summary>
        /// Logs a message if debug logging is enabled.
        /// Educational: Shows how to implement conditional logging.
        /// </summary>
        /// <param name="message">Message to log</param>
        private void Log(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[PowerUpEarningSystem] {message}");
        }
    }
    
    /// <summary>
    /// Achievement data structure.
    /// Educational: Shows how to define achievement data structures.
    /// </summary>
    [System.Serializable]
    public struct AchievementData
    {
        public string Id;
        public string Name;
        public string Description;
        public int TargetValue;
        public string PowerUpReward;
        public AchievementType AchievementType;
    }
    
    /// <summary>
    /// Types of achievements.
    /// Educational: Shows how to define achievement categories.
    /// </summary>
    public enum AchievementType
    {
        MatchCount,
        ScoreThreshold,
        ComboCount,
        TurnCount,
        TimeBased,
        Special
    }
}
