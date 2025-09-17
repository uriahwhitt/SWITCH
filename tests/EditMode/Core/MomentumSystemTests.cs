using NUnit.Framework;
using UnityEngine;
using SWITCH.Core;

namespace SWITCH.Tests.Core
{
    /// <summary>
    /// Unit tests for the MomentumSystem class.
    /// Tests heat generation, decay, multipliers, and power orb integration.
    /// </summary>
    [TestFixture]
    public class MomentumSystemTests
    {
        private GameObject testGameObject;
        private MomentumSystem momentumSystem;
        
        [SetUp]
        public void SetUp()
        {
            testGameObject = new GameObject("TestMomentumSystem");
            momentumSystem = testGameObject.AddComponent<MomentumSystem>();
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
        public void InitialState_ShouldBeZero()
        {
            // Act & Assert
            Assert.AreEqual(0f, momentumSystem.CurrentMomentum);
            Assert.AreEqual(MomentumSystem.HeatLevel.Cold, momentumSystem.CurrentHeatLevel);
            Assert.AreEqual(1f, momentumSystem.GetScoreMultiplier());
        }
        
        [Test]
        public void AddMatchHeat_Match3_ShouldNotAddHeat()
        {
            // Arrange
            float initialMomentum = momentumSystem.CurrentMomentum;
            
            // Act
            float heatGained = momentumSystem.AddMatchHeat(3);
            
            // Assert
            Assert.AreEqual(0f, heatGained);
            Assert.AreEqual(initialMomentum, momentumSystem.CurrentMomentum);
        }
        
        [Test]
        public void AddMatchHeat_Match4_ShouldAddHeat()
        {
            // Arrange
            float initialMomentum = momentumSystem.CurrentMomentum;
            
            // Act
            float heatGained = momentumSystem.AddMatchHeat(4);
            
            // Assert
            Assert.AreEqual(1f, heatGained);
            Assert.AreEqual(initialMomentum + 1f, momentumSystem.CurrentMomentum);
        }
        
        [Test]
        public void AddMatchHeat_Match5_ShouldAddMoreHeat()
        {
            // Arrange
            float initialMomentum = momentumSystem.CurrentMomentum;
            
            // Act
            float heatGained = momentumSystem.AddMatchHeat(5);
            
            // Assert
            Assert.AreEqual(2f, heatGained);
            Assert.AreEqual(initialMomentum + 2f, momentumSystem.CurrentMomentum);
        }
        
        [Test]
        public void AddCascadeHeat_ShouldAddCorrectHeat()
        {
            // Arrange
            float initialMomentum = momentumSystem.CurrentMomentum;
            int cascadeLevel = 3;
            
            // Act
            float heatGained = momentumSystem.AddCascadeHeat(cascadeLevel);
            
            // Assert
            Assert.AreEqual(1.5f, heatGained); // 0.5 * 3
            Assert.AreEqual(initialMomentum + 1.5f, momentumSystem.CurrentMomentum);
        }
        
        [Test]
        public void AddPatternHeat_LShape_ShouldAddHeat()
        {
            // Arrange
            float initialMomentum = momentumSystem.CurrentMomentum;
            
            // Act
            float heatGained = momentumSystem.AddPatternHeat(PatternType.LShape);
            
            // Assert
            Assert.AreEqual(1f, heatGained);
            Assert.AreEqual(initialMomentum + 1f, momentumSystem.CurrentMomentum);
        }
        
        [Test]
        public void AddPatternHeat_Cross_ShouldAddMoreHeat()
        {
            // Arrange
            float initialMomentum = momentumSystem.CurrentMomentum;
            
            // Act
            float heatGained = momentumSystem.AddPatternHeat(PatternType.Cross);
            
            // Assert
            Assert.AreEqual(1.5f, heatGained);
            Assert.AreEqual(initialMomentum + 1.5f, momentumSystem.CurrentMomentum);
        }
        
        [Test]
        public void TriggerPowerOrbBoost_ShouldSetMaxMomentum()
        {
            // Act
            momentumSystem.TriggerPowerOrbBoost();
            
            // Assert
            Assert.AreEqual(10f, momentumSystem.CurrentMomentum);
            Assert.AreEqual(MomentumSystem.HeatLevel.Inferno, momentumSystem.CurrentHeatLevel);
        }
        
        [Test]
        public void ApplyTurnEndDecay_ShouldReduceMomentum()
        {
            // Arrange
            momentumSystem.AddMatchHeat(4); // Add some heat first
            float initialMomentum = momentumSystem.CurrentMomentum;
            
            // Act
            momentumSystem.ApplyTurnEndDecay();
            
            // Assert
            Assert.AreEqual(initialMomentum - 1f, momentumSystem.CurrentMomentum);
        }
        
        [Test]
        public void ApplyTurnEndDecay_ShouldNotGoBelowZero()
        {
            // Arrange - momentum starts at 0
            
            // Act
            momentumSystem.ApplyTurnEndDecay();
            
            // Assert
            Assert.AreEqual(0f, momentumSystem.CurrentMomentum);
        }
        
        [Test]
        public void GetScoreMultiplier_ShouldScaleCorrectly()
        {
            // Test various momentum levels
            momentumSystem.AddMatchHeat(4); // 1.0 heat
            Assert.AreEqual(1.9f, momentumSystem.GetScoreMultiplier(), 0.01f);
            
            momentumSystem.AddMatchHeat(5); // +2.0 heat = 3.0 total
            Assert.AreEqual(3.7f, momentumSystem.GetScoreMultiplier(), 0.01f);
            
            momentumSystem.TriggerPowerOrbBoost(); // 10.0 heat
            Assert.AreEqual(10f, momentumSystem.GetScoreMultiplier(), 0.01f);
        }
        
        [Test]
        public void GetHeatLevel_ShouldReturnCorrectLevels()
        {
            // Cold (0-2)
            Assert.AreEqual(MomentumSystem.HeatLevel.Cold, momentumSystem.GetHeatLevel(0f));
            Assert.AreEqual(MomentumSystem.HeatLevel.Cold, momentumSystem.GetHeatLevel(2f));
            
            // Warm (3-4)
            Assert.AreEqual(MomentumSystem.HeatLevel.Warm, momentumSystem.GetHeatLevel(3f));
            Assert.AreEqual(MomentumSystem.HeatLevel.Warm, momentumSystem.GetHeatLevel(4f));
            
            // Hot (5-7)
            Assert.AreEqual(MomentumSystem.HeatLevel.Hot, momentumSystem.GetHeatLevel(5f));
            Assert.AreEqual(MomentumSystem.HeatLevel.Hot, momentumSystem.GetHeatLevel(7f));
            
            // Blazing (8-9)
            Assert.AreEqual(MomentumSystem.HeatLevel.Blazing, momentumSystem.GetHeatLevel(8f));
            Assert.AreEqual(MomentumSystem.HeatLevel.Blazing, momentumSystem.GetHeatLevel(9f));
            
            // Inferno (10)
            Assert.AreEqual(MomentumSystem.HeatLevel.Inferno, momentumSystem.GetHeatLevel(10f));
        }
        
        [Test]
        public void ResetMomentum_ShouldSetToZero()
        {
            // Arrange
            momentumSystem.AddMatchHeat(5);
            momentumSystem.AddCascadeHeat(2);
            
            // Act
            momentumSystem.ResetMomentum();
            
            // Assert
            Assert.AreEqual(0f, momentumSystem.CurrentMomentum);
            Assert.AreEqual(MomentumSystem.HeatLevel.Cold, momentumSystem.CurrentHeatLevel);
        }
        
        [Test]
        public void Momentum_ShouldNotExceedMaximum()
        {
            // Arrange - add way more heat than maximum
            for (int i = 0; i < 20; i++)
            {
                momentumSystem.AddMatchHeat(5); // 2 heat each
            }
            
            // Assert
            Assert.AreEqual(10f, momentumSystem.CurrentMomentum);
        }
        
        [Test]
        public void HeatPercentage_ShouldCalculateCorrectly()
        {
            // Arrange
            momentumSystem.AddMatchHeat(4); // 1.0 heat
            
            // Act & Assert
            Assert.AreEqual(0.1f, momentumSystem.HeatPercentage, 0.01f); // 1/10
            
            momentumSystem.TriggerPowerOrbBoost(); // 10.0 heat
            Assert.AreEqual(1f, momentumSystem.HeatPercentage, 0.01f); // 10/10
        }
        
        [Test]
        public void GetHeatColor_ShouldReturnCorrectColors()
        {
            // Cold
            momentumSystem.ResetMomentum();
            Color coldColor = momentumSystem.GetHeatColor();
            Assert.AreEqual(new Color(0.3f, 0.6f, 1f, 1f), coldColor);
            
            // Warm
            momentumSystem.AddMatchHeat(4);
            Color warmColor = momentumSystem.GetHeatColor();
            Assert.AreEqual(new Color(1f, 1f, 0.3f, 1f), warmColor);
            
            // Hot
            momentumSystem.AddMatchHeat(5);
            Color hotColor = momentumSystem.GetHeatColor();
            Assert.AreEqual(new Color(1f, 0.6f, 0.2f, 1f), hotColor);
            
            // Blazing
            momentumSystem.AddMatchHeat(5);
            Color blazingColor = momentumSystem.GetHeatColor();
            Assert.AreEqual(new Color(1f, 0.2f, 0.1f, 1f), blazingColor);
            
            // Inferno
            momentumSystem.TriggerPowerOrbBoost();
            Color infernoColor = momentumSystem.GetHeatColor();
            Assert.AreEqual(new Color(1f, 1f, 1f, 1f), infernoColor);
        }
    }
}
