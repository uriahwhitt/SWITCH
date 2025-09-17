using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using SWITCH.PowerUps;

namespace SWITCH.Tests.PowerUps
{
    /// <summary>
    /// Unit tests for the PowerOrbManager class.
    /// Tests orb spawning, management, and collection handling.
    /// </summary>
    [TestFixture]
    public class PowerOrbManagerTests
    {
        private GameObject testGameObject;
        private PowerOrbManager orbManager;
        private PowerOrbData testOrbData;
        private GameObject powerOrbPrefab;
        
        [SetUp]
        public void SetUp()
        {
            testGameObject = new GameObject("TestPowerOrbManager");
            orbManager = testGameObject.AddComponent<PowerOrbManager>();
            
            // Create test orb data
            testOrbData = ScriptableObject.CreateInstance<PowerOrbData>();
            testOrbData.color = OrbColor.Red;
            testOrbData.targetEdge = new Vector2Int(0, 0);
            testOrbData.baseScore = 5000;
            testOrbData.ageBonus = 500;
            testOrbData.baseSpawnChance = 1.0f; // 100% for testing
            testOrbData.centerSpawnPositions = new Vector2Int[]
            {
                new Vector2Int(3, 3),
                new Vector2Int(4, 3),
                new Vector2Int(3, 4),
                new Vector2Int(4, 4)
            };
            
            // Create test prefab
            powerOrbPrefab = new GameObject("PowerOrbPrefab");
            powerOrbPrefab.AddComponent<PowerOrb>();
            
            // Set orb manager properties via reflection
            var orbTypesField = typeof(PowerOrbManager).GetField("powerOrbTypes", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            orbTypesField.SetValue(orbManager, new PowerOrbData[] { testOrbData });
            
            var prefabField = typeof(PowerOrbManager).GetField("powerOrbPrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            prefabField.SetValue(orbManager, powerOrbPrefab);
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
            if (powerOrbPrefab != null)
            {
                Object.DestroyImmediate(powerOrbPrefab);
            }
        }
        
        [Test]
        public void TrySpawnPowerOrb_ShouldSpawnSuccessfully()
        {
            // Act
            bool result = orbManager.TrySpawnPowerOrb(testOrbData);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, orbManager.GetActiveOrbs().Count);
        }
        
        [Test]
        public void TrySpawnPowerOrb_MaxOrbsReached_ShouldNotSpawn()
        {
            // Arrange - Spawn 4 orbs (max limit)
            for (int i = 0; i < 4; i++)
            {
                orbManager.TrySpawnPowerOrb(testOrbData);
            }
            
            // Act
            bool result = orbManager.TrySpawnPowerOrb(testOrbData);
            
            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(4, orbManager.GetActiveOrbs().Count);
        }
        
        [Test]
        public void GetActiveOrbs_ShouldReturnCorrectList()
        {
            // Arrange
            orbManager.TrySpawnPowerOrb(testOrbData);
            orbManager.TrySpawnPowerOrb(testOrbData);
            
            // Act
            List<PowerOrb> activeOrbs = orbManager.GetActiveOrbs();
            
            // Assert
            Assert.AreEqual(2, activeOrbs.Count);
            Assert.IsTrue(activeOrbs[0].IsActive);
            Assert.IsTrue(activeOrbs[1].IsActive);
        }
        
        [Test]
        public void GetOrbAtPosition_ShouldReturnCorrectOrb()
        {
            // Arrange
            orbManager.TrySpawnPowerOrb(testOrbData);
            List<PowerOrb> activeOrbs = orbManager.GetActiveOrbs();
            Vector2Int orbPosition = activeOrbs[0].BoardPosition;
            
            // Act
            PowerOrb foundOrb = orbManager.GetOrbAtPosition(orbPosition);
            
            // Assert
            Assert.IsNotNull(foundOrb);
            Assert.AreEqual(activeOrbs[0], foundOrb);
        }
        
        [Test]
        public void GetOrbAtPosition_NoOrbAtPosition_ShouldReturnNull()
        {
            // Act
            PowerOrb foundOrb = orbManager.GetOrbAtPosition(new Vector2Int(0, 0));
            
            // Assert
            Assert.IsNull(foundOrb);
        }
        
        [Test]
        public void HasOrbAtPosition_ShouldReturnCorrectValue()
        {
            // Arrange
            orbManager.TrySpawnPowerOrb(testOrbData);
            List<PowerOrb> activeOrbs = orbManager.GetActiveOrbs();
            Vector2Int orbPosition = activeOrbs[0].BoardPosition;
            
            // Act & Assert
            Assert.IsTrue(orbManager.HasOrbAtPosition(orbPosition));
            Assert.IsFalse(orbManager.HasOrbAtPosition(new Vector2Int(0, 0)));
        }
        
        [Test]
        public void HandleOrbCollected_ShouldRemoveOrbFromTracking()
        {
            // Arrange
            orbManager.TrySpawnPowerOrb(testOrbData);
            List<PowerOrb> activeOrbs = orbManager.GetActiveOrbs();
            PowerOrb collectedOrb = activeOrbs[0];
            Vector2Int orbPosition = collectedOrb.BoardPosition;
            
            // Act
            orbManager.HandleOrbCollected(collectedOrb);
            
            // Assert
            Assert.AreEqual(0, orbManager.GetActiveOrbs().Count);
            Assert.IsFalse(orbManager.HasOrbAtPosition(orbPosition));
            Assert.IsNull(orbManager.GetOrbAtPosition(orbPosition));
        }
        
        [Test]
        public void HandleOrbLost_ShouldRemoveOrbFromTracking()
        {
            // Arrange
            orbManager.TrySpawnPowerOrb(testOrbData);
            List<PowerOrb> activeOrbs = orbManager.GetActiveOrbs();
            PowerOrb lostOrb = activeOrbs[0];
            Vector2Int orbPosition = lostOrb.BoardPosition;
            
            // Act
            orbManager.HandleOrbLost(lostOrb);
            
            // Assert
            Assert.AreEqual(0, orbManager.GetActiveOrbs().Count);
            Assert.IsFalse(orbManager.HasOrbAtPosition(orbPosition));
            Assert.IsNull(orbManager.GetOrbAtPosition(orbPosition));
        }
        
        [Test]
        public void ClearAllOrbs_ShouldRemoveAllOrbs()
        {
            // Arrange
            orbManager.TrySpawnPowerOrb(testOrbData);
            orbManager.TrySpawnPowerOrb(testOrbData);
            orbManager.TrySpawnPowerOrb(testOrbData);
            
            // Act
            orbManager.ClearAllOrbs();
            
            // Assert
            Assert.AreEqual(0, orbManager.GetActiveOrbs().Count);
        }
        
        [Test]
        public void ForceSpawnOrb_ShouldSpawnOrb()
        {
            // Act
            bool result = orbManager.ForceSpawnOrb(testOrbData);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, orbManager.GetActiveOrbs().Count);
        }
        
        [Test]
        public void GetSpawnStats_ShouldReturnCorrectStats()
        {
            // Arrange
            orbManager.TrySpawnPowerOrb(testOrbData);
            
            // Act
            PowerOrbSpawnStats stats = orbManager.GetSpawnStats();
            
            // Assert
            Assert.IsNotNull(stats);
            Assert.AreEqual(1, stats.activeOrbCount);
            Assert.IsNotNull(stats.orbPositions);
        }
        
        [Test]
        public void TrySpawnPowerOrb_ShouldInitializeOrbCorrectly()
        {
            // Act
            bool result = orbManager.TrySpawnPowerOrb(testOrbData);
            
            // Assert
            Assert.IsTrue(result);
            List<PowerOrb> activeOrbs = orbManager.GetActiveOrbs();
            PowerOrb spawnedOrb = activeOrbs[0];
            
            Assert.AreEqual(testOrbData, spawnedOrb.Data);
            Assert.IsTrue(spawnedOrb.IsActive);
            Assert.IsFalse(spawnedOrb.HasReachedEdge);
            Assert.AreEqual(0, spawnedOrb.Age);
        }
        
        [Test]
        public void TrySpawnPowerOrb_ShouldSetCorrectPosition()
        {
            // Act
            orbManager.TrySpawnPowerOrb(testOrbData);
            
            // Assert
            List<PowerOrb> activeOrbs = orbManager.GetActiveOrbs();
            PowerOrb spawnedOrb = activeOrbs[0];
            Vector2Int position = spawnedOrb.BoardPosition;
            
            // Should be one of the center spawn positions
            bool isValidPosition = false;
            foreach (var spawnPos in testOrbData.centerSpawnPositions)
            {
                if (position == spawnPos)
                {
                    isValidPosition = true;
                    break;
                }
            }
            
            Assert.IsTrue(isValidPosition);
        }
        
        [Test]
        public void TrySpawnPowerOrb_MultipleSpawns_ShouldUseDifferentPositions()
        {
            // Act
            orbManager.TrySpawnPowerOrb(testOrbData);
            orbManager.TrySpawnPowerOrb(testOrbData);
            orbManager.TrySpawnPowerOrb(testOrbData);
            orbManager.TrySpawnPowerOrb(testOrbData);
            
            // Assert
            List<PowerOrb> activeOrbs = orbManager.GetActiveOrbs();
            Assert.AreEqual(4, activeOrbs.Count);
            
            // All orbs should be at different positions
            HashSet<Vector2Int> positions = new HashSet<Vector2Int>();
            foreach (var orb in activeOrbs)
            {
                positions.Add(orb.BoardPosition);
            }
            
            Assert.AreEqual(4, positions.Count); // All unique positions
        }
        
        [Test]
        public void HandleOrbCollected_ShouldTriggerEvent()
        {
            // Arrange
            orbManager.TrySpawnPowerOrb(testOrbData);
            List<PowerOrb> activeOrbs = orbManager.GetActiveOrbs();
            PowerOrb collectedOrb = activeOrbs[0];
            bool eventTriggered = false;
            PowerOrb eventOrb = null;
            
            orbManager.OnOrbCollected += (orb) => 
            {
                eventTriggered = true;
                eventOrb = orb;
            };
            
            // Act
            orbManager.HandleOrbCollected(collectedOrb);
            
            // Assert
            Assert.IsTrue(eventTriggered);
            Assert.AreEqual(collectedOrb, eventOrb);
        }
        
        [Test]
        public void HandleOrbLost_ShouldTriggerEvent()
        {
            // Arrange
            orbManager.TrySpawnPowerOrb(testOrbData);
            List<PowerOrb> activeOrbs = orbManager.GetActiveOrbs();
            PowerOrb lostOrb = activeOrbs[0];
            bool eventTriggered = false;
            PowerOrb eventOrb = null;
            
            orbManager.OnOrbLost += (orb) => 
            {
                eventTriggered = true;
                eventOrb = orb;
            };
            
            // Act
            orbManager.HandleOrbLost(lostOrb);
            
            // Assert
            Assert.IsTrue(eventTriggered);
            Assert.AreEqual(lostOrb, eventOrb);
        }
    }
}
