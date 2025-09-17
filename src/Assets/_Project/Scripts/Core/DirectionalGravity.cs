/******************************************************************************
 * SWITCH - DirectionalGravity
 * Sprint: 1
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Player-controlled directional gravity system
 * Dependencies: BoardController, SwapCache, Tile
 * 
 * Educational Notes:
 * - Demonstrates physics-based tile movement with gravity
 * - Shows how to implement player-controlled game mechanics
 * - Performance: Optimized for 60 FPS with object pooling
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SWITCH.Core
{
    /// <summary>
    /// Directional gravity system for player-controlled tile flow.
    /// Educational: This demonstrates physics-based movement with gravity.
    /// Performance: Optimized for 60 FPS with efficient algorithms.
    /// </summary>
    public class DirectionalGravity : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float gravityStrength = 9.8f;
        [SerializeField] private float tileFallSpeed = 5f;
        [SerializeField] private float swapAnimationDuration = 0.3f;
        [SerializeField] private AnimationCurve fallCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("References")]
        [SerializeField] private BoardController boardController;
        [SerializeField] private SwapCache swapCache;
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private bool showGravityVectors = false;
        
        // Current gravity state
        private Direction currentGravityDirection = Direction.Down;
        private bool isProcessingGravity = false;
        private List<Tile> tilesToMove = new List<Tile>();
        
        // Events
        public System.Action<Direction> OnGravityChanged;
        public System.Action OnGravityComplete;
        public System.Action<Tile, Vector2Int> OnTileMoved;
        
        // Properties
        public Direction CurrentGravityDirection => currentGravityDirection;
        public bool IsProcessingGravity => isProcessingGravity;
        
        private void Awake()
        {
            if (boardController == null)
                boardController = FindObjectOfType<BoardController>();
            
            if (swapCache == null)
                swapCache = new SwapCache();
        }
        
        /// <summary>
        /// Sets the gravity direction based on a swap operation.
        /// Educational: Shows how player actions affect game physics.
        /// </summary>
        public void SetGravityFromSwap(Vector2Int tile1Pos, Vector2Int tile2Pos)
        {
            if (isProcessingGravity)
            {
                Log("Cannot change gravity while processing");
                return;
            }
            
            // Cache the swap data
            swapCache.CacheSwap(tile1Pos, tile2Pos);
            
            // Get gravity direction from swap
            Direction newGravity = swapCache.GetGravityDirection();
            
            if (newGravity != Direction.None && newGravity != currentGravityDirection)
            {
                SetGravityDirection(newGravity);
                Log($"Gravity changed to {newGravity} from swap {tile1Pos} -> {tile2Pos}");
            }
        }
        
        /// <summary>
        /// Sets the gravity direction directly.
        /// Educational: Shows how to implement direct control mechanisms.
        /// </summary>
        public void SetGravityDirection(Direction direction)
        {
            if (direction == Direction.None || direction == currentGravityDirection)
                return;
            
            currentGravityDirection = direction;
            OnGravityChanged?.Invoke(direction);
            Log($"Gravity direction set to {direction}");
        }
        
        /// <summary>
        /// Applies gravity to all tiles on the board.
        /// Educational: Shows how to implement physics-based movement.
        /// Performance: Optimized to maintain 60 FPS.
        /// </summary>
        public IEnumerator ApplyGravity()
        {
            if (isProcessingGravity)
            {
                Log("Gravity already processing");
                yield break;
            }
            
            isProcessingGravity = true;
            Log($"Applying gravity in direction: {currentGravityDirection}");
            
            // Get all tiles that need to move
            tilesToMove.Clear();
            GetTilesToMove();
            
            if (tilesToMove.Count == 0)
            {
                Log("No tiles need to move");
                isProcessingGravity = false;
                OnGravityComplete?.Invoke();
                yield break;
            }
            
            // Move tiles with animation
            yield return StartCoroutine(MoveTilesWithAnimation());
            
            // Fill empty spaces
            yield return StartCoroutine(FillEmptySpaces());
            
            isProcessingGravity = false;
            OnGravityComplete?.Invoke();
            Log("Gravity application complete");
        }
        
        /// <summary>
        /// Gets all tiles that need to move based on current gravity.
        /// Educational: Shows how to determine which objects are affected by physics.
        /// </summary>
        private void GetTilesToMove()
        {
            Vector2Int gravityVector = GetGravityVector();
            
            for (int x = 0; x < boardController.Width; x++)
            {
                for (int y = 0; y < boardController.Height; y++)
                {
                    Tile tile = boardController.GetTileAt(x, y);
                    if (tile != null && !tile.IsMoving)
                    {
                        Vector2Int newPosition = GetNewPosition(x, y, gravityVector);
                        if (newPosition != new Vector2Int(x, y))
                        {
                            tilesToMove.Add(tile);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Moves tiles with smooth animation.
        /// Educational: Shows how to implement smooth movement animations.
        /// Performance: Uses coroutines for efficient animation.
        /// </summary>
        private IEnumerator MoveTilesWithAnimation()
        {
            List<Coroutine> moveCoroutines = new List<Coroutine>();
            
            foreach (Tile tile in tilesToMove)
            {
                Vector2Int newPosition = GetNewPosition(tile.GridX, tile.GridY, GetGravityVector());
                Coroutine moveCoroutine = StartCoroutine(MoveTileSmoothly(tile, newPosition));
                moveCoroutines.Add(moveCoroutine);
            }
            
            // Wait for all tiles to finish moving
            foreach (Coroutine coroutine in moveCoroutines)
            {
                yield return coroutine;
            }
        }
        
        /// <summary>
        /// Moves a single tile smoothly to its new position.
        /// Educational: Shows how to implement smooth movement with curves.
        /// </summary>
        private IEnumerator MoveTileSmoothly(Tile tile, Vector2Int newPosition)
        {
            tile.SetMoving(true);
            
            Vector3 startPosition = tile.transform.position;
            Vector3 endPosition = boardController.GetWorldPosition(newPosition.x, newPosition.y);
            
            float elapsedTime = 0f;
            
            while (elapsedTime < swapAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / swapAnimationDuration;
                float curveValue = fallCurve.Evaluate(progress);
                
                tile.transform.position = Vector3.Lerp(startPosition, endPosition, curveValue);
                yield return null;
            }
            
            tile.transform.position = endPosition;
            tile.SetMoving(false);
            
            // Update tile's grid position
            OnTileMoved?.Invoke(tile, newPosition);
        }
        
        /// <summary>
        /// Fills empty spaces with new tiles.
        /// Educational: Shows how to maintain game state after physics.
        /// </summary>
        private IEnumerator FillEmptySpaces()
        {
            Vector2Int gravityVector = GetGravityVector();
            
            // Fill from the opposite direction of gravity
            for (int x = 0; x < boardController.Width; x++)
            {
                for (int y = 0; y < boardController.Height; y++)
                {
                    Tile tile = boardController.GetTileAt(x, y);
                    if (tile == null)
                    {
                        // Create new tile at this position
                        Vector3 worldPos = boardController.GetWorldPosition(x, y);
                        CreateNewTileAt(x, y, worldPos);
                        yield return new WaitForSeconds(0.05f); // Small delay for visual effect
                    }
                }
            }
        }
        
        /// <summary>
        /// Creates a new tile at the specified position.
        /// Educational: Shows how to instantiate game objects.
        /// </summary>
        private void CreateNewTileAt(int x, int y, Vector3 worldPosition)
        {
            // This would typically use object pooling
            // For now, we'll create a simple tile
            GameObject tileObj = new GameObject($"Tile_{x}_{y}");
            tileObj.transform.position = worldPosition;
            tileObj.transform.SetParent(boardController.transform);
            
            Tile tile = tileObj.AddComponent<Tile>();
            tile.Initialize(x, y);
            
            // Add visual components
            SpriteRenderer renderer = tileObj.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateTileSprite();
            renderer.color = GetRandomColor();
            
            // Add collider
            BoxCollider2D collider = tileObj.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }
        
        /// <summary>
        /// Gets the new position for a tile based on gravity.
        /// Educational: Shows how to calculate physics-based positions.
        /// </summary>
        private Vector2Int GetNewPosition(int x, int y, Vector2Int gravityVector)
        {
            Vector2Int newPos = new Vector2Int(x, y);
            
            // Apply gravity until we hit a wall or another tile
            while (true)
            {
                Vector2Int nextPos = newPos + gravityVector;
                
                // Check bounds
                if (!boardController.IsValidPosition(nextPos.x, nextPos.y))
                    break;
                
                // Check if position is occupied
                Tile tileAtNextPos = boardController.GetTileAt(nextPos.x, nextPos.y);
                if (tileAtNextPos != null)
                    break;
                
                newPos = nextPos;
            }
            
            return newPos;
        }
        
        /// <summary>
        /// Gets the gravity vector for the current direction.
        /// Educational: Shows how to convert directions to vectors.
        /// </summary>
        private Vector2Int GetGravityVector()
        {
            switch (currentGravityDirection)
            {
                case Direction.Up: return Vector2Int.up;
                case Direction.Down: return Vector2Int.down;
                case Direction.Left: return Vector2Int.left;
                case Direction.Right: return Vector2Int.right;
                default: return Vector2Int.zero;
            }
        }
        
        /// <summary>
        /// Creates a simple tile sprite for testing.
        /// Educational: Shows how to create procedural sprites.
        /// </summary>
        private Sprite CreateTileSprite()
        {
            Texture2D texture = new Texture2D(64, 64);
            Color[] pixels = new Color[64 * 64];
            
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        }
        
        /// <summary>
        /// Gets a random color for tiles.
        /// Educational: Shows how to randomize visual properties.
        /// </summary>
        private Color GetRandomColor()
        {
            Color[] colors = { Color.red, Color.blue, Color.yellow, Color.green, Color.magenta, Color.cyan };
            return colors[Random.Range(0, colors.Length)];
        }
        
        private void Log(string message)
        {
            if (debugMode)
            {
                Debug.Log($"[DirectionalGravity] {message}");
            }
        }
        
        private void OnDrawGizmos()
        {
            if (showGravityVectors && boardController != null)
            {
                Gizmos.color = Color.red;
                Vector2Int gravityVector = GetGravityVector();
                Vector3 center = boardController.BoardCenter;
                
                Gizmos.DrawRay(center, new Vector3(gravityVector.x, gravityVector.y, 0) * 2f);
            }
        }
    }
}
