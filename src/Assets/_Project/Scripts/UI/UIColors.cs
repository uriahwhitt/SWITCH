using UnityEngine;

namespace SWITCH.UI
{
    /// <summary>
    /// Static class containing the UI color scheme for SWITCH
    /// </summary>
    public static class UIColors
    {
        // Primary UI colors
        public static Color background = new Color(0.1f, 0.1f, 0.15f);     // Dark blue-gray
        public static Color scoreText = Color.white;                        // High contrast
        public static Color heatCold = new Color(0.3f, 0.5f, 0.8f);        // Cool blue
        public static Color heatWarm = new Color(1f, 1f, 0.3f);            // Yellow
        public static Color heatHot = new Color(1f, 0.6f, 0.2f);           // Orange
        public static Color heatBlazing = new Color(1f, 0.3f, 0.1f);       // Intense red
        public static Color heatInferno = new Color(1f, 1f, 1f);           // White
        public static Color powerReady = new Color(1f, 0.9f, 0.3f);        // Golden yellow
        
        // Edge glow colors for power orbs
        public static Color edgeTop = Color.blue;                           // Top edge
        public static Color edgeRight = Color.green;                        // Right edge
        public static Color edgeBottom = Color.yellow;                      // Bottom edge
        public static Color edgeLeft = Color.magenta;                       // Left edge
        
        // UI element colors
        public static Color menuButton = new Color(0.8f, 0.8f, 0.8f);      // Light gray
        public static Color cascadeText = new Color(1f, 0.8f, 0.2f);       // Orange-yellow
        public static Color pointsPopup = new Color(0.2f, 1f, 0.2f);       // Bright green
        
        // Ad banner colors
        public static Color adBackground = new Color(0.2f, 0.2f, 0.2f);    // Dark gray
        public static Color adText = new Color(0.7f, 0.7f, 0.7f);          // Light gray
        
        // Queue dot colors
        public static Color queueDotActive = new Color(1f, 1f, 1f);        // White
        public static Color queueDotInactive = new Color(0.4f, 0.4f, 0.4f); // Dark gray
        
        /// <summary>
        /// Gets the heat color based on heat level
        /// </summary>
        /// <param name="heatLevel">Heat level (0-10)</param>
        /// <returns>Color for the heat level</returns>
        public static Color GetHeatColor(float heatLevel)
        {
            if (heatLevel <= 2f) return heatCold;
            if (heatLevel <= 4f) return heatWarm;
            if (heatLevel <= 7f) return heatHot;
            if (heatLevel <= 9f) return heatBlazing;
            return heatInferno;
        }
        
        /// <summary>
        /// Gets the heat label based on heat level
        /// </summary>
        /// <param name="heatLevel">Heat level (0-10)</param>
        /// <returns>Label string for the heat level</returns>
        public static string GetHeatLabel(float heatLevel)
        {
            if (heatLevel <= 2f) return "COLD";
            if (heatLevel <= 4f) return "WARM";
            if (heatLevel <= 7f) return "HOT!";
            if (heatLevel <= 9f) return "BLAZING!";
            return "INFERNO!";
        }
    }
}
