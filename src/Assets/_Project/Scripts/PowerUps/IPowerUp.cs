/******************************************************************************
 * SWITCH - IPowerUp Interface
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Base interface for all power-up implementations
 * Dependencies: None
 * 
 * Educational Notes:
 * - Demonstrates interface-based design for extensible power-up system
 * - Shows how to create consistent power-up behavior contracts
 * - Performance: Lightweight interface with minimal overhead
 *****************************************************************************/

using UnityEngine;

namespace SWITCH.PowerUps
{
    /// <summary>
    /// Base interface for all power-up implementations.
    /// Educational: This demonstrates interface-based design for extensible power-up system.
    /// Performance: Lightweight interface with minimal overhead.
    /// </summary>
    public interface IPowerUp
    {
        /// <summary>
        /// Unique identifier for this power-up type.
        /// Educational: Shows how to create consistent identification system.
        /// </summary>
        string PowerUpId { get; }
        
        /// <summary>
        /// Display name for UI purposes.
        /// Educational: Separates internal ID from user-facing display.
        /// </summary>
        string DisplayName { get; }
        
        /// <summary>
        /// Description of what this power-up does.
        /// Educational: Provides clear documentation for each power-up.
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Icon sprite for UI display.
        /// Educational: Shows how to handle visual assets in interfaces.
        /// </summary>
        Sprite Icon { get; }
        
        /// <summary>
        /// Cost in points to use this power-up.
        /// Educational: Demonstrates resource management in power-up system.
        /// </summary>
        int Cost { get; }
        
        /// <summary>
        /// Cooldown time in seconds before this power-up can be used again.
        /// Educational: Shows how to implement usage restrictions.
        /// </summary>
        float CooldownTime { get; }
        
        /// <summary>
        /// Whether this power-up can be used in the current game state.
        /// Educational: Demonstrates state-based power-up availability.
        /// </summary>
        bool CanUse { get; }
        
        /// <summary>
        /// Executes the power-up effect.
        /// Educational: Shows how to implement consistent power-up execution.
        /// Performance: Should complete within one frame for smooth gameplay.
        /// </summary>
        /// <param name="context">Context information for power-up execution</param>
        /// <returns>True if power-up was successfully used</returns>
        bool Execute(PowerUpContext context);
        
        /// <summary>
        /// Validates if this power-up can be used in the given context.
        /// Educational: Shows how to implement pre-execution validation.
        /// </summary>
        /// <param name="context">Context to validate against</param>
        /// <returns>True if power-up can be used</returns>
        bool CanExecute(PowerUpContext context);
        
        /// <summary>
        /// Called when power-up is activated (before execution).
        /// Educational: Shows how to implement activation callbacks.
        /// </summary>
        void OnActivated();
        
        /// <summary>
        /// Called when power-up execution completes.
        /// Educational: Shows how to implement completion callbacks.
        /// </summary>
        /// <param name="success">Whether execution was successful</param>
        void OnExecuted(bool success);
    }
}
