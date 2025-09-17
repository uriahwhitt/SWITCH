using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using SWITCH.Core;
using SWITCH.PowerUps;

namespace SWITCH.Tests.Core
{
    /// <summary>
    /// Unit tests for the TurnScoreCalculator class.
    /// Tests score calculation, position multipliers, and pattern bonuses.
    /// </summary>
    [TestFixture]
    public class TurnScoreCalculatorTests
    {
        private GameObject testGameObject;
        private TurnScoreCalculator scoreCalculator;
        private MomentumSystem momentumSystem;
        private MockTile mockTile;
        
        [SetUp]
        public void SetUp()
        {
            testGameObject = new GameObject("TestTurnScoreCalculator");
            momentumSystem = testGameObject.AddComponent<MomentumSystem>();
            scoreCalculator = testGameObject.AddComponent<TurnScoreCalculator>();
            mockTile = new MockTile();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (testGameObject != null)
            {
                Object.DestroyImmediate(testGameObject);
            }
        }
        
        [Test]
        public void CalculateTurnScore_SimpleMatch3_ShouldCalculateCorrectly()
        {
            // Arrange
            var turnResult = new TurnResult
            {
                ClearedTiles = new List<Tile> { mockTile, mockTile, mockTile },
                MatchSizes = new List<int> { 3 },
                CascadeLevel = 0,
                HasLShape = false,
                HasCross = false,
                PowerOrbCollected = false
            };
            
            // Act
            var result = scoreCalculator.CalculateTurnScore(turnResult);
            
            // Assert
            Assert.AreEqual(30, result.BaseScore); // 3 tiles * 10 base * 1 edge multiplier
            Assert.AreEqual(0, result.PatternBonus);
            Assert.AreEqual(0, result.PowerOrbScore);
            Assert.AreEqual(30, result.TotalBeforeMultiplier);
            Assert.AreEqual(1f, result.Multiplier); // No heat = 1x multiplier
            Assert.AreEqual(30, result.FinalScore);
            Assert.AreEqual(0f, result.HeatGained); // Match-3 adds no heat
            Assert.AreEqual(0f, result.FinalHeat); // After decay
        }
        
        [Test]
        public void CalculateTurnScore_Match4WithHeat_ShouldCalculateCorrectly()
        {
            // Arrange
            var turnResult = new TurnResult
            {
                ClearedTiles = new List<Tile> { mockTile, mockTile, mockTile, mockTile },
                MatchSizes = new List<int> { 4 },
                CascadeLevel = 0,
                HasLShape = false,
                HasCross = false,
                PowerOrbCollected = false
            };
            
            // Act
            var result = scoreCalculator.CalculateTurnScore(turnResult);
            
            // Assert
            Assert.AreEqual(40, result.BaseScore); // 4 tiles * 10 base * 1 edge multiplier
            Assert.AreEqual(0, result.PatternBonus);
            Assert.AreEqual(0, result.PowerOrbScore);
            Assert.AreEqual(40, result.TotalBeforeMultiplier);
            Assert.AreEqual(1.9f, result.Multiplier, 0.01f); // 1 + (1 * 0.9) = 1.9x
            Assert.AreEqual(76, result.FinalScore); // 40 * 1.9 = 76
            Assert.AreEqual(1f, result.HeatGained); // Match-4 adds 1 heat
            Assert.AreEqual(0f, result.FinalHeat); // After decay (1 - 1 = 0)
        }
        
        [Test]
        public void CalculateTurnScore_CenterPosition_ShouldUseCenterMultiplier()
        {
            // Arrange
            var centerTile = new MockTile { Position = new Vector2Int(3, 3) };
            var turnResult = new TurnResult
            {
                ClearedTiles = new List<Tile> { centerTile, centerTile, centerTile },
                MatchSizes = new List<int> { 3 },
                CascadeLevel = 0,
                HasLShape = false,
                HasCross = false,
                PowerOrbCollected = false
            };
            
            // Act
            var result = scoreCalculator.CalculateTurnScore(turnResult);
            
            // Assert
            Assert.AreEqual(90, result.BaseScore); // 3 tiles * 10 base * 3 center multiplier
        }
        
        [Test]
        public void CalculateTurnScore_TransitionPosition_ShouldUseTransitionMultiplier()
        {
            // Arrange
            var transitionTile = new MockTile { Position = new Vector2Int(2, 2) };
            var turnResult = new TurnResult
            {
                ClearedTiles = new List<Tile> { transitionTile, transitionTile, transitionTile },
                MatchSizes = new List<int> { 3 },
                CascadeLevel = 0,
                HasLShape = false,
                HasCross = false,
                PowerOrbCollected = false
            };
            
            // Act
            var result = scoreCalculator.CalculateTurnScore(turnResult);
            
            // Assert
            Assert.AreEqual(60, result.BaseScore); // 3 tiles * 10 base * 2 transition multiplier
        }
        
        [Test]
        public void CalculateTurnScore_LShapePattern_ShouldAddBonus()
        {
            // Arrange
            var turnResult = new TurnResult
            {
                ClearedTiles = new List<Tile> { mockTile, mockTile, mockTile, mockTile, mockTile },
                MatchSizes = new List<int> { 5 },
                CascadeLevel = 0,
                HasLShape = true,
                HasCross = false,
                PowerOrbCollected = false
            };
            
            // Act
            var result = scoreCalculator.CalculateTurnScore(turnResult);
            
            // Assert
            Assert.AreEqual(50, result.PatternBonus); // L-shape bonus
            Assert.AreEqual(100, result.TotalBeforeMultiplier); // 50 base + 50 bonus
        }
        
        [Test]
        public void CalculateTurnScore_CrossPattern_ShouldAddBonus()
        {
            // Arrange
            var turnResult = new TurnResult
            {
                ClearedTiles = new List<Tile> { mockTile, mockTile, mockTile, mockTile, mockTile },
                MatchSizes = new List<int> { 5 },
                CascadeLevel = 0,
                HasLShape = false,
                HasCross = true,
                PowerOrbCollected = false
            };
            
            // Act
            var result = scoreCalculator.CalculateTurnScore(turnResult);
            
            // Assert
            Assert.AreEqual(100, result.PatternBonus); // Cross bonus
            Assert.AreEqual(150, result.TotalBeforeMultiplier); // 50 base + 100 bonus
        }
        
        [Test]
        public void CalculateTurnScore_Cascade_ShouldAddHeat()
        {
            // Arrange
            var turnResult = new TurnResult
            {
                ClearedTiles = new List<Tile> { mockTile, mockTile, mockTile },
                MatchSizes = new List<int> { 3 },
                CascadeLevel = 2,
                HasLShape = false,
                HasCross = false,
                PowerOrbCollected = false
            };
            
            // Act
            var result = scoreCalculator.CalculateTurnScore(turnResult);
            
            // Assert
            Assert.AreEqual(1f, result.HeatGained); // 0.5 * 2 cascade levels
            Assert.AreEqual(0f, result.FinalHeat); // After decay (1 - 1 = 0)
        }
        
        [Test]
        public void CalculateTurnScore_PowerOrbCollected_ShouldTriggerBoost()
        {
            // Arrange
            var powerOrb = new MockPowerOrb();
            var turnResult = new TurnResult
            {
                ClearedTiles = new List<Tile> { mockTile, mockTile, mockTile },
                MatchSizes = new List<int> { 3 },
                CascadeLevel = 0,
                HasLShape = false,
                HasCross = false,
                PowerOrbCollected = true,
                CollectedPowerOrb = powerOrb
            };
            
            // Act
            var result = scoreCalculator.CalculateTurnScore(turnResult);
            
            // Assert
            Assert.AreEqual(5000, result.PowerOrbScore);
            Assert.AreEqual(5030, result.TotalBeforeMultiplier); // 30 base + 5000 orb
            Assert.AreEqual(10f, result.Multiplier); // Max heat = 10x multiplier
            Assert.AreEqual(50300, result.FinalScore); // 5030 * 10
            Assert.AreEqual(10f, result.FinalHeat); // Max heat
        }
        
        [Test]
        public void CalculateTurnScore_ComplexTurn_ShouldCalculateCorrectly()
        {
            // Arrange - Match-5 L-shape with cascade and power orb
            var centerTile = new MockTile { Position = new Vector2Int(3, 3) };
            var powerOrb = new MockPowerOrb();
            var turnResult = new TurnResult
            {
                ClearedTiles = new List<Tile> { centerTile, centerTile, centerTile, centerTile, centerTile },
                MatchSizes = new List<int> { 5 },
                CascadeLevel = 1,
                HasLShape = true,
                HasCross = false,
                PowerOrbCollected = true,
                CollectedPowerOrb = powerOrb
            };
            
            // Act
            var result = scoreCalculator.CalculateTurnScore(turnResult);
            
            // Assert
            Assert.AreEqual(150, result.BaseScore); // 5 tiles * 10 base * 3 center multiplier
            Assert.AreEqual(50, result.PatternBonus); // L-shape bonus
            Assert.AreEqual(5000, result.PowerOrbScore);
            Assert.AreEqual(5200, result.TotalBeforeMultiplier);
            Assert.AreEqual(10f, result.Multiplier); // Power orb = max heat
            Assert.AreEqual(52000, result.FinalScore);
            Assert.AreEqual(10f, result.FinalHeat);
        }
        
        [Test]
        public void CalculateTurnScore_MultipleMatches_ShouldSumCorrectly()
        {
            // Arrange - Two separate matches
            var turnResult = new TurnResult
            {
                ClearedTiles = new List<Tile> { mockTile, mockTile, mockTile, mockTile, mockTile, mockTile },
                MatchSizes = new List<int> { 3, 3 },
                CascadeLevel = 0,
                HasLShape = false,
                HasCross = false,
                PowerOrbCollected = false
            };
            
            // Act
            var result = scoreCalculator.CalculateTurnScore(turnResult);
            
            // Assert
            Assert.AreEqual(60, result.BaseScore); // 6 tiles * 10 base * 1 edge multiplier
            Assert.AreEqual(0f, result.HeatGained); // Two match-3s = 0 heat
            Assert.AreEqual(0f, result.FinalHeat); // After decay
        }
    }
    
    /// <summary>
    /// Mock tile for testing
    /// </summary>
    public class MockTile : Tile
    {
        public override Vector2Int Position { get; set; } = new Vector2Int(0, 0);
        
        public override TileType Type => TileType.Normal;
        
        public override bool IsMovable => true;
        
        public override bool CanMatch => true;
    }
    
    /// <summary>
    /// Mock power orb for testing
    /// </summary>
    public class MockPowerOrb : PowerOrb
    {
        public override bool ReachedCorrectEdge() => true;
    }
}
