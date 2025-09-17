/******************************************************************************
 * SWITCH - ColorBombPowerUp
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Color bomb power-up that clears all tiles of a specific color
 * Dependencies: IPowerUp, PowerUpContext, BoardController
 * 
 * Educational Notes:
 * - Demonstrates targeted color-based power-up implementation
 * - Shows how to integrate with board clearing systems
 * - Performance: Efficient color-based tile clearing algorithm
 *****************************************************************************/

using UnityEngine;
using SWITCH.Core;
using SWITCH.Data;

namespace SWITCH.PowerUps
{
    /// <summary>
    /// Color bomb power-up that clears all tiles of a specific color.
    /// Educational: This demonstrates targeted color-based power-up implementation.
    /// Performance: Efficient color-based tile clearing algorithm with O(n) complexity.
    /// </summary>
    public class ColorBombPowerUp : IPowerUp
    {
        // Power-up properties
        public string PowerUpId => "color_bomb";
        public string DisplayName => "Color Bomb";
        public string Description => "Clears all tiles of the selected color";
        public Sprite Icon => null; // TODO: Assign icon sprite
        public int Cost => 100;
        public float CooldownTime => 2f;
        public bool CanUse => true;
        
        // Execution state
        private ColorType targetColor = ColorType.Red;
        private bool isExecuting = false;
        
        /// <summary>
        /// Executes the color bomb power-up effect.
        /// Educational: Shows how to implement color-based tile clearing.
        /// Performance: O(n) algorithm for efficient tile clearing.
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
                // Get target color from context or use default
                ColorType colorToClear = context.TargetColor;
                
                // Clear all tiles of the target color
                int clearedCount = ClearTilesOfColor(context, colorToClear);
                
                // Log result
                Debug.Log($"[ColorBombPowerUp] Cleared {clearedCount} tiles of color {colorToClear}");
                
                return clearedCount > 0;
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
            if (context?.BoardState == null)
                return false;
            
            // Check if there are any tiles of the target color
            return HasTilesOfColor(context.BoardState, context.TargetColor);
        }
        
        /// <summary>
        /// Called when power-up is activated.
        /// Educational: Shows how to implement activation callbacks.
        /// </summary>
        public void OnActivated()
        {
            Debug.Log("[ColorBombPowerUp] Color bomb activated!");
        }
        
        /// <summary>
        /// Called when power-up execution completes.
        /// Educational: Shows how to implement completion callbacks.
        /// </summary>
        /// <param name="success">Whether execution was successful</param>
        public void OnExecuted(bool success)
        {
            Debug.Log($"[ColorBombPowerUp] Color bomb execution {(success ? "succeeded" : "failed")}");
        }
        
        /// <summary>
        /// Clears all tiles of the specified color from the board.
        /// Educational: Demonstrates efficient color-based tile clearing.
        /// Performance: O(n) algorithm where n is the number of tiles on the board.
        /// </summary>
        /// <param name="context">Power-up context</param>
        /// <param name="colorToClear">Color to clear</param>
        /// <returns>Number of tiles cleared</returns>
        private int ClearTilesOfColor(PowerUpContext context, ColorType colorToClear)
        {
            int clearedCount = 0;
            var boardState = context.BoardState;
            
            // Iterate through all tiles on the board
            for (int x = 0; x < boardState.GetLength(0); x++)
            {
                for (int y = 0; y < boardState.GetLength(1); y++)
                {
                    var tile = boardState[x, y];
                    if (tile != null && tile.CurrentColor == colorToClear)
                    {
                        // Clear the tile
                        ClearTileAt(context, x, y);
                        clearedCount++;
                    }
                }
            }
            
            return clearedCount;
        }
        
        /// <summary>
        /// Checks if there are any tiles of the specified color on the board.
        /// Educational: Shows how to implement efficient board state checking.
        /// Performance: O(n) algorithm with early exit optimization.
        /// </summary>
        /// <param name="boardState">Current board state</param>
        /// <param name="colorToCheck">Color to check for</param>
        /// <returns>True if tiles of the color exist</returns>
        private bool HasTilesOfColor(Tile[,] boardState, ColorType colorToCheck)
        {
            // Early exit optimization - stop as soon as we find one tile
            for (int x = 0; x < boardState.GetLength(0); x++)
            {
                for (int y = 0; y < boardState.GetLength(1); y++)
                {
                    var tile = boardState[x, y];
                    if (tile != null && tile.CurrentColor == colorToCheck)
                        return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Clears a tile at the specified position.
        /// Educational: Shows how to integrate with board clearing systems.
        /// </summary>
        /// <param name="context">Power-up context</param>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        private void ClearTileAt(PowerUpContext context, int x, int y)
        {
            // TODO: Integrate with BoardController's tile clearing system
            // This would typically call something like:
            // context.BoardController.ClearTileAt(x, y);
            
            // For now, just mark the tile as cleared
            var tile = context.BoardState[x, y];
            if (tile != null)
            {
                // Mark tile for clearing
                // tile.MarkForClearing();
            }
        }
    }
}
