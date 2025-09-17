using NUnit.Framework;
using UnityEngine;
using SWITCH.PowerUps;

namespace SWITCH.Tests.PowerUps
{
    /// <summary>
    /// Unit tests for the PowerOrb class.
    /// Tests orb initialization, movement, collection, and scoring.
    /// </summary>
    [TestFixture]
    public class PowerOrbTests
    {
        private GameObject testGameObject;
        private PowerOrb powerOrb;
        private PowerOrbData testOrbData;
        
        [SetUp]
        public void SetUp()
        {
            testGameObject = new GameObject("TestPowerOrb");
            powerOrb = testGameObject.AddComponent<PowerOrb>();
            
            // Create test orb data
            testOrbData = ScriptableObject.CreateInstance<PowerOrbData>();
            testOrbData.color = OrbColor.Red;
            testOrbData.targetEdge = new Vector2Int(0, 0); // Left edge
            testOrbData.baseScore = 5000;
            testOrbData.ageBonus = 500;
            testOrbData.glowColor = Color.red;
            testOrbData.pulseSpeed = 1.5f;
            testOrbData.glowIntensity = 1.2f;
        }
        
        [TearDown]
        public void TearDown()
        {
            if (testGameObject != null)
            {
                Object.DestroyImmediate(testGameObject);
            }
            if (testOrbData != null)
            {
                Object.DestroyImmediate(testOrbData);
            }
        }
        
        [Test]
        public void Initialize_ShouldSetCorrectProperties()
        {
            // Arrange
            Vector2Int spawnPosition = new Vector2Int(3, 3);
            
            // Act
            powerOrb.Initialize(testOrbData, spawnPosition);
            
            // Assert
            Assert.AreEqual(testOrbData, powerOrb.Data);
            Assert.AreEqual(spawnPosition, powerOrb.BoardPosition);
            Assert.AreEqual(testOrbData.targetEdge, powerOrb.TargetEdge);
            Assert.AreEqual(0, powerOrb.Age);
            Assert.IsTrue(powerOrb.IsActive);
            Assert.IsFalse(powerOrb.HasReachedEdge);
        }
        
        [Test]
        public void GetScoreValue_ShouldCalculateCorrectly()
        {
            // Arrange
            powerOrb.Initialize(testOrbData, new Vector2Int(3, 3));
            
            // Act & Assert
            Assert.AreEqual(5000, powerOrb.GetScoreValue()); // Base score
            
            // Simulate aging
            for (int i = 0; i < 5; i++)
            {
                // Age the orb (normally done by coroutine)
                var ageField = typeof(PowerOrb).GetField("age", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                ageField.SetValue(powerOrb, i + 1);
            }
            
            Assert.AreEqual(7500, powerOrb.GetScoreValue()); // 5000 + (5 * 500)
        }
        
        [Test]
        public void ReachedCorrectEdge_ShouldReturnTrueWhenAtTarget()
        {
            // Arrange
            powerOrb.Initialize(testOrbData, new Vector2Int(3, 3));
            
            // Act - Move orb to target edge
            var positionField = typeof(PowerOrb).GetField("boardPosition", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            positionField.SetValue(powerOrb, new Vector2Int(0, 3)); // Left edge
            
            // Assert
            Assert.IsTrue(powerOrb.ReachedCorrectEdge());
        }
        
        [Test]
        public void ReachedCorrectEdge_ShouldReturnFalseWhenNotAtTarget()
        {
            // Arrange
            powerOrb.Initialize(testOrbData, new Vector2Int(3, 3));
            
            // Act - Move orb to wrong edge
            var positionField = typeof(PowerOrb).GetField("boardPosition", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            positionField.SetValue(powerOrb, new Vector2Int(7, 3)); // Right edge (wrong)
            
            // Assert
            Assert.IsFalse(powerOrb.ReachedCorrectEdge());
        }
        
        [Test]
        public void MoveTowardEdge_ShouldMoveTowardTarget()
        {
            // Arrange
            powerOrb.Initialize(testOrbData, new Vector2Int(3, 3));
            Vector2Int initialPosition = powerOrb.BoardPosition;
            
            // Act
            powerOrb.MoveTowardEdge();
            
            // Assert
            Vector2Int expectedPosition = initialPosition + new Vector2Int(-1, 0); // Move left
            Assert.AreEqual(expectedPosition, powerOrb.BoardPosition);
        }
        
        [Test]
        public void MoveTowardEdge_ShouldReachTargetEdge()
        {
            // Arrange
            powerOrb.Initialize(testOrbData, new Vector2Int(1, 3)); // One step from left edge
            
            // Act
            powerOrb.MoveTowardEdge();
            
            // Assert
            Assert.IsTrue(powerOrb.HasReachedEdge);
            Assert.IsFalse(powerOrb.IsActive);
        }
        
        [Test]
        public void ReachTargetEdge_ShouldSetCorrectState()
        {
            // Arrange
            powerOrb.Initialize(testOrbData, new Vector2Int(3, 3));
            bool eventTriggered = false;
            powerOrb.OnOrbCollected += (orb) => eventTriggered = true;
            
            // Act
            powerOrb.ReachTargetEdge();
            
            // Assert
            Assert.IsTrue(powerOrb.HasReachedEdge);
            Assert.IsFalse(powerOrb.IsActive);
            Assert.IsTrue(eventTriggered);
        }
        
        [Test]
        public void LoseOrb_ShouldSetCorrectState()
        {
            // Arrange
            powerOrb.Initialize(testOrbData, new Vector2Int(3, 3));
            bool eventTriggered = false;
            powerOrb.OnOrbLost += (orb) => eventTriggered = true;
            
            // Act
            powerOrb.LoseOrb();
            
            // Assert
            Assert.IsFalse(powerOrb.IsActive);
            Assert.IsFalse(powerOrb.HasReachedEdge);
            Assert.IsTrue(eventTriggered);
        }
        
        [Test]
        public void MoveTowardEdge_ShouldNotMoveWhenInactive()
        {
            // Arrange
            powerOrb.Initialize(testOrbData, new Vector2Int(3, 3));
            powerOrb.LoseOrb(); // Make inactive
            Vector2Int positionBeforeMove = powerOrb.BoardPosition;
            
            // Act
            powerOrb.MoveTowardEdge();
            
            // Assert
            Assert.AreEqual(positionBeforeMove, powerOrb.BoardPosition);
        }
        
        [Test]
        public void MoveTowardEdge_ShouldNotMoveWhenReachedEdge()
        {
            // Arrange
            powerOrb.Initialize(testOrbData, new Vector2Int(3, 3));
            powerOrb.ReachTargetEdge(); // Reach edge
            Vector2Int positionBeforeMove = powerOrb.BoardPosition;
            
            // Act
            powerOrb.MoveTowardEdge();
            
            // Assert
            Assert.AreEqual(positionBeforeMove, powerOrb.BoardPosition);
        }
        
        [Test]
        public void GetMovementDirection_ShouldReturnCorrectDirection()
        {
            // Test left edge target
            testOrbData.targetEdge = new Vector2Int(0, 0);
            powerOrb.Initialize(testOrbData, new Vector2Int(3, 3));
            
            var directionMethod = typeof(PowerOrb).GetMethod("GetMovementDirection", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Vector2Int direction = (Vector2Int)directionMethod.Invoke(powerOrb, null);
            
            Assert.AreEqual(new Vector2Int(-1, 0), direction);
        }
        
        [Test]
        public void GetMovementDirection_RightEdge_ShouldReturnCorrectDirection()
        {
            // Test right edge target
            testOrbData.targetEdge = new Vector2Int(7, 0);
            powerOrb.Initialize(testOrbData, new Vector2Int(3, 3));
            
            var directionMethod = typeof(PowerOrb).GetMethod("GetMovementDirection", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Vector2Int direction = (Vector2Int)directionMethod.Invoke(powerOrb, null);
            
            Assert.AreEqual(new Vector2Int(1, 0), direction);
        }
        
        [Test]
        public void GetMovementDirection_TopEdge_ShouldReturnCorrectDirection()
        {
            // Test top edge target
            testOrbData.targetEdge = new Vector2Int(0, 7);
            powerOrb.Initialize(testOrbData, new Vector2Int(3, 3));
            
            var directionMethod = typeof(PowerOrb).GetMethod("GetMovementDirection", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Vector2Int direction = (Vector2Int)directionMethod.Invoke(powerOrb, null);
            
            Assert.AreEqual(new Vector2Int(0, 1), direction);
        }
        
        [Test]
        public void GetMovementDirection_BottomEdge_ShouldReturnCorrectDirection()
        {
            // Test bottom edge target
            testOrbData.targetEdge = new Vector2Int(0, 0);
            powerOrb.Initialize(testOrbData, new Vector2Int(3, 3));
            
            var directionMethod = typeof(PowerOrb).GetMethod("GetMovementDirection", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Vector2Int direction = (Vector2Int)directionMethod.Invoke(powerOrb, null);
            
            Assert.AreEqual(new Vector2Int(-1, 0), direction); // Left edge, so move left
        }
        
        [Test]
        public void IsAtTargetEdge_ShouldReturnTrueWhenAtEdge()
        {
            // Arrange
            powerOrb.Initialize(testOrbData, new Vector2Int(3, 3));
            
            // Act
            var isAtEdgeMethod = typeof(PowerOrb).GetMethod("IsAtTargetEdge", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool isAtLeftEdge = (bool)isAtEdgeMethod.Invoke(powerOrb, new object[] { new Vector2Int(0, 3) });
            bool isAtRightEdge = (bool)isAtEdgeMethod.Invoke(powerOrb, new object[] { new Vector2Int(7, 3) });
            
            // Assert
            Assert.IsTrue(isAtLeftEdge); // At left edge (target)
            Assert.IsFalse(isAtRightEdge); // At right edge (not target)
        }
        
        [Test]
        public void GetOrbColor_ShouldReturnCorrectColor()
        {
            // Test different orb colors
            testOrbData.color = OrbColor.Red;
            powerOrb.Initialize(testOrbData, new Vector2Int(3, 3));
            
            var getColorMethod = typeof(PowerOrb).GetMethod("GetOrbColor", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Color redColor = (Color)getColorMethod.Invoke(powerOrb, new object[] { OrbColor.Red });
            Color blueColor = (Color)getColorMethod.Invoke(powerOrb, new object[] { OrbColor.Blue });
            
            Assert.AreEqual(Color.red, redColor);
            Assert.AreEqual(Color.blue, blueColor);
        }
    }
}
