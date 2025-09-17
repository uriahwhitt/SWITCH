/******************************************************************************
 * SWITCH - SwapCache
 * Sprint: 1
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Swap caching system for gravity direction extraction
 * Dependencies: None
 * 
 * Educational Notes:
 * - Demonstrates data caching for performance optimization
 * - Avoids recalculating gravity direction from cached swap data
 * - Performance: <0.1ms operation, no allocations
 *****************************************************************************/

using UnityEngine;

namespace SWITCH.Core
{
    /// <summary>
    /// Swap caching system for gravity direction extraction.
    /// Educational: This demonstrates data caching for performance optimization.
    /// Performance: Avoids recalculating gravity direction from cached swap data.
    /// </summary>
    [System.Serializable]
    public class SwapCache
    {
        [Header("Cache Data")]
        [SerializeField] private Vector2Int tile1Position;
        [SerializeField] private Vector2Int tile2Position;
        [SerializeField] private Direction swapDirection;
        [SerializeField] private float timestamp;
        [SerializeField] private bool isValid;
        
        [Header("Configuration")]
        [SerializeField] private float cacheTimeout = 1f;
        
        // Properties
        public Vector2Int Tile1Position => tile1Position;
        public Vector2Int Tile2Position => tile2Position;
        public Direction SwapDirection => swapDirection;
        public bool IsValid => isValid && !IsExpired;
        public bool IsExpired => Time.time - timestamp > cacheTimeout;
        
        /// <summary>
        /// Caches swap data for later gravity direction extraction.
        /// Educational: Shows how to cache expensive calculations.
        /// Performance: <0.1ms operation, no allocations.
        /// </summary>
        public void CacheSwap(Vector2Int pos1, Vector2Int pos2)
        {
            tile1Position = pos1;
            tile2Position = pos2;
            swapDirection = CalculateDirection(pos1, pos2);
            timestamp = Time.time;
            isValid = true;
        }
        
        /// <summary>
        /// Extracts gravity direction from cached swap data.
        /// Educational: Demonstrates how cached data improves performance.
        /// Performance: O(1) operation, no calculations needed.
        /// </summary>
        public Direction GetSwapDirection()
        {
            return isValid ? swapDirection : Direction.None;
        }
        
        /// <summary>
        /// Calculates the direction between two positions.
        /// Educational: Shows how to determine directional relationships.
        /// </summary>
        private Direction CalculateDirection(Vector2Int pos1, Vector2Int pos2)
        {
            Vector2Int difference = pos2 - pos1;
            
            // Determine primary direction
            if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
            {
                return difference.x > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                return difference.y > 0 ? Direction.Up : Direction.Down;
            }
        }
        
        /// <summary>
        /// Gets the gravity direction based on the swap.
        /// Educational: Shows how swap direction maps to gravity.
        /// </summary>
        public Direction GetGravityDirection()
        {
            if (!IsValid) return Direction.None;
            
            // Gravity is opposite to swap direction
            switch (swapDirection)
            {
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
                default: return Direction.None;
            }
        }
        
        /// <summary>
        /// Clears the cache data.
        /// Educational: Shows proper cache invalidation.
        /// </summary>
        public void Clear()
        {
            isValid = false;
            tile1Position = Vector2Int.zero;
            tile2Position = Vector2Int.zero;
            swapDirection = Direction.None;
            timestamp = 0f;
        }
        
        /// <summary>
        /// Checks if the cache contains a valid swap.
        /// Educational: Shows cache validation patterns.
        /// </summary>
        public bool HasValidSwap()
        {
            return isValid && !IsExpired;
        }
        
        /// <summary>
        /// Gets the time remaining before cache expires.
        /// Educational: Shows cache lifecycle management.
        /// </summary>
        public float GetTimeRemaining()
        {
            if (!isValid) return 0f;
            return Mathf.Max(0f, cacheTimeout - (Time.time - timestamp));
        }
        
        public override string ToString()
        {
            if (!isValid) return "SwapCache: Invalid";
            return $"SwapCache: {tile1Position} -> {tile2Position} ({swapDirection}) - {GetTimeRemaining():F2}s remaining";
        }
    }
    
    /// <summary>
    /// Direction enumeration for game mechanics.
    /// Educational: Shows how to use enums for state management.
    /// </summary>
    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right
    }
}
