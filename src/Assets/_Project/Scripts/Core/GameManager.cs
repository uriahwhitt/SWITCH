/******************************************************************************
 * SWITCH - GameManager
 * Sprint: 0
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Purpose: Central game state management and coordination
 * Performance Target: 60 FPS requirement
 *****************************************************************************/

using System;
using UnityEngine;

namespace SWITCH.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private int targetFPS = 60;
        
        [Header("References")]
        [SerializeField] private GameObject boardControllerPrefab;
        [SerializeField] private GameObject uiManagerPrefab;
        
        [Header("Scoring System")]
        [SerializeField] private MomentumSystem momentumSystem;
        [SerializeField] private TurnScoreCalculator scoreCalculator;
        [SerializeField] private PowerOrbManager powerOrbManager;
        
        // Singleton instance
        private static GameManager instance;
        public static GameManager Instance => instance;
        
        // Game state
        public GameState CurrentState { get; private set; } = GameState.Menu;
        
        // Scoring system
        public int CurrentScore { get; private set; } = 0;
        public int HighScore { get; private set; } = 0;
        
        // Properties
        public bool IsGameActive => CurrentState == GameState.Playing;
        public bool IsPaused => CurrentState == GameState.Paused;
        
        private void Awake()
        {
            // Singleton setup
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Initialize game
            InitializeGame();
        }
        
        private void Start()
        {
            // Set target frame rate
            Application.targetFrameRate = targetFPS;
            
            // Start in menu state
            ChangeState(GameState.Menu);
        }
        
        private void InitializeGame()
        {
            // Initialize core systems
            Log("Initializing SWITCH Game Manager");
            
            // Initialize scoring system components
            if (momentumSystem == null)
                momentumSystem = GetComponent<MomentumSystem>();
            if (scoreCalculator == null)
                scoreCalculator = GetComponent<TurnScoreCalculator>();
            if (powerOrbManager == null)
                powerOrbManager = FindObjectOfType<PowerOrbManager>();
            
            // TODO: Initialize other managers
            // - BoardController
            // - UIManager
            // - AudioManager
            // - AnalyticsManager
        }
        
        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;
            
            var previousState = CurrentState;
            CurrentState = newState;
            
            Log($"Game state changed: {previousState} -> {newState}");
            
            // Handle state transitions
            OnStateChanged(previousState, newState);
        }
        
        private void OnStateChanged(GameState previousState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Menu:
                    HandleMenuState();
                    break;
                case GameState.Playing:
                    HandlePlayingState();
                    break;
                case GameState.Paused:
                    HandlePausedState();
                    break;
                case GameState.GameOver:
                    HandleGameOverState();
                    break;
            }
        }
        
        private void HandleMenuState()
        {
            Time.timeScale = 1f;
            // TODO: Show main menu
        }
        
        private void HandlePlayingState()
        {
            Time.timeScale = 1f;
            // TODO: Start gameplay
        }
        
        private void HandlePausedState()
        {
            Time.timeScale = 0f;
            // TODO: Show pause menu
        }
        
        private void HandleGameOverState()
        {
            Time.timeScale = 0f;
            // TODO: Show game over screen
        }
        
        public void StartGame()
        {
            ChangeState(GameState.Playing);
        }
        
        public void PauseGame()
        {
            if (CurrentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
        }
        
        public void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
            }
        }
        
        public void EndGame()
        {
            ChangeState(GameState.GameOver);
        }
        
        public void ReturnToMenu()
        {
            ChangeState(GameState.Menu);
        }
        
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
        
        private void Log(string message)
        {
            if (debugMode)
            {
                Debug.Log($"[GameManager] {message}");
            }
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && IsGameActive)
            {
                PauseGame();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && IsGameActive)
            {
                PauseGame();
            }
        }
    }
    
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver
    }
}
