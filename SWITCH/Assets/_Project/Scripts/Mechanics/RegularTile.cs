using UnityEngine;

namespace Switch.Mechanics
{
    /// <summary>
    /// Regular matchable tile - the standard colored tile in the game
    /// </summary>
    public class RegularTile : Tile
    {
        protected override void Awake()
        {
            base.Awake();
            
            // Set properties for regular tile
            tileType = TileType.Regular;
            isMatchable = true;
            isMoveable = true;
            isSwappable = true;
            isClearable = true;
            generatesScore = true;
            blocksGravity = false;
            requiresEdgeToClear = false;
        }
        
        /// <summary>
        /// Regular tiles can match with other regular tiles of the same color
        /// </summary>
        public override bool CanMatch(Tile other)
        {
            return base.CanMatch(other) && other.Type == TileType.Regular;
        }
        
        /// <summary>
        /// Regular tiles can swap with any swappable tile
        /// </summary>
        public override bool CanSwapWith(Tile other)
        {
            return base.CanSwapWith(other);
        }
        
        /// <summary>
        /// Regular tile match behavior - standard scoring
        /// </summary>
        public override void OnMatched()
        {
            base.OnMatched();
            
            // Regular tiles generate standard score
            if (generatesScore)
            {
                // Score calculation would be handled by the game manager
                Debug.Log($"Regular tile matched at {gridPosition}, color: {colorIndex}");
            }
        }
        
        /// <summary>
        /// Regular tiles are affected by gravity normally
        /// </summary>
        public override void OnGravityApplied(Direction direction)
        {
            // Regular tiles fall normally - no special behavior needed
        }
        
        /// <summary>
        /// Update visuals for regular tile
        /// </summary>
        protected override void UpdateVisuals()
        {
            base.UpdateVisuals();
            
            // Regular tiles use standard colored sprites
            if (spriteRenderer != null)
            {
                spriteRenderer.color = GetColorFromIndex(colorIndex);
            }
        }
        
        /// <summary>
        /// Update sprite for regular tile (override to use cached sprites)
        /// </summary>
        protected override void UpdateSpriteForType()
        {
            if (spriteRenderer != null && spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = GetDefaultTileSprite();
            }
        }
        
        /// <summary>
        /// Get default sprite for regular tile (uses centralized cache)
        /// </summary>
        protected override Sprite GetDefaultTileSprite()
        {
            return TilePool.GetCachedColorSprite(colorIndex);
        }
        
        #region Debug
        
        [ContextMenu("Test Match")]
        private void DebugTestMatch()
        {
            OnMatched();
        }
        
        [ContextMenu("Test Gravity")]
        private void DebugTestGravity()
        {
            OnGravityApplied(Direction.Down);
        }
        
        #endregion
    }
}
