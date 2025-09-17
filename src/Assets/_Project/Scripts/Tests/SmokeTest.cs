using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace SWITCH.Tests
{
    /// <summary>
    /// Smoke tests to verify Unity project setup is working correctly
    /// </summary>
    public class SmokeTest
    {
        [Test]
        public void UnityProjectSetup_ShouldHaveCorrectVersion()
        {
            // Verify we're running in Unity
            Assert.IsNotNull(Application.unityVersion);
            Debug.Log($"Unity Version: {Application.unityVersion}");
            
            // Verify we're in the correct project
            Assert.AreEqual("SWITCH", Application.productName);
            Assert.AreEqual("Whitt's End", Application.companyName);
        }

        [Test]
        public void ProjectStructure_ShouldHaveRequiredFolders()
        {
            // Verify core folders exist
            string[] requiredFolders = {
                "Assets/_Project/Scripts/Core",
                "Assets/_Project/Scripts/UI",
                "Assets/_Project/Scripts/Data",
                "Assets/_Project/Scripts/PowerUps",
                "Assets/_Project/Scripts/Services",
                "Assets/_Project/Scenes",
                "Assets/_Project/Prefabs",
                "Assets/_Project/Materials",
                "Assets/_Project/Audio",
                "Assets/_Project/Sprites"
            };

            foreach (string folder in requiredFolders)
            {
                Assert.IsTrue(System.IO.Directory.Exists(folder), 
                    $"Required folder missing: {folder}");
            }
        }

        [Test]
        public void CoreScripts_ShouldExist()
        {
            // Verify core scripts exist
            string[] requiredScripts = {
                "Assets/_Project/Scripts/Core/GameManager.cs",
                "Assets/_Project/Scripts/Core/BoardController.cs",
                "Assets/_Project/Scripts/Core/Tile.cs",
                "Assets/_Project/Scripts/Core/MomentumSystem.cs",
                "Assets/_Project/Scripts/Core/TurnScoreCalculator.cs"
            };

            foreach (string script in requiredScripts)
            {
                Assert.IsTrue(System.IO.File.Exists(script), 
                    $"Required script missing: {script}");
            }
        }

        [Test]
        public void Scenes_ShouldExist()
        {
            // Verify scenes exist
            string[] requiredScenes = {
                "Assets/_Project/Scenes/Main.unity",
                "Assets/_Project/Scenes/Game.unity",
                "Assets/_Project/Scenes/Menu.unity"
            };

            foreach (string scene in requiredScenes)
            {
                Assert.IsTrue(System.IO.File.Exists(scene), 
                    $"Required scene missing: {scene}");
            }
        }

        [UnityTest]
        public IEnumerator GameManager_ShouldInitializeWithoutErrors()
        {
            // Create a temporary GameManager to test initialization
            GameObject testObject = new GameObject("TestGameManager");
            GameManager gameManager = testObject.AddComponent<GameManager>();
            
            // Wait one frame for initialization
            yield return null;
            
            // Verify GameManager exists and is enabled
            Assert.IsNotNull(gameManager);
            Assert.IsTrue(gameManager.enabled);
            
            // Clean up
            Object.DestroyImmediate(testObject);
        }

        [Test]
        public void ProjectSettings_ShouldBeConfigured()
        {
            // Verify project settings
            Assert.AreEqual("SWITCH", PlayerSettings.productName);
            Assert.AreEqual("Whitt's End", PlayerSettings.companyName);
            Assert.AreEqual("0.1.0", PlayerSettings.bundleVersion);
            
            // Verify mobile settings
            Assert.AreEqual(AndroidSdkVersions.AndroidApiLevel26, PlayerSettings.Android.minSdkVersion);
            Assert.AreEqual(AndroidSdkVersions.AndroidApiLevelAuto, PlayerSettings.Android.targetSdkVersion);
        }

        [Test]
        public void PackageManifest_ShouldHaveRequiredPackages()
        {
            // Verify package manifest exists
            string manifestPath = "Packages/manifest.json";
            Assert.IsTrue(System.IO.File.Exists(manifestPath), 
                "Package manifest.json missing");
            
            // Read and verify manifest content
            string manifestContent = System.IO.File.ReadAllText(manifestPath);
            Assert.IsTrue(manifestContent.Contains("com.unity.inputsystem"), 
                "Input System package missing");
            Assert.IsTrue(manifestContent.Contains("com.unity.textmeshpro"), 
                "TextMeshPro package missing");
            Assert.IsTrue(manifestContent.Contains("com.unity.2d.sprite"), 
                "2D Sprite package missing");
            Assert.IsTrue(manifestContent.Contains("com.unity.test-framework"), 
                "Test Framework package missing");
        }

        [Test]
        public void BuildSettings_ShouldHaveScenesConfigured()
        {
            // Verify build settings have scenes
            var scenes = EditorBuildSettings.scenes;
            Assert.Greater(scenes.Length, 0, "No scenes in build settings");
            
            // Verify main scenes are included
            bool hasMainScene = false;
            bool hasGameScene = false;
            bool hasMenuScene = false;
            
            foreach (var scene in scenes)
            {
                if (scene.path.Contains("Main.unity")) hasMainScene = true;
                if (scene.path.Contains("Game.unity")) hasGameScene = true;
                if (scene.path.Contains("Menu.unity")) hasMenuScene = true;
            }
            
            Assert.IsTrue(hasMainScene, "Main scene not in build settings");
            Assert.IsTrue(hasGameScene, "Game scene not in build settings");
            Assert.IsTrue(hasMenuScene, "Menu scene not in build settings");
        }
    }
}
