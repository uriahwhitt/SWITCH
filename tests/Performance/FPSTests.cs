/******************************************************************************
 * SWITCH - FPS Performance Tests
 * Sprint: 0
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Purpose: Performance tests to ensure 60 FPS target
 * Performance Target: 60 FPS requirement
 *****************************************************************************/

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace SWITCH.Tests.Performance
{
    public class FPSTests
    {
        private float targetFPS = 60f;
        private float tolerance = 5f; // 5 FPS tolerance
        
        [UnityTest]
        public IEnumerator GameManager_Performance_MaintainsTargetFPS()
        {
            // Arrange
            var gameManager = new GameObject("GameManager").AddComponent<SWITCH.Core.GameManager>();
            
            // Act
            gameManager.StartGame();
            
            // Wait for initialization
            yield return new WaitForSeconds(1f);
            
            // Measure FPS over time
            float totalFPS = 0f;
            int measurements = 0;
            float testDuration = 5f;
            float startTime = Time.time;
            
            while (Time.time - startTime < testDuration)
            {
                float fps = 1f / Time.deltaTime;
                totalFPS += fps;
                measurements++;
                yield return null;
            }
            
            // Calculate average FPS
            float averageFPS = totalFPS / measurements;
            
            // Assert
            Assert.GreaterOrEqual(averageFPS, targetFPS - tolerance, 
                $"Average FPS {averageFPS:F1} is below target {targetFPS} (tolerance: {tolerance})");
            
            // Cleanup
            Object.DestroyImmediate(gameManager.gameObject);
        }
        
        [UnityTest]
        public IEnumerator BoardController_Performance_MaintainsTargetFPS()
        {
            // Arrange
            var boardController = new GameObject("BoardController").AddComponent<SWITCH.Core.BoardController>();
            
            // Wait for initialization
            yield return new WaitForSeconds(1f);
            
            // Measure FPS during board operations
            float totalFPS = 0f;
            int measurements = 0;
            float testDuration = 3f;
            float startTime = Time.time;
            
            while (Time.time - startTime < testDuration)
            {
                // Perform board operations
                boardController.FillBoard();
                boardController.ClearBoard();
                
                float fps = 1f / Time.deltaTime;
                totalFPS += fps;
                measurements++;
                yield return null;
            }
            
            // Calculate average FPS
            float averageFPS = totalFPS / measurements;
            
            // Assert
            Assert.GreaterOrEqual(averageFPS, targetFPS - tolerance, 
                $"Average FPS {averageFPS:F1} is below target {targetFPS} (tolerance: {tolerance})");
            
            // Cleanup
            Object.DestroyImmediate(boardController.gameObject);
        }
        
        [UnityTest]
        public IEnumerator TileOperations_Performance_MaintainsTargetFPS()
        {
            // Arrange
            var boardController = new GameObject("BoardController").AddComponent<SWITCH.Core.BoardController>();
            
            // Wait for initialization
            yield return new WaitForSeconds(1f);
            
            // Measure FPS during tile operations
            float totalFPS = 0f;
            int measurements = 0;
            float testDuration = 3f;
            float startTime = Time.time;
            
            while (Time.time - startTime < testDuration)
            {
                // Perform tile operations
                for (int x = 0; x < boardController.Width; x++)
                {
                    for (int y = 0; y < boardController.Height; y++)
                    {
                        var tile = boardController.GetTileAt(x, y);
                        if (tile != null)
                        {
                            tile.SetRandomColor();
                        }
                    }
                }
                
                float fps = 1f / Time.deltaTime;
                totalFPS += fps;
                measurements++;
                yield return null;
            }
            
            // Calculate average FPS
            float averageFPS = totalFPS / measurements;
            
            // Assert
            Assert.GreaterOrEqual(averageFPS, targetFPS - tolerance, 
                $"Average FPS {averageFPS:F1} is below target {targetFPS} (tolerance: {tolerance})");
            
            // Cleanup
            Object.DestroyImmediate(boardController.gameObject);
        }
        
        [Test]
        public void Performance_Initialization_CompletesWithinTimeLimit()
        {
            // Arrange
            float timeLimit = 1f; // 1 second limit
            float startTime = Time.realtimeSinceStartup;
            
            // Act
            var gameManager = new GameObject("GameManager").AddComponent<SWITCH.Core.GameManager>();
            var boardController = new GameObject("BoardController").AddComponent<SWITCH.Core.BoardController>();
            
            float endTime = Time.realtimeSinceStartup;
            float duration = endTime - startTime;
            
            // Assert
            Assert.Less(duration, timeLimit, 
                $"Initialization took {duration:F3} seconds, exceeding limit of {timeLimit} seconds");
            
            // Cleanup
            Object.DestroyImmediate(gameManager.gameObject);
            Object.DestroyImmediate(boardController.gameObject);
        }
    }
}
