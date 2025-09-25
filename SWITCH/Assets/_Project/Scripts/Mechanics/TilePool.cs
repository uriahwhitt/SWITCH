using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Switch.Mechanics
{
    /// <summary>
    /// Unified object pool for all tile types
    /// Manages tile creation, recycling, and memory optimization
    /// </summary>
    public class TilePool : MonoBehaviour
    {
        [System.Serializable]
        public class TilePrefabMap
        {
            public TileType type;
            public GameObject prefab;
        }
        
        [Header("Tile Prefabs")]
        [SerializeField] private TilePrefabMap[] tilePrefabs;
        
        [Header("Pool Settings")]
        [SerializeField] private int initialPoolSize = 5; // Reduced from 20 to save memory
        [SerializeField] private int maxPoolSize = 100;
        
        // Singleton instance
        public static TilePool Instance { get; private set; }
        
        // Pool storage
        private Dictionary<TileType, Queue<Tile>> pools;
        private Dictionary<TileType, GameObject> prefabMap;
        
        // Active tiles tracking
        private List<Tile> activeTiles;
        
        // Centralized sprite cache to prevent memory leaks
        private static Dictionary<int, Sprite> colorSpriteCache = new Dictionary<int, Sprite>();
        private static Sprite whiteSquareSprite;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePool();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Initializes the tile pool system
        /// </summary>
        private void InitializePool()
        {
            pools = new Dictionary<TileType, Queue<Tile>>();
            prefabMap = new Dictionary<TileType, GameObject>();
            activeTiles = new List<Tile>();
            
            // Initialize prefab map
            if (tilePrefabs != null)
            {
                foreach (var prefabMap in tilePrefabs)
                {
                    if (prefabMap.prefab != null)
                    {
                        this.prefabMap[prefabMap.type] = prefabMap.prefab;
                    }
                }
            }
            
            // If no prefabs are assigned, we'll create tiles dynamically
            Debug.Log($"TilePool initialized with {prefabMap.Count} prefab types");
            
            // Initialize pools for each tile type
            foreach (TileType type in System.Enum.GetValues(typeof(TileType)))
            {
                if (type != TileType.Empty)
                {
                    pools[type] = new Queue<Tile>();
                    PrewarmPool(type, initialPoolSize);
                }
            }
            
            Debug.Log($"TilePool initialized with {(tilePrefabs != null ? tilePrefabs.Length : 0)} prefab types");
        }
        
        /// <summary>
        /// Prewarms a pool with initial tiles
        /// </summary>
        private void PrewarmPool(TileType type, int count)
        {
            Debug.Log($"Prewarming pool for {type} with {count} tiles");
            for (int i = 0; i < count; i++)
            {
                Tile tile = CreateNewTile(type);
                if (tile != null)
                {
                    tile.gameObject.SetActive(false);
                    pools[type].Enqueue(tile);
                }
            }
        }
        
        /// <summary>
        /// Gets a tile from the pool
        /// </summary>
        public Tile GetTile(TileType type, TileData data = null)
        {
            if (type == TileType.Empty)
            {
                return null;
            }
            
            Tile tile = null;
            
            // Try to get from pool
            if (pools.ContainsKey(type) && pools[type].Count > 0)
            {
                tile = pools[type].Dequeue();
            }
            else
            {
                // Create new tile if pool is empty
                tile = CreateNewTile(type);
            }
            
            if (tile != null)
            {
                tile.gameObject.SetActive(true);
                activeTiles.Add(tile);
                
                // Initialize with data if provided
                if (data != null)
                {
                    tile.Initialize(data, data.gridPosition);
                }
            }
            
            return tile;
        }
        
        /// <summary>
        /// Returns a tile to the pool
        /// </summary>
        public void ReturnTile(Tile tile)
        {
            if (tile == null) return;
            
            // Remove from active tiles
            activeTiles.Remove(tile);
            
            // Reset tile state
            tile.Reset();
            tile.gameObject.SetActive(false);
            
            // Return to appropriate pool
            if (pools.ContainsKey(tile.Type))
            {
                // Check pool size limit
                if (pools[tile.Type].Count < maxPoolSize)
                {
                    pools[tile.Type].Enqueue(tile);
                }
                else
                {
                    // Destroy if pool is full
                    Destroy(tile.gameObject);
                }
            }
        }
        
        /// <summary>
        /// Creates a new tile of the specified type
        /// </summary>
        private Tile CreateNewTile(TileType type)
        {
            if (prefabMap.ContainsKey(type))
            {
                GameObject prefab = prefabMap[type];
                GameObject tileObject = Instantiate(prefab, transform);
                Tile tile = tileObject.GetComponent<Tile>();
                
                if (tile == null)
                {
                    Debug.LogError($"Prefab for {type} does not have a Tile component!");
                    Destroy(tileObject);
                    return null;
                }
                
                return tile;
            }
            else
            {
                // Create basic tile if no prefab is available
                return CreateBasicTile(type);
            }
        }
        
        /// <summary>
        /// Creates a basic tile GameObject with required components
        /// </summary>
        private Tile CreateBasicTile(TileType type)
        {
            GameObject tileObject = new GameObject($"Tile_{type}");
            tileObject.transform.SetParent(transform);
            
            // Add SpriteRenderer
            SpriteRenderer spriteRenderer = tileObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 0;
            
            // Add BoxCollider2D
            BoxCollider2D collider = tileObject.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            
            // Add appropriate Tile component based on type
            Tile tile = null;
            switch (type)
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
        /// Gets pool statistics for debugging
        /// </summary>
        public Dictionary<TileType, int> GetPoolStats()
        {
            var stats = new Dictionary<TileType, int>();
            
            foreach (var pool in pools)
            {
                stats[pool.Key] = pool.Value.Count;
            }
            
            return stats;
        }
        
        /// <summary>
        /// Clears all pools (useful for testing)
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var pool in pools.Values)
            {
                while (pool.Count > 0)
                {
                    Tile tile = pool.Dequeue();
                    if (tile != null)
                    {
                        Destroy(tile.gameObject);
                    }
                }
            }
            
            activeTiles.Clear();
        }
        
        /// <summary>
        /// Returns all active tiles to their pools
        /// </summary>
        public void ReturnAllActiveTiles()
        {
            var tilesToReturn = new List<Tile>(activeTiles);
            foreach (Tile tile in tilesToReturn)
            {
                ReturnTile(tile);
            }
        }
        
        /// <summary>
        /// Gets a cached color sprite for the given color index
        /// Creates tiny 4x4 textures instead of large 32x32 ones to save memory
        /// </summary>
        public static Sprite GetCachedColorSprite(int colorIndex)
        {
            if (!colorSpriteCache.ContainsKey(colorIndex))
            {
                Debug.LogWarning($"Creating NEW texture for color {colorIndex}!");
                // Create ONE small texture for this color
                Texture2D texture = new Texture2D(4, 4); // Much smaller!
                Color color = GetColorFromIndex(colorIndex);
                Color[] pixels = new Color[16];
                for (int i = 0; i < 16; i++) pixels[i] = color;
                texture.SetPixels(pixels);
                texture.Apply();
                texture.filterMode = FilterMode.Point; // Crisp pixels
                colorSpriteCache[colorIndex] = Sprite.Create(texture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
            }
            else
            {
                Debug.Log($"Reusing cached sprite for color {colorIndex}");
            }
            return colorSpriteCache[colorIndex];
        }
        
        /// <summary>
        /// Gets a cached white square sprite for default tiles
        /// </summary>
        public static Sprite GetCachedWhiteSprite()
        {
            if (whiteSquareSprite == null)
            {
                // Create ONE small white texture
                Texture2D texture = new Texture2D(4, 4);
                Color[] pixels = new Color[16];
                for (int i = 0; i < 16; i++) pixels[i] = Color.white;
                texture.SetPixels(pixels);
                texture.Apply();
                texture.filterMode = FilterMode.Point;
                whiteSquareSprite = Sprite.Create(texture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
            }
            return whiteSquareSprite;
        }
        
        /// <summary>
        /// Converts color index to Unity Color
        /// </summary>
        private static Color GetColorFromIndex(int index)
        {
            switch (index)
            {
                case 0: return Color.red;
                case 1: return Color.blue;
                case 2: return Color.green;
                case 3: return Color.yellow;
                case 4: return Color.magenta;
                case 5: return new Color(1f, 0.5f, 0f); // Orange
                case 6: return new Color(1f, 0.8f, 0f); // Golden yellow for power orbs
                case 7: return new Color(0.3f, 0.3f, 0.3f); // Dark gray for blocking tiles
                default: return Color.white;
            }
        }
        
        #region Debug
        
        [ContextMenu("Show Pool Stats")]
        private void DebugShowPoolStats()
        {
            var stats = GetPoolStats();
            Debug.Log("=== Tile Pool Statistics ===");
            foreach (var stat in stats)
            {
                Debug.Log($"{stat.Key}: {stat.Value} tiles in pool");
            }
            Debug.Log($"Active tiles: {activeTiles.Count}");
            Debug.Log($"Cached sprites: {colorSpriteCache.Count}");
        }
        
        [ContextMenu("Clear All Pools")]
        private void DebugClearAllPools()
        {
            ClearAllPools();
            Debug.Log("All pools cleared");
        }
        
        [ContextMenu("Return All Active Tiles")]
        private void DebugReturnAllActiveTiles()
        {
            ReturnAllActiveTiles();
            Debug.Log("All active tiles returned to pools");
        }
        
        [ContextMenu("Profile Memory")]
        private void ProfileMemory()
        {
            Debug.Log("=== MEMORY PROFILING ===");
            Debug.Log($"Total GameObjects: {FindObjectsOfType<GameObject>().Length}");
            Debug.Log($"Total Tiles: {FindObjectsOfType<Tile>().Length}");
            Debug.Log($"Total Textures: {FindObjectsOfType<Texture2D>().Length}");
            Debug.Log($"Total Sprites: {FindObjectsOfType<Sprite>().Length}");
            Debug.Log($"Active tiles: {activeTiles.Count}");
            Debug.Log($"Cached sprites: {colorSpriteCache.Count}");
            
            // Show pool statistics
            var stats = GetPoolStats();
            Debug.Log("Pool Statistics:");
            foreach (var stat in stats)
            {
                Debug.Log($"  {stat.Key}: {stat.Value} tiles in pool");
            }
            
            // Show memory usage
            long totalMemory = System.GC.GetTotalMemory(false);
            Debug.Log($"GC Total Memory: {totalMemory / (1024 * 1024)} MB");
        }
        
        #endregion
    }
}
