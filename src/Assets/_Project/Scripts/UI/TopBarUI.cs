using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace SWITCH.UI
{
    /// <summary>
    /// Manages the top bar UI containing score display and menu button
    /// </summary>
    public class TopBarUI : MonoBehaviour
    {
        [Header("Score Display")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private bool animateScore = true;
        [SerializeField] private float scoreAnimationSpeed = 2f;
        
        [Header("Menu Button")]
        [SerializeField] private Button menuButton;
        [SerializeField] private Image menuIcon;
        
        [Header("Time Trial Mode")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private bool isTimeTrialMode = false;
        
        [Header("Visual Settings")]
        [SerializeField] private Color scoreColor = UIColors.scoreText;
        [SerializeField] private Color menuButtonColor = UIColors.menuButton;
        
        // References
        private GameManager gameManager;
        private long currentDisplayedScore = 0;
        private long targetScore = 0;
        private Coroutine scoreAnimationCoroutine;
        
        // Events
        public System.Action OnMenuButtonPressed;
        
        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.OnScoreChanged += OnScoreChanged;
            }
        }
        
        private void Start()
        {
            InitializeUI();
        }
        
        private void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.OnScoreChanged -= OnScoreChanged;
            }
        }
        
        /// <summary>
        /// Initializes the UI components
        /// </summary>
        private void InitializeUI()
        {
            // Initialize score text
            if (scoreText != null)
            {
                scoreText.color = scoreColor;
                scoreText.text = "0";
            }
            
            // Initialize menu button
            if (menuButton != null)
            {
                menuButton.onClick.AddListener(OnMenuButtonClick);
            }
            
            if (menuIcon != null)
            {
                menuIcon.color = menuButtonColor;
            }
            
            // Initialize timer text (hidden by default)
            if (timerText != null)
            {
                timerText.gameObject.SetActive(isTimeTrialMode);
            }
        }
        
        /// <summary>
        /// Called when the score changes
        /// </summary>
        /// <param name="newScore">New score value</param>
        private void OnScoreChanged(long newScore)
        {
            targetScore = newScore;
            
            if (animateScore)
            {
                AnimateScoreChange();
            }
            else
            {
                UpdateScoreDisplay(newScore);
            }
        }
        
        /// <summary>
        /// Animates the score change smoothly
        /// </summary>
        private void AnimateScoreChange()
        {
            if (scoreAnimationCoroutine != null)
            {
                StopCoroutine(scoreAnimationCoroutine);
            }
            
            scoreAnimationCoroutine = StartCoroutine(AnimateScoreCoroutine());
        }
        
        /// <summary>
        /// Score animation coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator AnimateScoreCoroutine()
        {
            long startScore = currentDisplayedScore;
            float elapsed = 0f;
            
            while (elapsed < 1f / scoreAnimationSpeed)
            {
                elapsed += Time.deltaTime;
                float t = elapsed * scoreAnimationSpeed;
                t = Mathf.SmoothStep(0f, 1f, t);
                
                currentDisplayedScore = (long)Mathf.Lerp(startScore, targetScore, t);
                UpdateScoreDisplay(currentDisplayedScore);
                
                yield return null;
            }
            
            currentDisplayedScore = targetScore;
            UpdateScoreDisplay(currentDisplayedScore);
        }
        
        /// <summary>
        /// Updates the score display text
        /// </summary>
        /// <param name="score">Score to display</param>
        private void UpdateScoreDisplay(long score)
        {
            if (scoreText != null)
            {
                scoreText.text = FormatScore(score);
            }
        }
        
        /// <summary>
        /// Formats the score with commas
        /// </summary>
        /// <param name="score">Score to format</param>
        /// <returns>Formatted score string</returns>
        private string FormatScore(long score)
        {
            return score.ToString("N0");
        }
        
        /// <summary>
        /// Called when menu button is clicked
        /// </summary>
        private void OnMenuButtonClick()
        {
            OnMenuButtonPressed?.Invoke();
        }
        
        /// <summary>
        /// Sets the time trial mode
        /// </summary>
        /// <param name="isTimeTrial">Whether time trial mode is active</param>
        public void SetTimeTrialMode(bool isTimeTrial)
        {
            isTimeTrialMode = isTimeTrial;
            if (timerText != null)
            {
                timerText.gameObject.SetActive(isTimeTrial);
            }
        }
        
        /// <summary>
        /// Updates the timer display
        /// </summary>
        /// <param name="timeRemaining">Time remaining in seconds</param>
        public void UpdateTimer(float timeRemaining)
        {
            if (timerText != null && isTimeTrialMode)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60f);
                int seconds = Mathf.FloorToInt(timeRemaining % 60f);
                timerText.text = $"{minutes}:{seconds:00}";
            }
        }
        
        /// <summary>
        /// Sets the score immediately without animation
        /// </summary>
        /// <param name="score">Score to set</param>
        public void SetScoreImmediate(long score)
        {
            targetScore = score;
            currentDisplayedScore = score;
            UpdateScoreDisplay(score);
        }
        
        #region Editor Debugging
        #if UNITY_EDITOR
        [Header("Editor Debug Controls")]
        [SerializeField] private bool showDebugControls = false;
        [SerializeField] private long debugScore = 1000000;
        
        private void OnGUI()
        {
            if (!showDebugControls) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 200, 150));
            GUILayout.Label("Top Bar UI Debug");
            GUILayout.Label($"Current Score: {currentDisplayedScore:N0}");
            GUILayout.Label($"Target Score: {targetScore:N0}");
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Test Score Animation"))
            {
                OnScoreChanged(debugScore);
            }
            
            if (GUILayout.Button("Set Score Immediate"))
            {
                SetScoreImmediate(debugScore);
            }
            
            GUILayout.EndArea();
        }
        #endif
        #endregion
    }
}
