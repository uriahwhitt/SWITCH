/******************************************************************************
 * SWITCH - GameManager Tests
 * Sprint: 0
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Purpose: Play mode tests for GameManager functionality
 * Performance Target: 60 FPS requirement
 *****************************************************************************/

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SWITCH.Core;

namespace SWITCH.Tests.PlayMode
{
    public class GameManagerTests
    {
        private GameManager gameManager;
        private GameObject gameManagerObject;
        
        [SetUp]
        public void Setup()
        {
            // Create GameManager for testing
            gameManagerObject = new GameObject("GameManager");
            gameManager = gameManagerObject.AddComponent<GameManager>();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (gameManagerObject != null)
            {
                Object.DestroyImmediate(gameManagerObject);
            }
        }
        
        [Test]
        public void GameManager_Singleton_ReturnsSameInstance()
        {
            // Arrange
            var instance1 = GameManager.Instance;
            var instance2 = GameManager.Instance;
            
            // Assert
            Assert.AreEqual(instance1, instance2);
            Assert.IsNotNull(instance1);
        }
        
        [Test]
        public void GameManager_InitialState_IsMenu()
        {
            // Assert
            Assert.AreEqual(GameState.Menu, gameManager.CurrentState);
        }
        
        [Test]
        public void GameManager_StartGame_ChangesStateToPlaying()
        {
            // Act
            gameManager.StartGame();
            
            // Assert
            Assert.AreEqual(GameState.Playing, gameManager.CurrentState);
            Assert.IsTrue(gameManager.IsGameActive);
        }
        
        [Test]
        public void GameManager_PauseGame_ChangesStateToPaused()
        {
            // Arrange
            gameManager.StartGame();
            
            // Act
            gameManager.PauseGame();
            
            // Assert
            Assert.AreEqual(GameState.Paused, gameManager.CurrentState);
            Assert.IsTrue(gameManager.IsPaused);
        }
        
        [Test]
        public void GameManager_ResumeGame_ChangesStateToPlaying()
        {
            // Arrange
            gameManager.StartGame();
            gameManager.PauseGame();
            
            // Act
            gameManager.ResumeGame();
            
            // Assert
            Assert.AreEqual(GameState.Playing, gameManager.CurrentState);
            Assert.IsTrue(gameManager.IsGameActive);
        }
        
        [Test]
        public void GameManager_EndGame_ChangesStateToGameOver()
        {
            // Arrange
            gameManager.StartGame();
            
            // Act
            gameManager.EndGame();
            
            // Assert
            Assert.AreEqual(GameState.GameOver, gameManager.CurrentState);
            Assert.IsFalse(gameManager.IsGameActive);
        }
        
        [Test]
        public void GameManager_ReturnToMenu_ChangesStateToMenu()
        {
            // Arrange
            gameManager.StartGame();
            
            // Act
            gameManager.ReturnToMenu();
            
            // Assert
            Assert.AreEqual(GameState.Menu, gameManager.CurrentState);
            Assert.IsFalse(gameManager.IsGameActive);
        }
        
        [UnityTest]
        public IEnumerator GameManager_StateChange_TriggersCorrectBehavior()
        {
            // Arrange
            gameManager.StartGame();
            
            // Act
            gameManager.PauseGame();
            
            // Assert
            Assert.AreEqual(0f, Time.timeScale);
            
            // Act
            gameManager.ResumeGame();
            
            // Assert
            Assert.AreEqual(1f, Time.timeScale);
            
            yield return null;
        }
    }
}
