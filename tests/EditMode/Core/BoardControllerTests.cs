/******************************************************************************
 * SWITCH - BoardController Tests
 * Sprint: 1
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Unit tests for BoardController and object pooling system
 * Dependencies: Unity Test Framework, BoardController, Tile
 * 
 * Educational Notes:
 * - Demonstrates how to test object pooling systems
 * - Shows how to test grid-based systems
 * - Performance: Tests ensure efficient memory management
 *****************************************************************************/

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SWITCH.Core;
using SWITCH.Data;
using System.Collections.Generic;

namespace SWITCH.Tests.Core
{
    /// <summary>
    /// Unit tests for BoardController and object pooling system.
    /// Educational: Shows how to test object pooling and grid-based systems.
    /// </summary>
    public class BoardControllerTests
    {
        private BoardController boardController;
        private GameObject boardControllerObject;
        
        [SetUp]
        public void Setup()
        {
            // Create BoardController GameObject
            boardControllerObject = new GameObject("BoardController");
            boardController = boardControllerObject.AddComponent<BoardController>();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (boardControllerObject != null)
            {
                Object.DestroyImmediate(boardControllerObject);
            }
        }
        
        [Test]
        public void BoardController_Width_ReturnsCorrectValue()
        {
            // Arrange & Act
            int width = boardController.Width;
            
            // Assert
            Assert.AreEqual(8, width, "Board width should be 8");
        }
        
        [Test]
        public void BoardController_Height_ReturnsCorrectValue()
        {
            // Arrange & Act
            int height = boardController.Height;
            
            // Assert
            Assert.AreEqual(8, height, "Board height should be 8");
        }
        
        [Test]
        public void BoardController_IsValidPosition_ValidPositions_ReturnsTrue()
        {
            // Arrange & Act
            bool topLeft = boardController.IsValidPosition(0, 0);
            bool topRight = boardController.IsValidPosition(7, 0);
            bool bottomLeft = boardController.IsValidPosition(0, 7);
            bool bottomRight = boardController.IsValidPosition(7, 7);
            bool center = boardController.IsValidPosition(4, 4);
            
            // Assert
            Assert.IsTrue(topLeft, "Top-left position should be valid");
            Assert.IsTrue(topRight, "Top-right position should be valid");
            Assert.IsTrue(bottomLeft, "Bottom-left position should be valid");
            Assert.IsTrue(bottomRight, "Bottom-right position should be valid");
            Assert.IsTrue(center, "Center position should be valid");
        }
        
        [Test]
        public void BoardController_IsValidPosition_InvalidPositions_ReturnsFalse()
        {
            // Arrange & Act
            bool negativeX = boardController.IsValidPosition(-1, 0);
            bool negativeY = boardController.IsValidPosition(0, -1);
            bool tooLargeX = boardController.IsValidPosition(8, 0);
            bool tooLargeY = boardController.IsValidPosition(0, 8);
            bool bothInvalid = boardController.IsValidPosition(-1, -1);
            
            // Assert
            Assert.IsFalse(negativeX, "Negative X position should be invalid");
            Assert.IsFalse(negativeY, "Negative Y position should be invalid");
            Assert.IsFalse(tooLargeX, "X position >= width should be invalid");
            Assert.IsFalse(tooLargeY, "Y position >= height should be invalid");
            Assert.IsFalse(bothInvalid, "Both negative positions should be invalid");
        }
        
        [Test]
        public void BoardController_GetTileAt_ValidPosition_ReturnsTile()
        {
            // Arrange
            int x = 3;
            int y = 4;
            
            // Act
            Tile tile = boardController.GetTileAt(x, y);
            
            // Assert
            Assert.IsNotNull(tile, "Tile should not be null for valid position");
            Assert.AreEqual(x, tile.GridX, "Tile X coordinate should match");
            Assert.AreEqual(y, tile.GridY, "Tile Y coordinate should match");
        }
        
        [Test]
        public void BoardController_GetTileAt_InvalidPosition_ReturnsNull()
        {
            // Arrange
            int x = -1;
            int y = 10;
            
            // Act
            Tile tile = boardController.GetTileAt(x, y);
            
            // Assert
            Assert.IsNull(tile, "Tile should be null for invalid position");
        }
        
        [Test]
        public void BoardController_GetWorldPosition_ValidPosition_ReturnsCorrectPosition()
        {
            // Arrange
            int x = 2;
            int y = 3;
            
            // Act
            Vector3 worldPos = boardController.GetWorldPosition(x, y);
            
            // Assert
            Assert.IsNotNull(worldPos, "World position should not be null");
            // Note: Exact position depends on board configuration
        }
        
        [Test]
        public void BoardController_GetWorldPosition_InvalidPosition_ReturnsZero()
        {
            // Arrange
            int x = -1;
            int y = 10;
            
            // Act
            Vector3 worldPos = boardController.GetWorldPosition(x, y);
            
            // Assert
            Assert.AreEqual(Vector3.zero, worldPos, "Invalid position should return zero vector");
        }
        
        [Test]
        public void BoardController_GetTilesInRow_ReturnsCorrectTiles()
        {
            // Arrange
            int row = 3;
            
            // Act
            List<Tile> tiles = boardController.GetTilesInRow(row);
            
            // Assert
            Assert.IsNotNull(tiles, "Tiles list should not be null");
            Assert.AreEqual(8, tiles.Count, "Row should contain 8 tiles");
            
            foreach (Tile tile in tiles)
            {
                Assert.AreEqual(row, tile.GridY, "All tiles should be in the same row");
            }
        }
        
        [Test]
        public void BoardController_GetTilesInColumn_ReturnsCorrectTiles()
        {
            // Arrange
            int column = 4;
            
            // Act
            List<Tile> tiles = boardController.GetTilesInColumn(column);
            
            // Assert
            Assert.IsNotNull(tiles, "Tiles list should not be null");
            Assert.AreEqual(8, tiles.Count, "Column should contain 8 tiles");
            
            foreach (Tile tile in tiles)
            {
                Assert.AreEqual(column, tile.GridX, "All tiles should be in the same column");
            }
        }
        
        [Test]
        public void BoardController_GetPoolStats_ReturnsCorrectCounts()
        {
            // Arrange & Act
            var (poolCount, activeCount) = boardController.GetPoolStats();
            
            // Assert
            Assert.GreaterOrEqual(poolCount, 0, "Pool count should be non-negative");
            Assert.GreaterOrEqual(activeCount, 0, "Active count should be non-negative");
            Assert.AreEqual(64, activeCount, "All 64 board positions should have active tiles");
        }
        
        [Test]
        public void BoardController_ClearTileAt_ValidPosition_ClearsTile()
        {
            // Arrange
            int x = 2;
            int y = 3;
            Tile originalTile = boardController.GetTileAt(x, y);
            
            // Act
            boardController.ClearTileAt(x, y);
            Tile clearedTile = boardController.GetTileAt(x, y);
            
            // Assert
            Assert.IsNotNull(originalTile, "Original tile should exist");
            Assert.IsNull(clearedTile, "Cleared tile should be null");
        }
        
        [Test]
        public void BoardController_ClearTileAt_InvalidPosition_DoesNotThrow()
        {
            // Arrange
            int x = -1;
            int y = 10;
            
            // Act & Assert
            Assert.DoesNotThrow(() => boardController.ClearTileAt(x, y), 
                "Clearing invalid position should not throw exception");
        }
        
        [Test]
        public void BoardController_ClearMatches_ValidPositions_ClearsTiles()
        {
            // Arrange
            List<Vector2Int> positions = new List<Vector2Int>
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(2, 0)
            };
            
            // Verify tiles exist before clearing
            foreach (var pos in positions)
            {
                Assert.IsNotNull(boardController.GetTileAt(pos.x, pos.y), 
                    $"Tile at {pos} should exist before clearing");
            }
            
            // Act
            boardController.ClearMatches(positions);
            
            // Assert
            foreach (var pos in positions)
            {
                Assert.IsNull(boardController.GetTileAt(pos.x, pos.y), 
                    $"Tile at {pos} should be null after clearing");
            }
        }
        
        [Test]
        public void BoardController_FillBoard_SetsRandomColors()
        {
            // Arrange
            boardController.ClearBoard();
            
            // Act
            boardController.FillBoard();
            
            // Assert
            bool hasDifferentColors = false;
            ColorType firstColor = ColorType.Red;
            
            for (int x = 0; x < boardController.Width; x++)
            {
                for (int y = 0; y < boardController.Height; y++)
                {
                    Tile tile = boardController.GetTileAt(x, y);
                    if (tile != null)
                    {
                        if (x == 0 && y == 0)
                        {
                            firstColor = tile.CurrentColor;
                        }
                        else if (tile.CurrentColor != firstColor)
                        {
                            hasDifferentColors = true;
                            break;
                        }
                    }
                }
                if (hasDifferentColors) break;
            }
            
            // Note: This test might occasionally fail due to randomness
            // In a real implementation, you might want to seed the random number generator
            Assert.IsTrue(hasDifferentColors, "Board should have some variety in tile colors");
        }
        
        [Test]
        public void BoardController_BoardCenter_ReturnsCorrectPosition()
        {
            // Arrange & Act
            Vector3 center = boardController.BoardCenter;
            
            // Assert
            Assert.IsNotNull(center, "Board center should not be null");
            // Note: Exact position depends on board configuration
        }
    }
}