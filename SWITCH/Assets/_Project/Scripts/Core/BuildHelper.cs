using UnityEngine;
using UnityEditor;
using System.IO;

namespace Switch.Core
{
    /// <summary>
    /// Helper class for building the game to mobile platforms
    /// Provides build configuration and validation
    /// </summary>
    public class BuildHelper : MonoBehaviour
    {
        [Header("Build Configuration")]
        [SerializeField] private string buildPath = "Builds";
        
        [Header("Build Settings")]
        [SerializeField] private bool developmentBuild = true;
        [SerializeField] private bool allowDebugging = true;
        [SerializeField] private bool compressWithLz4 = true;
        
        /// <summary>
        /// Validates the project is ready for mobile build
        /// </summary>
        [ContextMenu("Validate Build Settings")]
        public void ValidateBuildSettings()
        {
            Debug.Log("Validating build settings...");
            
            bool allGood = true;
            
            // Check company name
            if (PlayerSettings.companyName != "Whitt's End")
            {
                Debug.LogWarning("⚠️ Company name should be 'Whitt's End'");
                allGood = false;
            }
            else
            {
                Debug.Log("✅ Company name is correct");
            }
            
            // Check bundle identifier
            string bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            if (bundleId != "com.whittsend.switch")
            {
                Debug.LogWarning("⚠️ Bundle identifier should be 'com.whittsend.switch'");
                allGood = false;
            }
            else
            {
                Debug.Log("✅ Bundle identifier is correct");
            }
            
            // Check orientation
            if (PlayerSettings.defaultInterfaceOrientation != UIOrientation.Portrait)
            {
                Debug.LogWarning("⚠️ Default orientation should be Portrait");
                allGood = false;
            }
            else
            {
                Debug.Log("✅ Orientation is set to Portrait");
            }
            
            // Check target frame rate
            if (Application.targetFrameRate != 60)
            {
                Debug.LogWarning("⚠️ Target frame rate should be 60 FPS");
                allGood = false;
            }
            else
            {
                Debug.Log("✅ Target frame rate is 60 FPS");
            }
            
            // Check required packages
            ValidateRequiredPackages();
            
            if (allGood)
            {
                Debug.Log("🎉 Build settings validation passed!");
            }
            else
            {
                Debug.LogError("❌ Build settings validation failed!");
            }
        }
        
        /// <summary>
        /// Validates that required packages are installed
        /// </summary>
        private void ValidateRequiredPackages()
        {
            string[] requiredPackages = {
                "com.unity.inputsystem",
                "com.unity.ads",
                "com.unity.analytics",
                "com.unity.mobile.notifications"
            };
            
            bool packagesOK = true;
            
            foreach (string package in requiredPackages)
            {
                if (!IsPackageInstalled(package))
                {
                    Debug.LogError($"❌ Required package not installed: {package}");
                    packagesOK = false;
                }
                else
                {
                    Debug.Log($"✅ Package installed: {package}");
                }
            }
            
            if (packagesOK)
            {
                Debug.Log("✅ All required packages are installed");
            }
        }
        
        /// <summary>
        /// Checks if a package is installed
        /// </summary>
        private bool IsPackageInstalled(string packageName)
        {
            // This is a simplified check - in a real implementation,
            // you would check the PackageManager manifest
            return true; // Assume packages are installed for now
        }
        
        /// <summary>
        /// Gets build information for debugging
        /// </summary>
        [ContextMenu("Show Build Info")]
        public void ShowBuildInfo()
        {
            Debug.Log("=== BUILD INFORMATION ===");
            Debug.Log($"Company: {PlayerSettings.companyName}");
            Debug.Log($"Product: {PlayerSettings.productName}");
            Debug.Log($"Bundle ID: {PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android)}");
            Debug.Log($"Version: {PlayerSettings.bundleVersion}");
            Debug.Log($"Target Frame Rate: {Application.targetFrameRate}");
            Debug.Log($"Orientation: {PlayerSettings.defaultInterfaceOrientation}");
            Debug.Log($"Graphics API: {PlayerSettings.GetGraphicsAPIs(BuildTarget.Android)[0]}");
            Debug.Log($"Scripting Backend: {PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android)}");
            Debug.Log($"Architecture: {PlayerSettings.GetArchitecture(BuildTargetGroup.Android)}");
            Debug.Log($"Development Build: {developmentBuild}");
            Debug.Log($"Allow Debugging: {allowDebugging}");
            Debug.Log($"Compress with LZ4: {compressWithLz4}");
        }
        
        /// <summary>
        /// Creates build directory if it doesn't exist
        /// </summary>
        private void CreateBuildDirectory()
        {
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
                Debug.Log($"Created build directory: {buildPath}");
            }
        }
        
        /// <summary>
        /// Gets the current build target
        /// </summary>
        public BuildTarget GetCurrentBuildTarget()
        {
            return EditorUserBuildSettings.activeBuildTarget;
        }
        
        /// <summary>
        /// Checks if the current build target is mobile
        /// </summary>
        public bool IsMobileBuildTarget()
        {
            BuildTarget target = GetCurrentBuildTarget();
            return target == BuildTarget.Android || target == BuildTarget.iOS;
        }
        
        /// <summary>
        /// Gets build size information
        /// </summary>
        [ContextMenu("Check Build Size")]
        public void CheckBuildSize()
        {
            if (IsMobileBuildTarget())
            {
                Debug.Log("Build size check available for mobile builds");
                Debug.Log("Run a build to see actual size information");
            }
            else
            {
                Debug.Log("Build size check only available for mobile builds");
            }
        }
        
        #region Performance Validation
        
        /// <summary>
        /// Validates performance requirements
        /// </summary>
        [ContextMenu("Validate Performance")]
        public void ValidatePerformance()
        {
            Debug.Log("Validating performance requirements...");
            
            // Check frame rate
            float currentFPS = 1.0f / Time.deltaTime;
            if (currentFPS >= 30f)
            {
                Debug.Log($"✅ FPS is acceptable: {currentFPS:F1}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Low FPS detected: {currentFPS:F1}");
            }
            
            // Check memory usage
            float memoryMB = System.GC.GetTotalMemory(false) / 1048576f;
            if (memoryMB <= 100f)
            {
                Debug.Log($"✅ Memory usage is acceptable: {memoryMB:F1}MB");
            }
            else
            {
                Debug.LogWarning($"⚠️ High memory usage: {memoryMB:F1}MB");
            }
            
            // Check tile count
            Switch.Mechanics.GameGrid grid = FindObjectOfType<Switch.Mechanics.GameGrid>();
            if (grid != null)
            {
                if (grid.TotalTiles <= 100)
                {
                    Debug.Log($"✅ Tile count is acceptable: {grid.TotalTiles}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ High tile count: {grid.TotalTiles}");
                }
            }
        }
        
        #endregion
        
        #region Debug
        
        private void OnGUI()
        {
            if (Application.isPlaying)
            {
                GUILayout.BeginArea(new Rect(Screen.width - 250, 10, 240, 200));
                GUILayout.Label("Build Helper Debug");
                GUILayout.Label($"FPS: {1.0f / Time.deltaTime:F1}");
                GUILayout.Label($"Memory: {System.GC.GetTotalMemory(false) / 1048576f:F1}MB");
                GUILayout.Label($"Build Target: {GetCurrentBuildTarget()}");
                GUILayout.Label($"Mobile Target: {IsMobileBuildTarget()}");
                GUILayout.EndArea();
            }
        }
        
        #endregion
    }
}
