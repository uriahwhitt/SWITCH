using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace SWITCH.UI
{
    /// <summary>
    /// Manages the power-ups and feedback UI bar
    /// </summary>
    public class PowerFeedbackUI : MonoBehaviour
    {
        [Header("Power-up Slots")]
        [SerializeField] private Button[] powerSlots = new Button[3];
        [SerializeField] private Image[] powerIcons = new Image[3];
        [SerializeField] private TextMeshProUGUI[] powerCounts = new TextMeshProUGUI[3];
        [SerializeField] private string[] powerNames = { "🔥", "💣", "⚡" };
        
        [Header("Feedback Display")]
        [SerializeField] private TextMeshProUGUI cascadeText;
        [SerializeField] private TextMeshProUGUI pointsPopup;
        [SerializeField] private Animator feedbackAnimator;
        
        [Header("Animation Settings")]
        [SerializeField] private float cascadeAnimationDuration = 1f;
        [SerializeField] private float pointsAnimationDuration = 2f;
        [SerializeField] private float pointsFloatDistance = 50f;
        
        [Header("Visual Settings")]
        [SerializeField] private Color powerReadyColor = UIColors.powerReady;
        [SerializeField] private Color powerCooldownColor = Color.gray;
        [SerializeField] private Color cascadeTextColor = UIColors.cascadeText;
        [SerializeField] private Color pointsPopupColor = UIColors.pointsPopup;
        
        // Power-up state
        private int[] powerCounts = new int[3];
        private bool[] powerReady = new bool[3];
        private float[] powerCooldowns = new float[3];
        
        // Animation state
        private Coroutine cascadeAnimationCoroutine;
        private Coroutine pointsAnimationCoroutine;
        
        // Events
        public System.Action<int> OnPowerUpUsed;
        
        private void Start()
        {
            InitializeUI();
        }
        
        /// <summary>
        /// Initializes the UI components
        /// </summary>
        private void InitializeUI()
        {
            // Initialize power-up slots
            for (int i = 0; i < powerSlots.Length; i++)
            {
                if (powerSlots[i] != null)
                {
                    int index = i; // Capture for closure
                    powerSlots[i].onClick.AddListener(() => OnPowerUpClick(index));
                }
                
                if (powerIcons[i] != null)
                {
                    powerIcons[i].color = powerCooldownColor;
                }
                
                if (powerCounts[i] != null)
                {
                    powerCounts[i].text = "0";
                }
                
                powerReady[i] = false;
                powerCooldowns[i] = 0f;
            }
            
            // Initialize feedback text
            if (cascadeText != null)
            {
                cascadeText.color = cascadeTextColor;
                cascadeText.gameObject.SetActive(false);
            }
            
            if (pointsPopup != null)
            {
                pointsPopup.color = pointsPopupColor;
                pointsPopup.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Called when a power-up is clicked
        /// </summary>
        /// <param name="index">Index of the power-up</param>
        private void OnPowerUpClick(int index)
        {
            if (powerReady[index] && powerCounts[index] > 0)
            {
                UsePowerUp(index);
            }
        }
        
        /// <summary>
        /// Uses a power-up
        /// </summary>
        /// <param name="index">Index of the power-up to use</param>
        private void UsePowerUp(int index)
        {
            if (powerCounts[index] > 0)
            {
                powerCounts[index]--;
                UpdatePowerUpDisplay(index);
                OnPowerUpUsed?.Invoke(index);
                
                // Start cooldown
                StartCoroutine(PowerUpCooldown(index));
            }
        }
        
        /// <summary>
        /// Updates the display for a specific power-up
        /// </summary>
        /// <param name="index">Index of the power-up</param>
        private void UpdatePowerUpDisplay(int index)
        {
            if (powerCounts[index] != null)
            {
                powerCounts[index].text = powerCounts[index].ToString();
            }
            
            if (powerIcons[index] != null)
            {
                powerIcons[index].color = powerReady[index] ? powerReadyColor : powerCooldownColor;
            }
        }
        
        /// <summary>
        /// Power-up cooldown coroutine
        /// </summary>
        /// <param name="index">Index of the power-up</param>
        /// <returns>Coroutine</returns>
        private IEnumerator PowerUpCooldown(int index)
        {
            powerReady[index] = false;
            powerCooldowns[index] = 1f; // 1 second cooldown
            
            while (powerCooldowns[index] > 0f)
            {
                powerCooldowns[index] -= Time.deltaTime;
                yield return null;
            }
            
            powerReady[index] = true;
            UpdatePowerUpDisplay(index);
        }
        
        /// <summary>
        /// Adds a power-up to the inventory
        /// </summary>
        /// <param name="index">Index of the power-up</param>
        /// <param name="count">Number to add</param>
        public void AddPowerUp(int index, int count = 1)
        {
            if (index >= 0 && index < powerCounts.Length)
            {
                powerCounts[index] += count;
                UpdatePowerUpDisplay(index);
            }
        }
        
        /// <summary>
        /// Sets the count for a power-up
        /// </summary>
        /// <param name="index">Index of the power-up</param>
        /// <param name="count">New count</param>
        public void SetPowerUpCount(int index, int count)
        {
            if (index >= 0 && index < powerCounts.Length)
            {
                powerCounts[index] = count;
                UpdatePowerUpDisplay(index);
            }
        }
        
        /// <summary>
        /// Shows cascade feedback
        /// </summary>
        /// <param name="cascadeCount">Number of cascades</param>
        public void ShowCascadeFeedback(int cascadeCount)
        {
            if (cascadeText != null)
            {
                cascadeText.text = $"CASCADE x{cascadeCount}!";
                cascadeText.gameObject.SetActive(true);
                
                if (cascadeAnimationCoroutine != null)
                {
                    StopCoroutine(cascadeAnimationCoroutine);
                }
                
                cascadeAnimationCoroutine = StartCoroutine(CascadeAnimationCoroutine());
            }
        }
        
        /// <summary>
        /// Shows points popup
        /// </summary>
        /// <param name="points">Points earned</param>
        public void ShowPointsPopup(int points)
        {
            if (pointsPopup != null)
            {
                pointsPopup.text = $"+{points:N0}";
                pointsPopup.gameObject.SetActive(true);
                
                if (pointsAnimationCoroutine != null)
                {
                    StopCoroutine(pointsAnimationCoroutine);
                }
                
                pointsAnimationCoroutine = StartCoroutine(PointsAnimationCoroutine());
            }
        }
        
        /// <summary>
        /// Shows special achievement feedback
        /// </summary>
        /// <param name="achievement">Achievement text</param>
        public void ShowAchievementFeedback(string achievement)
        {
            if (cascadeText != null)
            {
                cascadeText.text = achievement;
                cascadeText.gameObject.SetActive(true);
                
                if (cascadeAnimationCoroutine != null)
                {
                    StopCoroutine(cascadeAnimationCoroutine);
                }
                
                cascadeAnimationCoroutine = StartCoroutine(CascadeAnimationCoroutine());
            }
        }
        
        /// <summary>
        /// Cascade animation coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator CascadeAnimationCoroutine()
        {
            if (cascadeText == null) yield break;
            
            // Scale punch effect
            Vector3 originalScale = cascadeText.transform.localScale;
            Vector3 punchScale = originalScale * 1.2f;
            
            float elapsed = 0f;
            while (elapsed < cascadeAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / cascadeAnimationDuration;
                
                // Scale animation
                if (t < 0.2f)
                {
                    float scaleT = t / 0.2f;
                    cascadeText.transform.localScale = Vector3.Lerp(originalScale, punchScale, scaleT);
                }
                else
                {
                    float scaleT = (t - 0.2f) / 0.8f;
                    cascadeText.transform.localScale = Vector3.Lerp(punchScale, originalScale, scaleT);
                }
                
                yield return null;
            }
            
            cascadeText.transform.localScale = originalScale;
            cascadeText.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Points animation coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator PointsAnimationCoroutine()
        {
            if (pointsPopup == null) yield break;
            
            Vector3 startPos = pointsPopup.transform.localPosition;
            Vector3 endPos = startPos + Vector3.up * pointsFloatDistance;
            Color startColor = pointsPopup.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
            
            float elapsed = 0f;
            while (elapsed < pointsAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / pointsAnimationDuration;
                
                pointsPopup.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
                pointsPopup.color = Color.Lerp(startColor, endColor, t);
                
                yield return null;
            }
            
            pointsPopup.gameObject.SetActive(false);
            pointsPopup.transform.localPosition = startPos;
            pointsPopup.color = startColor;
        }
        
        /// <summary>
        /// Updates all power-up displays
        /// </summary>
        private void UpdateAllPowerUpDisplays()
        {
            for (int i = 0; i < powerCounts.Length; i++)
            {
                UpdatePowerUpDisplay(i);
            }
        }
        
        /// <summary>
        /// Gets the count of a specific power-up
        /// </summary>
        /// <param name="index">Index of the power-up</param>
        /// <returns>Count of the power-up</returns>
        public int GetPowerUpCount(int index)
        {
            if (index >= 0 && index < powerCounts.Length)
            {
                return powerCounts[index];
            }
            return 0;
        }
        
        /// <summary>
        /// Checks if a power-up is ready to use
        /// </summary>
        /// <param name="index">Index of the power-up</param>
        /// <returns>True if ready to use</returns>
        public bool IsPowerUpReady(int index)
        {
            if (index >= 0 && index < powerReady.Length)
            {
                return powerReady[index] && powerCounts[index] > 0;
            }
            return false;
        }
        
        #region Editor Debugging
        #if UNITY_EDITOR
        [Header("Editor Debug Controls")]
        [SerializeField] private bool showDebugControls = false;
        
        private void OnGUI()
        {
            if (!showDebugControls) return;
            
            GUILayout.BeginArea(new Rect(430, 10, 200, 250));
            GUILayout.Label("Power Feedback UI Debug");
            
            GUILayout.Space(10);
            
            for (int i = 0; i < 3; i++)
            {
                GUILayout.Label($"Power {i} ({powerNames[i]}): {powerCounts[i]} (Ready: {powerReady[i]})");
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Add Fire Power"))
                AddPowerUp(0, 1);
            if (GUILayout.Button("Add Bomb Power"))
                AddPowerUp(1, 1);
            if (GUILayout.Button("Add Lightning Power"))
                AddPowerUp(2, 1);
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Test Cascade x3"))
                ShowCascadeFeedback(3);
            if (GUILayout.Button("Test Points +1500"))
                ShowPointsPopup(1500);
            if (GUILayout.Button("Test Achievement"))
                ShowAchievementFeedback("L-SHAPE!");
            
            GUILayout.EndArea();
        }
        #endif
        #endregion
    }
}
