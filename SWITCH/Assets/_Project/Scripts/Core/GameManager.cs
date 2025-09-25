using UnityEngine;
using System;

namespace Switch.Core
{
    /// <summary>
    /// Main game manager singleton that handles game state and core systems
    /// Implements basic state machine for Menu, Playing, and GameOver states
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game Configuration")]
        [SerializeField] private bool debugMode = true;
        [SerializeField] private float targetFrameRate = 60f;
        
        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Menu;
        
        // Singleton instance
        public static GameManager Instance { get; private set; }
        
        // Events
        public static event Action<GameState> OnGameStateChanged;
        public static event Action OnGameStarted;
        public static event Action OnGameEnded;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        
        // Properties
        public GameState CurrentState 
        { 
            get => currentState; 
            private set 
            {
                if (currentState != value)
                {
                    var previousState = currentState;
                    currentState = value;
                    OnGameStateChanged?.Invoke(currentState);
                    
                    if (debugMode)
                    {
                        Debug.Log($"Game State: {previousState} -> {currentState}");
                    }
                }
            }
        }
        
        public bool IsPlaying => currentState == GameState.Playing;
        public bool IsPaused => currentState == GameState.Paused;
        public bool IsGameOver => currentState == GameState.GameOver;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // Set target frame rate for mobile optimization
            Application.targetFrameRate = (int)targetFrameRate;
            
            // Start in menu state
            ChangeState(GameState.Menu);
        }
        
        private void InitializeGame()
        {
            if (debugMode)
            {
                Debug.Log("GameManager initialized");
            }
            
            // Subscribe to application events
            Application.focusChanged += OnApplicationFocusChanged;
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            Application.focusChanged -= OnApplicationFocusChanged;
        }
        
        #region State Management
        
        /// <summary>
        /// Changes the game state and triggers appropriate events
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (currentState == newState) return;
            
            // Exit current state
            ExitState(currentState);
            
            // Change state
            CurrentState = newState;
            
            // Enter new state
            EnterState(newState);
        }
        
        private void EnterState(GameState state)
        {
            switch (state)
            {
                case GameState.Menu:
                    EnterMenuState();
                    break;
                case GameState.Playing:
                    EnterPlayingState();
                    break;
                case GameState.Paused:
                    EnterPausedState();
                    break;
                case GameState.GameOver:
                    EnterGameOverState();
                    break;
            }
        }
        
        private void ExitState(GameState state)
        {
            switch (state)
            {
                case GameState.Menu:
                    ExitMenuState();
                    break;
                case GameState.Playing:
                    ExitPlayingState();
                    break;
                case GameState.Paused:
                    ExitPausedState();
                    break;
                case GameState.GameOver:
                    ExitGameOverState();
                    break;
            }
        }
        
        #endregion
        
        #region State Implementations
        
        private void EnterMenuState()
        {
            Time.timeScale = 1f;
            if (debugMode) Debug.Log("Entered Menu State");
        }
        
        private void ExitMenuState()
        {
            if (debugMode) Debug.Log("Exited Menu State");
        }
        
        private void EnterPlayingState()
        {
            Time.timeScale = 1f;
            OnGameStarted?.Invoke();
            if (debugMode) Debug.Log("Entered Playing State");
        }
        
        private void ExitPlayingState()
        {
            if (debugMode) Debug.Log("Exited Playing State");
        }
        
        private void EnterPausedState()
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke();
            if (debugMode) Debug.Log("Entered Paused State");
        }
        
        private void ExitPausedState()
        {
            Time.timeScale = 1f;
            OnGameResumed?.Invoke();
            if (debugMode) Debug.Log("Exited Paused State");
        }
        
        private void EnterGameOverState()
        {
            Time.timeScale = 0f;
            OnGameEnded?.Invoke();
            if (debugMode) Debug.Log("Entered Game Over State");
        }
        
        private void ExitGameOverState()
        {
            Time.timeScale = 1f;
            if (debugMode) Debug.Log("Exited Game Over State");
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Starts a new game
        /// </summary>
        public void StartGame()
        {
            if (currentState == GameState.Menu || currentState == GameState.GameOver)
            {
                ChangeState(GameState.Playing);
            }
        }
        
        /// <summary>
        /// Pauses the current game
        /// </summary>
        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
        }
        
        /// <summary>
        /// Resumes the paused game
        /// </summary>
        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
            }
        }
        
        /// <summary>
        /// Ends the current game
        /// </summary>
        public void EndGame()
        {
            if (currentState == GameState.Playing)
            {
                ChangeState(GameState.GameOver);
            }
        }
        
        /// <summary>
        /// Returns to the main menu
        /// </summary>
        public void ReturnToMenu()
        {
            ChangeState(GameState.Menu);
        }
        
        /// <summary>
        /// Toggles pause state
        /// </summary>
        public void TogglePause()
        {
            if (currentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
        }
        
        #endregion
        
        #region Application Events
        
        private void OnApplicationFocusChanged(bool hasFocus)
        {
            if (!hasFocus && currentState == GameState.Playing)
            {
                PauseGame();
            }
        }
        
        // Note: Application.pauseChanged is not available in Unity 2022.3
        // Use OnApplicationPause instead for mobile platforms
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && currentState == GameState.Playing)
            {
                PauseGame();
            }
        }
        
        #endregion
        
        #region Debug
        
        [ContextMenu("Start Game")]
        private void DebugStartGame()
        {
            StartGame();
        }
        
        [ContextMenu("End Game")]
        private void DebugEndGame()
        {
            EndGame();
        }
        
        [ContextMenu("Toggle Pause")]
        private void DebugTogglePause()
        {
            TogglePause();
        }
        
        #endregion
    }
    
    /// <summary>
    /// Enumeration of possible game states
    /// </summary>
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver
    }
}
