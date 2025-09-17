using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace SWITCH.UI
{
    /// <summary>
    /// Manages the pause menu overlay system
    /// </summary>
    public class MenuOverlayUI : MonoBehaviour
    {
        [Header("Menu Panel")]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private CanvasGroup menuCanvasGroup;
        [SerializeField] private TextMeshProUGUI menuTitle;
        
        [Header("Menu Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button howToPlayButton;
        [SerializeField] private Button achievementsButton;
        [SerializeField] private Button leaderboardsButton;
        [SerializeField] private Button shareScoreButton;
        [SerializeField] private Button mainMenuButton;
        
        [Header("Animation Settings")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.2f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Visual Settings")]
        [SerializeField] private Color menuBackgroundColor = new Color(0, 0, 0, 0.8f);
        [SerializeField] private Color buttonNormalColor = Color.white;
        [SerializeField] private Color buttonHighlightedColor = new Color(1f, 0.8f, 0.2f);
        
        // State
        private bool isMenuOpen = false;
        private Coroutine fadeCoroutine;
        
        // Events
        public System.Action OnResumeGame;
        public System.Action OnOpenSettings;
        public System.Action OnOpenHowToPlay;
        public System.Action OnOpenAchievements;
        public System.Action OnOpenLeaderboards;
        public System.Action OnShareScore;
        public System.Action OnReturnToMainMenu;
        
        private void Awake()
        {
            InitializeMenu();
        }
        
        private void Start()
        {
            // Start with menu hidden
            if (menuPanel != null)
            {
                menuPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// Initializes the menu components
        /// </summary>
        private void InitializeMenu()
        {
            // Initialize menu panel
            if (menuPanel != null)
            {
                menuPanel.SetActive(false);
            }
            
            // Initialize canvas group
            if (menuCanvasGroup == null)
            {
                menuCanvasGroup = menuPanel.GetComponent<CanvasGroup>();
            }
            
            if (menuCanvasGroup != null)
            {
                menuCanvasGroup.alpha = 0f;
                menuCanvasGroup.interactable = false;
                menuCanvasGroup.blocksRaycasts = false;
            }
            
            // Initialize menu title
            if (menuTitle != null)
            {
                menuTitle.text = "PAUSED";
            }
            
            // Initialize buttons
            InitializeButtons();
        }
        
        /// <summary>
        /// Initializes menu buttons
        /// </summary>
        private void InitializeButtons()
        {
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(OnResumeButtonClick);
            }
            
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsButtonClick);
            }
            
            if (howToPlayButton != null)
            {
                howToPlayButton.onClick.AddListener(OnHowToPlayButtonClick);
            }
            
            if (achievementsButton != null)
            {
                achievementsButton.onClick.AddListener(OnAchievementsButtonClick);
            }
            
            if (leaderboardsButton != null)
            {
                leaderboardsButton.onClick.AddListener(OnLeaderboardsButtonClick);
            }
            
            if (shareScoreButton != null)
            {
                shareScoreButton.onClick.AddListener(OnShareScoreButtonClick);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);
            }
        }
        
        /// <summary>
        /// Opens the menu overlay
        /// </summary>
        public void OpenMenu()
        {
            if (isMenuOpen) return;
            
            isMenuOpen = true;
            
            if (menuPanel != null)
            {
                menuPanel.SetActive(true);
            }
            
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            
            fadeCoroutine = StartCoroutine(FadeInCoroutine());
        }
        
        /// <summary>
        /// Closes the menu overlay
        /// </summary>
        public void CloseMenu()
        {
            if (!isMenuOpen) return;
            
            isMenuOpen = false;
            
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            
            fadeCoroutine = StartCoroutine(FadeOutCoroutine());
        }
        
        /// <summary>
        /// Toggles the menu overlay
        /// </summary>
        public void ToggleMenu()
        {
            if (isMenuOpen)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
        
        /// <summary>
        /// Fade in coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator FadeInCoroutine()
        {
            if (menuCanvasGroup == null) yield break;
            
            float elapsed = 0f;
            float startAlpha = menuCanvasGroup.alpha;
            
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeInDuration;
                t = fadeCurve.Evaluate(t);
                
                menuCanvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, t);
                
                yield return null;
            }
            
            menuCanvasGroup.alpha = 1f;
            menuCanvasGroup.interactable = true;
            menuCanvasGroup.blocksRaycasts = true;
        }
        
        /// <summary>
        /// Fade out coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator FadeOutCoroutine()
        {
            if (menuCanvasGroup == null) yield break;
            
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
            
            float elapsed = 0f;
            float startAlpha = menuCanvasGroup.alpha;
            
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeOutDuration;
                t = fadeCurve.Evaluate(t);
                
                menuCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
                
                yield return null;
            }
            
            menuCanvasGroup.alpha = 0f;
            
            if (menuPanel != null)
            {
                menuPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// Called when resume button is clicked
        /// </summary>
        private void OnResumeButtonClick()
        {
            CloseMenu();
            OnResumeGame?.Invoke();
        }
        
        /// <summary>
        /// Called when settings button is clicked
        /// </summary>
        private void OnSettingsButtonClick()
        {
            OnOpenSettings?.Invoke();
        }
        
        /// <summary>
        /// Called when how to play button is clicked
        /// </summary>
        private void OnHowToPlayButtonClick()
        {
            OnOpenHowToPlay?.Invoke();
        }
        
        /// <summary>
        /// Called when achievements button is clicked
        /// </summary>
        private void OnAchievementsButtonClick()
        {
            OnOpenAchievements?.Invoke();
        }
        
        /// <summary>
        /// Called when leaderboards button is clicked
        /// </summary>
        private void OnLeaderboardsButtonClick()
        {
            OnOpenLeaderboards?.Invoke();
        }
        
        /// <summary>
        /// Called when share score button is clicked
        /// </summary>
        private void OnShareScoreButtonClick()
        {
            OnShareScore?.Invoke();
        }
        
        /// <summary>
        /// Called when main menu button is clicked
        /// </summary>
        private void OnMainMenuButtonClick()
        {
            OnReturnToMainMenu?.Invoke();
        }
        
        /// <summary>
        /// Sets the menu title
        /// </summary>
        /// <param name="title">New title text</param>
        public void SetMenuTitle(string title)
        {
            if (menuTitle != null)
            {
                menuTitle.text = title;
            }
        }
        
        /// <summary>
        /// Enables or disables specific menu buttons
        /// </summary>
        /// <param name="buttonType">Type of button to modify</param>
        /// <param name="enabled">Whether to enable the button</param>
        public void SetButtonEnabled(MenuButtonType buttonType, bool enabled)
        {
            Button button = GetButtonByType(buttonType);
            if (button != null)
            {
                button.interactable = enabled;
            }
        }
        
        /// <summary>
        /// Gets a button by its type
        /// </summary>
        /// <param name="buttonType">Type of button</param>
        /// <returns>Button component or null</returns>
        private Button GetButtonByType(MenuButtonType buttonType)
        {
            switch (buttonType)
            {
                case MenuButtonType.Resume:
                    return resumeButton;
                case MenuButtonType.Settings:
                    return settingsButton;
                case MenuButtonType.HowToPlay:
                    return howToPlayButton;
                case MenuButtonType.Achievements:
                    return achievementsButton;
                case MenuButtonType.Leaderboards:
                    return leaderboardsButton;
                case MenuButtonType.ShareScore:
                    return shareScoreButton;
                case MenuButtonType.MainMenu:
                    return mainMenuButton;
                default:
                    return null;
            }
        }
        
        /// <summary>
        /// Checks if the menu is currently open
        /// </summary>
        /// <returns>True if menu is open</returns>
        public bool IsMenuOpen()
        {
            return isMenuOpen;
        }
        
        /// <summary>
        /// Sets the menu background color
        /// </summary>
        /// <param name="color">New background color</param>
        public void SetBackgroundColor(Color color)
        {
            menuBackgroundColor = color;
            
            // Apply to background image if available
            Image backgroundImage = menuPanel.GetComponent<Image>();
            if (backgroundImage != null)
            {
                backgroundImage.color = color;
            }
        }
        
        public enum MenuButtonType
        {
            Resume,
            Settings,
            HowToPlay,
            Achievements,
            Leaderboards,
            ShareScore,
            MainMenu
        }
        
        #region Editor Debugging
        #if UNITY_EDITOR
        [Header("Editor Debug Controls")]
        [SerializeField] private bool showDebugControls = false;
        
        private void OnGUI()
        {
            if (!showDebugControls) return;
            
            GUILayout.BeginArea(new Rect(10, 600, 200, 150));
            GUILayout.Label("Menu Overlay UI Debug");
            GUILayout.Label($"Menu Open: {isMenuOpen}");
            GUILayout.Label($"Canvas Alpha: {(menuCanvasGroup != null ? menuCanvasGroup.alpha : 0f):F2}");
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Open Menu"))
                OpenMenu();
            if (GUILayout.Button("Close Menu"))
                CloseMenu();
            if (GUILayout.Button("Toggle Menu"))
                ToggleMenu();
            
            GUILayout.EndArea();
        }
        #endif
        #endregion
    }
}
