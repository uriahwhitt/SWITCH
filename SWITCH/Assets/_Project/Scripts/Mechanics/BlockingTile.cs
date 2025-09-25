using UnityEngine;

namespace Switch.Mechanics
{
    /// <summary>
    /// Blocking tile - obstacle that must reach edge to be cleared
    /// Can be swapped but only if it creates a match
    /// </summary>
    public class BlockingTile : Tile
    {
        protected override void Awake()
        {
            base.Awake();
            
            // Set properties for blocking tile
            tileType = TileType.Blocking;
            isMatchable = false;
            isMoveable = true;
            isSwappable = true;
            isClearable = true;
            generatesScore = false;
            blocksGravity = false;
            requiresEdgeToClear = true;
        }
        
        /// <summary>
        /// Blocking tiles cannot match with other tiles
        /// </summary>
        public override bool CanMatch(Tile other)
        {
            return false; // Blocking tiles never match
        }
        
        /// <summary>
        /// Blocking tiles can swap but only if the OTHER tile will create a match
        /// </summary>
        public override bool CanSwapWith(Tile other)
        {
            // Can only swap if the other tile is regular and would create a match
            if (other.Type == TileType.Regular)
            {
                // TODO: Implement BoardValidator.WouldCreateMatch logic
                // For now, allow all swaps with regular tiles
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Blocking tiles don't have match behavior
        /// </summary>
        public override void OnMatched()
        {
            // Blocking tiles don't match, so this shouldn't be called
            Debug.LogWarning("OnMatched called on BlockingTile - this shouldn't happen!");
        }
        
        /// <summary>
        /// Blocking tiles are affected by gravity normally
        /// </summary>
        public override void OnGravityApplied(Direction direction)
        {
            // Blocking tiles fall normally
        }
        
        /// <summary>
        /// Called when blocking tile reaches edge - remove it
        /// </summary>
        public override void OnReachedEdge()
        {
            base.OnReachedEdge();
            
            // Remove blocking tile when it reaches edge
            if (TilePool.Instance != null)
            {
                TilePool.Instance.ReturnTile(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Update visuals for blocking tile
        /// </summary>
        protected override void UpdateVisuals()
        {
            base.UpdateVisuals();
            
            // Blocking tiles use gray color from sprite cache (index 7)
            // Don't override the color here since it's set in the cached sprite
        }
        
        /// <summary>
        /// Update sprite for blocking tile (override to use cached sprites)
        /// </summary>
        protected override void UpdateSpriteForType()
        {
            if (spriteRenderer != null && spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = GetDefaultTileSprite();
            }
        }
        
        /// <summary>
        /// Get default sprite for blocking tile (uses centralized cache)
        /// </summary>
        protected override Sprite GetDefaultTileSprite()
        {
            return TilePool.GetCachedColorSprite(7); // Use index 7 for gray blocking tiles
        }
        
        #region Debug
        
        [ContextMenu("Test Edge Reached")]
        private void DebugTestEdgeReached()
        {
            OnReachedEdge();
        }
        
        [ContextMenu("Test Swap Check")]
        private void DebugTestSwapCheck()
        {
            // Create a test regular tile
            GameObject testObject = new GameObject("TestRegularTile");
            RegularTile testTile = testObject.AddComponent<RegularTile>();
            
            bool canSwap = CanSwapWith(testTile);
            Debug.Log($"Can swap with regular tile: {canSwap}");
            
            Destroy(testObject);
        }
        
        #endregion
    }
}
