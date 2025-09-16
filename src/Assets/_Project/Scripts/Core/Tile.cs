/******************************************************************************
 * SWITCH - Tile
 * Sprint: 0
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Purpose: Individual tile behavior and state management
 * Performance Target: 60 FPS requirement
 *****************************************************************************/

using UnityEngine;
using SWITCH.Data;

namespace SWITCH.Core
{
    public class Tile : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D tileCollider;
        [SerializeField] private Animator tileAnimator;
        
        [Header("State")]
        [SerializeField] private int gridX;
        [SerializeField] private int gridY;
        [SerializeField] private ColorType currentColor;
        [SerializeField] private bool isMatched = false;
        [SerializeField] private bool isMoving = false;
        
        // Properties
        public int GridX => gridX;
        public int GridY => gridY;
        public ColorType CurrentColor => currentColor;
        public bool IsMatched => isMatched;
        public bool IsMoving => isMoving;
        public Vector2Int GridPosition => new Vector2Int(gridX, gridY);
        
        // Events
        public System.Action<Tile> OnTileClicked;
        public System.Action<Tile> OnTileMatched;
        
        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (tileCollider == null)
                tileCollider = GetComponent<Collider2D>();
            
            if (tileAnimator == null)
                tileAnimator = GetComponent<Animator>();
        }
        
        public void Initialize(int x, int y)
        {
            gridX = x;
            gridY = y;
            isMatched = false;
            isMoving = false;
            
            // Set random color for now
            SetRandomColor();
        }
        
        public void SetColor(ColorType color)
        {
            currentColor = color;
            UpdateVisuals();
        }
        
        public void SetRandomColor()
        {
            ColorType[] colors = System.Enum.GetValues(typeof(ColorType)) as ColorType[];
            ColorType randomColor = colors[Random.Range(0, colors.Length)];
            SetColor(randomColor);
        }
        
        private void UpdateVisuals()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = GetColorForType(currentColor);
            }
        }
        
        private Color GetColorForType(ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.Red: return Color.red;
                case ColorType.Blue: return Color.blue;
                case ColorType.Yellow: return Color.yellow;
                case ColorType.Orange: return new Color(1f, 0.5f, 0f);
                case ColorType.Green: return Color.green;
                case ColorType.Violet: return new Color(0.5f, 0f, 1f);
                default: return Color.white;
            }
        }
        
        public void SetMatched(bool matched)
        {
            isMatched = matched;
            
            if (matched)
            {
                OnTileMatched?.Invoke(this);
                PlayMatchAnimation();
            }
        }
        
        public void SetMoving(bool moving)
        {
            isMoving = moving;
        }
        
        public void Clear()
        {
            isMatched = false;
            isMoving = false;
            gameObject.SetActive(false);
        }
        
        public void Reset()
        {
            isMatched = false;
            isMoving = false;
            gameObject.SetActive(true);
            SetRandomColor();
        }
        
        private void PlayMatchAnimation()
        {
            if (tileAnimator != null)
            {
                tileAnimator.SetTrigger("Match");
            }
        }
        
        private void OnMouseDown()
        {
            OnTileClicked?.Invoke(this);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Handle tile interactions
            Tile otherTile = other.GetComponent<Tile>();
            if (otherTile != null)
            {
                // Handle tile collision logic
            }
        }
        
        public bool CanMatchWith(Tile other)
        {
            if (other == null) return false;
            return currentColor == other.currentColor;
        }
        
        public override string ToString()
        {
            return $"Tile({gridX},{gridY}) - {currentColor}";
        }
    }
}
