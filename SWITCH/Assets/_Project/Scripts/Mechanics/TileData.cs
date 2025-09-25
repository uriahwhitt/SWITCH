using UnityEngine;

namespace Switch.Mechanics
{
    /// <summary>
    /// Data structure for tile information
    /// Used for serialization and tile state management
    /// </summary>
    [System.Serializable]
    public class TileData
    {
        [Header("Core Properties")]
        public TileType tileType;
        public int colorIndex; // 0-5 for regular colors
        public Vector2Int gridPosition;
        
        [Header("Behavior Flags")]
        public bool isMatchable;
        public bool isMoveable;
        public bool isSwappable;
        public bool isClearable;
        public bool generatesScore;
        public bool blocksGravity;
        public bool requiresEdgeToClear;
        
        [Header("Special Properties")]
        public Direction targetEdge; // For PowerOrbs
        public float ageInTurns; // For aging mechanics
        public int hitPoints; // For multi-hit tiles
        
        public TileData()
        {
            tileType = TileType.Regular;
            colorIndex = 0;
            gridPosition = Vector2Int.zero;
            isMatchable = true;
            isMoveable = true;
            isSwappable = true;
            isClearable = true;
            generatesScore = true;
            blocksGravity = false;
            requiresEdgeToClear = false;
            targetEdge = Direction.None;
            ageInTurns = 0f;
            hitPoints = 1;
        }
        
        public TileData(TileType type, int color, Vector2Int position)
        {
            tileType = type;
            colorIndex = color;
            gridPosition = position;
            
            // Set default properties based on tile type
            SetDefaultPropertiesForType(type);
        }
        
        private void SetDefaultPropertiesForType(TileType type)
        {
            switch (type)
            {
                case TileType.Regular:
                    isMatchable = true;
                    isMoveable = true;
                    isSwappable = true;
                    isClearable = true;
                    generatesScore = true;
                    blocksGravity = false;
                    requiresEdgeToClear = false;
                    break;
                    
                case TileType.Blocking:
                    isMatchable = false;
                    isMoveable = true;
                    isSwappable = true;
                    isClearable = true;
                    generatesScore = false;
                    blocksGravity = false;
                    requiresEdgeToClear = true;
                    break;
                    
                case TileType.PowerOrb:
                    isMatchable = false;
                    isMoveable = true;
                    isSwappable = false;
                    isClearable = true;
                    generatesScore = true;
                    blocksGravity = false;
                    requiresEdgeToClear = true;
                    break;
                    
                case TileType.Rainbow:
                    isMatchable = true;
                    isMoveable = true;
                    isSwappable = true;
                    isClearable = true;
                    generatesScore = true;
                    blocksGravity = false;
                    requiresEdgeToClear = false;
                    break;
                    
                case TileType.Bomb:
                    isMatchable = true;
                    isMoveable = true;
                    isSwappable = true;
                    isClearable = true;
                    generatesScore = true;
                    blocksGravity = false;
                    requiresEdgeToClear = false;
                    break;
                    
                case TileType.Lightning:
                    isMatchable = true;
                    isMoveable = true;
                    isSwappable = true;
                    isClearable = true;
                    generatesScore = true;
                    blocksGravity = false;
                    requiresEdgeToClear = false;
                    break;
                    
                case TileType.Empty:
                default:
                    isMatchable = false;
                    isMoveable = false;
                    isSwappable = false;
                    isClearable = false;
                    generatesScore = false;
                    blocksGravity = false;
                    requiresEdgeToClear = false;
                    break;
            }
        }
        
        /// <summary>
        /// Creates a copy of this TileData
        /// </summary>
        public TileData Clone()
        {
            return new TileData
            {
                tileType = this.tileType,
                colorIndex = this.colorIndex,
                gridPosition = this.gridPosition,
                isMatchable = this.isMatchable,
                isMoveable = this.isMoveable,
                isSwappable = this.isSwappable,
                isClearable = this.isClearable,
                generatesScore = this.generatesScore,
                blocksGravity = this.blocksGravity,
                requiresEdgeToClear = this.requiresEdgeToClear,
                targetEdge = this.targetEdge,
                ageInTurns = this.ageInTurns,
                hitPoints = this.hitPoints
            };
        }
    }
    
    /// <summary>
    /// Enumeration of tile types
    /// </summary>
    public enum TileType
    {
        Empty,      // No tile
        Regular,    // Normal colored tile
        Blocking,   // Obstacle that must reach edge
        PowerOrb,   // Collectible power orb
        Rainbow,    // Matches any color
        Bomb,       // Clears area when matched
        Lightning,  // Clears row/column
    }
    
    /// <summary>
    /// Enumeration of directions for edge detection
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
