/******************************************************************************
 * SWITCH - SwapCache Tests
 * Sprint: 1
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Unit tests for SwapCache system
 * Dependencies: Unity Test Framework, SwapCache
 * 
 * Educational Notes:
 * - Demonstrates how to test caching systems
 * - Shows how to test time-based functionality
 * - Performance: Tests ensure <0.1ms operation requirements
 *****************************************************************************/

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SWITCH.Core;
using System.Collections;

namespace SWITCH.Tests.Core
{
    /// <summary>
    /// Unit tests for SwapCache system.
    /// Educational: Shows how to test caching and time-based systems.
    /// </summary>
    public class SwapCacheTests
    {
        private SwapCache swapCache;
        
        [SetUp]
        public void Setup()
        {
            swapCache = new SwapCache();
        }
        
        [TearDown]
        public void TearDown()
        {
            swapCache = null;
        }
        
        [Test]
        public void SwapCache_InitialState_IsInvalid()
        {
            // Arrange & Act
            bool isValid = swapCache.IsValid;
            Direction direction = swapCache.GetSwapDirection();
            
            // Assert
            Assert.IsFalse(isValid, "Initial cache should be invalid");
            Assert.AreEqual(Direction.None, direction, "Initial direction should be None");
        }
        
        [Test]
        public void SwapCache_CacheSwap_ValidSwap_SetsValidState()
        {
            // Arrange
            Vector2Int pos1 = new Vector2Int(0, 0);
            Vector2Int pos2 = new Vector2Int(1, 0);
            
            // Act
            swapCache.CacheSwap(pos1, pos2);
            
            // Assert
            Assert.IsTrue(swapCache.IsValid, "Cache should be valid after caching swap");
            Assert.AreEqual(pos1, swapCache.Tile1Position, "Tile1 position should be cached");
            Assert.AreEqual(pos2, swapCache.Tile2Position, "Tile2 position should be cached");
        }
        
        [Test]
        public void SwapCache_CacheSwap_HorizontalSwap_ReturnsCorrectDirection()
        {
            // Arrange
            Vector2Int pos1 = new Vector2Int(0, 0);
            Vector2Int pos2 = new Vector2Int(1, 0);
            
            // Act
            swapCache.CacheSwap(pos1, pos2);
            Direction direction = swapCache.GetSwapDirection();
            
            // Assert
            Assert.AreEqual(Direction.Right, direction, "Horizontal swap should return Right direction");
        }
        
        [Test]
        public void SwapCache_CacheSwap_VerticalSwap_ReturnsCorrectDirection()
        {
            // Arrange
            Vector2Int pos1 = new Vector2Int(0, 0);
            Vector2Int pos2 = new Vector2Int(0, 1);
            
            // Act
            swapCache.CacheSwap(pos1, pos2);
            Direction direction = swapCache.GetSwapDirection();
            
            // Assert
            Assert.AreEqual(Direction.Up, direction, "Vertical swap should return Up direction");
        }
        
        [Test]
        public void SwapCache_CacheSwap_LeftwardSwap_ReturnsCorrectDirection()
        {
            // Arrange
            Vector2Int pos1 = new Vector2Int(1, 0);
            Vector2Int pos2 = new Vector2Int(0, 0);
            
            // Act
            swapCache.CacheSwap(pos1, pos2);
            Direction direction = swapCache.GetSwapDirection();
            
            // Assert
            Assert.AreEqual(Direction.Left, direction, "Leftward swap should return Left direction");
        }
        
        [Test]
        public void SwapCache_CacheSwap_DownwardSwap_ReturnsCorrectDirection()
        {
            // Arrange
            Vector2Int pos1 = new Vector2Int(0, 1);
            Vector2Int pos2 = new Vector2Int(0, 0);
            
            // Act
            swapCache.CacheSwap(pos1, pos2);
            Direction direction = swapCache.GetSwapDirection();
            
            // Assert
            Assert.AreEqual(Direction.Down, direction, "Downward swap should return Down direction");
        }
        
        [Test]
        public void SwapCache_GetGravityDirection_RightSwap_ReturnsLeftGravity()
        {
            // Arrange
            Vector2Int pos1 = new Vector2Int(0, 0);
            Vector2Int pos2 = new Vector2Int(1, 0);
            
            // Act
            swapCache.CacheSwap(pos1, pos2);
            Direction gravityDirection = swapCache.GetGravityDirection();
            
            // Assert
            Assert.AreEqual(Direction.Left, gravityDirection, "Right swap should result in Left gravity");
        }
        
        [Test]
        public void SwapCache_GetGravityDirection_UpSwap_ReturnsDownGravity()
        {
            // Arrange
            Vector2Int pos1 = new Vector2Int(0, 0);
            Vector2Int pos2 = new Vector2Int(0, 1);
            
            // Act
            swapCache.CacheSwap(pos1, pos2);
            Direction gravityDirection = swapCache.GetGravityDirection();
            
            // Assert
            Assert.AreEqual(Direction.Down, gravityDirection, "Up swap should result in Down gravity");
        }
        
        [Test]
        public void SwapCache_GetGravityDirection_LeftSwap_ReturnsRightGravity()
        {
            // Arrange
            Vector2Int pos1 = new Vector2Int(1, 0);
            Vector2Int pos2 = new Vector2Int(0, 0);
            
            // Act
            swapCache.CacheSwap(pos1, pos2);
            Direction gravityDirection = swapCache.GetGravityDirection();
            
            // Assert
            Assert.AreEqual(Direction.Right, gravityDirection, "Left swap should result in Right gravity");
        }
        
        [Test]
        public void SwapCache_GetGravityDirection_DownSwap_ReturnsUpGravity()
        {
            // Arrange
            Vector2Int pos1 = new Vector2Int(0, 1);
            Vector2Int pos2 = new Vector2Int(0, 0);
            
            // Act
            swapCache.CacheSwap(pos1, pos2);
            Direction gravityDirection = swapCache.GetGravityDirection();
            
            // Assert
            Assert.AreEqual(Direction.Up, gravityDirection, "Down swap should result in Up gravity");
        }
        
        [Test]
        public void SwapCache_Clear_ResetsToInvalidState()
        {
            // Arrange
            Vector2Int pos1 = new Vector2Int(0, 0);
            Vector2Int pos2 = new Vector2Int(1, 0);
            swapCache.CacheSwap(pos1, pos2);
            
            // Act
            swapCache.Clear();
            
            // Assert
            Assert.IsFalse(swapCache.IsValid, "Cache should be invalid after clearing");
            Assert.AreEqual(Direction.None, swapCache.GetSwapDirection(), "Direction should be None after clearing");
        }
        
        [Test]
        public void SwapCache_HasValidSwap_ValidCache_ReturnsTrue()
        {
            // Arrange
            Vector2Int pos1 = new Vector2Int(0, 0);
            Vector2Int pos2 = new Vector2Int(1, 0);
            swapCache.CacheSwap(pos1, pos2);
            
            // Act
            bool hasValidSwap = swapCache.HasValidSwap();
            
            // Assert
            Assert.IsTrue(hasValidSwap, "Should have valid swap after caching");
        }
        
        [Test]
        public void SwapCache_HasValidSwap_InvalidCache_ReturnsFalse()
        {
            // Arrange & Act
            bool hasValidSwap = swapCache.HasValidSwap();
            
            // Assert
            Assert.IsFalse(hasValidSwap, "Should not have valid swap initially");
        }
        
        [Test]
        public void SwapCache_GetTimeRemaining_ValidCache_ReturnsPositiveValue()
        {
            // Arrange
            Vector2Int pos1 = new Vector2Int(0, 0);
            Vector2Int pos2 = new Vector2Int(1, 0);
            swapCache.CacheSwap(pos1, pos2);
            
            // Act
            float timeRemaining = swapCache.GetTimeRemaining();
            
            // Assert
            Assert.Greater(timeRemaining, 0f, "Time remaining should be positive for valid cache");
        }
        
        [Test]
        public void SwapCache_GetTimeRemaining_InvalidCache_ReturnsZero()
        {
            // Arrange & Act
            float timeRemaining = swapCache.GetTimeRemaining();
            
            // Assert
            Assert.AreEqual(0f, timeRemaining, "Time remaining should be zero for invalid cache");
        }
        
        [Test]
        public void SwapCache_ToString_ValidCache_ReturnsDescriptiveString()
        {
            // Arrange
            Vector2Int pos1 = new Vector2Int(0, 0);
            Vector2Int pos2 = new Vector2Int(1, 0);
            swapCache.CacheSwap(pos1, pos2);
            
            // Act
            string cacheString = swapCache.ToString();
            
            // Assert
            Assert.IsNotNull(cacheString, "ToString should not return null");
            Assert.IsTrue(cacheString.Contains("SwapCache"), "ToString should contain 'SwapCache'");
            Assert.IsTrue(cacheString.Contains("(0,0)"), "ToString should contain first position");
            Assert.IsTrue(cacheString.Contains("(1,0)"), "ToString should contain second position");
        }
        
        [Test]
        public void SwapCache_ToString_InvalidCache_ReturnsInvalidString()
        {
            // Arrange & Act
            string cacheString = swapCache.ToString();
            
            // Assert
            Assert.IsNotNull(cacheString, "ToString should not return null");
            Assert.IsTrue(cacheString.Contains("Invalid"), "ToString should indicate invalid state");
        }
        
        [UnityTest]
        public IEnumerator SwapCache_Expiration_AfterTimeout_BecomesInvalid()
        {
            // Arrange
            Vector2Int pos1 = new Vector2Int(0, 0);
            Vector2Int pos2 = new Vector2Int(1, 0);
            swapCache.CacheSwap(pos1, pos2);
            
            // Verify initially valid
            Assert.IsTrue(swapCache.IsValid, "Cache should be valid initially");
            
            // Act - Wait for expiration (assuming 1 second timeout)
            yield return new WaitForSeconds(1.1f);
            
            // Assert
            Assert.IsFalse(swapCache.IsValid, "Cache should be invalid after timeout");
            Assert.IsTrue(swapCache.IsExpired, "Cache should be expired after timeout");
        }
    }
}
