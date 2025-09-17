using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace SWITCH.UI
{
    /// <summary>
    /// Manages the heat meter UI with visual states and particle effects
    /// </summary>
    public class HeatMeterUI : MonoBehaviour
    {
        [Header("Heat Meter Components")]
        [SerializeField] private Slider heatBar;
        [SerializeField] private Image heatBarFill;
        [SerializeField] private TextMeshProUGUI multiplierText;
        [SerializeField] private TextMeshProUGUI heatLabel;
        
        [Header("Visual Effects")]
        [SerializeField] private ParticleSystem heatParticles;
        [SerializeField] private ParticleSystem flameParticles;
        [SerializeField] private ParticleSystem infernoParticles;
        
        [Header("Animation Settings")]
        [SerializeField] private float colorTransitionSpeed = 2f;
        [SerializeField] private float pulseSpeed = 3f;
        [SerializeField] private float pulseIntensity = 0.2f;
        [SerializeField] private float glowIntensity = 0.3f;
        
        [Header("Heat Level Thresholds")]
        [SerializeField] private float coldThreshold = 2f;
        [SerializeField] private float warmThreshold = 4f;
        [SerializeField] private float hotThreshold = 7f;
        [SerializeField] private float blazingThreshold = 9f;
        [SerializeField] private float maxHeat = 10f;
        
        // References
        private MomentumSystem momentumSystem;
        
        // Animation state
        private Coroutine colorTransitionCoroutine;
        private Coroutine pulseCoroutine;
        private Color currentColor;
        private bool isPulsing = false;
        private float currentHeat = 0f;
        
        // Events
        public System.Action<float> OnHeatLevelChanged;
        
        private void Awake()
        {
            momentumSystem = FindObjectOfType<MomentumSystem>();
            if (momentumSystem != null)
            {
                momentumSystem.OnMomentumChanged += OnMomentumChanged;
                momentumSystem.OnHeatLevelChanged += OnHeatLevelChanged;
            }
        }
        
        private void Start()
        {
            InitializeUI();
        }
        
        private void OnDestroy()
        {
            if (momentumSystem != null)
            {
                momentumSystem.OnMomentumChanged -= OnMomentumChanged;
                momentumSystem.OnHeatLevelChanged -= OnHeatLevelChanged;
            }
        }
        
        /// <summary>
        /// Initializes the UI components
        /// </summary>
        private void InitializeUI()
        {
            // Initialize heat bar
            if (heatBar != null)
            {
                heatBar.minValue = 0f;
                heatBar.maxValue = maxHeat;
                heatBar.value = 0f;
            }
            
            // Initialize heat bar fill color
            if (heatBarFill != null)
            {
                heatBarFill.color = UIColors.heatCold;
                currentColor = UIColors.heatCold;
            }
            
            // Initialize text elements
            UpdateMultiplierText();
            UpdateHeatLabel(0f);
        }
        
        /// <summary>
        /// Called when momentum changes
        /// </summary>
        /// <param name="newMomentum">New momentum value</param>
        private void OnMomentumChanged(float newMomentum)
        {
            UpdateHeatMeter(newMomentum);
            UpdateMultiplierText();
        }
        
        /// <summary>
        /// Called when heat level changes
        /// </summary>
        /// <param name="newHeat">New heat value</param>
        private void OnHeatLevelChanged(float newHeat)
        {
            currentHeat = newHeat;
            UpdateHeatLabel(newHeat);
            UpdateVisualEffects(newHeat);
            OnHeatLevelChanged?.Invoke(newHeat);
        }
        
        /// <summary>
        /// Updates the heat meter slider
        /// </summary>
        /// <param name="heat">Current heat value</param>
        private void UpdateHeatMeter(float heat)
        {
            if (heatBar != null)
            {
                heatBar.value = heat;
            }
        }
        
        /// <summary>
        /// Updates the multiplier text display
        /// </summary>
        private void UpdateMultiplierText()
        {
            if (multiplierText != null && momentumSystem != null)
            {
                float multiplier = momentumSystem.GetScoreMultiplier();
                multiplierText.text = $"x{multiplier:F1}";
            }
        }
        
        /// <summary>
        /// Updates the heat label text
        /// </summary>
        /// <param name="heat">Current heat value</param>
        private void UpdateHeatLabel(float heat)
        {
            if (heatLabel != null)
            {
                heatLabel.text = UIColors.GetHeatLabel(heat);
            }
        }
        
        /// <summary>
        /// Updates visual effects based on heat level
        /// </summary>
        /// <param name="heat">Current heat value</param>
        private void UpdateVisualEffects(float heat)
        {
            Color targetColor = UIColors.GetHeatColor(heat);
            TransitionToColor(targetColor);
            UpdateParticleEffects(heat);
            UpdatePulsingEffect(heat);
        }
        
        /// <summary>
        /// Transitions the heat meter color smoothly
        /// </summary>
        /// <param name="targetColor">Target color</param>
        private void TransitionToColor(Color targetColor)
        {
            if (colorTransitionCoroutine != null)
            {
                StopCoroutine(colorTransitionCoroutine);
            }
            
            colorTransitionCoroutine = StartCoroutine(TransitionColorCoroutine(targetColor));
        }
        
        /// <summary>
        /// Color transition coroutine
        /// </summary>
        /// <param name="targetColor">Target color</param>
        /// <returns>Coroutine</returns>
        private IEnumerator TransitionColorCoroutine(Color targetColor)
        {
            Color startColor = currentColor;
            float elapsed = 0f;
            
            while (elapsed < colorTransitionSpeed)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / colorTransitionSpeed;
                currentColor = Color.Lerp(startColor, targetColor, t);
                
                if (heatBarFill != null)
                {
                    heatBarFill.color = currentColor;
                }
                
                yield return null;
            }
            
            currentColor = targetColor;
            if (heatBarFill != null)
            {
                heatBarFill.color = currentColor;
            }
        }
        
        /// <summary>
        /// Updates particle effects based on heat level
        /// </summary>
        /// <param name="heat">Current heat value</param>
        private void UpdateParticleEffects(float heat)
        {
            // Heat particles (Warm+)
            if (heatParticles != null)
            {
                if (heat >= warmThreshold)
                {
                    if (!heatParticles.isPlaying)
                        heatParticles.Play();
                }
                else
                {
                    if (heatParticles.isPlaying)
                        heatParticles.Stop();
                }
            }
            
            // Flame particles (Hot+)
            if (flameParticles != null)
            {
                if (heat >= hotThreshold)
                {
                    if (!flameParticles.isPlaying)
                        flameParticles.Play();
                }
                else
                {
                    if (flameParticles.isPlaying)
                        flameParticles.Stop();
                }
            }
            
            // Inferno particles (Inferno only)
            if (infernoParticles != null)
            {
                if (heat >= maxHeat)
                {
                    if (!infernoParticles.isPlaying)
                        infernoParticles.Play();
                }
                else
                {
                    if (infernoParticles.isPlaying)
                        infernoParticles.Stop();
                }
            }
        }
        
        /// <summary>
        /// Updates pulsing effect based on heat level
        /// </summary>
        /// <param name="heat">Current heat value</param>
        private void UpdatePulsingEffect(float heat)
        {
            bool shouldPulse = heat >= hotThreshold;
            
            if (shouldPulse && !isPulsing)
            {
                StartPulsing();
            }
            else if (!shouldPulse && isPulsing)
            {
                StopPulsing();
            }
        }
        
        /// <summary>
        /// Starts the pulsing effect
        /// </summary>
        private void StartPulsing()
        {
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
            }
            
            isPulsing = true;
            pulseCoroutine = StartCoroutine(PulseCoroutine());
        }
        
        /// <summary>
        /// Stops the pulsing effect
        /// </summary>
        private void StopPulsing()
        {
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
                pulseCoroutine = null;
            }
            
            isPulsing = false;
            
            // Reset scale to normal
            if (heatBar != null)
            {
                heatBar.transform.localScale = Vector3.one;
            }
        }
        
        /// <summary>
        /// Pulsing effect coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator PulseCoroutine()
        {
            while (isPulsing)
            {
                float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
                
                if (heatBar != null)
                {
                    heatBar.transform.localScale = Vector3.one * pulse;
                }
                
                yield return null;
            }
        }
        
        /// <summary>
        /// Triggers special inferno effect
        /// </summary>
        public void TriggerInfernoEffect()
        {
            StartCoroutine(InfernoFlashEffect());
        }
        
        /// <summary>
        /// Inferno flash effect coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator InfernoFlashEffect()
        {
            if (heatBarFill == null) yield break;
            
            Color originalColor = heatBarFill.color;
            Color flashColor = new Color(1f, 1f, 1f, 1f);
            
            // Flash white
            heatBarFill.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            
            // Return to original
            heatBarFill.color = originalColor;
        }
        
        /// <summary>
        /// Sets the heat value immediately without animation
        /// </summary>
        /// <param name="heat">Heat value to set</param>
        public void SetHeatImmediate(float heat)
        {
            currentHeat = heat;
            UpdateHeatMeter(heat);
            UpdateHeatLabel(heat);
            UpdateVisualEffects(heat);
        }
        
        #region Editor Debugging
        #if UNITY_EDITOR
        [Header("Editor Debug Controls")]
        [SerializeField] private bool showDebugControls = false;
        [SerializeField] private float debugHeat = 5f;
        
        private void OnGUI()
        {
            if (!showDebugControls) return;
            
            GUILayout.BeginArea(new Rect(10, 170, 200, 200));
            GUILayout.Label("Heat Meter UI Debug");
            GUILayout.Label($"Current Heat: {currentHeat:F1}");
            GUILayout.Label($"Is Pulsing: {isPulsing}");
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Test Cold (0)"))
                SetHeatImmediate(0f);
            if (GUILayout.Button("Test Warm (3)"))
                SetHeatImmediate(3f);
            if (GUILayout.Button("Test Hot (6)"))
                SetHeatImmediate(6f);
            if (GUILayout.Button("Test Blazing (9)"))
                SetHeatImmediate(9f);
            if (GUILayout.Button("Test Inferno (10)"))
                SetHeatImmediate(10f);
            if (GUILayout.Button("Trigger Inferno Effect"))
                TriggerInfernoEffect();
            
            GUILayout.EndArea();
        }
        #endif
        #endregion
    }
}
