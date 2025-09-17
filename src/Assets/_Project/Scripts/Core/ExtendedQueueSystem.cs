/******************************************************************************
 * SWITCH - ExtendedQueueSystem
 * Sprint: 1
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: 15-tile queue system (10 visible + 5 buffer)
 * Dependencies: TileData, Tile
 * 
 * Educational Notes:
 * - Demonstrates advanced queue management with buffer system
 * - Shows how to implement anti-frustration algorithms
 * - Performance: Optimized for smooth tile distribution
 *****************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using SWITCH.Data;

namespace SWITCH.Core
{
    /// <summary>
    /// Extended queue system with 15 tiles (10 visible + 5 buffer).
    /// Educational: This demonstrates advanced queue management with buffer system.
    /// Performance: Optimized for smooth tile distribution and anti-frustration.
    /// </summary>
    public class ExtendedQueueSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private int visibleTiles = 10;
        [SerializeField] private int bufferTiles = 5;
        [SerializeField] private int totalTiles => visibleTiles + bufferTiles;
        
        [Header("Anti-Frustration")]
        [SerializeField] private bool enableAntiFrustration = true;
        [SerializeField] private int maxSameColorInRow = 3;
        [SerializeField] private float colorDistributionWeight = 0.7f;
        
        [Header("References")]
        [SerializeField] private Transform queueParent;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private TileDataAsset[] tileDataAssets;
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private bool showQueueVisualization = false;
        
        // Queue state
        private Queue<TileData> tileQueue = new Queue<TileData>();
        private List<TileData> visibleTilesList = new List<TileData>();
        private List<TileData> bufferTilesList = new List<TileData>();
        
        // Anti-frustration tracking
        private Dictionary<ColorType, int> recentColorCount = new Dictionary<ColorType, int>();
        private List<ColorType> recentColors = new List<ColorType>();
        private int recentColorHistory = 10;
        
        // Events
        public System.Action<TileData> OnTileDrawn;
        public System.Action OnQueueRefilled;
        public System.Action<ColorType> OnColorDistributionChanged;
        
        // Properties
        public int VisibleTileCount => visibleTilesList.Count;
        public int BufferTileCount => bufferTilesList.Count;
        public int TotalTileCount => tileQueue.Count;
        public bool IsQueueEmpty => tileQueue.Count == 0;
        
        private void Awake()
        {
            InitializeQueue();
        }
        
        /// <summary>
        /// Initializes the queue system with initial tiles.
        /// Educational: Shows how to set up complex data structures.
        /// </summary>
        private void InitializeQueue()
        {
            tileQueue.Clear();
            visibleTilesList.Clear();
            bufferTilesList.Clear();
            recentColorCount.Clear();
            recentColors.Clear();
            
            // Initialize color tracking
            foreach (ColorType color in System.Enum.GetValues(typeof(ColorType)))
            {
                recentColorCount[color] = 0;
            }
            
            // Fill initial queue
            RefillQueue();
            
            Log($"Queue initialized with {tileQueue.Count} tiles");
        }
        
        /// <summary>
        /// Refills the queue with new tiles using anti-frustration algorithm.
        /// Educational: Shows how to implement smart distribution algorithms.
        /// </summary>
        public void RefillQueue()
        {
            int tilesNeeded = totalTiles - tileQueue.Count;
            
            for (int i = 0; i < tilesNeeded; i++)
            {
                TileData newTile = GenerateSmartTile();
                tileQueue.Enqueue(newTile);
            }
            
            UpdateVisibleTiles();
            OnQueueRefilled?.Invoke();
            
            Log($"Queue refilled with {tilesNeeded} new tiles. Total: {tileQueue.Count}");
        }
        
        /// <summary>
        /// Generates a smart tile using anti-frustration algorithm.
        /// Educational: Shows how to implement intelligent game design.
        /// </summary>
        private TileData GenerateSmartTile()
        {
            if (!enableAntiFrustration)
            {
                return GenerateRandomTile();
            }
            
            // Get available colors
            List<ColorType> availableColors = GetAvailableColors();
            
            // Apply anti-frustration rules
            List<ColorType> validColors = ApplyAntiFrustrationRules(availableColors);
            
            // Select color with weighted distribution
            ColorType selectedColor = SelectColorWithWeight(validColors);
            
            // Create tile data
            TileData tileData = new TileData
            {
                colorType = selectedColor,
                tileType = TileType.Normal,
                scoreValue = 100,
                color = GetColorForType(selectedColor)
            };
            
            // Update tracking
            UpdateColorTracking(selectedColor);
            
            return tileData;
        }
        
        /// <summary>
        /// Gets available colors from tile data assets.
        /// Educational: Shows how to work with ScriptableObject data.
        /// </summary>
        private List<ColorType> GetAvailableColors()
        {
            List<ColorType> colors = new List<ColorType>();
            
            if (tileDataAssets != null && tileDataAssets.Length > 0)
            {
                foreach (var asset in tileDataAssets)
                {
                    if (asset != null && asset.tileData != null)
                    {
                        colors.Add(asset.tileData.colorType);
                    }
                }
            }
            
            // Fallback to all colors if no assets
            if (colors.Count == 0)
            {
                foreach (ColorType color in System.Enum.GetValues(typeof(ColorType)))
                {
                    colors.Add(color);
                }
            }
            
            return colors;
        }
        
        /// <summary>
        /// Applies anti-frustration rules to color selection.
        /// Educational: Shows how to implement game balance algorithms.
        /// </summary>
        private List<ColorType> ApplyAntiFrustrationRules(List<ColorType> availableColors)
        {
            List<ColorType> validColors = new List<ColorType>();
            
            foreach (ColorType color in availableColors)
            {
                bool isValid = true;
                
                // Check for too many same colors in recent history
                if (recentColorCount[color] >= maxSameColorInRow)
                {
                    isValid = false;
                }
                
                // Check for color distribution balance
                if (IsColorOverrepresented(color))
                {
                    isValid = false;
                }
                
                if (isValid)
                {
                    validColors.Add(color);
                }
            }
            
            // If no valid colors, allow all colors (fallback)
            if (validColors.Count == 0)
            {
                validColors.AddRange(availableColors);
            }
            
            return validColors;
        }
        
        /// <summary>
        /// Checks if a color is overrepresented in recent history.
        /// Educational: Shows how to implement statistical analysis.
        /// </summary>
        private bool IsColorOverrepresented(ColorType color)
        {
            if (recentColors.Count < recentColorHistory) return false;
            
            int colorCount = 0;
            for (int i = recentColors.Count - recentColorHistory; i < recentColors.Count; i++)
            {
                if (recentColors[i] == color)
                {
                    colorCount++;
                }
            }
            
            float colorRatio = (float)colorCount / recentColorHistory;
            return colorRatio > colorDistributionWeight;
        }
        
        /// <summary>
        /// Selects a color with weighted distribution.
        /// Educational: Shows how to implement weighted random selection.
        /// </summary>
        private ColorType SelectColorWithWeight(List<ColorType> validColors)
        {
            if (validColors.Count == 1) return validColors[0];
            
            // Calculate weights (less recent colors get higher weight)
            List<float> weights = new List<float>();
            foreach (ColorType color in validColors)
            {
                float weight = 1f - (float)recentColorCount[color] / maxSameColorInRow;
                weights.Add(Mathf.Max(0.1f, weight));
            }
            
            // Select based on weights
            float totalWeight = 0f;
            foreach (float weight in weights)
            {
                totalWeight += weight;
            }
            
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;
            
            for (int i = 0; i < validColors.Count; i++)
            {
                currentWeight += weights[i];
                if (randomValue <= currentWeight)
                {
                    return validColors[i];
                }
            }
            
            return validColors[validColors.Count - 1]; // Fallback
        }
        
        /// <summary>
        /// Generates a completely random tile.
        /// Educational: Shows how to implement fallback mechanisms.
        /// </summary>
        private TileData GenerateRandomTile()
        {
            ColorType[] colors = System.Enum.GetValues(typeof(ColorType)) as ColorType[];
            ColorType randomColor = colors[Random.Range(0, colors.Length)];
            
            return new TileData
            {
                colorType = randomColor,
                tileType = TileType.Normal,
                scoreValue = 100,
                color = GetColorForType(randomColor)
            };
        }
        
        /// <summary>
        /// Updates color tracking for anti-frustration.
        /// Educational: Shows how to maintain statistical data.
        /// </summary>
        private void UpdateColorTracking(ColorType color)
        {
            recentColorCount[color]++;
            recentColors.Add(color);
            
            // Maintain history size
            if (recentColors.Count > recentColorHistory * 2)
            {
                ColorType removedColor = recentColors[0];
                recentColors.RemoveAt(0);
                recentColorCount[removedColor] = Mathf.Max(0, recentColorCount[removedColor] - 1);
            }
            
            OnColorDistributionChanged?.Invoke(color);
        }
        
        /// <summary>
        /// Draws a tile from the queue.
        /// Educational: Shows how to implement queue operations.
        /// </summary>
        public TileData DrawTile()
        {
            if (tileQueue.Count == 0)
            {
                Log("Queue is empty, refilling...");
                RefillQueue();
            }
            
            if (tileQueue.Count == 0)
            {
                Log("Failed to refill queue!");
                return null;
            }
            
            TileData drawnTile = tileQueue.Dequeue();
            UpdateVisibleTiles();
            
            OnTileDrawn?.Invoke(drawnTile);
            Log($"Drew tile: {drawnTile.colorType}");
            
            return drawnTile;
        }
        
        /// <summary>
        /// Updates the visible tiles list.
        /// Educational: Shows how to maintain UI state.
        /// </summary>
        private void UpdateVisibleTiles()
        {
            visibleTilesList.Clear();
            bufferTilesList.Clear();
            
            TileData[] queueArray = tileQueue.ToArray();
            
            // Add visible tiles
            for (int i = 0; i < Mathf.Min(visibleTiles, queueArray.Length); i++)
            {
                visibleTilesList.Add(queueArray[i]);
            }
            
            // Add buffer tiles
            for (int i = visibleTiles; i < Mathf.Min(totalTiles, queueArray.Length); i++)
            {
                bufferTilesList.Add(queueArray[i]);
            }
        }
        
        /// <summary>
        /// Gets the next tile without removing it from the queue.
        /// Educational: Shows how to peek at queue contents.
        /// </summary>
        public TileData PeekNextTile()
        {
            if (tileQueue.Count == 0) return null;
            
            TileData[] queueArray = tileQueue.ToArray();
            return queueArray[0];
        }
        
        /// <summary>
        /// Gets the next few tiles for preview.
        /// Educational: Shows how to implement preview systems.
        /// </summary>
        public List<TileData> GetNextTiles(int count)
        {
            List<TileData> nextTiles = new List<TileData>();
            TileData[] queueArray = tileQueue.ToArray();
            
            for (int i = 0; i < Mathf.Min(count, queueArray.Length); i++)
            {
                nextTiles.Add(queueArray[i]);
            }
            
            return nextTiles;
        }
        
        /// <summary>
        /// Gets color for a specific color type.
        /// Educational: Shows how to map enums to values.
        /// </summary>
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
        
        private void Log(string message)
        {
            if (debugMode)
            {
                Debug.Log($"[ExtendedQueueSystem] {message}");
            }
        }
        
        private void OnDrawGizmos()
        {
            if (showQueueVisualization)
            {
                // Draw queue visualization
                Gizmos.color = Color.green;
                Vector3 startPos = transform.position;
                
                for (int i = 0; i < visibleTilesList.Count; i++)
                {
                    Vector3 pos = startPos + Vector3.right * i * 0.5f;
                    Gizmos.DrawWireCube(pos, Vector3.one * 0.4f);
                }
                
                Gizmos.color = Color.yellow;
                for (int i = 0; i < bufferTilesList.Count; i++)
                {
                    Vector3 pos = startPos + Vector3.right * (visibleTiles + i) * 0.5f + Vector3.up * 0.5f;
                    Gizmos.DrawWireCube(pos, Vector3.one * 0.3f);
                }
            }
        }
    }
}
