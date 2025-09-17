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
        
        [Header("Core Systems")]
        [SerializeField] private BoardController boardController;
        [SerializeField] private DirectionalGravity directionalGravity;
        [SerializeField] private ExtendedQueueSystem queueSystem;
        [SerializeField] private MatchDetector matchDetector;
        [SerializeField] private DoubleTapDetector doubleTapDetector;
        
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
            
            // Initialize core game systems
            if (boardController == null)
                boardController = FindObjectOfType<BoardController>();
            if (directionalGravity == null)
                directionalGravity = FindObjectOfType<DirectionalGravity>();
            if (queueSystem == null)
                queueSystem = FindObjectOfType<ExtendedQueueSystem>();
            if (matchDetector == null)
                matchDetector = FindObjectOfType<MatchDetector>();
            if (doubleTapDetector == null)
                doubleTapDetector = FindObjectOfType<DoubleTapDetector>();
            
            // Initialize scoring system components
            if (momentumSystem == null)
                momentumSystem = GetComponent<MomentumSystem>();
            if (scoreCalculator == null)
                scoreCalculator = GetComponent<TurnScoreCalculator>();
            if (powerOrbManager == null)
                powerOrbManager = FindObjectOfType<PowerOrbManager>();
            
            // Set up event connections
            SetupEventConnections();
            
            Log("Game Manager initialization complete");
        }
        
        /// <summary>
        /// Sets up event connections between systems.
        /// Educational: Shows how to implement event-driven architecture.
        /// </summary>
        private void SetupEventConnections()
        {
            // Connect double-tap detection to gravity system
            if (doubleTapDetector != null)
            {
                doubleTapDetector.OnDoubleTapDetected += HandleDoubleTap;
            }
            
            // Connect match detection to scoring
            if (matchDetector != null)
            {
                matchDetector.OnMatchesFound += HandleMatchesFound;
            }
            
            // Connect gravity system to match detection
            if (directionalGravity != null)
            {
                directionalGravity.OnGravityComplete += HandleGravityComplete;
            }
        }
        
        /// <summary>
        /// Handles double-tap input for tile selection.
        /// Educational: Shows how to implement input handling.
        /// </summary>
        private void HandleDoubleTap(Vector2 position)
        {
            if (!IsGameActive) return;
            
            Log($"Double tap detected at {position}");
            
            // Convert screen position to world position
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(position);
            worldPos.z = 0f;
            
            // Find tile at position
            Tile clickedTile = FindTileAtPosition(worldPos);
            if (clickedTile != null)
            {
                HandleTileSelection(clickedTile);
            }
        }
        
        /// <summary>
        /// Handles tile selection logic.
        /// Educational: Shows how to implement tile interaction.
        /// </summary>
        private void HandleTileSelection(Tile tile)
        {
            Log($"Tile selected: {tile}");
            
            // TODO: Implement tile selection logic
            // - Highlight selected tile
            // - Allow for tile swapping
            // - Update gravity direction
        }
        
        /// <summary>
        /// Finds a tile at the given world position.
        /// Educational: Shows how to implement position-based object finding.
        /// </summary>
        private Tile FindTileAtPosition(Vector3 worldPosition)
        {
            if (boardController == null) return null;
            
            // Simple position-based search
            for (int x = 0; x < boardController.Width; x++)
            {
                for (int y = 0; y < boardController.Height; y++)
                {
                    Tile tile = boardController.GetTileAt(x, y);
                    if (tile != null)
                    {
                        float distance = Vector3.Distance(tile.transform.position, worldPosition);
                        if (distance < 0.5f) // Within tile bounds
                        {
                            return tile;
                        }
                    }
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Handles matches found by the match detector.
        /// Educational: Shows how to process match results.
        /// </summary>
        private void HandleMatchesFound(List<MatchData> matches)
        {
            if (matches.Count == 0) return;
            
            Log($"Processing {matches.Count} matches");
            
            // Clear matched tiles
            if (boardController != null)
            {
                List<Vector2Int> positionsToClear = new List<Vector2Int>();
                foreach (var match in matches)
                {
                    positionsToClear.AddRange(match.positions);
                }
                boardController.ClearMatches(positionsToClear);
            }
            
            // Create turn result and handle scoring
            if (matchDetector != null)
            {
                TurnResult turnResult = matchDetector.CreateTurnResult();
                HandleTurnComplete(turnResult);
            }
        }
        
        /// <summary>
        /// Handles gravity completion.
        /// Educational: Shows how to chain game mechanics.
        /// </summary>
        private void HandleGravityComplete()
        {
            Log("Gravity complete, checking for new matches");
            
            // Check for new matches after gravity
            if (matchDetector != null)
            {
                var newMatches = matchDetector.DetectAllMatches();
                if (newMatches.Count > 0)
                {
                    HandleMatchesFound(newMatches);
                }
            }
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
