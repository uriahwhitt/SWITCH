using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SWITCH.Core
{
    /// <summary>
    /// Calculates complete turn scores including base points, pattern bonuses,
    /// momentum multipliers, and power orb effects.
    /// </summary>
    public class TurnScoreCalculator : MonoBehaviour
    {
        [Header("Base Scoring Configuration")]
        [SerializeField] private int tileBaseValue = 10;
        [SerializeField] private int edgeMultiplier = 1;
        [SerializeField] private int transitionMultiplier = 2;
        [SerializeField] private int centerMultiplier = 3;
        
        [Header("Pattern Bonuses")]
        [SerializeField] private int lShapeBonus = 50;
        [SerializeField] private int crossBonus = 100;
        
        [Header("Power Orb Configuration")]
        [SerializeField] private int powerOrbBasePoints = 5000;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        
        // Reference to momentum system
        private MomentumSystem momentumSystem;
        
        private void Awake()
        {
            momentumSystem = GetComponent<MomentumSystem>();
            if (momentumSystem == null)
            {
                momentumSystem = FindObjectOfType<MomentumSystem>();
            }
        }
        
        /// <summary>
        /// Calculates the complete score for a turn
        /// </summary>
        /// <param name="result">Turn result containing all match information</param>
        /// <returns>Final calculated score for the turn</returns>
        public TurnScoreResult CalculateTurnScore(TurnResult result)
        {
            var scoreResult = new TurnScoreResult();
            
            // 1. Calculate base points from tiles
            scoreResult.BaseScore = CalculateBaseScore(result.ClearedTiles);
            
            // 2. Add pattern bonuses
            scoreResult.PatternBonus = CalculatePatternBonus(result);
            
            // 3. Calculate heat gain from this turn
            float heatGained = CalculateHeatGain(result);
            
            // 4. Apply power orb effects if collected
            if (result.PowerOrbCollected)
            {
                momentumSystem.TriggerPowerOrbBoost();
                scoreResult.PowerOrbScore = powerOrbBasePoints;
            }
            else
            {
                // Apply normal heat gain
                ApplyHeatGain(heatGained, result);
            }
            
            // 5. Calculate total before multiplier
            scoreResult.TotalBeforeMultiplier = scoreResult.BaseScore + 
                                              scoreResult.PatternBonus + 
                                              scoreResult.PowerOrbScore;
            
            // 6. Apply momentum multiplier
            float multiplier = momentumSystem.GetScoreMultiplier();
            scoreResult.Multiplier = multiplier;
            scoreResult.FinalScore = Mathf.RoundToInt(scoreResult.TotalBeforeMultiplier * multiplier);
            
            // 7. Apply turn-end decay
            momentumSystem.ApplyTurnEndDecay();
            
            // 8. Set heat information
            scoreResult.HeatGained = heatGained;
            scoreResult.FinalHeat = momentumSystem.CurrentMomentum;
            scoreResult.HeatLevel = momentumSystem.CurrentHeatLevel;
            
            LogDebug($"Turn Score: {scoreResult.FinalScore} (Base: {scoreResult.BaseScore}, " +
                    $"Pattern: {scoreResult.PatternBonus}, Orb: {scoreResult.PowerOrbScore}, " +
                    $"Multiplier: {multiplier:F1}x, Heat: {scoreResult.FinalHeat:F1})");
            
            return scoreResult;
        }
        
        /// <summary>
        /// Calculates base score from cleared tiles
        /// </summary>
        /// <param name="clearedTiles">List of tiles that were cleared</param>
        /// <returns>Base score from tile values and positions</returns>
        private int CalculateBaseScore(List<Tile> clearedTiles)
        {
            int baseScore = 0;
            
            foreach (var tile in clearedTiles)
            {
                int positionMultiplier = GetPositionMultiplier(tile.Position);
                int tilePoints = tileBaseValue * positionMultiplier;
                baseScore += tilePoints;
            }
            
            return baseScore;
        }
        
        /// <summary>
        /// Calculates pattern bonus points
        /// </summary>
        /// <param name="result">Turn result containing pattern information</param>
        /// <returns>Total pattern bonus points</returns>
        private int CalculatePatternBonus(TurnResult result)
        {
            int bonus = 0;
            
            if (result.HasLShape)
            {
                bonus += lShapeBonus;
            }
            
            if (result.HasCross)
            {
                bonus += crossBonus;
            }
            
            return bonus;
        }
        
        /// <summary>
        /// Calculates total heat gained from this turn
        /// </summary>
        /// <param name="result">Turn result containing match information</param>
        /// <returns>Total heat gained</returns>
        private float CalculateHeatGain(TurnResult result)
        {
            float heatGained = 0f;
            
            // Heat from match sizes
            foreach (int matchSize in result.MatchSizes)
            {
                heatGained += GetMatchHeat(matchSize);
            }
            
            // Heat from cascades
            if (result.CascadeLevel > 0)
            {
                heatGained += momentumSystem.AddCascadeHeat(result.CascadeLevel);
            }
            
            // Heat from patterns
            if (result.HasLShape)
            {
                heatGained += momentumSystem.AddPatternHeat(PatternType.LShape);
            }
            
            if (result.HasCross)
            {
                heatGained += momentumSystem.AddPatternHeat(PatternType.Cross);
            }
            
            return heatGained;
        }
        
        /// <summary>
        /// Applies heat gain to momentum system
        /// </summary>
        /// <param name="heatGained">Amount of heat gained</param>
        /// <param name="result">Turn result for additional context</param>
        private void ApplyHeatGain(float heatGained, TurnResult result)
        {
            // Apply heat from each match
            foreach (int matchSize in result.MatchSizes)
            {
                momentumSystem.AddMatchHeat(matchSize);
            }
        }
        
        /// <summary>
        /// Gets position multiplier based on tile position
        /// </summary>
        /// <param name="position">Tile position on board</param>
        /// <returns>Position multiplier value</returns>
        private int GetPositionMultiplier(Vector2Int position)
        {
            int x = position.x;
            int y = position.y;
            
            // Edge positions (columns/rows 0,7)
            if (x == 0 || x == 7 || y == 0 || y == 7)
            {
                return edgeMultiplier;
            }
            
            // Center positions (3,3), (4,3), (3,4), (4,4)
            if ((x == 3 || x == 4) && (y == 3 || y == 4))
            {
                return centerMultiplier;
            }
            
            // Transition positions (middle ring)
            return transitionMultiplier;
        }
        
        /// <summary>
        /// Gets heat value for a specific match size
        /// </summary>
        /// <param name="matchSize">Size of the match</param>
        /// <returns>Heat value for this match size</returns>
        private float GetMatchHeat(int matchSize)
        {
            switch (matchSize)
            {
                case 3: return 0f;      // No heat - just maintains
                case 4: return 1.0f;    // Good heat boost
                case 5:
                default: return 2.0f;   // Excellent heat boost
            }
        }
        
        /// <summary>
        /// Debug logging helper
        /// </summary>
        /// <param name="message">Message to log</param>
        private void LogDebug(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[TurnScoreCalculator] {message}");
            }
        }
        
        #region Editor Debugging
        #if UNITY_EDITOR
        [Header("Editor Debug Controls")]
        [SerializeField] private bool showDebugControls = false;
        
        private void OnGUI()
        {
            if (!showDebugControls) return;
            
            GUILayout.BeginArea(new Rect(320, 10, 300, 200));
            GUILayout.Label("Turn Score Calculator Debug");
            GUILayout.Label($"Tile Base Value: {tileBaseValue}");
            GUILayout.Label($"L-Shape Bonus: {lShapeBonus}");
            GUILayout.Label($"Cross Bonus: {crossBonus}");
            GUILayout.Label($"Power Orb Points: {powerOrbBasePoints}");
            
            GUILayout.Space(10);
            GUILayout.Label("Position Multipliers:");
            GUILayout.Label($"Edge: {edgeMultiplier}x");
            GUILayout.Label($"Transition: {transitionMultiplier}x");
            GUILayout.Label($"Center: {centerMultiplier}x");
            
            GUILayout.EndArea();
        }
        #endif
        #endregion
    }
    
    /// <summary>
    /// Result of a turn score calculation
    /// </summary>
    [System.Serializable]
    public class TurnScoreResult
    {
        public int BaseScore;
        public int PatternBonus;
        public int PowerOrbScore;
        public int TotalBeforeMultiplier;
        public float Multiplier;
        public int FinalScore;
        public float HeatGained;
        public float FinalHeat;
        public MomentumSystem.HeatLevel HeatLevel;
    }
    
    /// <summary>
    /// Contains all information about a turn's results
    /// </summary>
    [System.Serializable]
    public class TurnResult
    {
        public List<Tile> ClearedTiles = new List<Tile>();
        public List<int> MatchSizes = new List<int>();
        public int CascadeLevel;
        public bool HasLShape;
        public bool HasCross;
        public bool PowerOrbCollected;
        public PowerOrb CollectedPowerOrb;
    }
}
