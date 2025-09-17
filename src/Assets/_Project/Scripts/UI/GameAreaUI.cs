using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SWITCH.UI
{
    /// <summary>
    /// Manages the main game area UI including queue panel and 8x8 game grid
    /// </summary>
    public class GameAreaUI : MonoBehaviour
    {
        [Header("Queue Panel")]
        [SerializeField] private GameObject queuePanel;
        [SerializeField] private Transform queueContainer;
        [SerializeField] private GameObject queueDotPrefab;
        [SerializeField] private int queueSize = 10;
        
        [Header("Game Board")]
        [SerializeField] private GameObject boardGrid;
        [SerializeField] private Transform tileContainer;
        [SerializeField] private GameObject tileSlotPrefab;
        [SerializeField] private int boardSize = 8;
        
        [Header("Edge Glows")]
        [SerializeField] private LineRenderer[] edgeGlows = new LineRenderer[4];
        [SerializeField] private Color[] edgeColors = {
            UIColors.edgeTop,      // Top edge
            UIColors.edgeRight,    // Right edge
            UIColors.edgeBottom,   // Bottom edge
            UIColors.edgeLeft      // Left edge
        };
        
        [Header("Visual Settings")]
        [SerializeField] private Color queueDotActive = UIColors.queueDotActive;
        [SerializeField] private Color queueDotInactive = UIColors.queueDotInactive;
        [SerializeField] private float edgeGlowIntensity = 0.8f;
        
        // Queue management
        private List<Image> queueDots = new List<Image>();
        private int currentQueueIndex = 0;
        
        // Board management
        private GameObject[,] tileSlots;
        private Vector2[,] tilePositions;
        
        // Edge glow management
        private bool[] edgeGlowActive = new bool[4];
        
        // References
        private BoardController boardController;
        
        // Events
        public System.Action<int> OnQueueDotClicked;
        public System.Action<Vector2Int> OnTileSlotClicked;
        
        private void Awake()
        {
            boardController = FindObjectOfType<BoardController>();
            if (boardController != null)
            {
                boardController.OnBoardChanged += OnBoardChanged;
            }
        }
        
        private void Start()
        {
            InitializeQueue();
            InitializeBoard();
            InitializeEdgeGlows();
        }
        
        private void OnDestroy()
        {
            if (boardController != null)
            {
                boardController.OnBoardChanged -= OnBoardChanged;
            }
        }
        
        /// <summary>
        /// Initializes the queue panel with dots
        /// </summary>
        private void InitializeQueue()
        {
            if (queueContainer == null || queueDotPrefab == null) return;
            
            // Clear existing dots
            foreach (Transform child in queueContainer)
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
            
            queueDots.Clear();
            
            // Create queue dots
            for (int i = 0; i < queueSize; i++)
            {
                GameObject dotObj = Instantiate(queueDotPrefab, queueContainer);
                Image dotImage = dotObj.GetComponent<Image>();
                
                if (dotImage != null)
                {
                    queueDots.Add(dotImage);
                    dotImage.color = queueDotInactive;
                }
                
                // Add click handler
                Button dotButton = dotObj.GetComponent<Button>();
                if (dotButton != null)
                {
                    int index = i; // Capture for closure
                    dotButton.onClick.AddListener(() => OnQueueDotClick(index));
                }
            }
        }
        
        /// <summary>
        /// Initializes the game board with tile slots
        /// </summary>
        private void InitializeBoard()
        {
            if (tileContainer == null || tileSlotPrefab == null) return;
            
            // Clear existing slots
            foreach (Transform child in tileContainer)
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
            
            tileSlots = new GameObject[boardSize, boardSize];
            tilePositions = new Vector2[boardSize, boardSize];
            
            // Create tile slots
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    GameObject slotObj = Instantiate(tileSlotPrefab, tileContainer);
                    tileSlots[x, y] = slotObj;
                    
                    // Calculate position (center the board)
                    float offsetX = (x - boardSize * 0.5f + 0.5f) * 50f; // 50 units per tile
                    float offsetY = (y - boardSize * 0.5f + 0.5f) * 50f;
                    Vector2 position = new Vector2(offsetX, offsetY);
                    
                    slotObj.transform.localPosition = position;
                    tilePositions[x, y] = position;
                    
                    // Add click handler
                    Button slotButton = slotObj.GetComponent<Button>();
                    if (slotButton != null)
                    {
                        Vector2Int gridPos = new Vector2Int(x, y); // Capture for closure
                        slotButton.onClick.AddListener(() => OnTileSlotClick(gridPos));
                    }
                }
            }
        }
        
        /// <summary>
        /// Initializes the edge glow effects
        /// </summary>
        private void InitializeEdgeGlows()
        {
            for (int i = 0; i < edgeGlows.Length; i++)
            {
                if (edgeGlows[i] != null)
                {
                    edgeGlows[i].color = edgeColors[i];
                    edgeGlows[i].enabled = false;
                    edgeGlowActive[i] = false;
                }
            }
        }
        
        /// <summary>
        /// Called when a queue dot is clicked
        /// </summary>
        /// <param name="index">Index of the clicked dot</param>
        private void OnQueueDotClick(int index)
        {
            OnQueueDotClicked?.Invoke(index);
        }
        
        /// <summary>
        /// Called when a tile slot is clicked
        /// </summary>
        /// <param name="gridPos">Grid position of the clicked slot</param>
        private void OnTileSlotClick(Vector2Int gridPos)
        {
            OnTileSlotClicked?.Invoke(gridPos);
        }
        
        /// <summary>
        /// Called when the board changes
        /// </summary>
        private void OnBoardChanged()
        {
            // Update tile slot appearances based on board state
            UpdateTileSlots();
        }
        
        /// <summary>
        /// Updates the tile slots based on current board state
        /// </summary>
        private void UpdateTileSlots()
        {
            if (boardController == null || tileSlots == null) return;
            
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    if (tileSlots[x, y] != null)
                    {
                        // Update slot appearance based on board state
                        // This would be implemented based on the actual board data structure
                        UpdateTileSlotAppearance(x, y);
                    }
                }
            }
        }
        
        /// <summary>
        /// Updates the appearance of a specific tile slot
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        private void UpdateTileSlotAppearance(int x, int y)
        {
            // This would be implemented based on the actual tile data structure
            // For now, just ensure the slot is visible
            if (tileSlots[x, y] != null)
            {
                tileSlots[x, y].SetActive(true);
            }
        }
        
        /// <summary>
        /// Updates the queue display
        /// </summary>
        /// <param name="queueData">Queue data to display</param>
        public void UpdateQueue(List<object> queueData)
        {
            if (queueDots == null) return;
            
            for (int i = 0; i < queueDots.Count && i < queueData.Count; i++)
            {
                if (queueDots[i] != null)
                {
                    // Update dot appearance based on queue data
                    bool isActive = queueData[i] != null;
                    queueDots[i].color = isActive ? queueDotActive : queueDotInactive;
                }
            }
        }
        
        /// <summary>
        /// Sets the current queue index
        /// </summary>
        /// <param name="index">Current queue index</param>
        public void SetCurrentQueueIndex(int index)
        {
            currentQueueIndex = index;
            
            // Update queue dot appearances
            for (int i = 0; i < queueDots.Count; i++)
            {
                if (queueDots[i] != null)
                {
                    if (i == index)
                    {
                        queueDots[i].color = queueDotActive;
                    }
                    else
                    {
                        queueDots[i].color = queueDotInactive;
                    }
                }
            }
        }
        
        /// <summary>
        /// Activates an edge glow
        /// </summary>
        /// <param name="edgeIndex">Index of the edge (0=top, 1=right, 2=bottom, 3=left)</param>
        /// <param name="active">Whether to activate the glow</param>
        public void SetEdgeGlow(int edgeIndex, bool active)
        {
            if (edgeIndex >= 0 && edgeIndex < edgeGlows.Length && edgeGlows[edgeIndex] != null)
            {
                edgeGlows[edgeIndex].enabled = active;
                edgeGlowActive[edgeIndex] = active;
            }
        }
        
        /// <summary>
        /// Sets all edge glows
        /// </summary>
        /// <param name="top">Top edge glow</param>
        /// <param name="right">Right edge glow</param>
        /// <param name="bottom">Bottom edge glow</param>
        /// <param name="left">Left edge glow</param>
        public void SetAllEdgeGlows(bool top, bool right, bool bottom, bool left)
        {
            SetEdgeGlow(0, top);
            SetEdgeGlow(1, right);
            SetEdgeGlow(2, bottom);
            SetEdgeGlow(3, left);
        }
        
        /// <summary>
        /// Gets the world position of a tile slot
        /// </summary>
        /// <param name="gridPos">Grid position</param>
        /// <returns>World position</returns>
        public Vector3 GetTileSlotPosition(Vector2Int gridPos)
        {
            if (gridPos.x >= 0 && gridPos.x < boardSize && gridPos.y >= 0 && gridPos.y < boardSize)
            {
                return tileSlots[gridPos.x, gridPos.y].transform.position;
            }
            return Vector3.zero;
        }
        
        /// <summary>
        /// Gets the grid position from world position
        /// </summary>
        /// <param name="worldPos">World position</param>
        /// <returns>Grid position</returns>
        public Vector2Int GetGridPositionFromWorld(Vector3 worldPos)
        {
            // This would be implemented based on the actual positioning system
            // For now, return a default position
            return Vector2Int.zero;
        }
        
        #region Editor Debugging
        #if UNITY_EDITOR
        [Header("Editor Debug Controls")]
        [SerializeField] private bool showDebugControls = false;
        
        private void OnGUI()
        {
            if (!showDebugControls) return;
            
            GUILayout.BeginArea(new Rect(220, 10, 200, 200));
            GUILayout.Label("Game Area UI Debug");
            GUILayout.Label($"Queue Size: {queueDots.Count}");
            GUILayout.Label($"Current Queue Index: {currentQueueIndex}");
            GUILayout.Label($"Board Size: {boardSize}x{boardSize}");
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Test Queue Update"))
            {
                SetCurrentQueueIndex((currentQueueIndex + 1) % queueSize);
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Toggle Top Edge"))
                SetEdgeGlow(0, !edgeGlowActive[0]);
            if (GUILayout.Button("Toggle Right Edge"))
                SetEdgeGlow(1, !edgeGlowActive[1]);
            if (GUILayout.Button("Toggle Bottom Edge"))
                SetEdgeGlow(2, !edgeGlowActive[2]);
            if (GUILayout.Button("Toggle Left Edge"))
                SetEdgeGlow(3, !edgeGlowActive[3]);
            
            GUILayout.EndArea();
        }
        #endif
        #endregion
    }
}
