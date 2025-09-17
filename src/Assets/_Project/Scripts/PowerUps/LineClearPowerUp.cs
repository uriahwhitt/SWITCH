/******************************************************************************
 * SWITCH - LineClearPowerUp
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Line clear power-up that clears an entire row or column
 * Dependencies: IPowerUp, PowerUpContext, BoardController
 * 
 * Educational Notes:
 * - Demonstrates line-based power-up implementation
 * - Shows how to handle directional clearing
 * - Performance: Efficient line clearing algorithm
 *****************************************************************************/

using UnityEngine;
using SWITCH.Core;
using SWITCH.Data;

namespace SWITCH.PowerUps
{
    /// <summary>
    /// Line clear power-up that clears an entire row or column.
    /// Educational: This demonstrates line-based power-up implementation.
    /// Performance: Efficient line clearing algorithm with O(n) complexity.
    /// </summary>
    public class LineClearPowerUp : IPowerUp
    {
        // Power-up properties
        public string PowerUpId => "line_clear";
        public string DisplayName => "Line Clear";
        public string Description => "Clears an entire row or column";
        public Sprite Icon => null; // TODO: Assign icon sprite
        public int Cost => 150;
        public float CooldownTime => 3f;
        public bool CanUse => true;
        
        // Execution state
        private bool isExecuting = false;
        private LineDirection lineDirection = LineDirection.Horizontal;
        
        /// <summary>
        /// Direction for line clearing.
        /// Educational: Shows how to define power-up behavior options.
        /// </summary>
        public enum LineDirection
        {
            Horizontal,
            Vertical,
            Both
        }
        
        /// <summary>
        /// Executes the line clear power-up effect.
        /// Educational: Shows how to implement line-based tile clearing.
        /// Performance: O(n) algorithm for efficient line clearing.
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
                
                // Determine line direction from context or use default
                LineDirection direction = GetLineDirectionFromContext(context);
                
                // Clear the line(s)
                int clearedCount = ClearLineAt(context, targetPos, direction);
                
                // Log result
                Debug.Log($"[LineClearPowerUp] Cleared {clearedCount} tiles in {direction} line at {targetPos}");
                
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
            
            // Check if there are tiles to clear in the line
            return HasTilesInLine(context.BoardState, targetPos, GetLineDirectionFromContext(context));
        }
        
        /// <summary>
        /// Called when power-up is activated.
        /// Educational: Shows how to implement activation callbacks.
        /// </summary>
        public void OnActivated()
        {
            Debug.Log("[LineClearPowerUp] Line clear activated!");
        }
        
        /// <summary>
        /// Called when power-up execution completes.
        /// Educational: Shows how to implement completion callbacks.
        /// </summary>
        /// <param name="success">Whether execution was successful</param>
        public void OnExecuted(bool success)
        {
            Debug.Log($"[LineClearPowerUp] Line clear execution {(success ? "succeeded" : "failed")}");
        }
        
        /// <summary>
        /// Gets the line direction from the context.
        /// Educational: Shows how to extract parameters from context.
        /// </summary>
        /// <param name="context">Power-up context</param>
        /// <returns>Line direction to use</returns>
        private LineDirection GetLineDirectionFromContext(PowerUpContext context)
        {
            // Try to get direction from custom parameters
            var direction = context.GetCustomParameter<LineDirection>(0);
            if (direction != default(LineDirection))
                return direction;
            
            // Default to horizontal
            return LineDirection.Horizontal;
        }
        
        /// <summary>
        /// Clears a line at the specified position and direction.
        /// Educational: Demonstrates efficient line clearing algorithm.
        /// Performance: O(n) algorithm where n is the board dimension.
        /// </summary>
        /// <param name="context">Power-up context</param>
        /// <param name="position">Position to clear line at</param>
        /// <param name="direction">Direction of line to clear</param>
        /// <returns>Number of tiles cleared</returns>
        private int ClearLineAt(PowerUpContext context, Vector2Int position, LineDirection direction)
        {
            int clearedCount = 0;
            var boardState = context.BoardState;
            
            switch (direction)
            {
                case LineDirection.Horizontal:
                    clearedCount = ClearHorizontalLine(context, position.y);
                    break;
                case LineDirection.Vertical:
                    clearedCount = ClearVerticalLine(context, position.x);
                    break;
                case LineDirection.Both:
                    clearedCount = ClearHorizontalLine(context, position.y) + 
                                  ClearVerticalLine(context, position.x);
                    break;
            }
            
            return clearedCount;
        }
        
        /// <summary>
        /// Clears a horizontal line at the specified Y coordinate.
        /// Educational: Shows how to implement horizontal line clearing.
        /// Performance: O(width) algorithm for horizontal clearing.
        /// </summary>
        /// <param name="context">Power-up context</param>
        /// <param name="y">Y coordinate of line to clear</param>
        /// <returns>Number of tiles cleared</returns>
        private int ClearHorizontalLine(PowerUpContext context, int y)
        {
            int clearedCount = 0;
            var boardState = context.BoardState;
            int width = boardState.GetLength(0);
            
            for (int x = 0; x < width; x++)
            {
                if (boardState[x, y] != null)
                {
                    ClearTileAt(context, x, y);
                    clearedCount++;
                }
            }
            
            return clearedCount;
        }
        
        /// <summary>
        /// Clears a vertical line at the specified X coordinate.
        /// Educational: Shows how to implement vertical line clearing.
        /// Performance: O(height) algorithm for vertical clearing.
        /// </summary>
        /// <param name="context">Power-up context</param>
        /// <param name="x">X coordinate of line to clear</param>
        /// <returns>Number of tiles cleared</returns>
        private int ClearVerticalLine(PowerUpContext context, int x)
        {
            int clearedCount = 0;
            var boardState = context.BoardState;
            int height = boardState.GetLength(1);
            
            for (int y = 0; y < height; y++)
            {
                if (boardState[x, y] != null)
                {
                    ClearTileAt(context, x, y);
                    clearedCount++;
                }
            }
            
            return clearedCount;
        }
        
        /// <summary>
        /// Checks if there are tiles in the specified line.
        /// Educational: Shows how to implement efficient line checking.
        /// Performance: O(n) algorithm with early exit optimization.
        /// </summary>
        /// <param name="boardState">Current board state</param>
        /// <param name="position">Position to check line at</param>
        /// <param name="direction">Direction of line to check</param>
        /// <returns>True if there are tiles in the line</returns>
        private bool HasTilesInLine(Tile[,] boardState, Vector2Int position, LineDirection direction)
        {
            switch (direction)
            {
                case LineDirection.Horizontal:
                    return HasTilesInHorizontalLine(boardState, position.y);
                case LineDirection.Vertical:
                    return HasTilesInVerticalLine(boardState, position.x);
                case LineDirection.Both:
                    return HasTilesInHorizontalLine(boardState, position.y) || 
                           HasTilesInVerticalLine(boardState, position.x);
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Checks if there are tiles in a horizontal line.
        /// Educational: Shows how to implement horizontal line checking.
        /// </summary>
        /// <param name="boardState">Current board state</param>
        /// <param name="y">Y coordinate of line to check</param>
        /// <returns>True if there are tiles in the line</returns>
        private bool HasTilesInHorizontalLine(Tile[,] boardState, int y)
        {
            int width = boardState.GetLength(0);
            for (int x = 0; x < width; x++)
            {
                if (boardState[x, y] != null)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Checks if there are tiles in a vertical line.
        /// Educational: Shows how to implement vertical line checking.
        /// </summary>
        /// <param name="boardState">Current board state</param>
        /// <param name="x">X coordinate of line to check</param>
        /// <returns>True if there are tiles in the line</returns>
        private bool HasTilesInVerticalLine(Tile[,] boardState, int x)
        {
            int height = boardState.GetLength(1);
            for (int y = 0; y < height; y++)
            {
                if (boardState[x, y] != null)
                    return true;
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
    }
}
