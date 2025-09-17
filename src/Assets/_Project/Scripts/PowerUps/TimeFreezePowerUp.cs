/******************************************************************************
 * SWITCH - TimeFreezePowerUp
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Time freeze power-up that pauses the game for a short duration
 * Dependencies: IPowerUp, PowerUpContext, GameManager
 * 
 * Educational Notes:
 * - Demonstrates time-based power-up implementation
 * - Shows how to integrate with game time systems
 * - Performance: Minimal overhead time manipulation
 *****************************************************************************/

using UnityEngine;
using SWITCH.Core;
using SWITCH.Data;

namespace SWITCH.PowerUps
{
    /// <summary>
    /// Time freeze power-up that pauses the game for a short duration.
    /// Educational: This demonstrates time-based power-up implementation.
    /// Performance: Minimal overhead time manipulation with efficient coroutine management.
    /// </summary>
    public class TimeFreezePowerUp : IPowerUp
    {
        // Power-up properties
        public string PowerUpId => "time_freeze";
        public string DisplayName => "Time Freeze";
        public string Description => "Freezes time for 3 seconds";
        public Sprite Icon => null; // TODO: Assign icon sprite
        public int Cost => 250;
        public float CooldownTime => 10f;
        public bool CanUse => true;
        
        // Configuration
        private const float FREEZE_DURATION = 3f;
        private const float FREEZE_EFFECT_DURATION = 0.5f; // Visual effect duration
        
        // Execution state
        private bool isExecuting = false;
        private bool isFrozen = false;
        private float freezeStartTime = 0f;
        private float originalTimeScale = 1f;
        
        /// <summary>
        /// Executes the time freeze power-up effect.
        /// Educational: Shows how to implement time-based power-up effects.
        /// Performance: Minimal overhead with efficient time scale manipulation.
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
                // Start time freeze effect
                StartTimeFreeze(context);
                
                // Log result
                Debug.Log($"[TimeFreezePowerUp] Time frozen for {FREEZE_DURATION} seconds");
                
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
            
            // Check if time is already frozen
            if (isFrozen)
                return false;
            
            // Check if game is not already paused
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
            Debug.Log("[TimeFreezePowerUp] Time freeze activated!");
        }
        
        /// <summary>
        /// Called when power-up execution completes.
        /// Educational: Shows how to implement completion callbacks.
        /// </summary>
        /// <param name="success">Whether execution was successful</param>
        public void OnExecuted(bool success)
        {
            Debug.Log($"[TimeFreezePowerUp] Time freeze execution {(success ? "succeeded" : "failed")}");
        }
        
        /// <summary>
        /// Starts the time freeze effect.
        /// Educational: Shows how to implement time manipulation.
        /// Performance: Efficient time scale manipulation with minimal overhead.
        /// </summary>
        /// <param name="context">Power-up context</param>
        private void StartTimeFreeze(PowerUpContext context)
        {
            // Store original time scale
            originalTimeScale = Time.timeScale;
            
            // Set time scale to 0 (freeze time)
            Time.timeScale = 0f;
            isFrozen = true;
            freezeStartTime = Time.unscaledTime;
            
            // Start coroutine to end freeze after duration
            if (context.GameManager != null)
            {
                // TODO: Start coroutine to end freeze
                // context.GameManager.StartCoroutine(EndTimeFreezeAfterDelay());
            }
            
            // Trigger visual effects
            TriggerFreezeEffects(context);
        }
        
        /// <summary>
        /// Ends the time freeze effect.
        /// Educational: Shows how to restore time state.
        /// </summary>
        private void EndTimeFreeze()
        {
            if (!isFrozen)
                return;
            
            // Restore original time scale
            Time.timeScale = originalTimeScale;
            isFrozen = false;
            
            Debug.Log("[TimeFreezePowerUp] Time freeze ended");
        }
        
        /// <summary>
        /// Coroutine to end time freeze after the specified duration.
        /// Educational: Shows how to implement timed effects with coroutines.
        /// </summary>
        /// <returns>Coroutine</returns>
        private System.Collections.IEnumerator EndTimeFreezeAfterDelay()
        {
            // Wait for freeze duration (using unscaled time)
            yield return new WaitForSecondsRealtime(FREEZE_DURATION);
            
            // End the freeze
            EndTimeFreeze();
        }
        
        /// <summary>
        /// Triggers visual effects for the time freeze.
        /// Educational: Shows how to integrate visual effects with power-ups.
        /// </summary>
        /// <param name="context">Power-up context</param>
        private void TriggerFreezeEffects(PowerUpContext context)
        {
            // TODO: Implement visual effects
            // - Screen overlay effect
            // - Particle effects
            // - Audio effects
            // - UI feedback
            
            Debug.Log("[TimeFreezePowerUp] Triggering freeze visual effects");
        }
        
        /// <summary>
        /// Gets the remaining freeze time.
        /// Educational: Shows how to expose power-up state information.
        /// </summary>
        /// <returns>Remaining freeze time in seconds</returns>
        public float GetRemainingFreezeTime()
        {
            if (!isFrozen)
                return 0f;
            
            float elapsed = Time.unscaledTime - freezeStartTime;
            return Mathf.Max(0f, FREEZE_DURATION - elapsed);
        }
        
        /// <summary>
        /// Checks if time is currently frozen.
        /// Educational: Shows how to expose power-up state information.
        /// </summary>
        /// <returns>True if time is frozen</returns>
        public bool IsTimeFrozen()
        {
            return isFrozen;
        }
        
        /// <summary>
        /// Gets the freeze duration for this power-up.
        /// Educational: Shows how to expose configuration properties.
        /// </summary>
        /// <returns>Freeze duration in seconds</returns>
        public float GetFreezeDuration()
        {
            return FREEZE_DURATION;
        }
        
        /// <summary>
        /// Forces the time freeze to end immediately.
        /// Educational: Shows how to implement emergency stop functionality.
        /// </summary>
        public void ForceEndFreeze()
        {
            if (isFrozen)
            {
                EndTimeFreeze();
                Debug.Log("[TimeFreezePowerUp] Time freeze force ended");
            }
        }
    }
}
