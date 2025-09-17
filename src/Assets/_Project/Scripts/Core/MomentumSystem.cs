using UnityEngine;
using System;

namespace SWITCH.Core
{
    /// <summary>
    /// Manages the momentum-based heat system for SWITCH scoring.
    /// Players build "heat" through complex matches and cascades, with momentum
    /// naturally decaying to create constant pressure for excellence.
    /// </summary>
    public class MomentumSystem : MonoBehaviour
    {
        [Header("Heat Configuration")]
        [SerializeField] private float maxMomentum = 10f;
        [SerializeField] private float turnEndDecay = 1.0f;
        [SerializeField] private float multiplierScale = 0.9f;
        
        [Header("Heat Generation Values")]
        [SerializeField] private float match3Heat = 0f;      // No heat - just maintains
        [SerializeField] private float match4Heat = 1.0f;    // Good heat boost
        [SerializeField] private float match5Heat = 2.0f;    // Excellent heat boost
        [SerializeField] private float cascadeHeat = 0.5f;   // Per cascade level (cumulative)
        [SerializeField] private float lShapeHeat = 1.0f;    // Pattern bonus heat
        [SerializeField] private float crossHeat = 1.5f;     // Rare pattern heat
        
        [Header("Power Orb Configuration")]
        [SerializeField] private float powerOrbHeatBoost = 10f; // Instant maximum heat
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        
        // Current momentum state
        private float momentum = 0f;
        
        // Events for UI and audio systems
        public event Action<float> OnMomentumChanged;
        public event Action<float> OnHeatLevelChanged;
        public event Action OnMaxHeatReached;
        public event Action OnHeatDecay;
        
        // Properties
        public float CurrentMomentum => momentum;
        public float MaxMomentum => maxMomentum;
        public float HeatPercentage => momentum / maxMomentum;
        public HeatLevel CurrentHeatLevel => GetHeatLevel(momentum);
        
        public enum HeatLevel
        {
            Cold,    // 0-2
            Warm,    // 3-4
            Hot,     // 5-7
            Blazing, // 8-9
            Inferno  // 10
        }
        
        private void Start()
        {
            // Initialize momentum to 0
            SetMomentum(0f);
        }
        
        /// <summary>
        /// Adds heat based on match size
        /// </summary>
        /// <param name="matchSize">Size of the match (3, 4, 5+)</param>
        /// <returns>Heat gained from this match</returns>
        public float AddMatchHeat(int matchSize)
        {
            float heatGained = 0f;
            
            switch (matchSize)
            {
                case 3:
                    heatGained = match3Heat;
                    break;
                case 4:
                    heatGained = match4Heat;
                    break;
                case 5:
                default:
                    heatGained = match5Heat;
                    break;
            }
            
            if (heatGained > 0f)
            {
                AddHeat(heatGained);
            }
            
            LogDebug($"Match-{matchSize} added {heatGained} heat. Current: {momentum:F1}");
            return heatGained;
        }
        
        /// <summary>
        /// Adds heat from cascade levels
        /// </summary>
        /// <param name="cascadeLevel">Number of cascade levels</param>
        /// <returns>Heat gained from cascades</returns>
        public float AddCascadeHeat(int cascadeLevel)
        {
            float heatGained = cascadeHeat * cascadeLevel;
            
            if (heatGained > 0f)
            {
                AddHeat(heatGained);
            }
            
            LogDebug($"Cascade level {cascadeLevel} added {heatGained} heat. Current: {momentum:F1}");
            return heatGained;
        }
        
        /// <summary>
        /// Adds heat from special pattern matches
        /// </summary>
        /// <param name="patternType">Type of special pattern</param>
        /// <returns>Heat gained from pattern</returns>
        public float AddPatternHeat(PatternType patternType)
        {
            float heatGained = 0f;
            
            switch (patternType)
            {
                case PatternType.LShape:
                case PatternType.TShape:
                    heatGained = lShapeHeat;
                    break;
                case PatternType.Cross:
                    heatGained = crossHeat;
                    break;
            }
            
            if (heatGained > 0f)
            {
                AddHeat(heatGained);
            }
            
            LogDebug($"Pattern {patternType} added {heatGained} heat. Current: {momentum:F1}");
            return heatGained;
        }
        
        /// <summary>
        /// Instantly sets momentum to maximum (Power Orb effect)
        /// </summary>
        public void TriggerPowerOrbBoost()
        {
            float previousMomentum = momentum;
            SetMomentum(powerOrbHeatBoost);
            
            LogDebug($"Power Orb boost! {previousMomentum:F1} -> {momentum:F1}");
            OnMaxHeatReached?.Invoke();
        }
        
        /// <summary>
        /// Applies turn-end decay to momentum
        /// </summary>
        public void ApplyTurnEndDecay()
        {
            float previousMomentum = momentum;
            float newMomentum = Mathf.Max(0f, momentum - turnEndDecay);
            SetMomentum(newMomentum);
            
            if (previousMomentum > newMomentum)
            {
                LogDebug($"Turn decay: {previousMomentum:F1} -> {newMomentum:F1}");
                OnHeatDecay?.Invoke();
            }
        }
        
        /// <summary>
        /// Gets the current score multiplier based on momentum
        /// </summary>
        /// <returns>Score multiplier (1.0x to 10.0x)</returns>
        public float GetScoreMultiplier()
        {
            return 1f + (momentum * multiplierScale);
        }
        
        /// <summary>
        /// Gets the heat level category for the current momentum
        /// </summary>
        /// <param name="heat">Heat value to categorize</param>
        /// <returns>Heat level category</returns>
        public HeatLevel GetHeatLevel(float heat)
        {
            if (heat >= 10f) return HeatLevel.Inferno;
            if (heat >= 8f) return HeatLevel.Blazing;
            if (heat >= 5f) return HeatLevel.Hot;
            if (heat >= 3f) return HeatLevel.Warm;
            return HeatLevel.Cold;
        }
        
        /// <summary>
        /// Gets the color associated with the current heat level
        /// </summary>
        /// <returns>Color for UI display</returns>
        public Color GetHeatColor()
        {
            switch (CurrentHeatLevel)
            {
                case HeatLevel.Cold:
                    return new Color(0.3f, 0.6f, 1f, 1f); // Blue
                case HeatLevel.Warm:
                    return new Color(1f, 1f, 0.3f, 1f); // Yellow
                case HeatLevel.Hot:
                    return new Color(1f, 0.6f, 0.2f, 1f); // Orange
                case HeatLevel.Blazing:
                    return new Color(1f, 0.2f, 0.1f, 1f); // Red
                case HeatLevel.Inferno:
                    return new Color(1f, 1f, 1f, 1f); // White
                default:
                    return Color.white;
            }
        }
        
        /// <summary>
        /// Resets momentum to zero (for new game)
        /// </summary>
        public void ResetMomentum()
        {
            SetMomentum(0f);
            LogDebug("Momentum reset to 0");
        }
        
        /// <summary>
        /// Sets momentum to a specific value (internal use)
        /// </summary>
        /// <param name="newMomentum">New momentum value</param>
        private void SetMomentum(float newMomentum)
        {
            float previousMomentum = momentum;
            momentum = Mathf.Clamp(newMomentum, 0f, maxMomentum);
            
            // Trigger events if momentum changed
            if (Mathf.Abs(momentum - previousMomentum) > 0.01f)
            {
                OnMomentumChanged?.Invoke(momentum);
                
                // Check if heat level changed
                HeatLevel previousLevel = GetHeatLevel(previousMomentum);
                HeatLevel currentLevel = GetHeatLevel(momentum);
                
                if (previousLevel != currentLevel)
                {
                    OnHeatLevelChanged?.Invoke(momentum);
                }
            }
        }
        
        /// <summary>
        /// Adds heat to current momentum
        /// </summary>
        /// <param name="heatAmount">Amount of heat to add</param>
        private void AddHeat(float heatAmount)
        {
            SetMomentum(momentum + heatAmount);
        }
        
        /// <summary>
        /// Debug logging helper
        /// </summary>
        /// <param name="message">Message to log</param>
        private void LogDebug(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[MomentumSystem] {message}");
            }
        }
        
        #region Editor Debugging
        #if UNITY_EDITOR
        [Header("Editor Debug Controls")]
        [SerializeField] private bool showDebugControls = false;
        
        private void OnGUI()
        {
            if (!showDebugControls) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"Momentum: {momentum:F1}/{maxMomentum}");
            GUILayout.Label($"Multiplier: {GetScoreMultiplier():F1}x");
            GUILayout.Label($"Heat Level: {CurrentHeatLevel}");
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Add Match-3 Heat"))
                AddMatchHeat(3);
            if (GUILayout.Button("Add Match-4 Heat"))
                AddMatchHeat(4);
            if (GUILayout.Button("Add Match-5 Heat"))
                AddMatchHeat(5);
            if (GUILayout.Button("Add Cascade Heat"))
                AddCascadeHeat(1);
            if (GUILayout.Button("Power Orb Boost"))
                TriggerPowerOrbBoost();
            if (GUILayout.Button("Apply Decay"))
                ApplyTurnEndDecay();
            if (GUILayout.Button("Reset"))
                ResetMomentum();
            
            GUILayout.EndArea();
        }
        #endif
        #endregion
    }
    
    /// <summary>
    /// Types of special patterns that can generate bonus heat
    /// </summary>
    public enum PatternType
    {
        LShape,
        TShape,
        Cross
    }
}
