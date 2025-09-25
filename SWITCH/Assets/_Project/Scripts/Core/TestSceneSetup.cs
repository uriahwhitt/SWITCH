using UnityEngine;
using Switch.Mechanics;

namespace Switch.Core
{
    /// <summary>
    /// Sets up the test scene with all necessary components
    /// Creates GameManager, Grid, and TouchInputHandler automatically
    /// </summary>
    public class TestSceneSetup : MonoBehaviour
    {
        [Header("Setup Configuration")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private bool createGameManager = true;
        [SerializeField] private bool createGrid = true;
        [SerializeField] private bool createTouchInput = true;
        [SerializeField] private bool createSmokeTest = true;
        
        [Header("Grid Settings")]
        [SerializeField] private int gridWidth = 8;
        [SerializeField] private int gridHeight = 8;
        [SerializeField] private float tileSize = 1.5f;
        
        [Header("Camera Settings")]
        [SerializeField] private bool setupCamera = true;
        [SerializeField] private float cameraDistance = 10f;
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupTestScene();
            }
        }
        
        /// <summary>
        /// Sets up the complete test scene
        /// </summary>
        [ContextMenu("Setup Test Scene")]
        public void SetupTestScene()
        {
            Debug.Log("Setting up test scene...");
            
            if (createGameManager)
            {
                SetupGameManager();
            }
            
            if (createGrid)
            {
                SetupTilePool(); // Create TilePool first
                SetupGrid();
            }
            
            if (createTouchInput)
            {
                SetupTouchInput();
            }
            
            if (createSmokeTest)
            {
                SetupSmokeTest();
            }
            
            if (setupCamera)
            {
                SetupCamera();
            }
            
            Debug.Log("Test scene setup complete!");
        }
        
        /// <summary>
        /// Creates and configures the GameManager
        /// </summary>
        private void SetupGameManager()
        {
            // Check if GameManager already exists
            if (GameManager.Instance != null)
            {
                Debug.Log("GameManager already exists, skipping creation");
                return;
            }
            
            GameObject gameManagerObject = new GameObject("GameManager");
            GameManager gameManager = gameManagerObject.AddComponent<GameManager>();
            
            Debug.Log("GameManager created and configured");
        }
        
        /// <summary>
        /// Creates and configures the Grid
        /// </summary>
        private void SetupGrid()
        {
            // Check if GameGrid already exists
            Switch.Mechanics.GameGrid existingGrid = FindObjectOfType<Switch.Mechanics.GameGrid>();
            if (existingGrid != null)
            {
                Debug.Log("Grid already exists, skipping creation");
                return;
            }
            
            GameObject gridObject = new GameObject("GameGrid");
            Switch.Mechanics.GameGrid grid = gridObject.AddComponent<Switch.Mechanics.GameGrid>();
            
            // Configure grid settings
            var gridType = typeof(Switch.Mechanics.GameGrid);
            var widthField = gridType.GetField("gridWidth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var heightField = gridType.GetField("gridHeight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tileSizeField = gridType.GetField("tileSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (widthField != null) widthField.SetValue(grid, gridWidth);
            if (heightField != null) heightField.SetValue(grid, gridHeight);
            if (tileSizeField != null) tileSizeField.SetValue(grid, tileSize);
            
            Debug.Log($"Grid created with {gridWidth}x{gridHeight} configuration");
        }
        
        /// <summary>
        /// Creates and configures the TilePool
        /// </summary>
        private void SetupTilePool()
        {
            // Check if TilePool already exists
            Switch.Mechanics.TilePool existingPool = FindObjectOfType<Switch.Mechanics.TilePool>();
            if (existingPool != null)
            {
                Debug.Log("TilePool already exists, skipping creation");
                return;
            }
            
            GameObject poolObject = new GameObject("TilePool");
            Switch.Mechanics.TilePool tilePool = poolObject.AddComponent<Switch.Mechanics.TilePool>();
            
            Debug.Log("TilePool created and configured");
        }
        
        /// <summary>
        /// Creates and configures the TouchInputHandler
        /// </summary>
        private void SetupTouchInput()
        {
            // Check if TouchInputHandler already exists
            TouchInputHandler existingInput = FindObjectOfType<TouchInputHandler>();
            if (existingInput != null)
            {
                Debug.Log("TouchInputHandler already exists, skipping creation");
                return;
            }
            
            GameObject inputObject = new GameObject("TouchInputHandler");
            TouchInputHandler touchInput = inputObject.AddComponent<TouchInputHandler>();
            
            Debug.Log("TouchInputHandler created and configured");
        }
        
        /// <summary>
        /// Creates and configures the SmokeTest
        /// </summary>
        private void SetupSmokeTest()
        {
            // Check if SmokeTest already exists
            SmokeTest existingSmokeTest = FindObjectOfType<SmokeTest>();
            if (existingSmokeTest != null)
            {
                Debug.Log("SmokeTest already exists, skipping creation");
                return;
            }
            
            GameObject smokeTestObject = new GameObject("SmokeTest");
            SmokeTest smokeTest = smokeTestObject.AddComponent<SmokeTest>();
            
            Debug.Log("SmokeTest created and configured");
        }
        
        /// <summary>
        /// Sets up the camera for optimal grid viewing
        /// </summary>
        private void SetupCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                // Find existing camera
                mainCamera = FindObjectOfType<Camera>();
            }
            
            if (mainCamera != null)
            {
                // Position camera to view the grid properly
                float cellSize = tileSize + 0.1f; // Include spacing
                float gridWidth = this.gridWidth * cellSize;
                float gridHeight = this.gridHeight * cellSize;
                
                // Calculate camera position to fit the grid
                float cameraSize = Mathf.Max(gridWidth, gridHeight) * 0.6f;
                mainCamera.orthographicSize = cameraSize;
                mainCamera.transform.position = new Vector3(0, 0, -cameraDistance);
                
                Debug.Log($"Camera positioned at (0, 0, -{cameraDistance}) with orthographic size {cameraSize}");
            }
            else
            {
                Debug.LogWarning("No camera found for setup");
            }
        }
        
        /// <summary>
        /// Connects the input system to the grid for tile selection
        /// </summary>
        private void ConnectInputToGrid()
        {
            TouchInputHandler touchInput = FindObjectOfType<TouchInputHandler>();
            Switch.Mechanics.GameGrid grid = FindObjectOfType<Switch.Mechanics.GameGrid>();
            
            if (touchInput != null && grid != null)
            {
                // Subscribe to tap events to handle tile selection
                TouchInputHandler.OnTap += (worldPosition) =>
                {
                    // Find the tile at the tapped position
                    Switch.Mechanics.Tile closestTile = FindClosestTile(worldPosition, grid);
                    if (closestTile != null)
                    {
                        closestTile.ToggleSelection();
                    }
                };
                
                Debug.Log("Input system connected to grid");
            }
        }
        
        /// <summary>
        /// Finds the closest tile to a world position
        /// </summary>
        private Switch.Mechanics.Tile FindClosestTile(Vector2 worldPosition, Switch.Mechanics.GameGrid grid)
        {
            Switch.Mechanics.Tile closestTile = null;
            float closestDistance = float.MaxValue;
            
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    Switch.Mechanics.Tile tile = grid.GetTile(x, y);
                    if (tile != null)
                    {
                        float distance = Vector2.Distance(worldPosition, tile.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTile = tile;
                        }
                    }
                }
            }
            
            return closestTile;
        }
        
        /// <summary>
        /// Validates the scene setup
        /// </summary>
        [ContextMenu("Validate Setup")]
        public void ValidateSetup()
        {
            Debug.Log("Validating scene setup...");
            
            bool allGood = true;
            
            // Check GameManager
            if (GameManager.Instance == null)
            {
                Debug.LogError("❌ GameManager not found!");
                allGood = false;
            }
            else
            {
                Debug.Log("✅ GameManager found");
            }
            
            // Check GameGrid
            Switch.Mechanics.GameGrid grid = FindObjectOfType<Switch.Mechanics.GameGrid>();
            if (grid == null)
            {
                Debug.LogError("❌ Grid not found!");
                allGood = false;
            }
            else
            {
                Debug.Log($"✅ Grid found with {grid.TotalTiles} tiles");
            }
            
            // Check TouchInputHandler
            TouchInputHandler touchInput = FindObjectOfType<TouchInputHandler>();
            if (touchInput == null)
            {
                Debug.LogError("❌ TouchInputHandler not found!");
                allGood = false;
            }
            else
            {
                Debug.Log("✅ TouchInputHandler found");
            }
            
            // Check SmokeTest
            SmokeTest smokeTest = FindObjectOfType<SmokeTest>();
            if (smokeTest == null)
            {
                Debug.LogError("❌ SmokeTest not found!");
                allGood = false;
            }
            else
            {
                Debug.Log("✅ SmokeTest found");
            }
            
            if (allGood)
            {
                Debug.Log("🎉 All components validated successfully!");
            }
            else
            {
                Debug.LogError("❌ Scene setup validation failed!");
            }
        }
        
        /// <summary>
        /// Cleans up the scene (removes all created objects)
        /// </summary>
        [ContextMenu("Cleanup Scene")]
        public void CleanupScene()
        {
            Debug.Log("Cleaning up scene...");
            
            // Remove GameManager
            if (GameManager.Instance != null)
            {
                DestroyImmediate(GameManager.Instance.gameObject);
            }
            
            // Remove GameGrid
            Switch.Mechanics.GameGrid grid = FindObjectOfType<Switch.Mechanics.GameGrid>();
            if (grid != null)
            {
                DestroyImmediate(grid.gameObject);
            }
            
            // Remove TouchInputHandler
            TouchInputHandler touchInput = FindObjectOfType<TouchInputHandler>();
            if (touchInput != null)
            {
                DestroyImmediate(touchInput.gameObject);
            }
            
            // Remove SmokeTest
            SmokeTest smokeTest = FindObjectOfType<SmokeTest>();
            if (smokeTest != null)
            {
                DestroyImmediate(smokeTest.gameObject);
            }
            
            Debug.Log("Scene cleanup complete");
        }
    }
}
