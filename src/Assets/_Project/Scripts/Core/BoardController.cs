/******************************************************************************
 * SWITCH - BoardController
 * Sprint: 0
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Purpose: Manages 8x8 game board and tile placement
 * Performance Target: 60 FPS requirement
 *****************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace SWITCH.Core
{
    public class BoardController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private int boardWidth = 8;
        [SerializeField] private int boardHeight = 8;
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private float tileSpacing = 0.1f;
        
        [Header("References")]
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform boardParent;
        
        // Board state
        private Tile[,] board;
        private Vector3 boardCenter;
        
        // Properties
        public int Width => boardWidth;
        public int Height => boardHeight;
        public Vector3 BoardCenter => boardCenter;
        
        private void Awake()
        {
            InitializeBoard();
        }
        
        private void InitializeBoard()
        {
            // Create board array
            board = new Tile[boardWidth, boardHeight];
            
            // Calculate board center
            boardCenter = transform.position;
            
            // Create tile grid
            CreateTileGrid();
        }
        
        private void CreateTileGrid()
        {
            if (boardParent == null)
            {
                boardParent = new GameObject("Board").transform;
                boardParent.SetParent(transform);
            }
            
            Vector3 startPosition = CalculateStartPosition();
            
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    Vector3 position = startPosition + new Vector3(
                        x * (tileSize + tileSpacing),
                        y * (tileSize + tileSpacing),
                        0f
                    );
                    
                    CreateTileAt(x, y, position);
                }
            }
        }
        
        private Vector3 CalculateStartPosition()
        {
            float totalWidth = boardWidth * tileSize + (boardWidth - 1) * tileSpacing;
            float totalHeight = boardHeight * tileSize + (boardHeight - 1) * tileSpacing;
            
            return boardCenter - new Vector3(totalWidth * 0.5f, totalHeight * 0.5f, 0f);
        }
        
        private void CreateTileAt(int x, int y, Vector3 position)
        {
            GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity, boardParent);
            Tile tile = tileObj.GetComponent<Tile>();
            
            if (tile == null)
            {
                tile = tileObj.AddComponent<Tile>();
            }
            
            tile.Initialize(x, y);
            board[x, y] = tile;
        }
        
        public Tile GetTileAt(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return board[x, y];
            }
            return null;
        }
        
        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < boardWidth && y >= 0 && y < boardHeight;
        }
        
        public Vector3 GetWorldPosition(int x, int y)
        {
            if (!IsValidPosition(x, y)) return Vector3.zero;
            
            Vector3 startPosition = CalculateStartPosition();
            return startPosition + new Vector3(
                x * (tileSize + tileSpacing),
                y * (tileSize + tileSpacing),
                0f
            );
        }
        
        public List<Tile> GetTilesInRow(int row)
        {
            var tiles = new List<Tile>();
            for (int x = 0; x < boardWidth; x++)
            {
                if (board[x, row] != null)
                {
                    tiles.Add(board[x, row]);
                }
            }
            return tiles;
        }
        
        public List<Tile> GetTilesInColumn(int column)
        {
            var tiles = new List<Tile>();
            for (int y = 0; y < boardHeight; y++)
            {
                if (board[column, y] != null)
                {
                    tiles.Add(board[column, y]);
                }
            }
            return tiles;
        }
        
        public void ClearBoard()
        {
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (board[x, y] != null)
                    {
                        board[x, y].Clear();
                    }
                }
            }
        }
        
        public void FillBoard()
        {
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (board[x, y] != null)
                    {
                        board[x, y].SetRandomColor();
                    }
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            if (board == null) return;
            
            Gizmos.color = Color.yellow;
            Vector3 startPosition = CalculateStartPosition();
            
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    Vector3 position = startPosition + new Vector3(
                        x * (tileSize + tileSpacing),
                        y * (tileSize + tileSpacing),
                        0f
                    );
                    
                    Gizmos.DrawWireCube(position, Vector3.one * tileSize);
                }
            }
        }
    }
}
