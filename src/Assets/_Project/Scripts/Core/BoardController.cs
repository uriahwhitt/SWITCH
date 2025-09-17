/******************************************************************************
 * SWITCH - BoardController
 * Sprint: 0
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Purpose: Manages 8x8 game board and tile placement
 * Performance Target: 60 FPS requirement
 *****************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace SWITCH.Core
{
    public class BoardController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private int boardWidth = 8;
        [SerializeField] private int boardHeight = 8;
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private float tileSpacing = 0.1f;
        
        [Header("References")]
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform boardParent;
        [SerializeField] private PowerOrbManager powerOrbManager;
        
        [Header("Object Pooling")]
        [SerializeField] private int poolSize = 100;
        [SerializeField] private bool expandPool = true;
        
        // Board state
        private Tile[,] board;
        private Vector3 boardCenter;
        
        // Object pooling
        private Queue<GameObject> tilePool = new Queue<GameObject>();
        private List<GameObject> activeTiles = new List<GameObject>();
        
        // Properties
        public int Width => boardWidth;
        public int Height => boardHeight;
        public Vector3 BoardCenter => boardCenter;
        
        private void Awake()
        {
            InitializeBoard();
        }
        
        private void InitializeBoard()
        {
            // Create board array
            board = new Tile[boardWidth, boardHeight];
            
            // Calculate board center
            boardCenter = transform.position;
            
            // Initialize object pool
            InitializeTilePool();
            
            // Create tile grid
            CreateTileGrid();
        }
        
        /// <summary>
        /// Initializes the object pool with pre-allocated GameObjects.
        /// Educational: This method shows how to pre-allocate objects for performance.
        /// Performance: Reduces garbage collection by reusing objects instead of creating/destroying.
        /// </summary>
        private void InitializeTilePool()
        {
            Log("Initializing tile object pool...");
            
            for (int i = 0; i < poolSize; i++)
            {
                GameObject tileObj = CreatePooledTile();
                tilePool.Enqueue(tileObj);
            }
            
            Log($"Tile pool initialized with {poolSize} objects");
        }
        
        /// <summary>
        /// Creates a pooled tile GameObject.
        /// Educational: Shows how to create objects for pooling.
        /// </summary>
        private GameObject CreatePooledTile()
        {
            GameObject tileObj;
            
            if (tilePrefab != null)
            {
                tileObj = Instantiate(tilePrefab);
            }
            else
            {
                // Create a basic tile if no prefab is assigned
                tileObj = new GameObject("PooledTile");
                tileObj.AddComponent<SpriteRenderer>();
                tileObj.AddComponent<BoxCollider2D>();
                tileObj.AddComponent<Tile>();
            }
            
            tileObj.SetActive(false);
            tileObj.transform.SetParent(boardParent);
            
            return tileObj;
        }
        
        /// <summary>
        /// Gets a tile from the object pool.
        /// Educational: Shows how to retrieve objects from a pool.
        /// Performance: O(1) operation, no instantiation needed.
        /// </summary>
        private GameObject GetPooledTile()
        {
            if (tilePool.Count > 0)
            {
                return tilePool.Dequeue();
            }
            else if (expandPool)
            {
                Log("Expanding tile pool...");
                GameObject newTile = CreatePooledTile();
                return newTile;
            }
            else
            {
                Log("Tile pool exhausted and expansion disabled!");
                return null;
            }
        }
        
        /// <summary>
        /// Returns a tile to the object pool.
        /// Educational: Shows how to return objects to a pool.
        /// Performance: O(1) operation, no destruction needed.
        /// </summary>
        private void ReturnTileToPool(GameObject tileObj)
        {
            if (tileObj == null) return;
            
            tileObj.SetActive(false);
            tileObj.transform.SetParent(boardParent);
            tilePool.Enqueue(tileObj);
            
            if (activeTiles.Contains(tileObj))
            {
                activeTiles.Remove(tileObj);
            }
        }
        
        private void CreateTileGrid()
        {
            if (boardParent == null)
            {
                boardParent = new GameObject("Board").transform;
                boardParent.SetParent(transform);
            }
            
            Vector3 startPosition = CalculateStartPosition();
            
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    Vector3 position = startPosition + new Vector3(
                        x * (tileSize + tileSpacing),
                        y * (tileSize + tileSpacing),
                        0f
                    );
                    
                    CreateTileAt(x, y, position);
                }
            }
        }
        
        private Vector3 CalculateStartPosition()
        {
            float totalWidth = boardWidth * tileSize + (boardWidth - 1) * tileSpacing;
            float totalHeight = boardHeight * tileSize + (boardHeight - 1) * tileSpacing;
            
            return boardCenter - new Vector3(totalWidth * 0.5f, totalHeight * 0.5f, 0f);
        }
        
        private void CreateTileAt(int x, int y, Vector3 position)
        {
            GameObject tileObj = GetPooledTile();
            if (tileObj == null)
            {
                Log($"Failed to get pooled tile for position ({x}, {y})");
                return;
            }
            
            // Set up the tile
            tileObj.transform.position = position;
            tileObj.transform.rotation = Quaternion.identity;
            tileObj.SetActive(true);
            activeTiles.Add(tileObj);
            
            Tile tile = tileObj.GetComponent<Tile>();
            if (tile == null)
            {
                tile = tileObj.AddComponent<Tile>();
            }
            
            tile.Initialize(x, y);
            board[x, y] = tile;
        }
        
        public Tile GetTileAt(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return board[x, y];
            }
            return null;
        }
        
        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < boardWidth && y >= 0 && y < boardHeight;
        }
        
        public Vector3 GetWorldPosition(int x, int y)
        {
            if (!IsValidPosition(x, y)) return Vector3.zero;
            
            Vector3 startPosition = CalculateStartPosition();
            return startPosition + new Vector3(
                x * (tileSize + tileSpacing),
                y * (tileSize + tileSpacing),
                0f
            );
        }
        
        public List<Tile> GetTilesInRow(int row)
        {
            var tiles = new List<Tile>();
            for (int x = 0; x < boardWidth; x++)
            {
                if (board[x, row] != null)
                {
                    tiles.Add(board[x, row]);
                }
            }
            return tiles;
        }
        
        public List<Tile> GetTilesInColumn(int column)
        {
            var tiles = new List<Tile>();
            for (int y = 0; y < boardHeight; y++)
            {
                if (board[column, y] != null)
                {
                    tiles.Add(board[column, y]);
                }
            }
            return tiles;
        }
        
        public void ClearBoard()
        {
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (board[x, y] != null)
                    {
                        board[x, y].Clear();
                    }
                }
            }
        }
        
        public void ClearMatches(List<Vector2Int> positions)
        {
            foreach (var position in positions)
            {
                // Check for power orb at position
                if (powerOrbManager != null)
                {
                    var orb = powerOrbManager.GetOrbAtPosition(position);
                    if (orb != null)
                    {
                        orb.LoseOrb(); // Orb destroyed by match
                    }
                }
                
                // Clear tile at position
                if (IsValidPosition(position) && board[position.x, position.y] != null)
                {
                    ClearTileAt(position);
                }
            }
        }
        
        /// <summary>
        /// Clears a tile at the specified position and returns it to the pool.
        /// Educational: Shows how to properly manage pooled objects.
        /// Performance: O(1) operation, no destruction needed.
        /// </summary>
        public void ClearTileAt(Vector2Int position)
        {
            if (!IsValidPosition(position)) return;
            
            Tile tile = board[position.x, position.y];
            if (tile != null)
            {
                GameObject tileObj = tile.gameObject;
                tile.Clear();
                board[position.x, position.y] = null;
                ReturnTileToPool(tileObj);
            }
        }
        
        /// <summary>
        /// Clears a tile at the specified coordinates and returns it to the pool.
        /// Educational: Shows how to properly manage pooled objects.
        /// Performance: O(1) operation, no destruction needed.
        /// </summary>
        public void ClearTileAt(int x, int y)
        {
            ClearTileAt(new Vector2Int(x, y));
        }
        
        public void FillBoard()
        {
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (board[x, y] != null)
                    {
                        board[x, y].SetRandomColor();
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets pool statistics for debugging.
        /// Educational: Shows how to monitor pool performance.
        /// </summary>
        public (int poolCount, int activeCount) GetPoolStats()
        {
            return (tilePool.Count, activeTiles.Count);
        }
        
        /// <summary>
        /// Logs a message with the BoardController prefix.
        /// Educational: Shows how to implement consistent logging.
        /// </summary>
        private void Log(string message)
        {
            Debug.Log($"[BoardController] {message}");
        }
        
        private void OnDrawGizmos()
        {
            if (board == null) return;
            
            Gizmos.color = Color.yellow;
            Vector3 startPosition = CalculateStartPosition();
            
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    Vector3 position = startPosition + new Vector3(
                        x * (tileSize + tileSpacing),
                        y * (tileSize + tileSpacing),
                        0f
                    );
                    
                    Gizmos.DrawWireCube(position, Vector3.one * tileSize);
                }
            }
        }
    }
}
