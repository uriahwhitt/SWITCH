/******************************************************************************
 * SWITCH - AntiFrustrationSystem
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Anti-frustration system that ensures players always have valid moves
 * Dependencies: BoardController, MatchDetector, SmartTileDistribution
 * 
 * Educational Notes:
 * - Demonstrates player experience optimization
 * - Shows how to implement guaranteed move systems
 * - Performance: Efficient move validation with minimal overhead
 *****************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SWITCH.Data;

namespace SWITCH.Core
{
    /// <summary>
    /// Anti-frustration system that ensures players always have valid moves.
    /// Educational: This demonstrates player experience optimization.
    /// Performance: Efficient move validation with minimal overhead.
    /// </summary>
    public class AntiFrustrationSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private bool enableAntiFrustration = true;
        [SerializeField] private float frustrationThreshold = 0.2f;
        
        [Header("Guaranteed Move Settings")]
        [SerializeField] private int minGuaranteedMoves = 3;
        [SerializeField] private int maxGuaranteedMoves = 8;
        [SerializeField] private float guaranteedMoveProbability = 0.8f;
        [SerializeField] private int maxConsecutiveNoMoves = 2;
        
        [Header("Move Quality Settings")]
        [SerializeField] private float goodMoveThreshold = 0.6f;
        [SerializeField] private float excellentMoveThreshold = 0.8f;
        [SerializeField] private int minMoveQuality = 3;
        [SerializeField] private int maxMoveQuality = 10;
        
        [Header("References")]
        [SerializeField] private BoardController boardController;
        [SerializeField] private MatchDetector matchDetector;
        [SerializeField] private SmartTileDistribution smartDistribution;
        
        // System state
        private List<Vector2Int> availableMoves = new List<Vector2Int>();
        private List<MoveQuality> moveQualities = new List<MoveQuality>();
        private int consecutiveNoMoves = 0;
        private float currentFrustrationLevel = 0f;
        private bool isAnalyzing = false;
        
        // Performance tracking
        private int totalMovesAnalyzed = 0;
        private int guaranteedMovesGenerated = 0;
        private int frustrationEventsPrevented = 0;
        
        // Events
        public System.Action<List<Vector2Int>> OnAvailableMovesUpdated;
        public System.Action<Vector2Int> OnGuaranteedMoveGenerated;
        public System.Action<float> OnFrustrationLevelChanged;
        public System.Action OnFrustrationEventPrevented;
        
        // Properties
        public List<Vector2Int> AvailableMoves => new List<Vector2Int>(availableMoves);
        public int AvailableMoveCount => availableMoves.Count;
        public float CurrentFrustrationLevel => currentFrustrationLevel;
        public bool HasGuaranteedMoves => availableMoves.Count >= minGuaranteedMoves;
        
        private void Awake()
        {
            // Get references if not assigned
            if (boardController == null)
                boardController = FindObjectOfType<BoardController>();
            if (matchDetector == null)
                matchDetector = FindObjectOfType<MatchDetector>();
            if (smartDistribution == null)
                smartDistribution = FindObjectOfType<SmartTileDistribution>();
        }
        
        private void Start()
        {
            InitializeAntiFrustrationSystem();
        }
        
        /// <summary>
        /// Initializes the anti-frustration system.
        /// Educational: Shows how to set up player experience systems.
        /// </summary>
        private void InitializeAntiFrustrationSystem()
        {
            Log("Initializing Anti-Frustration System");
            
            // Reset system state
            ResetSystemState();
            
            // Perform initial analysis
            AnalyzeAvailableMoves();
            
            Log("Anti-Frustration System initialized");
        }
        
        /// <summary>
        /// Resets the system state.
        /// Educational: Shows how to reset tracking systems.
        /// </summary>
        private void ResetSystemState()
        {
            availableMoves.Clear();
            moveQualities.Clear();
            consecutiveNoMoves = 0;
            currentFrustrationLevel = 0f;
            totalMovesAnalyzed = 0;
            guaranteedMovesGenerated = 0;
            frustrationEventsPrevented = 0;
        }
        
        /// <summary>
        /// Analyzes available moves on the current board.
        /// Educational: Shows how to implement move analysis systems.
        /// </summary>
        public void AnalyzeAvailableMoves()
        {
            if (isAnalyzing)
                return;
            
            isAnalyzing = true;
            
            try
            {
                // Clear previous analysis
                availableMoves.Clear();
                moveQualities.Clear();
                
                // Analyze board for available moves
                AnalyzeBoardForMoves();
                
                // Update frustration level
                UpdateFrustrationLevel();
                
                // Generate guaranteed moves if needed
                if (availableMoves.Count < minGuaranteedMoves)
                {
                    GenerateGuaranteedMoves();
                }
                
                // Fire events
                OnAvailableMovesUpdated?.Invoke(new List<Vector2Int>(availableMoves));
                
                totalMovesAnalyzed++;
                
                Log($"Move analysis complete: {availableMoves.Count} moves available, frustration: {currentFrustrationLevel:F2}");
            }
            finally
            {
                isAnalyzing = false;
            }
        }
        
        /// <summary>
        /// Analyzes the board for available moves.
        /// Educational: Shows how to implement board analysis.
        /// </summary>
        private void AnalyzeBoardForMoves()
        {
            // TODO: Implement actual board analysis
            // This would analyze the current board state and find all possible moves
            
            // For now, generate some placeholder moves
            GeneratePlaceholderMoves();
        }
        
        /// <summary>
        /// Generates placeholder moves for testing.
        /// Educational: Shows how to create test data.
        /// </summary>
        private void GeneratePlaceholderMoves()
        {
            // Generate random moves for testing
            int moveCount = UnityEngine.Random.Range(1, 10);
            
            for (int i = 0; i < moveCount; i++)
            {
                Vector2Int move = new Vector2Int(
                    UnityEngine.Random.Range(0, 8),
                    UnityEngine.Random.Range(0, 8)
                );
                
                if (!availableMoves.Contains(move))
                {
                    availableMoves.Add(move);
                    
                    // Generate move quality
                    MoveQuality quality = GenerateMoveQuality(move);
                    moveQualities.Add(quality);
                }
            }
        }
        
        /// <summary>
        /// Generates move quality for a specific move.
        /// Educational: Shows how to evaluate move quality.
        /// </summary>
        /// <param name="move">Move to evaluate</param>
        /// <returns>Move quality data</returns>
        private MoveQuality GenerateMoveQuality(Vector2Int move)
        {
            // TODO: Implement actual move quality evaluation
            // This would analyze the move and determine its quality
            
            float quality = UnityEngine.Random.Range(0f, 1f);
            int score = UnityEngine.Random.Range(minMoveQuality, maxMoveQuality + 1);
            
            return new MoveQuality
            {
                position = move,
                quality = quality,
                score = score,
                isGuaranteed = quality > guaranteedMoveProbability
            };
        }
        
        /// <summary>
        /// Updates the frustration level based on available moves.
        /// Educational: Shows how to implement frustration tracking.
        /// </summary>
        private void UpdateFrustrationLevel()
        {
            float previousFrustration = currentFrustrationLevel;
            
            if (availableMoves.Count < minGuaranteedMoves)
            {
                currentFrustrationLevel = Mathf.Min(1f, currentFrustrationLevel + 0.3f);
                consecutiveNoMoves++;
            }
            else if (availableMoves.Count > maxGuaranteedMoves)
            {
                currentFrustrationLevel = Mathf.Max(0f, currentFrustrationLevel - 0.1f);
                consecutiveNoMoves = 0;
            }
            else
            {
                consecutiveNoMoves = 0;
            }
            
            // Check for frustration events
            if (consecutiveNoMoves >= maxConsecutiveNoMoves)
            {
                PreventFrustrationEvent();
            }
            
            if (Mathf.Abs(currentFrustrationLevel - previousFrustration) > 0.01f)
            {
                OnFrustrationLevelChanged?.Invoke(currentFrustrationLevel);
            }
        }
        
        /// <summary>
        /// Prevents a frustration event by generating guaranteed moves.
        /// Educational: Shows how to implement frustration prevention.
        /// </summary>
        private void PreventFrustrationEvent()
        {
            Log("Preventing frustration event - generating guaranteed moves");
            
            // Generate guaranteed moves
            GenerateGuaranteedMoves();
            
            // Reset consecutive no moves counter
            consecutiveNoMoves = 0;
            
            // Update frustration level
            currentFrustrationLevel = Mathf.Max(0f, currentFrustrationLevel - 0.5f);
            
            // Fire events
            OnFrustrationEventPrevented?.Invoke();
            OnFrustrationLevelChanged?.Invoke(currentFrustrationLevel);
            
            frustrationEventsPrevented++;
        }
        
        /// <summary>
        /// Generates guaranteed moves to ensure player has options.
        /// Educational: Shows how to implement guaranteed move generation.
        /// </summary>
        private void GenerateGuaranteedMoves()
        {
            int movesNeeded = minGuaranteedMoves - availableMoves.Count;
            
            if (movesNeeded <= 0)
                return;
            
            Log($"Generating {movesNeeded} guaranteed moves");
            
            for (int i = 0; i < movesNeeded; i++)
            {
                Vector2Int guaranteedMove = GenerateGuaranteedMove();
                
                if (!availableMoves.Contains(guaranteedMove))
                {
                    availableMoves.Add(guaranteedMove);
                    
                    // Create high-quality move
                    MoveQuality quality = new MoveQuality
                    {
                        position = guaranteedMove,
                        quality = excellentMoveThreshold,
                        score = maxMoveQuality,
                        isGuaranteed = true
                    };
                    moveQualities.Add(quality);
                    
                    // Fire event
                    OnGuaranteedMoveGenerated?.Invoke(guaranteedMove);
                    
                    guaranteedMovesGenerated++;
                }
            }
        }
        
        /// <summary>
        /// Generates a single guaranteed move.
        /// Educational: Shows how to generate guaranteed moves.
        /// </summary>
        /// <returns>Guaranteed move position</returns>
        private Vector2Int GenerateGuaranteedMove()
        {
            // TODO: Implement actual guaranteed move generation
            // This would analyze the board and find a position that will definitely create a match
            
            // For now, generate a random position
            return new Vector2Int(
                UnityEngine.Random.Range(0, 8),
                UnityEngine.Random.Range(0, 8)
            );
        }
        
        /// <summary>
        /// Gets the best available move.
        /// Educational: Shows how to select optimal moves.
        /// </summary>
        /// <returns>Best move position or Vector2Int.zero if none available</returns>
        public Vector2Int GetBestMove()
        {
            if (availableMoves.Count == 0)
                return Vector2Int.zero;
            
            // Find the move with the highest quality
            MoveQuality bestMove = moveQualities.OrderByDescending(m => m.quality).First();
            
            return bestMove.position;
        }
        
        /// <summary>
        /// Gets all moves above a certain quality threshold.
        /// Educational: Shows how to filter moves by quality.
        /// </summary>
        /// <param name="threshold">Quality threshold</param>
        /// <returns>List of high-quality moves</returns>
        public List<Vector2Int> GetHighQualityMoves(float threshold = 0.7f)
        {
            return moveQualities
                .Where(m => m.quality >= threshold)
                .Select(m => m.position)
                .ToList();
        }
        
        /// <summary>
        /// Gets all guaranteed moves.
        /// Educational: Shows how to filter guaranteed moves.
        /// </summary>
        /// <returns>List of guaranteed moves</returns>
        public List<Vector2Int> GetGuaranteedMoves()
        {
            return moveQualities
                .Where(m => m.isGuaranteed)
                .Select(m => m.position)
                .ToList();
        }
        
        /// <summary>
        /// Checks if a specific move is available.
        /// Educational: Shows how to validate move availability.
        /// </summary>
        /// <param name="move">Move to check</param>
        /// <returns>True if move is available</returns>
        public bool IsMoveAvailable(Vector2Int move)
        {
            return availableMoves.Contains(move);
        }
        
        /// <summary>
        /// Gets the quality of a specific move.
        /// Educational: Shows how to query move quality.
        /// </summary>
        /// <param name="move">Move to check</param>
        /// <returns>Move quality or 0 if not available</returns>
        public float GetMoveQuality(Vector2Int move)
        {
            var moveQuality = moveQualities.FirstOrDefault(m => m.position == move);
            return moveQuality.quality;
        }
        
        /// <summary>
        /// Forces a move to be available (for testing or special cases).
        /// Educational: Shows how to manipulate move availability.
        /// </summary>
        /// <param name="move">Move to force available</param>
        /// <param name="quality">Quality of the forced move</param>
        public void ForceMoveAvailable(Vector2Int move, float quality = 1f)
        {
            if (!availableMoves.Contains(move))
            {
                availableMoves.Add(move);
                
                MoveQuality moveQuality = new MoveQuality
                {
                    position = move,
                    quality = quality,
                    score = Mathf.RoundToInt(quality * maxMoveQuality),
                    isGuaranteed = true
                };
                moveQualities.Add(moveQuality);
                
                Log($"Forced move available: {move} with quality {quality}");
            }
        }
        
        /// <summary>
        /// Gets system statistics.
        /// Educational: Shows how to provide system analytics.
        /// </summary>
        /// <returns>System statistics</returns>
        public AntiFrustrationStats GetSystemStats()
        {
            return new AntiFrustrationStats
            {
                TotalMovesAnalyzed = totalMovesAnalyzed,
                GuaranteedMovesGenerated = guaranteedMovesGenerated,
                FrustrationEventsPrevented = frustrationEventsPrevented,
                CurrentFrustrationLevel = currentFrustrationLevel,
                AvailableMoveCount = availableMoves.Count,
                ConsecutiveNoMoves = consecutiveNoMoves
            };
        }
        
        /// <summary>
        /// Resets the system for a new game.
        /// Educational: Shows how to reset systems for new games.
        /// </summary>
        public void ResetForNewGame()
        {
            ResetSystemState();
            AnalyzeAvailableMoves();
            Log("System reset for new game");
        }
        
        /// <summary>
        /// Logs a message if debug logging is enabled.
        /// Educational: Shows how to implement conditional logging.
        /// </summary>
        /// <param name="message">Message to log</param>
        private void Log(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[AntiFrustrationSystem] {message}");
        }
    }
    
    /// <summary>
    /// Move quality data structure.
    /// Educational: Shows how to define move quality data.
    /// </summary>
    [System.Serializable]
    public struct MoveQuality
    {
        public Vector2Int position;
        public float quality;
        public int score;
        public bool isGuaranteed;
    }
    
    /// <summary>
    /// Anti-frustration system statistics.
    /// Educational: Shows how to create analytics data structures.
    /// </summary>
    [System.Serializable]
    public struct AntiFrustrationStats
    {
        public int TotalMovesAnalyzed;
        public int GuaranteedMovesGenerated;
        public int FrustrationEventsPrevented;
        public float CurrentFrustrationLevel;
        public int AvailableMoveCount;
        public int ConsecutiveNoMoves;
    }
}
