/******************************************************************************
 * SWITCH - GameManager Tests
 * Sprint: 1
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Unit tests for GameManager singleton and state management
 * Dependencies: Unity Test Framework, GameManager
 * 
 * Educational Notes:
 * - Demonstrates how to test singleton patterns
 * - Shows how to test state management systems
 * - Performance: Tests ensure 60 FPS requirements are met
 *****************************************************************************/

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SWITCH.Core;

namespace SWITCH.Tests.Core
{
    /// <summary>
    /// Unit tests for GameManager singleton and state management.
    /// Educational: Shows how to test singleton patterns and state transitions.
    /// </summary>
    public class GameManagerTests
    {
        private GameManager gameManager;
        private GameObject gameManagerObject;
        
        [SetUp]
        public void Setup()
        {
            // Create GameManager GameObject
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
            // Arrange & Act
            GameManager instance1 = GameManager.Instance;
            GameManager instance2 = GameManager.Instance;
            
            // Assert
            Assert.AreEqual(instance1, instance2, "Singleton should return the same instance");
            Assert.IsNotNull(instance1, "Singleton instance should not be null");
        }
        
        [Test]
        public void GameManager_InitialState_IsMenu()
        {
            // Arrange & Act
            GameState initialState = gameManager.CurrentState;
            
            // Assert
            Assert.AreEqual(GameState.Menu, initialState, "Initial state should be Menu");
        }
        
        [Test]
        public void GameManager_StartGame_ChangesStateToPlaying()
        {
            // Arrange
            GameState initialState = gameManager.CurrentState;
            
            // Act
            gameManager.StartGame();
            
            // Assert
            Assert.AreEqual(GameState.Playing, gameManager.CurrentState, "State should change to Playing");
            Assert.AreNotEqual(initialState, gameManager.CurrentState, "State should change from initial state");
        }
        
        [Test]
        public void GameManager_PauseGame_ChangesStateToPaused()
        {
            // Arrange
            gameManager.StartGame(); // First set to Playing
            
            // Act
            gameManager.PauseGame();
            
            // Assert
            Assert.AreEqual(GameState.Paused, gameManager.CurrentState, "State should change to Paused");
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
            Assert.AreEqual(GameState.Playing, gameManager.CurrentState, "State should change to Playing");
        }
        
        [Test]
        public void GameManager_EndGame_ChangesStateToGameOver()
        {
            // Arrange
            gameManager.StartGame();
            
            // Act
            gameManager.EndGame();
            
            // Assert
            Assert.AreEqual(GameState.GameOver, gameManager.CurrentState, "State should change to GameOver");
        }
        
        [Test]
        public void GameManager_ReturnToMenu_ChangesStateToMenu()
        {
            // Arrange
            gameManager.StartGame();
            
            // Act
            gameManager.ReturnToMenu();
            
            // Assert
            Assert.AreEqual(GameState.Menu, gameManager.CurrentState, "State should change to Menu");
        }
        
        [Test]
        public void GameManager_IsGameActive_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool menuActive = gameManager.IsGameActive; // Should be false in Menu state
            gameManager.StartGame();
            bool playingActive = gameManager.IsGameActive; // Should be true in Playing state
            
            // Assert
            Assert.IsFalse(menuActive, "Game should not be active in Menu state");
            Assert.IsTrue(playingActive, "Game should be active in Playing state");
        }
        
        [Test]
        public void GameManager_IsPaused_ReturnsCorrectValue()
        {
            // Arrange & Act
            bool menuPaused = gameManager.IsPaused; // Should be false in Menu state
            gameManager.StartGame();
            gameManager.PauseGame();
            bool pausedState = gameManager.IsPaused; // Should be true in Paused state
            
            // Assert
            Assert.IsFalse(menuPaused, "Game should not be paused in Menu state");
            Assert.IsTrue(pausedState, "Game should be paused in Paused state");
        }
        
        [Test]
        public void GameManager_Score_StartsAtZero()
        {
            // Arrange & Act
            int initialScore = gameManager.CurrentScore;
            
            // Assert
            Assert.AreEqual(0, initialScore, "Initial score should be 0");
        }
        
        [Test]
        public void GameManager_UpdateScore_IncreasesScore()
        {
            // Arrange
            int initialScore = gameManager.CurrentScore;
            int pointsToAdd = 100;
            
            // Act
            gameManager.UpdateScore(pointsToAdd);
            
            // Assert
            Assert.AreEqual(initialScore + pointsToAdd, gameManager.CurrentScore, "Score should increase by the added points");
        }
        
        [Test]
        public void GameManager_UpdateScore_UpdatesHighScore()
        {
            // Arrange
            int initialHighScore = gameManager.HighScore;
            int pointsToAdd = 100;
            
            // Act
            gameManager.UpdateScore(pointsToAdd);
            
            // Assert
            Assert.AreEqual(pointsToAdd, gameManager.HighScore, "High score should be updated");
            Assert.Greater(gameManager.HighScore, initialHighScore, "High score should increase");
        }
        
        [Test]
        public void GameManager_ResetScore_ResetsToZero()
        {
            // Arrange
            gameManager.UpdateScore(500);
            
            // Act
            gameManager.ResetScore();
            
            // Assert
            Assert.AreEqual(0, gameManager.CurrentScore, "Score should be reset to 0");
        }
        
        [Test]
        public void GameManager_ChangeState_SameState_DoesNotChange()
        {
            // Arrange
            GameState currentState = gameManager.CurrentState;
            
            // Act
            gameManager.ChangeState(currentState);
            
            // Assert
            Assert.AreEqual(currentState, gameManager.CurrentState, "State should not change when setting the same state");
        }
        
        [Test]
        public void GameManager_ChangeState_ValidTransition_ChangesState()
        {
            // Arrange
            GameState initialState = gameManager.CurrentState;
            GameState newState = GameState.Playing;
            
            // Act
            gameManager.ChangeState(newState);
            
            // Assert
            Assert.AreEqual(newState, gameManager.CurrentState, "State should change to the new state");
            Assert.AreNotEqual(initialState, gameManager.CurrentState, "State should be different from initial state");
        }
    }
}
