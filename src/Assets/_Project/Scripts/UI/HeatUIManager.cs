using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace SWITCH.UI
{
    /// <summary>
    /// Manages the visual heat meter UI and screen effects.
    /// Displays heat level, multiplier, and provides visual feedback.
    /// </summary>
    public class HeatUIManager : MonoBehaviour
    {
        [Header("Heat Meter UI")]
        [SerializeField] private Slider heatMeter;
        [SerializeField] private Image heatMeterFill;
        [SerializeField] private TextMeshProUGUI multiplierText;
        [SerializeField] private TextMeshProUGUI heatLevelText;
        
        [Header("Heat Level Colors")]
        [SerializeField] private Color coldColor = new Color(0.3f, 0.6f, 1f, 1f);     // Blue
        [SerializeField] private Color warmColor = new Color(1f, 1f, 0.3f, 1f);     // Yellow
        [SerializeField] private Color hotColor = new Color(1f, 0.6f, 0.2f, 1f);    // Orange
        [SerializeField] private Color blazingColor = new Color(1f, 0.2f, 0.1f, 1f); // Red
        [SerializeField] private Color infernoColor = new Color(1f, 1f, 1f, 1f);     // White
        
        [Header("Visual Effects")]
        [SerializeField] private ParticleSystem heatParticles;
        [SerializeField] private ParticleSystem flameParticles;
        [SerializeField] private ParticleSystem infernoParticles;
        [SerializeField] private Image screenEdgeGlow;
        
        [Header("Animation Settings")]
        [SerializeField] private float colorTransitionSpeed = 2f;
        [SerializeField] private float pulseSpeed = 3f;
        [SerializeField] private float pulseIntensity = 0.2f;
        [SerializeField] private float screenGlowIntensity = 0.3f;
        
        [Header("Heat Level Labels")]
        [SerializeField] private string coldLabel = "COLD";
        [SerializeField] private string warmLabel = "WARM";
        [SerializeField] private string hotLabel = "HOT";
        [SerializeField] private string blazingLabel = "BLAZING";
        [SerializeField] private string infernoLabel = "INFERNO!";
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        
        // References
        private MomentumSystem momentumSystem;
        
        // Animation state
        private Coroutine colorTransitionCoroutine;
        private Coroutine pulseCoroutine;
        private Coroutine screenGlowCoroutine;
        private Color currentColor;
        private bool isPulsing = false;
        
        private void Awake()
        {
            momentumSystem = FindObjectOfType<MomentumSystem>();
            if (momentumSystem == null)
            {
                Debug.LogError("[HeatUIManager] MomentumSystem not found!");
                return;
            }
            
            // Subscribe to momentum events
            momentumSystem.OnMomentumChanged += OnMomentumChanged;
            momentumSystem.OnHeatLevelChanged += OnHeatLevelChanged;
            momentumSystem.OnMaxHeatReached += OnMaxHeatReached;
        }
        
        private void Start()
        {
            InitializeUI();
            currentColor = coldColor;
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (momentumSystem != null)
            {
                momentumSystem.OnMomentumChanged -= OnMomentumChanged;
                momentumSystem.OnHeatLevelChanged -= OnHeatLevelChanged;
                momentumSystem.OnMaxHeatReached -= OnMaxHeatReached;
            }
        }
        
        /// <summary>
        /// Initializes UI components
        /// </summary>
        private void InitializeUI()
        {
            // Initialize heat meter
            if (heatMeter != null)
            {
                heatMeter.minValue = 0f;
                heatMeter.maxValue = momentumSystem.MaxMomentum;
                heatMeter.value = 0f;
            }
            
            // Initialize heat meter fill color
            if (heatMeterFill != null)
            {
                heatMeterFill.color = coldColor;
            }
            
            // Initialize text elements
            UpdateMultiplierText();
            UpdateHeatLevelText(MomentumSystem.HeatLevel.Cold);
            
            // Initialize screen edge glow
            if (screenEdgeGlow != null)
            {
                screenEdgeGlow.color = new Color(1f, 1f, 1f, 0f);
            }
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
            var newHeatLevel = momentumSystem.GetHeatLevel(newHeat);
            UpdateHeatLevelText(newHeatLevel);
            UpdateVisualEffects(newHeatLevel);
        }
        
        /// <summary>
        /// Called when maximum heat is reached
        /// </summary>
        private void OnMaxHeatReached()
        {
            TriggerInfernoEffect();
        }
        
        /// <summary>
        /// Updates the heat meter slider
        /// </summary>
        /// <param name="momentum">Current momentum value</param>
        private void UpdateHeatMeter(float momentum)
        {
            if (heatMeter != null)
            {
                heatMeter.value = momentum;
            }
        }
        
        /// <summary>
        /// Updates the multiplier text display
        /// </summary>
        private void UpdateMultiplierText()
        {
            if (multiplierText != null)
            {
                float multiplier = momentumSystem.GetScoreMultiplier();
                multiplierText.text = $"x{multiplier:F1}";
            }
        }
        
        /// <summary>
        /// Updates the heat level text display
        /// </summary>
        /// <param name="heatLevel">Current heat level</param>
        private void UpdateHeatLevelText(MomentumSystem.HeatLevel heatLevel)
        {
            if (heatLevelText != null)
            {
                string label = GetHeatLevelLabel(heatLevel);
                heatLevelText.text = label;
            }
        }
        
        /// <summary>
        /// Gets the label for a heat level
        /// </summary>
        /// <param name="heatLevel">Heat level</param>
        /// <returns>Label string</returns>
        private string GetHeatLevelLabel(MomentumSystem.HeatLevel heatLevel)
        {
            switch (heatLevel)
            {
                case MomentumSystem.HeatLevel.Cold: return coldLabel;
                case MomentumSystem.HeatLevel.Warm: return warmLabel;
                case MomentumSystem.HeatLevel.Hot: return hotLabel;
                case MomentumSystem.HeatLevel.Blazing: return blazingLabel;
                case MomentumSystem.HeatLevel.Inferno: return infernoLabel;
                default: return coldLabel;
            }
        }
        
        /// <summary>
        /// Updates visual effects based on heat level
        /// </summary>
        /// <param name="heatLevel">Current heat level</param>
        private void UpdateVisualEffects(MomentumSystem.HeatLevel heatLevel)
        {
            Color targetColor = GetHeatLevelColor(heatLevel);
            TransitionToColor(targetColor);
            
            // Update particle effects
            UpdateParticleEffects(heatLevel);
            
            // Update screen glow
            UpdateScreenGlow(heatLevel);
            
            // Update pulsing effect
            UpdatePulsingEffect(heatLevel);
        }
        
        /// <summary>
        /// Gets the color for a heat level
        /// </summary>
        /// <param name="heatLevel">Heat level</param>
        /// <returns>Color for this heat level</returns>
        private Color GetHeatLevelColor(MomentumSystem.HeatLevel heatLevel)
        {
            switch (heatLevel)
            {
                case MomentumSystem.HeatLevel.Cold: return coldColor;
                case MomentumSystem.HeatLevel.Warm: return warmColor;
                case MomentumSystem.HeatLevel.Hot: return hotColor;
                case MomentumSystem.HeatLevel.Blazing: return blazingColor;
                case MomentumSystem.HeatLevel.Inferno: return infernoColor;
                default: return coldColor;
            }
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
                
                if (heatMeterFill != null)
                {
                    heatMeterFill.color = currentColor;
                }
                
                yield return null;
            }
            
            currentColor = targetColor;
            if (heatMeterFill != null)
            {
                heatMeterFill.color = currentColor;
            }
        }
        
        /// <summary>
        /// Updates particle effects based on heat level
        /// </summary>
        /// <param name="heatLevel">Current heat level</param>
        private void UpdateParticleEffects(MomentumSystem.HeatLevel heatLevel)
        {
            // Heat particles (Warm+)
            if (heatParticles != null)
            {
                if (heatLevel >= MomentumSystem.HeatLevel.Warm)
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
                if (heatLevel >= MomentumSystem.HeatLevel.Hot)
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
                if (heatLevel == MomentumSystem.HeatLevel.Inferno)
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
        /// Updates screen edge glow effect
        /// </summary>
        /// <param name="heatLevel">Current heat level</param>
        private void UpdateScreenGlow(MomentumSystem.HeatLevel heatLevel)
        {
            if (screenEdgeGlow == null) return;
            
            float glowIntensity = 0f;
            
            switch (heatLevel)
            {
                case MomentumSystem.HeatLevel.Blazing:
                    glowIntensity = screenGlowIntensity * 0.5f;
                    break;
                case MomentumSystem.HeatLevel.Inferno:
                    glowIntensity = screenGlowIntensity;
                    break;
            }
            
            if (screenGlowCoroutine != null)
            {
                StopCoroutine(screenGlowCoroutine);
            }
            
            screenGlowCoroutine = StartCoroutine(UpdateScreenGlowCoroutine(glowIntensity));
        }
        
        /// <summary>
        /// Screen glow update coroutine
        /// </summary>
        /// <param name="targetIntensity">Target glow intensity</param>
        /// <returns>Coroutine</returns>
        private IEnumerator UpdateScreenGlowCoroutine(float targetIntensity)
        {
            Color startColor = screenEdgeGlow.color;
            Color targetColor = new Color(currentColor.r, currentColor.g, currentColor.b, targetIntensity);
            float elapsed = 0f;
            
            while (elapsed < colorTransitionSpeed)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / colorTransitionSpeed;
                screenEdgeGlow.color = Color.Lerp(startColor, targetColor, t);
                yield return null;
            }
            
            screenEdgeGlow.color = targetColor;
        }
        
        /// <summary>
        /// Updates pulsing effect based on heat level
        /// </summary>
        /// <param name="heatLevel">Current heat level</param>
        private void UpdatePulsingEffect(MomentumSystem.HeatLevel heatLevel)
        {
            bool shouldPulse = heatLevel >= MomentumSystem.HeatLevel.Hot;
            
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
            if (heatMeter != null)
            {
                heatMeter.transform.localScale = Vector3.one;
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
                
                if (heatMeter != null)
                {
                    heatMeter.transform.localScale = Vector3.one * pulse;
                }
                
                yield return null;
            }
        }
        
        /// <summary>
        /// Triggers special inferno effect
        /// </summary>
        private void TriggerInfernoEffect()
        {
            // Flash effect
            StartCoroutine(InfernoFlashEffect());
            
            // Screen shake (if available)
            // CameraShake.Instance?.Shake(0.5f, 0.3f);
        }
        
        /// <summary>
        /// Inferno flash effect coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator InfernoFlashEffect()
        {
            if (screenEdgeGlow == null) yield break;
            
            Color originalColor = screenEdgeGlow.color;
            Color flashColor = new Color(1f, 1f, 1f, 0.8f);
            
            // Flash white
            screenEdgeGlow.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            
            // Return to original
            screenEdgeGlow.color = originalColor;
        }
        
        /// <summary>
        /// Debug logging helper
        /// </summary>
        /// <param name="message">Message to log</param>
        private void LogDebug(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[HeatUIManager] {message}");
            }
        }
        
        #region Editor Debugging
        #if UNITY_EDITOR
        [Header("Editor Debug Controls")]
        [SerializeField] private bool showDebugControls = false;
        
        private void OnGUI()
        {
            if (!showDebugControls) return;
            
            GUILayout.BeginArea(new Rect(320, 220, 300, 200));
            GUILayout.Label("Heat UI Manager Debug");
            GUILayout.Label($"Current Color: {currentColor}");
            GUILayout.Label($"Is Pulsing: {isPulsing}");
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Test Cold"))
                UpdateVisualEffects(MomentumSystem.HeatLevel.Cold);
            if (GUILayout.Button("Test Warm"))
                UpdateVisualEffects(MomentumSystem.HeatLevel.Warm);
            if (GUILayout.Button("Test Hot"))
                UpdateVisualEffects(MomentumSystem.HeatLevel.Hot);
            if (GUILayout.Button("Test Blazing"))
                UpdateVisualEffects(MomentumSystem.HeatLevel.Blazing);
            if (GUILayout.Button("Test Inferno"))
                UpdateVisualEffects(MomentumSystem.HeatLevel.Inferno);
            if (GUILayout.Button("Trigger Inferno Effect"))
                TriggerInfernoEffect();
            
            GUILayout.EndArea();
        }
        #endif
        #endregion
    }
}
