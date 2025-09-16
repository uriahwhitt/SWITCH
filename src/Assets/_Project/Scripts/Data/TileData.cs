/******************************************************************************
 * SWITCH - TileData
 * Sprint: 0
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Purpose: Data structure for tile information and properties
 * Performance Target: 60 FPS requirement
 *****************************************************************************/

using UnityEngine;

namespace SWITCH.Data
{
    [System.Serializable]
    public class TileData
    {
        [Header("Basic Properties")]
        public TileType tileType;
        public ColorType colorType;
        public Sprite sprite;
        public Sprite shapeSprite; // For accessibility
        
        [Header("Gameplay Properties")]
        public int scoreValue = 100;
        public bool isSpecial = false;
        public SpecialTileType specialType = SpecialTileType.None;
        
        [Header("Visual Properties")]
        public Color color = Color.white;
        public float scale = 1f;
        public bool hasGlow = false;
        public Color glowColor = Color.white;
        
        [Header("Audio Properties")]
        public AudioClip matchSound;
        public AudioClip specialSound;
    }
    
    public enum TileType
    {
        Normal,
        Special,
        PowerUp,
        Obstacle
    }
    
    public enum ColorType
    {
        Red,    // Square
        Blue,   // Circle
        Yellow, // Triangle
        Orange, // Diamond
        Green,  // Star
        Violet  // Hexagon
    }
    
    public enum SpecialTileType
    {
        None,
        Bomb,
        Lightning,
        Rainbow,
        Multiplier
    }
    
    [CreateAssetMenu(fileName = "New Tile Data", menuName = "SWITCH/Tile Data")]
    public class TileDataAsset : ScriptableObject
    {
        public TileData tileData;
        
        private void OnValidate()
        {
            if (tileData != null)
            {
                // Ensure color matches color type
                tileData.color = GetColorForType(tileData.colorType);
            }
        }
        
        private Color GetColorForType(ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.Red: return Color.red;
                case ColorType.Blue: return Color.blue;
                case ColorType.Yellow: return Color.yellow;
                case ColorType.Orange: return new Color(1f, 0.5f, 0f);
                case ColorType.Green: return Color.green;
                case ColorType.Violet: return new Color(0.5f, 0f, 1f);
                default: return Color.white;
            }
        }
    }
}
