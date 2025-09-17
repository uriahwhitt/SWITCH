/******************************************************************************
 * SWITCH - ShapeAccessibilitySystem
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Shape accessibility system for special patterns and accessibility
 * Dependencies: BoardController, MatchDetector, Tile
 * 
 * Educational Notes:
 * - Demonstrates accessibility and pattern recognition systems
 * - Shows how to implement special shape detection
 * - Performance: Efficient pattern matching with minimal overhead
 *****************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SWITCH.Data;

namespace SWITCH.Core
{
    /// <summary>
    /// Shape accessibility system for special patterns and accessibility.
    /// Educational: This demonstrates accessibility and pattern recognition systems.
    /// Performance: Efficient pattern matching with minimal overhead.
    /// </summary>
    public class ShapeAccessibilitySystem : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private bool enableShapeDetection = true;
        [SerializeField] private bool enableAccessibilityFeatures = true;
        
        [Header("Shape Detection Settings")]
        [SerializeField] private float shapeDetectionThreshold = 0.8f;
        [SerializeField] private int minShapeSize = 3;
        [SerializeField] private int maxShapeSize = 8;
        [SerializeField] private bool enableLShapeDetection = true;
        [SerializeField] private bool enableTShapeDetection = true;
        [SerializeField] private bool enableCrossShapeDetection = true;
        
        [Header("Accessibility Settings")]
        [SerializeField] private bool enableColorBlindSupport = true;
        [SerializeField] private bool enableHighContrastMode = false;
        [SerializeField] private bool enablePatternHints = true;
        [SerializeField] private float hintDisplayDuration = 3f;
        
        [Header("References")]
        [SerializeField] private BoardController boardController;
        [SerializeField] private MatchDetector matchDetector;
        [SerializeField] private GameManager gameManager;
        
        // Shape detection state
        private List<DetectedShape> detectedShapes = new List<DetectedShape>();
        private List<ShapePattern> availablePatterns = new List<ShapePattern>();
        private Dictionary<Vector2Int, List<DetectedShape>> shapeMap = new Dictionary<Vector2Int, List<DetectedShape>>();
        
        // Accessibility state
        private bool isColorBlindMode = false;
        private bool isHighContrastMode = false;
        private List<Vector2Int> highlightedPositions = new List<Vector2Int>();
        private float lastHintTime = 0f;
        
        // Performance tracking
        private int totalShapesDetected = 0;
        private int totalPatternsMatched = 0;
        private int totalHintsShown = 0;
        
        // Events
        public System.Action<DetectedShape> OnShapeDetected;
        public System.Action<DetectedShape> OnShapeCompleted;
        public System.Action<List<Vector2Int>> OnPatternHintShown;
        public System.Action<bool> OnColorBlindModeChanged;
        public System.Action<bool> OnHighContrastModeChanged;
        
        // Properties
        public List<DetectedShape> DetectedShapes => new List<DetectedShape>(detectedShapes);
        public bool IsColorBlindMode => isColorBlindMode;
        public bool IsHighContrastMode => isHighContrastMode;
        public int TotalShapesDetected => totalShapesDetected;
        public int TotalPatternsMatched => totalPatternsMatched;
        
        private void Awake()
        {
            // Get references if not assigned
            if (boardController == null)
                boardController = FindObjectOfType<BoardController>();
            if (matchDetector == null)
                matchDetector = FindObjectOfType<MatchDetector>();
            if (gameManager == null)
                gameManager = FindObjectOfType<GameManager>();
        }
        
        private void Start()
        {
            InitializeShapeAccessibilitySystem();
        }
        
        /// <summary>
        /// Initializes the shape accessibility system.
        /// Educational: Shows how to set up accessibility systems.
        /// </summary>
        private void InitializeShapeAccessibilitySystem()
        {
            Log("Initializing Shape Accessibility System");
            
            // Initialize shape patterns
            InitializeShapePatterns();
            
            // Reset system state
            ResetSystemState();
            
            Log("Shape Accessibility System initialized");
        }
        
        /// <summary>
        /// Initializes available shape patterns.
        /// Educational: Shows how to define shape patterns.
        /// </summary>
        private void InitializeShapePatterns()
        {
            availablePatterns.Clear();
            
            // L-Shape pattern
            if (enableLShapeDetection)
            {
                availablePatterns.Add(new ShapePattern
                {
                    patternId = "l_shape",
                    patternName = "L-Shape",
                    patternType = ShapeType.LShape,
                    positions = new List<Vector2Int>
                    {
                        new Vector2Int(0, 0),
                        new Vector2Int(1, 0),
                        new Vector2Int(2, 0),
                        new Vector2Int(0, 1),
                        new Vector2Int(0, 2)
                    },
                    scoreMultiplier = 2f,
                    isAccessible = true
                });
            }
            
            // T-Shape pattern
            if (enableTShapeDetection)
            {
                availablePatterns.Add(new ShapePattern
                {
                    patternId = "t_shape",
                    patternName = "T-Shape",
                    patternType = ShapeType.TShape,
                    positions = new List<Vector2Int>
                    {
                        new Vector2Int(0, 0),
                        new Vector2Int(1, 0),
                        new Vector2Int(2, 0),
                        new Vector2Int(1, 1),
                        new Vector2Int(1, 2)
                    },
                    scoreMultiplier = 2.5f,
                    isAccessible = true
                });
            }
            
            // Cross pattern
            if (enableCrossShapeDetection)
            {
                availablePatterns.Add(new ShapePattern
                {
                    patternId = "cross_shape",
                    patternName = "Cross",
                    patternType = ShapeType.Cross,
                    positions = new List<Vector2Int>
                    {
                        new Vector2Int(1, 0),
                        new Vector2Int(0, 1),
                        new Vector2Int(1, 1),
                        new Vector2Int(2, 1),
                        new Vector2Int(1, 2)
                    },
                    scoreMultiplier = 3f,
                    isAccessible = true
                });
            }
            
            Log($"Initialized {availablePatterns.Count} shape patterns");
        }
        
        /// <summary>
        /// Resets the system state.
        /// Educational: Shows how to reset tracking systems.
        /// </summary>
        private void ResetSystemState()
        {
            detectedShapes.Clear();
            shapeMap.Clear();
            highlightedPositions.Clear();
            totalShapesDetected = 0;
            totalPatternsMatched = 0;
            totalHintsShown = 0;
        }
        
        /// <summary>
        /// Analyzes the board for shape patterns.
        /// Educational: Shows how to implement pattern recognition.
        /// </summary>
        public void AnalyzeBoardForShapes()
        {
            if (!enableShapeDetection)
                return;
            
            Log("Analyzing board for shape patterns");
            
            // Clear previous detections
            detectedShapes.Clear();
            shapeMap.Clear();
            
            // Analyze each pattern
            foreach (var pattern in availablePatterns)
            {
                AnalyzePattern(pattern);
            }
            
            // Update shape map
            UpdateShapeMap();
            
            Log($"Shape analysis complete: {detectedShapes.Count} shapes detected");
        }
        
        /// <summary>
        /// Analyzes a specific pattern on the board.
        /// Educational: Shows how to implement pattern matching.
        /// </summary>
        /// <param name="pattern">Pattern to analyze</param>
        private void AnalyzePattern(ShapePattern pattern)
        {
            // TODO: Implement actual pattern analysis
            // This would scan the board for the pattern and detect matches
            
            // For now, generate some placeholder detections
            int detectionCount = UnityEngine.Random.Range(0, 3);
            
            for (int i = 0; i < detectionCount; i++)
            {
                DetectedShape shape = new DetectedShape
                {
                    shapeId = System.Guid.NewGuid().ToString(),
                    pattern = pattern,
                    centerPosition = new Vector2Int(
                        UnityEngine.Random.Range(0, 8),
                        UnityEngine.Random.Range(0, 8)
                    ),
                    confidence = UnityEngine.Random.Range(0.7f, 1f),
                    isCompleted = false,
                    detectionTime = Time.time
                };
                
                detectedShapes.Add(shape);
                totalShapesDetected++;
                
                OnShapeDetected?.Invoke(shape);
            }
        }
        
        /// <summary>
        /// Updates the shape map for quick lookups.
        /// Educational: Shows how to create efficient lookup structures.
        /// </summary>
        private void UpdateShapeMap()
        {
            shapeMap.Clear();
            
            foreach (var shape in detectedShapes)
            {
                foreach (var position in shape.pattern.positions)
                {
                    Vector2Int worldPosition = shape.centerPosition + position;
                    
                    if (!shapeMap.ContainsKey(worldPosition))
                        shapeMap[worldPosition] = new List<DetectedShape>();
                    
                    shapeMap[worldPosition].Add(shape);
                }
            }
        }
        
        /// <summary>
        /// Gets shapes at a specific position.
        /// Educational: Shows how to query shape data.
        /// </summary>
        /// <param name="position">Position to check</param>
        /// <returns>List of shapes at the position</returns>
        public List<DetectedShape> GetShapesAtPosition(Vector2Int position)
        {
            return shapeMap.TryGetValue(position, out var shapes) ? new List<DetectedShape>(shapes) : new List<DetectedShape>();
        }
        
        /// <summary>
        /// Checks if a position is part of a detected shape.
        /// Educational: Shows how to check shape membership.
        /// </summary>
        /// <param name="position">Position to check</param>
        /// <returns>True if position is part of a shape</returns>
        public bool IsPositionInShape(Vector2Int position)
        {
            return shapeMap.ContainsKey(position);
        }
        
        /// <summary>
        /// Shows pattern hints to help players.
        /// Educational: Shows how to implement accessibility hints.
        /// </summary>
        public void ShowPatternHints()
        {
            if (!enablePatternHints || !enableAccessibilityFeatures)
                return;
            
            if (Time.time - lastHintTime < hintDisplayDuration)
                return;
            
            List<Vector2Int> hintPositions = new List<Vector2Int>();
            
            // Find incomplete shapes to hint
            foreach (var shape in detectedShapes)
            {
                if (!shape.isCompleted && shape.confidence > shapeDetectionThreshold)
                {
                    hintPositions.AddRange(shape.pattern.positions.Select(pos => shape.centerPosition + pos));
                }
            }
            
            if (hintPositions.Count > 0)
            {
                highlightedPositions = hintPositions;
                OnPatternHintShown?.Invoke(new List<Vector2Int>(hintPositions));
                totalHintsShown++;
                lastHintTime = Time.time;
                
                Log($"Showed pattern hints for {hintPositions.Count} positions");
            }
        }
        
        /// <summary>
        /// Toggles color blind mode.
        /// Educational: Shows how to implement accessibility features.
        /// </summary>
        public void ToggleColorBlindMode()
        {
            if (!enableColorBlindSupport)
                return;
            
            isColorBlindMode = !isColorBlindMode;
            OnColorBlindModeChanged?.Invoke(isColorBlindMode);
            
            Log($"Color blind mode {(isColorBlindMode ? "enabled" : "disabled")}");
        }
        
        /// <summary>
        /// Toggles high contrast mode.
        /// Educational: Shows how to implement accessibility features.
        /// </summary>
        public void ToggleHighContrastMode()
        {
            if (!enableHighContrastMode)
                return;
            
            isHighContrastMode = !isHighContrastMode;
            OnHighContrastModeChanged?.Invoke(isHighContrastMode);
            
            Log($"High contrast mode {(isHighContrastMode ? "enabled" : "disabled")}");
        }
        
        /// <summary>
        /// Completes a detected shape.
        /// Educational: Shows how to handle shape completion.
        /// </summary>
        /// <param name="shapeId">ID of shape to complete</param>
        public void CompleteShape(string shapeId)
        {
            var shape = detectedShapes.FirstOrDefault(s => s.shapeId == shapeId);
            if (shape.shapeId == null)
                return;
            
            shape.isCompleted = true;
            shape.completionTime = Time.time;
            
            // Update the shape in the list
            int index = detectedShapes.FindIndex(s => s.shapeId == shapeId);
            if (index >= 0)
            {
                detectedShapes[index] = shape;
            }
            
            totalPatternsMatched++;
            OnShapeCompleted?.Invoke(shape);
            
            Log($"Shape completed: {shape.pattern.patternName} at {shape.centerPosition}");
        }
        
        /// <summary>
        /// Gets all incomplete shapes.
        /// Educational: Shows how to filter shape data.
        /// </summary>
        /// <returns>List of incomplete shapes</returns>
        public List<DetectedShape> GetIncompleteShapes()
        {
            return detectedShapes.Where(s => !s.isCompleted).ToList();
        }
        
        /// <summary>
        /// Gets all completed shapes.
        /// Educational: Shows how to filter shape data.
        /// </summary>
        /// <returns>List of completed shapes</returns>
        public List<DetectedShape> GetCompletedShapes()
        {
            return detectedShapes.Where(s => s.isCompleted).ToList();
        }
        
        /// <summary>
        /// Gets shapes by type.
        /// Educational: Shows how to filter shapes by type.
        /// </summary>
        /// <param name="shapeType">Type of shapes to get</param>
        /// <returns>List of shapes of the specified type</returns>
        public List<DetectedShape> GetShapesByType(ShapeType shapeType)
        {
            return detectedShapes.Where(s => s.pattern.patternType == shapeType).ToList();
        }
        
        /// <summary>
        /// Gets system statistics.
        /// Educational: Shows how to provide system analytics.
        /// </summary>
        /// <returns>System statistics</returns>
        public ShapeAccessibilityStats GetSystemStats()
        {
            return new ShapeAccessibilityStats
            {
                TotalShapesDetected = totalShapesDetected,
                TotalPatternsMatched = totalPatternsMatched,
                TotalHintsShown = totalHintsShown,
                ActiveShapesCount = detectedShapes.Count,
                IncompleteShapesCount = GetIncompleteShapes().Count,
                CompletedShapesCount = GetCompletedShapes().Count,
                IsColorBlindMode = isColorBlindMode,
                IsHighContrastMode = isHighContrastMode
            };
        }
        
        /// <summary>
        /// Resets the system for a new game.
        /// Educational: Shows how to reset systems for new games.
        /// </summary>
        public void ResetForNewGame()
        {
            ResetSystemState();
            Log("Shape accessibility system reset for new game");
        }
        
        /// <summary>
        /// Logs a message if debug logging is enabled.
        /// Educational: Shows how to implement conditional logging.
        /// </summary>
        /// <param name="message">Message to log</param>
        private void Log(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[ShapeAccessibilitySystem] {message}");
        }
    }
    
    /// <summary>
    /// Shape types for pattern recognition.
    /// Educational: Shows how to define shape categories.
    /// </summary>
    public enum ShapeType
    {
        LShape,
        TShape,
        Cross,
        Square,
        Line,
        Custom
    }
    
    /// <summary>
    /// Shape pattern data structure.
    /// Educational: Shows how to define pattern data structures.
    /// </summary>
    [System.Serializable]
    public struct ShapePattern
    {
        public string patternId;
        public string patternName;
        public ShapeType patternType;
        public List<Vector2Int> positions;
        public float scoreMultiplier;
        public bool isAccessible;
    }
    
    /// <summary>
    /// Detected shape data structure.
    /// Educational: Shows how to define detected shape data.
    /// </summary>
    [System.Serializable]
    public struct DetectedShape
    {
        public string shapeId;
        public ShapePattern pattern;
        public Vector2Int centerPosition;
        public float confidence;
        public bool isCompleted;
        public float detectionTime;
        public float completionTime;
    }
    
    /// <summary>
    /// Shape accessibility system statistics.
    /// Educational: Shows how to create analytics data structures.
    /// </summary>
    [System.Serializable]
    public struct ShapeAccessibilityStats
    {
        public int TotalShapesDetected;
        public int TotalPatternsMatched;
        public int TotalHintsShown;
        public int ActiveShapesCount;
        public int IncompleteShapesCount;
        public int CompletedShapesCount;
        public bool IsColorBlindMode;
        public bool IsHighContrastMode;
    }
}
