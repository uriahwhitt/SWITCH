using UnityEngine;

namespace Switch.Mechanics
{
    /// <summary>
    /// Power Orb - collectible that must reach specific edge
    /// Cannot be matched, cannot be swapped, generates high score when collected
    /// </summary>
    public class PowerOrb : Tile
    {
        [Header("Power Orb Properties")]
        [SerializeField] private Direction targetEdge = Direction.Down;
        [SerializeField] private float ageInTurns = 0f;
        [SerializeField] private int baseScore = 5000;
        [SerializeField] private int ageBonus = 500;
        
        protected override void Awake()
        {
            base.Awake();
            
            // Set properties for power orb
            tileType = TileType.PowerOrb;
            isMatchable = false;
            isMoveable = true;
            isSwappable = false;
            isClearable = true;
            generatesScore = true;
            blocksGravity = false;
            requiresEdgeToClear = true;
        }
        
        /// <summary>
        /// Power orbs cannot match with other tiles
        /// </summary>
        public override bool CanMatch(Tile other)
        {
            return false; // Power orbs never match
        }
        
        /// <summary>
        /// Power orbs cannot be swapped
        /// </summary>
        public override bool CanSwapWith(Tile other)
        {
            return false; // Power orbs cannot be swapped
        }
        
        /// <summary>
        /// Power orbs don't have match behavior
        /// </summary>
        public override void OnMatched()
        {
            // Power orbs don't match, so this shouldn't be called
            Debug.LogWarning("OnMatched called on PowerOrb - this shouldn't happen!");
        }
        
        /// <summary>
        /// Power orbs are affected by gravity normally
        /// </summary>
        public override void OnGravityApplied(Direction direction)
        {
            // Power orbs fall normally
        }
        
        /// <summary>
        /// Called when power orb reaches edge - award points if correct edge
        /// </summary>
        public override void OnReachedEdge()
        {
            base.OnReachedEdge();
            
            // Check if reached the target edge
            Direction reachedEdge = GetEdgeForPosition(gridPosition);
            
            if (reachedEdge == targetEdge)
            {
                // Success! Award points
                int points = baseScore + (int)(ageInTurns * ageBonus);
                
                // TODO: Integrate with GameManager for scoring
                Debug.Log($"Power Orb collected! Score: {points} (Age: {ageInTurns} turns)");
                
                // TODO: Set heat to maximum
                // GameManager.Instance.SetHeat(10f);
            }
            else
            {
                Debug.Log($"Power Orb reached wrong edge: {reachedEdge} (target: {targetEdge})");
            }
            
            // Remove orb either way
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
        /// Initialize power orb with specific target edge
        /// </summary>
        public void InitializeWithTarget(TileData data, Vector2Int position, Direction target)
        {
            Initialize(data, position);
            targetEdge = target;
            ageInTurns = 0f;
        }
        
        /// <summary>
        /// Age the power orb (called each turn)
        /// </summary>
        public void AgeOrb()
        {
            ageInTurns += 1f;
        }
        
        /// <summary>
        /// Gets the edge that a position is closest to
        /// </summary>
        private Direction GetEdgeForPosition(Vector2Int position)
        {
            // Simple edge detection - in a real implementation,
            // this would check against the actual grid bounds
            if (position.y <= 0) return Direction.Down;
            if (position.y >= 7) return Direction.Up;
            if (position.x <= 0) return Direction.Left;
            if (position.x >= 7) return Direction.Right;
            
            return Direction.None;
        }
        
        /// <summary>
        /// Update visuals for power orb
        /// </summary>
        protected override void UpdateVisuals()
        {
            base.UpdateVisuals();
            
            // Power orbs use golden color from sprite cache (index 6)
            // Don't override the color here since it's set in the cached sprite
        }
        
        /// <summary>
        /// Update sprite for power orb (override to use cached sprites)
        /// </summary>
        protected override void UpdateSpriteForType()
        {
            if (spriteRenderer != null && spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = GetDefaultTileSprite();
            }
        }
        
        /// <summary>
        /// Get default sprite for power orb (uses centralized cache)
        /// </summary>
        protected override Sprite GetDefaultTileSprite()
        {
            return TilePool.GetCachedColorSprite(6); // Use index 6 for golden orbs
        }
        
        #region Debug
        
        [ContextMenu("Test Edge Reached")]
        private void DebugTestEdgeReached()
        {
            OnReachedEdge();
        }
        
        [ContextMenu("Age Orb")]
        private void DebugAgeOrb()
        {
            AgeOrb();
            Debug.Log($"Power Orb aged. Current age: {ageInTurns} turns");
        }
        
        [ContextMenu("Set Target Edge")]
        private void DebugSetTargetEdge()
        {
            targetEdge = Direction.Down;
            Debug.Log($"Power Orb target edge set to: {targetEdge}");
        }
        
        #endregion
    }
}
