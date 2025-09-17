using UnityEngine;
using System.Collections.Generic;

namespace SWITCH.UI
{
    /// <summary>
    /// Main UI manager that coordinates all UI components
    /// </summary>
    public class MainUIManager : MonoBehaviour
    {
        [Header("UI Component References")]
        [SerializeField] private TopBarUI topBarUI;
        [SerializeField] private HeatMeterUI heatMeterUI;
        [SerializeField] private GameAreaUI gameAreaUI;
        [SerializeField] private PowerFeedbackUI powerFeedbackUI;
        [SerializeField] private AdBannerUI adBannerUI;
        [SerializeField] private MenuOverlayUI menuOverlayUI;
        [SerializeField] private UIScaler uiScaler;
        
        [Header("UI Layout Settings")]
        [SerializeField] private bool autoFindComponents = true;
        [SerializeField] private bool initializeOnStart = true;
        
        // References
        private GameManager gameManager;
        private BoardController boardController;
        private MomentumSystem momentumSystem;
        
        // State
        private bool isInitialized = false;
        
        // Events
        public System.Action OnUIInitialized;
        public System.Action OnMenuOpened;
        public System.Action OnMenuClosed;
        
        private void Awake()
        {
            if (autoFindComponents)
            {
                FindUIComponents();
            }
            
            GetGameReferences();
        }
        
        private void Start()
        {
            if (initializeOnStart)
            {
                InitializeUI();
            }
        }
        
        /// <summary>
        /// Finds UI components automatically
        /// </summary>
        private void FindUIComponents()
        {
            if (topBarUI == null)
                topBarUI = FindObjectOfType<TopBarUI>();
            
            if (heatMeterUI == null)
                heatMeterUI = FindObjectOfType<HeatMeterUI>();
            
            if (gameAreaUI == null)
                gameAreaUI = FindObjectOfType<GameAreaUI>();
            
            if (powerFeedbackUI == null)
                powerFeedbackUI = FindObjectOfType<PowerFeedbackUI>();
            
            if (adBannerUI == null)
                adBannerUI = FindObjectOfType<AdBannerUI>();
            
            if (menuOverlayUI == null)
                menuOverlayUI = FindObjectOfType<MenuOverlayUI>();
            
            if (uiScaler == null)
                uiScaler = FindObjectOfType<UIScaler>();
        }
        
        /// <summary>
        /// Gets references to game systems
        /// </summary>
        private void GetGameReferences()
        {
            gameManager = FindObjectOfType<GameManager>();
            boardController = FindObjectOfType<BoardController>();
            momentumSystem = FindObjectOfType<MomentumSystem>();
        }
        
        /// <summary>
        /// Initializes the UI system
        /// </summary>
        public void InitializeUI()
        {
            if (isInitialized) return;
            
            // Initialize UI scaler first
            if (uiScaler != null)
            {
                uiScaler.ForceAdjustment();
            }
            
            // Initialize individual UI components
            InitializeTopBar();
            InitializeHeatMeter();
            InitializeGameArea();
            InitializePowerFeedback();
            InitializeAdBanner();
            InitializeMenuOverlay();
            
            // Connect events
            ConnectEvents();
            
            isInitialized = true;
            OnUIInitialized?.Invoke();
            
            Debug.Log("[MainUIManager] UI system initialized successfully");
        }
        
        /// <summary>
        /// Initializes the top bar UI
        /// </summary>
        private void InitializeTopBar()
        {
            if (topBarUI != null)
            {
                topBarUI.OnMenuButtonPressed += OnMenuButtonPressed;
            }
        }
        
        /// <summary>
        /// Initializes the heat meter UI
        /// </summary>
        private void InitializeHeatMeter()
        {
            if (heatMeterUI != null)
            {
                // Heat meter is self-initializing
            }
        }
        
        /// <summary>
        /// Initializes the game area UI
        /// </summary>
        private void InitializeGameArea()
        {
            if (gameAreaUI != null)
            {
                gameAreaUI.OnQueueDotClicked += OnQueueDotClicked;
                gameAreaUI.OnTileSlotClicked += OnTileSlotClicked;
            }
        }
        
        /// <summary>
        /// Initializes the power feedback UI
        /// </summary>
        private void InitializePowerFeedback()
        {
            if (powerFeedbackUI != null)
            {
                powerFeedbackUI.OnPowerUpUsed += OnPowerUpUsed;
            }
        }
        
        /// <summary>
        /// Initializes the ad banner UI
        /// </summary>
        private void InitializeAdBanner()
        {
            if (adBannerUI != null)
            {
                // Ad banner is self-initializing
            }
        }
        
        /// <summary>
        /// Initializes the menu overlay UI
        /// </summary>
        private void InitializeMenuOverlay()
        {
            if (menuOverlayUI != null)
            {
                menuOverlayUI.OnResumeGame += OnResumeGame;
                menuOverlayUI.OnOpenSettings += OnOpenSettings;
                menuOverlayUI.OnOpenHowToPlay += OnOpenHowToPlay;
                menuOverlayUI.OnOpenAchievements += OnOpenAchievements;
                menuOverlayUI.OnOpenLeaderboards += OnOpenLeaderboards;
                menuOverlayUI.OnShareScore += OnShareScore;
                menuOverlayUI.OnReturnToMainMenu += OnReturnToMainMenu;
            }
        }
        
        /// <summary>
        /// Connects UI events to game systems
        /// </summary>
        private void ConnectEvents()
        {
            // Connect game manager events
            if (gameManager != null)
            {
                gameManager.OnScoreChanged += OnScoreChanged;
                gameManager.OnGameStateChanged += OnGameStateChanged;
            }
            
            // Connect momentum system events
            if (momentumSystem != null)
            {
                momentumSystem.OnMomentumChanged += OnMomentumChanged;
                momentumSystem.OnHeatLevelChanged += OnHeatLevelChanged;
            }
        }
        
        /// <summary>
        /// Called when menu button is pressed
        /// </summary>
        private void OnMenuButtonPressed()
        {
            if (menuOverlayUI != null)
            {
                menuOverlayUI.ToggleMenu();
                
                if (menuOverlayUI.IsMenuOpen())
                {
                    OnMenuOpened?.Invoke();
                }
                else
                {
                    OnMenuClosed?.Invoke();
                }
            }
        }
        
        /// <summary>
        /// Called when a queue dot is clicked
        /// </summary>
        /// <param name="index">Index of the clicked dot</param>
        private void OnQueueDotClicked(int index)
        {
            // Handle queue dot click
            Debug.Log($"[MainUIManager] Queue dot {index} clicked");
        }
        
        /// <summary>
        /// Called when a tile slot is clicked
        /// </summary>
        /// <param name="gridPos">Grid position of the clicked slot</param>
        private void OnTileSlotClicked(Vector2Int gridPos)
        {
            // Handle tile slot click
            Debug.Log($"[MainUIManager] Tile slot {gridPos} clicked");
        }
        
        /// <summary>
        /// Called when a power-up is used
        /// </summary>
        /// <param name="powerIndex">Index of the power-up used</param>
        private void OnPowerUpUsed(int powerIndex)
        {
            // Handle power-up usage
            Debug.Log($"[MainUIManager] Power-up {powerIndex} used");
        }
        
        /// <summary>
        /// Called when score changes
        /// </summary>
        /// <param name="newScore">New score value</param>
        private void OnScoreChanged(long newScore)
        {
            // Score is handled by TopBarUI automatically
        }
        
        /// <summary>
        /// Called when game state changes
        /// </summary>
        /// <param name="newState">New game state</param>
        private void OnGameStateChanged(GameManager.GameState newState)
        {
            // Handle game state changes
            switch (newState)
            {
                case GameManager.GameState.Paused:
                    if (menuOverlayUI != null)
                    {
                        menuOverlayUI.SetMenuTitle("PAUSED");
                    }
                    break;
                case GameManager.GameState.GameOver:
                    if (menuOverlayUI != null)
                    {
                        menuOverlayUI.SetMenuTitle("GAME OVER");
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Called when momentum changes
        /// </summary>
        /// <param name="newMomentum">New momentum value</param>
        private void OnMomentumChanged(float newMomentum)
        {
            // Momentum is handled by HeatMeterUI automatically
        }
        
        /// <summary>
        /// Called when heat level changes
        /// </summary>
        /// <param name="newHeat">New heat value</param>
        private void OnHeatLevelChanged(float newHeat)
        {
            // Heat level is handled by HeatMeterUI automatically
        }
        
        /// <summary>
        /// Called when resume game is requested
        /// </summary>
        private void OnResumeGame()
        {
            if (gameManager != null)
            {
                gameManager.ResumeGame();
            }
        }
        
        /// <summary>
        /// Called when settings are requested
        /// </summary>
        private void OnOpenSettings()
        {
            Debug.Log("[MainUIManager] Opening settings...");
        }
        
        /// <summary>
        /// Called when how to play is requested
        /// </summary>
        private void OnOpenHowToPlay()
        {
            Debug.Log("[MainUIManager] Opening how to play...");
        }
        
        /// <summary>
        /// Called when achievements are requested
        /// </summary>
        private void OnOpenAchievements()
        {
            Debug.Log("[MainUIManager] Opening achievements...");
        }
        
        /// <summary>
        /// Called when leaderboards are requested
        /// </summary>
        private void OnOpenLeaderboards()
        {
            Debug.Log("[MainUIManager] Opening leaderboards...");
        }
        
        /// <summary>
        /// Called when share score is requested
        /// </summary>
        private void OnShareScore()
        {
            Debug.Log("[MainUIManager] Sharing score...");
        }
        
        /// <summary>
        /// Called when return to main menu is requested
        /// </summary>
        private void OnReturnToMainMenu()
        {
            if (gameManager != null)
            {
                gameManager.ReturnToMainMenu();
            }
        }
        
        /// <summary>
        /// Shows cascade feedback
        /// </summary>
        /// <param name="cascadeCount">Number of cascades</param>
        public void ShowCascadeFeedback(int cascadeCount)
        {
            if (powerFeedbackUI != null)
            {
                powerFeedbackUI.ShowCascadeFeedback(cascadeCount);
            }
        }
        
        /// <summary>
        /// Shows points popup
        /// </summary>
        /// <param name="points">Points earned</param>
        public void ShowPointsPopup(int points)
        {
            if (powerFeedbackUI != null)
            {
                powerFeedbackUI.ShowPointsPopup(points);
            }
        }
        
        /// <summary>
        /// Shows achievement feedback
        /// </summary>
        /// <param name="achievement">Achievement text</param>
        public void ShowAchievementFeedback(string achievement)
        {
            if (powerFeedbackUI != null)
            {
                powerFeedbackUI.ShowAchievementFeedback(achievement);
            }
        }
        
        /// <summary>
        /// Updates the queue display
        /// </summary>
        /// <param name="queueData">Queue data to display</param>
        public void UpdateQueue(List<object> queueData)
        {
            if (gameAreaUI != null)
            {
                gameAreaUI.UpdateQueue(queueData);
            }
        }
        
        /// <summary>
        /// Sets the current queue index
        /// </summary>
        /// <param name="index">Current queue index</param>
        public void SetCurrentQueueIndex(int index)
        {
            if (gameAreaUI != null)
            {
                gameAreaUI.SetCurrentQueueIndex(index);
            }
        }
        
        /// <summary>
        /// Sets edge glows for power orbs
        /// </summary>
        /// <param name="top">Top edge glow</param>
        /// <param name="right">Right edge glow</param>
        /// <param name="bottom">Bottom edge glow</param>
        /// <param name="left">Left edge glow</param>
        public void SetEdgeGlows(bool top, bool right, bool bottom, bool left)
        {
            if (gameAreaUI != null)
            {
                gameAreaUI.SetAllEdgeGlows(top, right, bottom, left);
            }
        }
        
        /// <summary>
        /// Adds a power-up to the inventory
        /// </summary>
        /// <param name="index">Index of the power-up</param>
        /// <param name="count">Number to add</param>
        public void AddPowerUp(int index, int count = 1)
        {
            if (powerFeedbackUI != null)
            {
                powerFeedbackUI.AddPowerUp(index, count);
            }
        }
        
        /// <summary>
        /// Sets the premium user status
        /// </summary>
        /// <param name="isPremium">Whether user is premium</param>
        public void SetPremiumUser(bool isPremium)
        {
            if (adBannerUI != null)
            {
                adBannerUI.SetPremiumUser(isPremium);
            }
        }
        
        /// <summary>
        /// Gets the current device type
        /// </summary>
        /// <returns>Current device type</returns>
        public UIScaler.DeviceType GetDeviceType()
        {
            if (uiScaler != null)
            {
                return uiScaler.GetDeviceType();
            }
            return UIScaler.DeviceType.Unknown;
        }
        
        /// <summary>
        /// Checks if the UI is initialized
        /// </summary>
        /// <returns>True if UI is initialized</returns>
        public bool IsInitialized()
        {
            return isInitialized;
        }
        
        #region Editor Debugging
        #if UNITY_EDITOR
        [Header("Editor Debug Controls")]
        [SerializeField] private bool showDebugControls = false;
        
        private void OnGUI()
        {
            if (!showDebugControls) return;
            
            GUILayout.BeginArea(new Rect(10, 800, 300, 200));
            GUILayout.Label("Main UI Manager Debug");
            GUILayout.Label($"Initialized: {isInitialized}");
            GUILayout.Label($"Device Type: {GetDeviceType()}");
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Initialize UI"))
                InitializeUI();
            if (GUILayout.Button("Test Cascade x3"))
                ShowCascadeFeedback(3);
            if (GUILayout.Button("Test Points +1500"))
                ShowPointsPopup(1500);
            if (GUILayout.Button("Test Achievement"))
                ShowAchievementFeedback("L-SHAPE!");
            if (GUILayout.Button("Add Fire Power"))
                AddPowerUp(0, 1);
            
            GUILayout.EndArea();
        }
        #endif
        #endregion
    }
}
