/******************************************************************************
 * SWITCH - ScoreMultiplierPowerUp
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Score multiplier power-up that increases score for a limited time
 * Dependencies: IPowerUp, PowerUpContext, GameManager, MomentumSystem
 * 
 * Educational Notes:
 * - Demonstrates score modification power-up implementation
 * - Shows how to integrate with scoring systems
 * - Performance: Efficient score multiplier management
 *****************************************************************************/

using UnityEngine;
using SWITCH.Core;
using SWITCH.Data;

namespace SWITCH.PowerUps
{
    /// <summary>
    /// Score multiplier power-up that increases score for a limited time.
    /// Educational: This demonstrates score modification power-up implementation.
    /// Performance: Efficient score multiplier management with minimal overhead.
    /// </summary>
    public class ScoreMultiplierPowerUp : IPowerUp
    {
        // Power-up properties
        public string PowerUpId => "score_multiplier";
        public string DisplayName => "Score Multiplier";
        public string Description => "Doubles score for 10 seconds";
        public Sprite Icon => null; // TODO: Assign icon sprite
        public int Cost => 300;
        public float CooldownTime => 15f;
        public bool CanUse => true;
        
        // Configuration
        private const float MULTIPLIER_DURATION = 10f;
        private const float MULTIPLIER_VALUE = 2f;
        private const float VISUAL_EFFECT_DURATION = 0.5f;
        
        // Execution state
        private bool isExecuting = false;
        private bool isActive = false;
        private float multiplierStartTime = 0f;
        private float originalMultiplier = 1f;
        
        /// <summary>
        /// Executes the score multiplier power-up effect.
        /// Educational: Shows how to implement score modification power-up effects.
        /// Performance: Efficient score multiplier management with minimal overhead.
        /// </summary>
        /// <param name="context">Context for power-up execution</param>
        /// <returns>True if power-up was successfully used</returns>
        public bool Execute(PowerUpContext context)
        {
            if (isExecuting)
                return false;
            
            if (!CanExecute(context))
                return false;
            
            isExecuting = true;
            
            try
            {
                // Start score multiplier effect
                StartScoreMultiplier(context);
                
                // Log result
                Debug.Log($"[ScoreMultiplierPowerUp] Score multiplier active for {MULTIPLIER_DURATION} seconds");
                
                return true;
            }
            finally
            {
                isExecuting = false;
            }
        }
        
        /// <summary>
        /// Validates if this power-up can be used in the given context.
        /// Educational: Shows how to implement pre-execution validation.
        /// </summary>
        /// <param name="context">Context to validate against</param>
        /// <returns>True if power-up can be used</returns>
        public bool CanExecute(PowerUpContext context)
        {
            if (context?.GameManager == null)
                return false;
            
            // Check if game is currently active
            if (!context.GameManager.IsGameActive)
                return false;
            
            // Check if multiplier is already active
            if (isActive)
                return false;
            
            // Check if game is not paused
            if (context.GameManager.IsPaused)
                return false;
            
            return true;
        }
        
        /// <summary>
        /// Called when power-up is activated.
        /// Educational: Shows how to implement activation callbacks.
        /// </summary>
        public void OnActivated()
        {
            Debug.Log("[ScoreMultiplierPowerUp] Score multiplier activated!");
        }
        
        /// <summary>
        /// Called when power-up execution completes.
        /// Educational: Shows how to implement completion callbacks.
        /// </summary>
        /// <param name="success">Whether execution was successful</param>
        public void OnExecuted(bool success)
        {
            Debug.Log($"[ScoreMultiplierPowerUp] Score multiplier execution {(success ? "succeeded" : "failed")}");
        }
        
        /// <summary>
        /// Starts the score multiplier effect.
        /// Educational: Shows how to implement score modification.
        /// Performance: Efficient score multiplier management with minimal overhead.
        /// </summary>
        /// <param name="context">Power-up context</param>
        private void StartScoreMultiplier(PowerUpContext context)
        {
            // Store original multiplier
            originalMultiplier = GetCurrentScoreMultiplier(context);
            
            // Set score multiplier
            SetScoreMultiplier(context, MULTIPLIER_VALUE);
            isActive = true;
            multiplierStartTime = Time.time;
            
            // Start coroutine to end multiplier after duration
            if (context.GameManager != null)
            {
                // TODO: Start coroutine to end multiplier
                // context.GameManager.StartCoroutine(EndScoreMultiplierAfterDelay());
            }
            
            // Trigger visual effects
            TriggerMultiplierEffects(context);
        }
        
        /// <summary>
        /// Ends the score multiplier effect.
        /// Educational: Shows how to restore score state.
        /// </summary>
        /// <param name="context">Power-up context</param>
        private void EndScoreMultiplier(PowerUpContext context)
        {
            if (!isActive)
                return;
            
            // Restore original multiplier
            SetScoreMultiplier(context, originalMultiplier);
            isActive = false;
            
            Debug.Log("[ScoreMultiplierPowerUp] Score multiplier ended");
        }
        
        /// <summary>
        /// Coroutine to end score multiplier after the specified duration.
        /// Educational: Shows how to implement timed effects with coroutines.
        /// </summary>
        /// <returns>Coroutine</returns>
        private System.Collections.IEnumerator EndScoreMultiplierAfterDelay()
        {
            // Wait for multiplier duration
            yield return new WaitForSeconds(MULTIPLIER_DURATION);
            
            // End the multiplier
            // EndScoreMultiplier(context);
        }
        
        /// <summary>
        /// Gets the current score multiplier from the game systems.
        /// Educational: Shows how to query game state for score information.
        /// </summary>
        /// <param name="context">Power-up context</param>
        /// <returns>Current score multiplier</returns>
        private float GetCurrentScoreMultiplier(PowerUpContext context)
        {
            // TODO: Integrate with actual scoring system
            // This would typically get the multiplier from:
            // - MomentumSystem
            // - TurnScoreCalculator
            // - GameManager
            
            return 1f; // Default multiplier
        }
        
        /// <summary>
        /// Sets the score multiplier in the game systems.
        /// Educational: Shows how to modify game state for score effects.
        /// </summary>
        /// <param name="context">Power-up context</param>
        /// <param name="multiplier">New multiplier value</param>
        private void SetScoreMultiplier(PowerUpContext context, float multiplier)
        {
            // TODO: Integrate with actual scoring system
            // This would typically set the multiplier in:
            // - MomentumSystem
            // - TurnScoreCalculator
            // - GameManager
            
            Debug.Log($"[ScoreMultiplierPowerUp] Setting score multiplier to {multiplier}");
        }
        
        /// <summary>
        /// Triggers visual effects for the score multiplier.
        /// Educational: Shows how to integrate visual effects with power-ups.
        /// </summary>
        /// <param name="context">Power-up context</param>
        private void TriggerMultiplierEffects(PowerUpContext context)
        {
            // TODO: Implement visual effects
            // - Score display highlighting
            // - Particle effects
            // - Audio effects
            // - UI feedback
            
            Debug.Log("[ScoreMultiplierPowerUp] Triggering multiplier visual effects");
        }
        
        /// <summary>
        /// Gets the remaining multiplier time.
        /// Educational: Shows how to expose power-up state information.
        /// </summary>
        /// <returns>Remaining multiplier time in seconds</returns>
        public float GetRemainingMultiplierTime()
        {
            if (!isActive)
                return 0f;
            
            float elapsed = Time.time - multiplierStartTime;
            return Mathf.Max(0f, MULTIPLIER_DURATION - elapsed);
        }
        
        /// <summary>
        /// Checks if the score multiplier is currently active.
        /// Educational: Shows how to expose power-up state information.
        /// </summary>
        /// <returns>True if multiplier is active</returns>
        public bool IsMultiplierActive()
        {
            return isActive;
        }
        
        /// <summary>
        /// Gets the multiplier value for this power-up.
        /// Educational: Shows how to expose configuration properties.
        /// </summary>
        /// <returns>Multiplier value</returns>
        public float GetMultiplierValue()
        {
            return MULTIPLIER_VALUE;
        }
        
        /// <summary>
        /// Gets the multiplier duration for this power-up.
        /// Educational: Shows how to expose configuration properties.
        /// </summary>
        /// <returns>Multiplier duration in seconds</returns>
        public float GetMultiplierDuration()
        {
            return MULTIPLIER_DURATION;
        }
        
        /// <summary>
        /// Forces the score multiplier to end immediately.
        /// Educational: Shows how to implement emergency stop functionality.
        /// </summary>
        /// <param name="context">Power-up context</param>
        public void ForceEndMultiplier(PowerUpContext context)
        {
            if (isActive)
            {
                EndScoreMultiplier(context);
                Debug.Log("[ScoreMultiplierPowerUp] Score multiplier force ended");
            }
        }
    }
}
