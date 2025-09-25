using UnityEngine;
using System;

namespace Switch.Mechanics
{
    /// <summary>
    /// Base Tile Class - ALL tiles inherit from this
    /// Provides unified interface for all tile types in the game
    /// </summary>
    public abstract class Tile : MonoBehaviour
    {
        [Header("Core Properties")]
        [SerializeField] protected TileType tileType;
        [SerializeField] protected int colorIndex; // 0-5 for regular colors
        [SerializeField] protected Vector2Int gridPosition;
        [SerializeField] protected bool isMatchable;
        [SerializeField] protected bool isMoveable;
        [SerializeField] protected bool isSwappable;
        [SerializeField] protected bool isClearable;
        [SerializeField] protected bool generatesScore;
        [SerializeField] protected bool blocksGravity;
        [SerializeField] protected bool requiresEdgeToClear;
        
        [Header("Visual")]
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Animator animator;
        
        [Header("Selection")]
        [SerializeField] protected bool isSelected = false;
        [SerializeField] protected bool isMatched = false;
        [SerializeField] protected float selectionScale = 1.1f;
        [SerializeField] protected float animationDuration = 0.2f;
        
        // Core Properties
        public TileType Type => tileType;
        public int ColorIndex => colorIndex;
        public Vector2Int GridPosition => gridPosition;
        
        /// <summary>
        /// Sets the grid position (used when repositioning tiles)
        /// </summary>
        public void SetGridPosition(Vector2Int position)
        {
            gridPosition = position;
        }
        public bool IsMatchable => isMatchable;
        public bool IsMoveable => isMoveable;
        public bool IsSwappable => isSwappable;
        public bool IsClearable => isClearable;
        public bool GeneratesScore => generatesScore;
        public bool BlocksGravity => blocksGravity;
        public bool RequiresEdgeToClear => requiresEdgeToClear;
        
        public bool IsSelected 
        { 
            get => isSelected; 
            set 
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    UpdateVisualState();
                }
            }
        }
        public bool IsMatched 
        { 
            get => isMatched; 
            set 
            {
                if (isMatched != value)
                {
                    isMatched = value;
                    UpdateVisualState();
                }
            }
        }
        
        // Events
        public static event Action<Tile> OnTileSelected;
        public static event Action<Tile> OnTileDeselected;
        public static event Action<Tile> OnTileMatched;
        public static event Action<Tile> OnTileCleared;
        
        protected Vector3 originalScale;
        protected bool isAnimating = false;
        protected float currentTileSize = 1f;
        
        protected virtual void Awake()
        {
            // Get components if not assigned
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (animator == null)
                animator = GetComponent<Animator>();
            
            originalScale = transform.localScale;
        }
        
        protected virtual void Start()
        {
            // UpdateVisuals() is already called in Initialize(), no need to call it again here
        }
        
        /// <summary>
        /// Initializes the tile with data and position
        /// </summary>
        public virtual void Initialize(TileData data, Vector2Int position)
        {
            tileType = data.tileType;
            colorIndex = data.colorIndex;
            gridPosition = position;
            isMatchable = data.isMatchable;
            isMoveable = data.isMoveable;
            isSwappable = data.isSwappable;
            isClearable = data.isClearable;
            generatesScore = data.generatesScore;
            blocksGravity = data.blocksGravity;
            requiresEdgeToClear = data.requiresEdgeToClear;
            
            UpdateVisuals();
        }
        
        /// <summary>
        /// Gets the tile data for serialization
        /// </summary>
        public virtual TileData GetData()
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
                requiresEdgeToClear = this.requiresEdgeToClear
            };
        }
        
        /// <summary>
        /// Virtual method for match behavior customization
        /// </summary>
        public virtual bool CanMatch(Tile other)
        {
            return isMatchable && other.isMatchable && 
                   colorIndex == other.colorIndex;
        }
        
        /// <summary>
        /// Virtual method for swap behavior customization
        /// </summary>
        public virtual bool CanSwapWith(Tile other)
        {
            return isSwappable && other.isSwappable;
        }
        
        /// <summary>
        /// Virtual method called when tile is matched
        /// </summary>
        public virtual void OnMatched()
        {
            IsMatched = true;
            OnTileMatched?.Invoke(this);
        }
        
        /// <summary>
        /// Virtual method called when gravity is applied
        /// </summary>
        public virtual void OnGravityApplied(Direction direction)
        {
            // Override for special gravity behaviors
        }
        
        /// <summary>
        /// Virtual method called when tile reaches edge
        /// </summary>
        public virtual void OnReachedEdge()
        {
            if (requiresEdgeToClear)
            {
                OnTileCleared?.Invoke(this);
            }
        }
        
        /// <summary>
        /// Handles tile selection/deselection
        /// </summary>
        public void ToggleSelection()
        {
            IsSelected = !IsSelected;
        }
        
        /// <summary>
        /// Sets the tile size and updates visual components
        /// </summary>
        public virtual void SetTileSize(float size)
        {
            currentTileSize = size;
            transform.localScale = Vector3.one * size;
        }
        
        /// <summary>
        /// Updates the tile size for sprite and collider
        /// </summary>
        protected virtual void UpdateTileSize()
        {
            // Remove the spriteRenderer.size line completely
            // Just update the collider
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.size = Vector2.one;  // Keep at 1,1 since we're scaling the transform
            }
        }
        
        /// <summary>
        /// Resets the tile to default state
        /// </summary>
        public virtual void Reset()
        {
            IsSelected = false;
            IsMatched = false;
            transform.localScale = originalScale;
            isAnimating = false;
        }
        
        /// <summary>
        /// Updates the visual state based on current properties
        /// </summary>
        protected virtual void UpdateVisualState()
        {
            if (spriteRenderer == null) return;
            
            // Handle selection visual feedback
            if (isSelected && !isAnimating)
            {
                StartCoroutine(AnimateSelection());
            }
            else if (!isSelected)
            {
                transform.localScale = originalScale;
            }
            
            // Handle matched state
            if (isMatched)
            {
                spriteRenderer.color = Color.gray;
            }
        }
        
        /// <summary>
        /// Virtual method for updating visuals - override in derived classes
        /// </summary>
        protected virtual void UpdateVisuals()
        {
            if (spriteRenderer == null) return;
            
            // Debug.Log($"UpdateVisuals called on {GetType().Name} at {gridPosition}");
            
            // Update sprite based on tile type first
            UpdateSpriteForType();
            
            // Only set color for regular tiles (other types use colored sprites)
            if (tileType == TileType.Regular)
            {
                spriteRenderer.color = GetColorFromIndex(colorIndex);
            }
            else
            {
                // For special tiles, use white color so the sprite's color shows through
                spriteRenderer.color = Color.white;
            }
        }
        
        /// <summary>
        /// Updates sprite based on tile type - override in derived classes
        /// </summary>
        protected virtual void UpdateSpriteForType()
        {
            // Default implementation - override in derived classes
            if (spriteRenderer.sprite == null)
            {
                Debug.Log($"UpdateSpriteForType called on {GetType().Name} at {gridPosition} - setting sprite");
                spriteRenderer.sprite = GetDefaultTileSprite();
            }
        }
        
        /// <summary>
        /// Gets default tile sprite (uses centralized cache)
        /// </summary>
        protected virtual Sprite GetDefaultTileSprite()
        {
            if (TilePool.Instance != null)
                return TilePool.GetCachedColorSprite(colorIndex);
            return null; // Fallback
        }
        
        /// <summary>
        /// Animates the selection effect
        /// </summary>
        protected virtual System.Collections.IEnumerator AnimateSelection()
        {
            isAnimating = true;
            
            // Scale up
            float elapsed = 0f;
            Vector3 targetScale = originalScale * selectionScale;
            
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }
            
            transform.localScale = targetScale;
            isAnimating = false;
        }
        
        /// <summary>
        /// Converts color index to Unity Color
        /// </summary>
        protected virtual Color GetColorFromIndex(int index)
        {
            switch (index)
            {
                case 0: return Color.red;
                case 1: return Color.blue;
                case 2: return Color.green;
                case 3: return Color.yellow;
                case 4: return Color.magenta;
                case 5: return new Color(1f, 0.5f, 0f); // Orange
                default: return Color.white;
            }
        }
        
        /// <summary>
        /// Handles mouse/touch input
        /// </summary>
        private void OnMouseDown()
        {
            if (!isMatched)
            {
                ToggleSelection();
                
                if (isSelected)
                {
                    OnTileSelected?.Invoke(this);
                }
                else
                {
                    OnTileDeselected?.Invoke(this);
                }
            }
        }
        
        #region Debug
        
        [ContextMenu("Select Tile")]
        private void DebugSelect()
        {
            IsSelected = true;
        }
        
        [ContextMenu("Deselect Tile")]
        private void DebugDeselect()
        {
            IsSelected = false;
        }
        
        [ContextMenu("Mark as Matched")]
        private void DebugMatch()
        {
            OnMatched();
        }
        
        #endregion
    }
}
