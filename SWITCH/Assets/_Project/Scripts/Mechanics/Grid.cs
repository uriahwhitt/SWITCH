using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Switch.Mechanics
{
    /// <summary>
    /// Manages the 8x8 game grid and tile interactions
    /// Updated to work with unified tile system and object pooling
    /// </summary>
    public class GameGrid : MonoBehaviour
    {
        [Header("Grid Configuration")]
        [SerializeField] private int gridWidth = 8;
        [SerializeField] private int gridHeight = 8;
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private float tileSpacing = 0.1f;
        
        [Header("Grid Visual")]
        [SerializeField] private Color gridBackgroundColor = Color.gray;
        [SerializeField] private bool showGridLines = true;
        
        [Header("Tile Distribution")]
        [SerializeField] private float regularTileChance = 0.8f;
        [SerializeField] private float blockingTileChance = 0.15f;
        [SerializeField] private float powerOrbChance = 0.05f;
        
        // Grid data
        private Tile[,] gridTiles;
        private List<Tile> selectedTiles = new List<Tile>();
        
        // Properties
        public int Width => gridWidth;
        public int Height => gridHeight;
        public float TileSize => tileSize;
        public int TotalTiles => gridWidth * gridHeight;
        public int SelectedTileCount => selectedTiles.Count;
        
        // Events
        public static event System.Action<GameGrid> OnGridCreated;
        public static event System.Action<Tile> OnTileSelected;
        public static event System.Action<Tile> OnTileDeselected;
        
        private void Awake()
        {
            // Subscribe to tile events
            Tile.OnTileSelected += HandleTileSelected;
            Tile.OnTileDeselected += HandleTileDeselected;
        }
        
        private void Start()
        {
            CreateGrid();
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            Tile.OnTileSelected -= HandleTileSelected;
            Tile.OnTileDeselected -= HandleTileDeselected;
        }
        
        /// <summary>
        /// Creates the 8x8 grid of tiles
        /// </summary>
        public void CreateGrid()
        {
            // Initialize grid array
            gridTiles = new Tile[gridWidth, gridHeight];
            
            // Calculate grid center offset
            Vector3 gridOffset = GetGridOffset();
            
            // Create tiles
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    CreateTile(x, y, gridOffset);
                }
            }
            
            // Populate with random colors
            PopulateGridWithRandomColors();
            
            OnGridCreated?.Invoke(this);
            Debug.Log($"Grid created: {gridWidth}x{gridHeight} with {TotalTiles} tiles (using shared sprites for memory efficiency)");
        }
        
        /// <summary>
        /// Creates a single tile at the specified grid position using the tile pool
        /// </summary>
        private void CreateTile(int x, int y, Vector3 gridOffset)
        {
            // Determine tile type based on distribution
            TileType tileType = GetRandomTileType();
            
            // Create tile data
            TileData tileData = new TileData(tileType, GetRandomColorIndex(), new Vector2Int(x, y));
            
            // Get tile from pool
            Tile tile = null;
            if (TilePool.Instance != null)
            {
                tile = TilePool.Instance.GetTile(tileType, tileData);
            }
            else
            {
                // Fallback: create tile directly
                tile = CreateTileDirectly(tileType, tileData);
            }
            
            if (tile != null)
            {
                // Position the tile
                Vector3 worldPosition = CalculateTilePosition(x, y, gridOffset);
                tile.transform.position = worldPosition;
                tile.transform.SetParent(transform);
                
                // Tile is already initialized by TilePool.GetTile(), just update position
                tile.SetGridPosition(new Vector2Int(x, y));
                
                // Set tile size to fill the entire grid cell (including spacing)
                float cellSize = tileSize + tileSpacing;
                tile.SetTileSize(cellSize); // This will set the scale properly
                
                // Store in grid array
                gridTiles[x, y] = tile;
                
                // Name the tile
                tile.name = $"Tile_{x}_{y}_{tileType}";
            }
        }
        
        /// <summary>
        /// Creates a tile directly (fallback when pool is not available)
        /// </summary>
        private Tile CreateTileDirectly(TileType tileType, TileData tileData)
        {
            GameObject tileObject = new GameObject($"Tile_{tileType}");
            tileObject.transform.SetParent(transform);
            
            // Add SpriteRenderer
            SpriteRenderer spriteRenderer = tileObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 0;
            
            // Add BoxCollider2D
            BoxCollider2D collider = tileObject.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            
            // Add appropriate Tile component based on type
            Tile tile = null;
            switch (tileType)
            {
                case TileType.Regular:
                    tile = tileObject.AddComponent<RegularTile>();
                    break;
                case TileType.Blocking:
                    tile = tileObject.AddComponent<BlockingTile>();
                    break;
                case TileType.PowerOrb:
                    tile = tileObject.AddComponent<PowerOrb>();
                    break;
                default:
                    tile = tileObject.AddComponent<RegularTile>(); // Default to regular
                    break;
            }
            
            return tile;
        }
        
        /// <summary>
        /// Gets a random tile type based on distribution settings
        /// </summary>
        private TileType GetRandomTileType()
        {
            float random = Random.Range(0f, 1f);
            TileType selected;
            
            if (random < powerOrbChance)
                selected = TileType.PowerOrb;
            else if (random < powerOrbChance + blockingTileChance)
                selected = TileType.Blocking;
            else
                selected = TileType.Regular;
            
            Debug.Log($"Random: {random:F3}, Selected: {selected} (PowerOrb: {powerOrbChance}, Blocking: {blockingTileChance}, Regular: {regularTileChance})");
            return selected;
        }
        
        /// <summary>
        /// Gets a random color index (0-5)
        /// </summary>
        private int GetRandomColorIndex()
        {
            return Random.Range(0, 6); // 0-5 for regular colors
        }
        
        // REMOVED: GetDefaultTileSprite() method was creating textures unnecessarily
        // All tiles now use the centralized sprite cache in TilePool
        
        /// <summary>
        /// Calculates the world position for a tile at grid coordinates
        /// </summary>
        private Vector3 CalculateTilePosition(int x, int y, Vector3 gridOffset)
        {
            // Calculate position with proper cell size offset
            float cellSize = tileSize + tileSpacing;
            float xPos = x * cellSize + (cellSize * 0.5f);
            float yPos = y * cellSize + (cellSize * 0.5f);
            return gridOffset + new Vector3(xPos, yPos, 0f);
        }
        
        /// <summary>
        /// Populates the grid with random tile colors (legacy method - now handled in CreateTile)
        /// </summary>
        private void PopulateGridWithRandomColors()
        {
            // This method is now handled during tile creation
            // Keeping for compatibility but tiles are already initialized with random colors
        }
        
        /// <summary>
        /// Gets the tile at the specified grid position
        /// </summary>
        public Tile GetTile(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return gridTiles[x, y];
            }
            return null;
        }
        
        /// <summary>
        /// Gets the tile at the specified grid position
        /// </summary>
        public Tile GetTile(Vector2Int position)
        {
            return GetTile(position.x, position.y);
        }
        
        /// <summary>
        /// Checks if the given position is valid within the grid
        /// </summary>
        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
        }
        
        /// <summary>
        /// Checks if the given position is valid within the grid
        /// </summary>
        public bool IsValidPosition(Vector2Int position)
        {
            return IsValidPosition(position.x, position.y);
        }
        
        /// <summary>
        /// Gets all tiles of a specific color index
        /// </summary>
        public List<Tile> GetTilesOfColor(int colorIndex)
        {
            List<Tile> tiles = new List<Tile>();
            
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Tile tile = gridTiles[x, y];
                    if (tile != null && tile.ColorIndex == colorIndex)
                    {
                        tiles.Add(tile);
                    }
                }
            }
            
            return tiles;
        }
        
        /// <summary>
        /// Gets all tiles of a specific type
        /// </summary>
        public List<Tile> GetTilesOfType(TileType type)
        {
            List<Tile> tiles = new List<Tile>();
            
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Tile tile = gridTiles[x, y];
                    if (tile != null && tile.Type == type)
                    {
                        tiles.Add(tile);
                    }
                }
            }
            
            return tiles;
        }
        
        /// <summary>
        /// Places a tile at the specified position
        /// </summary>
        public void PlaceTile(Tile tile, Vector2Int position)
        {
            if (IsValidPosition(position))
            {
                // Remove existing tile if any
                Tile existingTile = gridTiles[position.x, position.y];
                if (existingTile != null && TilePool.Instance != null)
                {
                    TilePool.Instance.ReturnTile(existingTile);
                }
                
                // Place new tile
                gridTiles[position.x, position.y] = tile;
                tile.transform.position = CalculateTilePosition(position.x, position.y, GetGridOffset());
                tile.Initialize(tile.GetData(), position);
            }
        }
        
        /// <summary>
        /// Swaps two tiles at the specified positions
        /// </summary>
        public bool SwapTiles(Vector2Int pos1, Vector2Int pos2)
        {
            if (!IsValidPosition(pos1) || !IsValidPosition(pos2))
                return false;
            
            Tile tile1 = gridTiles[pos1.x, pos1.y];
            Tile tile2 = gridTiles[pos2.x, pos2.y];
            
            // Check if tiles can swap
            if (tile1 != null && tile2 != null && tile1.CanSwapWith(tile2))
            {
                // Perform swap
                gridTiles[pos1.x, pos1.y] = tile2;
                gridTiles[pos2.x, pos2.y] = tile1;
                
                // Update positions
                Vector3 pos1World = CalculateTilePosition(pos1.x, pos1.y, GetGridOffset());
                Vector3 pos2World = CalculateTilePosition(pos2.x, pos2.y, GetGridOffset());
                
                tile1.transform.position = pos2World;
                tile2.transform.position = pos1World;
                
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Gets the grid offset for positioning calculations
        /// </summary>
        private Vector3 GetGridOffset()
        {
            float cellSize = tileSize + tileSpacing;
            float totalWidth = gridWidth * cellSize - tileSpacing;
            float totalHeight = gridHeight * cellSize - tileSpacing;
            return new Vector3(-totalWidth * 0.5f, -totalHeight * 0.5f, 0f);
        }
        
        /// <summary>
        /// Clears all selected tiles
        /// </summary>
        public void ClearSelection()
        {
            foreach (Tile tile in selectedTiles)
            {
                tile.IsSelected = false;
            }
            selectedTiles.Clear();
        }
        
        /// <summary>
        /// Handles tile selection
        /// </summary>
        private void HandleTileSelected(Tile tile)
        {
            if (!selectedTiles.Contains(tile))
            {
                selectedTiles.Add(tile);
                OnTileSelected?.Invoke(tile);
                
                Debug.Log($"Tile selected: {tile.GridPosition}, Total selected: {selectedTiles.Count}");
            }
        }
        
        /// <summary>
        /// Handles tile deselection
        /// </summary>
        private void HandleTileDeselected(Tile tile)
        {
            if (selectedTiles.Contains(tile))
            {
                selectedTiles.Remove(tile);
                OnTileDeselected?.Invoke(tile);
                
                Debug.Log($"Tile deselected: {tile.GridPosition}, Total selected: {selectedTiles.Count}");
            }
        }
        
        /// <summary>
        /// Resets the entire grid
        /// </summary>
        public void ResetGrid()
        {
            ClearSelection();
            
            // Return all tiles to pool
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Tile tile = gridTiles[x, y];
                    if (tile != null && TilePool.Instance != null)
                    {
                        TilePool.Instance.ReturnTile(tile);
                    }
                    gridTiles[x, y] = null;
                }
            }
            
            // Recreate grid with new tiles
            CreateGrid();
        }
        
        #region Debug
        
        [ContextMenu("Create Grid")]
        private void DebugCreateGrid()
        {
            CreateGrid();
        }
        
        [ContextMenu("Reset Grid")]
        private void DebugResetGrid()
        {
            ResetGrid();
        }
        
        [ContextMenu("Clear Selection")]
        private void DebugClearSelection()
        {
            ClearSelection();
        }
        
        [ContextMenu("Log Grid Composition")]
        private void LogGridComposition()
        {
            int regularCount = 0, blockingCount = 0, orbCount = 0;
            
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Tile tile = gridTiles[x, y];
                    if (tile != null)
                    {
                        switch (tile.Type)
                        {
                            case TileType.Regular:
                                regularCount++;
                                break;
                            case TileType.Blocking:
                                blockingCount++;
                                break;
                            case TileType.PowerOrb:
                                orbCount++;
                                break;
                        }
                    }
                }
            }
            
            Debug.Log($"Grid Composition: Regular={regularCount}, Blocking={blockingCount}, PowerOrbs={orbCount}");
            Debug.Log($"Percentages: Regular={regularCount/64f:P}, Blocking={blockingCount/64f:P}, Orbs={orbCount/64f:P}");
        }
        
        private void OnDrawGizmos()
        {
            if (showGridLines && gridTiles != null)
            {
                Gizmos.color = Color.white;
                
                // Draw grid lines
                for (int x = 0; x <= gridWidth; x++)
                {
                    Vector3 start = transform.position + new Vector3(x * (tileSize + tileSpacing) - (gridWidth * (tileSize + tileSpacing)) * 0.5f, 
                                                                   -(gridHeight * (tileSize + tileSpacing)) * 0.5f, 0);
                    Vector3 end = start + new Vector3(0, gridHeight * (tileSize + tileSpacing), 0);
                    Gizmos.DrawLine(start, end);
                }
                
                for (int y = 0; y <= gridHeight; y++)
                {
                    Vector3 start = transform.position + new Vector3(-(gridWidth * (tileSize + tileSpacing)) * 0.5f,
                                                                   y * (tileSize + tileSpacing) - (gridHeight * (tileSize + tileSpacing)) * 0.5f, 0);
                    Vector3 end = start + new Vector3(gridWidth * (tileSize + tileSpacing), 0, 0);
                    Gizmos.DrawLine(start, end);
                }
            }
        }
        
        #endregion
    }
}
