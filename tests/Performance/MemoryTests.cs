/******************************************************************************
 * SWITCH - Memory Performance Tests
 * Sprint: 0
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Purpose: Memory performance tests to ensure <200MB usage
 * Performance Target: <200MB memory usage
 *****************************************************************************/

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace SWITCH.Tests.Performance
{
    public class MemoryTests
    {
        private float memoryLimit = 200f; // 200MB limit
        private float tolerance = 50f; // 50MB tolerance
        
        [Test]
        public void Memory_Initialization_WithinLimit()
        {
            // Arrange
            long initialMemory = System.GC.GetTotalMemory(false);
            
            // Act
            var gameManager = new GameObject("GameManager").AddComponent<SWITCH.Core.GameManager>();
            var boardController = new GameObject("BoardController").AddComponent<SWITCH.Core.BoardController>();
            
            // Force garbage collection
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            
            long finalMemory = System.GC.GetTotalMemory(false);
            float memoryUsed = (finalMemory - initialMemory) / (1024f * 1024f); // Convert to MB
            
            // Assert
            Assert.Less(memoryUsed, memoryLimit, 
                $"Memory usage {memoryUsed:F1}MB exceeds limit of {memoryLimit}MB");
            
            // Cleanup
            Object.DestroyImmediate(gameManager.gameObject);
            Object.DestroyImmediate(boardController.gameObject);
        }
        
        [UnityTest]
        public IEnumerator Memory_Gameplay_WithinLimit()
        {
            // Arrange
            var gameManager = new GameObject("GameManager").AddComponent<SWITCH.Core.GameManager>();
            var boardController = new GameObject("BoardController").AddComponent<SWITCH.Core.BoardController>();
            
            // Wait for initialization
            yield return new WaitForSeconds(1f);
            
            // Act - Simulate gameplay
            for (int i = 0; i < 100; i++)
            {
                boardController.FillBoard();
                boardController.ClearBoard();
                
                // Check memory every 10 iterations
                if (i % 10 == 0)
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    System.GC.Collect();
                    
                    long currentMemory = System.GC.GetTotalMemory(false);
                    float memoryUsed = currentMemory / (1024f * 1024f); // Convert to MB
                    
                    Assert.Less(memoryUsed, memoryLimit + tolerance, 
                        $"Memory usage {memoryUsed:F1}MB exceeds limit of {memoryLimit + tolerance}MB");
                }
                
                yield return null;
            }
            
            // Cleanup
            Object.DestroyImmediate(gameManager.gameObject);
            Object.DestroyImmediate(boardController.gameObject);
        }
        
        [UnityTest]
        public IEnumerator Memory_NoLeaks_OverTime()
        {
            // Arrange
            var gameManager = new GameObject("GameManager").AddComponent<SWITCH.Core.GameManager>();
            var boardController = new GameObject("BoardController").AddComponent<SWITCH.Core.BoardController>();
            
            // Wait for initialization
            yield return new WaitForSeconds(1f);
            
            // Measure initial memory
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            
            long initialMemory = System.GC.GetTotalMemory(false);
            
            // Act - Simulate extended gameplay
            for (int i = 0; i < 1000; i++)
            {
                boardController.FillBoard();
                boardController.ClearBoard();
                
                // Check memory every 100 iterations
                if (i % 100 == 0)
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    System.GC.Collect();
                    
                    long currentMemory = System.GC.GetTotalMemory(false);
                    float memoryIncrease = (currentMemory - initialMemory) / (1024f * 1024f); // Convert to MB
                    
                    // Memory increase should be minimal
                    Assert.Less(memoryIncrease, 10f, 
                        $"Memory leak detected: {memoryIncrease:F1}MB increase over {i} iterations");
                }
                
                yield return null;
            }
            
            // Cleanup
            Object.DestroyImmediate(gameManager.gameObject);
            Object.DestroyImmediate(boardController.gameObject);
        }
        
        [Test]
        public void Memory_GarbageCollection_Efficient()
        {
            // Arrange
            long initialMemory = System.GC.GetTotalMemory(false);
            
            // Act - Create and destroy objects
            for (int i = 0; i < 1000; i++)
            {
                var obj = new GameObject($"TestObject_{i}");
                Object.DestroyImmediate(obj);
            }
            
            // Force garbage collection
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            
            long finalMemory = System.GC.GetTotalMemory(false);
            float memoryIncrease = (finalMemory - initialMemory) / (1024f * 1024f); // Convert to MB
            
            // Assert - Memory should be cleaned up efficiently
            Assert.Less(memoryIncrease, 5f, 
                $"Garbage collection inefficient: {memoryIncrease:F1}MB increase after cleanup");
        }
        
        [UnityTest]
        public IEnumerator Memory_ObjectPooling_Efficient()
        {
            // Arrange
            var boardController = new GameObject("BoardController").AddComponent<SWITCH.Core.BoardController>();
            
            // Wait for initialization
            yield return new WaitForSeconds(1f);
            
            // Measure initial memory
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            
            long initialMemory = System.GC.GetTotalMemory(false);
            
            // Act - Simulate object pooling operations
            for (int i = 0; i < 500; i++)
            {
                boardController.FillBoard();
                boardController.ClearBoard();
                
                // Check memory every 50 iterations
                if (i % 50 == 0)
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    System.GC.Collect();
                    
                    long currentMemory = System.GC.GetTotalMemory(false);
                    float memoryIncrease = (currentMemory - initialMemory) / (1024f * 1024f); // Convert to MB
                    
                    // Memory increase should be minimal with object pooling
                    Assert.Less(memoryIncrease, 20f, 
                        $"Object pooling inefficient: {memoryIncrease:F1}MB increase over {i} iterations");
                }
                
                yield return null;
            }
            
            // Cleanup
            Object.DestroyImmediate(boardController.gameObject);
        }
    }
}
