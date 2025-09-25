using UnityEngine;

namespace Switch.Core
{
    /// <summary>
    /// Simple smoke test to verify basic Unity functionality
    /// Tests scene loading, script compilation, FPS, and memory usage
    /// </summary>
    public class SmokeTest : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private bool runTestsOnStart = true;
        [SerializeField] private float testDuration = 5f;
        
        private float testStartTime;
        private bool testsCompleted = false;
        
        void Start()
        {
            if (runTestsOnStart)
            {
                RunSmokeTests();
            }
        }
        
        void Update()
        {
            if (!testsCompleted && Time.time - testStartTime >= testDuration)
            {
                CompleteTests();
            }
        }
        
        private void RunSmokeTests()
        {
            testStartTime = Time.time;
            testsCompleted = false;
            
            Debug.Log("=== SWITCH SMOKE TESTS STARTING ===");
            Debug.Log("✓ Scene loads successfully");
            Debug.Log("✓ Scripts compile without errors");
            
            // Test basic Unity functionality
            TestBasicUnityFeatures();
            
            // Start performance monitoring
            StartCoroutine(MonitorPerformance());
        }
        
        private void TestBasicUnityFeatures()
        {
            // Test GameObject creation
            GameObject testObject = new GameObject("SmokeTestObject");
            Debug.Log("✓ GameObject creation works");
            
            // Test component attachment
            var testComponent = testObject.AddComponent<BoxCollider2D>();
            Debug.Log("✓ Component attachment works");
            
            // Clean up
            DestroyImmediate(testObject);
            Debug.Log("✓ Object destruction works");
        }
        
        private System.Collections.IEnumerator MonitorPerformance()
        {
            float frameCount = 0;
            float totalTime = 0;
            
            while (!testsCompleted)
            {
                frameCount++;
                totalTime += Time.deltaTime;
                
                if (frameCount % 60 == 0) // Log every 60 frames
                {
                    float currentFPS = frameCount / totalTime;
                    float memoryMB = System.GC.GetTotalMemory(false) / 1048576f;
                    
                    Debug.Log($"Performance: FPS={currentFPS:F1}, Memory={memoryMB:F1}MB");
                    
                    // Check for performance issues
                    if (currentFPS < 30f)
                    {
                        Debug.LogWarning($"⚠️ Low FPS detected: {currentFPS:F1}");
                    }
                    
                    if (memoryMB > 100f)
                    {
                        Debug.LogWarning($"⚠️ High memory usage: {memoryMB:F1}MB");
                    }
                }
                
                yield return null;
            }
        }
        
        private void CompleteTests()
        {
            testsCompleted = true;
            
            float finalFPS = 1.0f / Time.deltaTime;
            float finalMemory = System.GC.GetTotalMemory(false) / 1048576f;
            
            Debug.Log("=== SMOKE TESTS COMPLETED ===");
            Debug.Log($"✓ Final FPS: {finalFPS:F1}");
            Debug.Log($"✓ Final Memory: {finalMemory:F1}MB");
            Debug.Log($"✓ Test Duration: {testDuration}s");
            
            // Performance validation
            bool performanceOK = finalFPS >= 30f && finalMemory <= 100f;
            
            if (performanceOK)
            {
                Debug.Log("✅ ALL TESTS PASSED - Ready for development!");
            }
            else
            {
                Debug.LogError("❌ PERFORMANCE ISSUES DETECTED - Check configuration");
            }
        }
        
        [ContextMenu("Run Smoke Tests")]
        public void RunTestsManually()
        {
            RunSmokeTests();
        }
    }
}
