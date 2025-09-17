/******************************************************************************
 * SWITCH - PowerUpContext
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Context data passed to power-ups during execution
 * Dependencies: Core game systems
 * 
 * Educational Notes:
 * - Demonstrates context pattern for passing execution data
 * - Shows how to provide power-ups with necessary game state
 * - Performance: Lightweight data structure with minimal allocations
 *****************************************************************************/

using UnityEngine;
using SWITCH.Core;
using SWITCH.Data;

namespace SWITCH.PowerUps
{
    /// <summary>
    /// Context data passed to power-ups during execution.
    /// Educational: This demonstrates the context pattern for passing execution data.
    /// Performance: Lightweight data structure with minimal allocations.
    /// </summary>
    public class PowerUpContext
    {
        /// <summary>
        /// Current game manager instance.
        /// Educational: Provides access to core game systems.
        /// </summary>
        public GameManager GameManager { get; set; }
        
        /// <summary>
        /// Board controller for tile manipulation.
        /// Educational: Shows how to provide board access to power-ups.
        /// </summary>
        public BoardController BoardController { get; set; }
        
        /// <summary>
        /// Current board state for analysis.
        /// Educational: Demonstrates how to provide game state to power-ups.
        /// </summary>
        public Tile[,] BoardState { get; set; }
        
        /// <summary>
        /// Target position for targeted power-ups.
        /// Educational: Shows how to handle position-based power-ups.
        /// </summary>
        public Vector2Int TargetPosition { get; set; }
        
        /// <summary>
        /// Target color for color-based power-ups.
        /// Educational: Demonstrates color-based power-up targeting.
        /// </summary>
        public ColorType TargetColor { get; set; }
        
        /// <summary>
        /// Current player score for cost calculations.
        /// Educational: Shows how to provide resource information.
        /// </summary>
        public int CurrentScore { get; set; }
        
        /// <summary>
        /// Current momentum level for enhanced effects.
        /// Educational: Demonstrates how to integrate with scoring system.
        /// </summary>
        public float CurrentMomentum { get; set; }
        
        /// <summary>
        /// Whether this is a free power-up (no cost).
        /// Educational: Shows how to handle different power-up sources.
        /// </summary>
        public bool IsFree { get; set; }
        
        /// <summary>
        /// Additional parameters for custom power-up behavior.
        /// Educational: Provides extensibility for complex power-ups.
        /// </summary>
        public object[] CustomParameters { get; set; }
        
        /// <summary>
        /// Creates a new power-up context with default values.
        /// Educational: Shows how to create context objects safely.
        /// </summary>
        public PowerUpContext()
        {
            TargetPosition = Vector2Int.zero;
            TargetColor = ColorType.Red;
            CurrentScore = 0;
            CurrentMomentum = 0f;
            IsFree = false;
            CustomParameters = new object[0];
        }
        
        /// <summary>
        /// Creates a power-up context with basic game references.
        /// Educational: Demonstrates factory pattern for context creation.
        /// </summary>
        /// <param name="gameManager">Current game manager</param>
        /// <param name="boardController">Current board controller</param>
        /// <param name="boardState">Current board state</param>
        public PowerUpContext(GameManager gameManager, BoardController boardController, Tile[,] boardState)
        {
            GameManager = gameManager;
            BoardController = boardController;
            BoardState = boardState;
            TargetPosition = Vector2Int.zero;
            TargetColor = ColorType.Red;
            CurrentScore = gameManager?.CurrentScore ?? 0;
            CurrentMomentum = 0f;
            IsFree = false;
            CustomParameters = new object[0];
        }
        
        /// <summary>
        /// Validates that the context has required data for power-up execution.
        /// Educational: Shows how to implement context validation.
        /// </summary>
        /// <returns>True if context is valid</returns>
        public bool IsValid()
        {
            return GameManager != null && 
                   BoardController != null && 
                   BoardState != null;
        }
        
        /// <summary>
        /// Gets a custom parameter by index with type safety.
        /// Educational: Demonstrates type-safe parameter access.
        /// </summary>
        /// <typeparam name="T">Expected parameter type</typeparam>
        /// <param name="index">Parameter index</param>
        /// <returns>Parameter value or default if not found</returns>
        public T GetCustomParameter<T>(int index)
        {
            if (CustomParameters == null || index < 0 || index >= CustomParameters.Length)
                return default(T);
                
            if (CustomParameters[index] is T)
                return (T)CustomParameters[index];
                
            return default(T);
        }
        
        /// <summary>
        /// Sets a custom parameter at the specified index.
        /// Educational: Shows how to safely set custom parameters.
        /// </summary>
        /// <param name="index">Parameter index</param>
        /// <param name="value">Parameter value</param>
        public void SetCustomParameter(int index, object value)
        {
            if (CustomParameters == null)
                CustomParameters = new object[index + 1];
            else if (index >= CustomParameters.Length)
            {
                var newArray = new object[index + 1];
                CustomParameters.CopyTo(newArray, 0);
                CustomParameters = newArray;
            }
            
            CustomParameters[index] = value;
        }
    }
}
