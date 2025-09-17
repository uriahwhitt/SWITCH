/******************************************************************************
 * SWITCH - AreaClearPowerUp
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Area clear power-up that clears a 3x3 area around a target position
 * Dependencies: IPowerUp, PowerUpContext, BoardController
 * 
 * Educational Notes:
 * - Demonstrates area-based power-up implementation
 * - Shows how to handle boundary checking for area effects
 * - Performance: Efficient area clearing algorithm
 *****************************************************************************/

using UnityEngine;
using SWITCH.Core;
using SWITCH.Data;

namespace SWITCH.PowerUps
{
    /// <summary>
    /// Area clear power-up that clears a 3x3 area around a target position.
    /// Educational: This demonstrates area-based power-up implementation.
    /// Performance: Efficient area clearing algorithm with O(area_size) complexity.
    /// </summary>
    public class AreaClearPowerUp : IPowerUp
    {
        // Power-up properties
        public string PowerUpId => "area_clear";
        public string DisplayName => "Area Clear";
        public string Description => "Clears a 3x3 area around the target position";
        public Sprite Icon => null; // TODO: Assign icon sprite
        public int Cost => 200;
        public float CooldownTime => 4f;
        public bool CanUse => true;
        
        // Configuration
        private const int AREA_SIZE = 3;
        private const int AREA_RADIUS = 1; // Half of area size
        
        // Execution state
        private bool isExecuting = false;
        
        /// <summary>
        /// Executes the area clear power-up effect.
        /// Educational: Shows how to implement area-based tile clearing.
        /// Performance: O(area_size²) algorithm for efficient area clearing.
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
                // Get target position from context
                Vector2Int targetPos = context.TargetPosition;
                
                // Clear the area around the target position
                int clearedCount = ClearAreaAt(context, targetPos);
                
                // Log result
                Debug.Log($"[AreaClearPowerUp] Cleared {clearedCount} tiles in 3x3 area at {targetPos}");
                
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
            
            // Check if target position is valid
            Vector2Int targetPos = context.TargetPosition;
            if (!IsValidPosition(context.BoardState, targetPos))
                return false;
            
            // Check if there are tiles to clear in the area
            return HasTilesInArea(context.BoardState, targetPos);
        }
        
        /// <summary>
        /// Called when power-up is activated.
        /// Educational: Shows how to implement activation callbacks.
        /// </summary>
        public void OnActivated()
        {
            Debug.Log("[AreaClearPowerUp] Area clear activated!");
        }
        
        /// <summary>
        /// Called when power-up execution completes.
        /// Educational: Shows how to implement completion callbacks.
        /// </summary>
        /// <param name="success">Whether execution was successful</param>
        public void OnExecuted(bool success)
        {
            Debug.Log($"[AreaClearPowerUp] Area clear execution {(success ? "succeeded" : "failed")}");
        }
        
        /// <summary>
        /// Clears a 3x3 area around the specified position.
        /// Educational: Demonstrates efficient area clearing algorithm.
        /// Performance: O(area_size²) algorithm where area_size is 3.
        /// </summary>
        /// <param name="context">Power-up context</param>
        /// <param name="centerPosition">Center position of area to clear</param>
        /// <returns>Number of tiles cleared</returns>
        private int ClearAreaAt(PowerUpContext context, Vector2Int centerPosition)
        {
            int clearedCount = 0;
            var boardState = context.BoardState;
            
            // Calculate area bounds
            int startX = Mathf.Max(0, centerPosition.x - AREA_RADIUS);
            int endX = Mathf.Min(boardState.GetLength(0) - 1, centerPosition.x + AREA_RADIUS);
            int startY = Mathf.Max(0, centerPosition.y - AREA_RADIUS);
            int endY = Mathf.Min(boardState.GetLength(1) - 1, centerPosition.y + AREA_RADIUS);
            
            // Clear tiles in the area
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if (boardState[x, y] != null)
                    {
                        ClearTileAt(context, x, y);
                        clearedCount++;
                    }
                }
            }
            
            return clearedCount;
        }
        
        /// <summary>
        /// Checks if there are tiles in the 3x3 area around the specified position.
        /// Educational: Shows how to implement efficient area checking.
        /// Performance: O(area_size²) algorithm with early exit optimization.
        /// </summary>
        /// <param name="boardState">Current board state</param>
        /// <param name="centerPosition">Center position of area to check</param>
        /// <returns>True if there are tiles in the area</returns>
        private bool HasTilesInArea(Tile[,] boardState, Vector2Int centerPosition)
        {
            // Calculate area bounds
            int startX = Mathf.Max(0, centerPosition.x - AREA_RADIUS);
            int endX = Mathf.Min(boardState.GetLength(0) - 1, centerPosition.x + AREA_RADIUS);
            int startY = Mathf.Max(0, centerPosition.y - AREA_RADIUS);
            int endY = Mathf.Min(boardState.GetLength(1) - 1, centerPosition.y + AREA_RADIUS);
            
            // Check for tiles in the area (early exit optimization)
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if (boardState[x, y] != null)
                        return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Validates if a position is within board bounds.
        /// Educational: Shows how to implement bounds checking.
        /// </summary>
        /// <param name="boardState">Current board state</param>
        /// <param name="position">Position to validate</param>
        /// <returns>True if position is valid</returns>
        private bool IsValidPosition(Tile[,] boardState, Vector2Int position)
        {
            return position.x >= 0 && position.x < boardState.GetLength(0) &&
                   position.y >= 0 && position.y < boardState.GetLength(1);
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
        
        /// <summary>
        /// Gets the area size for this power-up.
        /// Educational: Shows how to expose configuration properties.
        /// </summary>
        /// <returns>Area size (3x3)</returns>
        public int GetAreaSize()
        {
            return AREA_SIZE;
        }
        
        /// <summary>
        /// Gets the area radius for this power-up.
        /// Educational: Shows how to expose configuration properties.
        /// </summary>
        /// <returns>Area radius (1)</returns>
        public int GetAreaRadius()
        {
            return AREA_RADIUS;
        }
    }
}
