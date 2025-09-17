/******************************************************************************
 * SWITCH - PerformanceProfiler
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Performance profiling and optimization system
 * Dependencies: Unity Profiler, System.Diagnostics
 * 
 * Educational Notes:
 * - Demonstrates performance monitoring and optimization
 * - Shows how to implement profiling systems
 * - Performance: Minimal overhead profiling with efficient data collection
 *****************************************************************************/

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace SWITCH.Performance
{
    /// <summary>
    /// Performance profiling and optimization system.
    /// Educational: This demonstrates performance monitoring and optimization.
    /// Performance: Minimal overhead profiling with efficient data collection.
    /// </summary>
    public class PerformanceProfiler : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private bool enableProfiling = true;
        [SerializeField] private bool enableRealTimeMonitoring = true;
        
        [Header("Performance Targets")]
        [SerializeField] private float targetFPS = 60f;
        [SerializeField] private float maxFrameTime = 16.67f; // 60 FPS
        [SerializeField] private float maxMemoryUsage = 200f; // MB
        [SerializeField] private int maxDrawCalls = 100;
        [SerializeField] private int maxTriangles = 100000;
        
        [Header("Monitoring Settings")]
        [SerializeField] private float monitoringInterval = 1f;
        [SerializeField] private int maxSamples = 1000;
        [SerializeField] private bool enableDetailedProfiling = false;
        
        // Performance data
        private List<PerformanceSample> performanceSamples = new List<PerformanceSample>();
        private Dictionary<string, PerformanceMetric> performanceMetrics = new Dictionary<string, PerformanceMetric>();
        private Stopwatch frameTimer = new Stopwatch();
        private float lastMonitoringTime = 0f;
        
        // Current performance state
        private float currentFPS = 0f;
        private float currentFrameTime = 0f;
        private float currentMemoryUsage = 0f;
        private int currentDrawCalls = 0;
        private int currentTriangles = 0;
        private bool isPerformanceGood = true;
        
        // Performance tracking
        private int totalFrames = 0;
        private int droppedFrames = 0;
        private float totalFrameTime = 0f;
        private float peakMemoryUsage = 0f;
        private int performanceWarnings = 0;
        
        // Events
        public System.Action<PerformanceSample> OnPerformanceSample;
        public System.Action<PerformanceWarning> OnPerformanceWarning;
        public System.Action<bool> OnPerformanceStateChanged;
        
        // Singleton instance
        private static PerformanceProfiler instance;
        public static PerformanceProfiler Instance => instance;
        
        // Properties
        public float CurrentFPS => currentFPS;
        public float CurrentFrameTime => currentFrameTime;
        public float CurrentMemoryUsage => currentMemoryUsage;
        public bool IsPerformanceGood => isPerformanceGood;
        public int TotalFrames => totalFrames;
        public int DroppedFrames => droppedFrames;
        
        private void Awake()
        {
            // Singleton setup
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            InitializePerformanceProfiler();
        }
        
        private void Update()
        {
            if (enableProfiling)
            {
                UpdatePerformanceMonitoring();
            }
        }
        
        /// <summary>
        /// Initializes the performance profiler.
        /// Educational: Shows how to set up performance monitoring.
        /// </summary>
        private void InitializePerformanceProfiler()
        {
            Log("Initializing Performance Profiler");
            
            // Initialize performance metrics
            InitializePerformanceMetrics();
            
            // Reset performance data
            ResetPerformanceData();
            
            Log("Performance Profiler initialized");
        }
        
        /// <summary>
        /// Initializes performance metrics.
        /// Educational: Shows how to set up performance metrics.
        /// </summary>
        private void InitializePerformanceMetrics()
        {
            performanceMetrics.Clear();
            
            // FPS metric
            performanceMetrics["fps"] = new PerformanceMetric
            {
                metricId = "fps",
                metricName = "Frames Per Second",
                targetValue = targetFPS,
                warningThreshold = targetFPS * 0.8f,
                criticalThreshold = targetFPS * 0.6f,
                isHigherBetter = true
            };
            
            // Frame time metric
            performanceMetrics["frame_time"] = new PerformanceMetric
            {
                metricId = "frame_time",
                metricName = "Frame Time (ms)",
                targetValue = maxFrameTime,
                warningThreshold = maxFrameTime * 1.2f,
                criticalThreshold = maxFrameTime * 1.5f,
                isHigherBetter = false
            };
            
            // Memory usage metric
            performanceMetrics["memory"] = new PerformanceMetric
            {
                metricId = "memory",
                metricName = "Memory Usage (MB)",
                targetValue = maxMemoryUsage,
                warningThreshold = maxMemoryUsage * 0.8f,
                criticalThreshold = maxMemoryUsage * 0.9f,
                isHigherBetter = false
            };
            
            // Draw calls metric
            performanceMetrics["draw_calls"] = new PerformanceMetric
            {
                metricId = "draw_calls",
                metricName = "Draw Calls",
                targetValue = maxDrawCalls,
                warningThreshold = maxDrawCalls * 0.8f,
                criticalThreshold = maxDrawCalls * 0.9f,
                isHigherBetter = false
            };
            
            // Triangles metric
            performanceMetrics["triangles"] = new PerformanceMetric
            {
                metricId = "triangles",
                metricName = "Triangles",
                targetValue = maxTriangles,
                warningThreshold = maxTriangles * 0.8f,
                criticalThreshold = maxTriangles * 0.9f,
                isHigherBetter = false
            };
            
            Log($"Initialized {performanceMetrics.Count} performance metrics");
        }
        
        /// <summary>
        /// Resets performance data.
        /// Educational: Shows how to reset performance tracking.
        /// </summary>
        private void ResetPerformanceData()
        {
            performanceSamples.Clear();
            totalFrames = 0;
            droppedFrames = 0;
            totalFrameTime = 0f;
            peakMemoryUsage = 0f;
            performanceWarnings = 0;
            isPerformanceGood = true;
        }
        
        /// <summary>
        /// Updates performance monitoring.
        /// Educational: Shows how to implement performance monitoring.
        /// </summary>
        private void UpdatePerformanceMonitoring()
        {
            // Start frame timing
            frameTimer.Restart();
            
            // Update frame count
            totalFrames++;
            
            // Calculate FPS
            currentFPS = 1f / Time.unscaledDeltaTime;
            currentFrameTime = Time.unscaledDeltaTime * 1000f; // Convert to milliseconds
            
            // Update memory usage
            currentMemoryUsage = Profiler.GetTotalAllocatedMemory(Profiler.Area.All) / (1024f * 1024f); // Convert to MB
            
            // Update render stats
            UpdateRenderStats();
            
            // Check for dropped frames
            if (currentFrameTime > maxFrameTime)
            {
                droppedFrames++;
            }
            
            // Update total frame time
            totalFrameTime += currentFrameTime;
            
            // Update peak memory usage
            if (currentMemoryUsage > peakMemoryUsage)
            {
                peakMemoryUsage = currentMemoryUsage;
            }
            
            // Check performance state
            CheckPerformanceState();
            
            // Collect performance sample
            if (Time.time - lastMonitoringTime >= monitoringInterval)
            {
                CollectPerformanceSample();
                lastMonitoringTime = Time.time;
            }
            
            // Stop frame timing
            frameTimer.Stop();
        }
        
        /// <summary>
        /// Updates render statistics.
        /// Educational: Shows how to collect render statistics.
        /// </summary>
        private void UpdateRenderStats()
        {
            // TODO: Implement actual render stat collection
            // This would typically use Unity's rendering statistics
            
            // For now, use placeholder values
            currentDrawCalls = UnityEngine.Random.Range(50, 150);
            currentTriangles = UnityEngine.Random.Range(50000, 150000);
        }
        
        /// <summary>
        /// Checks the current performance state.
        /// Educational: Shows how to evaluate performance state.
        /// </summary>
        private void CheckPerformanceState()
        {
            bool wasPerformanceGood = isPerformanceGood;
            isPerformanceGood = true;
            
            // Check each metric
            foreach (var metric in performanceMetrics.Values)
            {
                float currentValue = GetCurrentMetricValue(metric.metricId);
                
                if (IsMetricCritical(metric, currentValue))
                {
                    isPerformanceGood = false;
                    TriggerPerformanceWarning(metric, currentValue, PerformanceWarningLevel.Critical);
                }
                else if (IsMetricWarning(metric, currentValue))
                {
                    TriggerPerformanceWarning(metric, currentValue, PerformanceWarningLevel.Warning);
                }
            }
            
            // Fire state change event
            if (wasPerformanceGood != isPerformanceGood)
            {
                OnPerformanceStateChanged?.Invoke(isPerformanceGood);
            }
        }
        
        /// <summary>
        /// Gets the current value for a metric.
        /// Educational: Shows how to get metric values.
        /// </summary>
        /// <param name="metricId">Metric ID</param>
        /// <returns>Current metric value</returns>
        private float GetCurrentMetricValue(string metricId)
        {
            switch (metricId)
            {
                case "fps":
                    return currentFPS;
                case "frame_time":
                    return currentFrameTime;
                case "memory":
                    return currentMemoryUsage;
                case "draw_calls":
                    return currentDrawCalls;
                case "triangles":
                    return currentTriangles;
                default:
                    return 0f;
            }
        }
        
        /// <summary>
        /// Checks if a metric is in warning state.
        /// Educational: Shows how to check metric thresholds.
        /// </summary>
        /// <param name="metric">Performance metric</param>
        /// <param name="currentValue">Current value</param>
        /// <returns>True if metric is in warning state</returns>
        private bool IsMetricWarning(PerformanceMetric metric, float currentValue)
        {
            if (metric.isHigherBetter)
            {
                return currentValue < metric.warningThreshold;
            }
            else
            {
                return currentValue > metric.warningThreshold;
            }
        }
        
        /// <summary>
        /// Checks if a metric is in critical state.
        /// Educational: Shows how to check metric thresholds.
        /// </summary>
        /// <param name="metric">Performance metric</param>
        /// <param name="currentValue">Current value</param>
        /// <returns>True if metric is in critical state</returns>
        private bool IsMetricCritical(PerformanceMetric metric, float currentValue)
        {
            if (metric.isHigherBetter)
            {
                return currentValue < metric.criticalThreshold;
            }
            else
            {
                return currentValue > metric.criticalThreshold;
            }
        }
        
        /// <summary>
        /// Triggers a performance warning.
        /// Educational: Shows how to handle performance warnings.
        /// </summary>
        /// <param name="metric">Performance metric</param>
        /// <param name="currentValue">Current value</param>
        /// <param name="level">Warning level</param>
        private void TriggerPerformanceWarning(PerformanceMetric metric, float currentValue, PerformanceWarningLevel level)
        {
            PerformanceWarning warning = new PerformanceWarning
            {
                metricId = metric.metricId,
                metricName = metric.metricName,
                currentValue = currentValue,
                targetValue = metric.targetValue,
                warningLevel = level,
                timestamp = Time.time
            };
            
            OnPerformanceWarning?.Invoke(warning);
            performanceWarnings++;
            
            Log($"Performance warning: {metric.metricName} = {currentValue:F2} ({level})");
        }
        
        /// <summary>
        /// Collects a performance sample.
        /// Educational: Shows how to collect performance samples.
        /// </summary>
        private void CollectPerformanceSample()
        {
            PerformanceSample sample = new PerformanceSample
            {
                timestamp = Time.time,
                fps = currentFPS,
                frameTime = currentFrameTime,
                memoryUsage = currentMemoryUsage,
                drawCalls = currentDrawCalls,
                triangles = currentTriangles,
                isPerformanceGood = isPerformanceGood
            };
            
            performanceSamples.Add(sample);
            
            // Limit sample count
            if (performanceSamples.Count > maxSamples)
            {
                performanceSamples.RemoveAt(0);
            }
            
            OnPerformanceSample?.Invoke(sample);
        }
        
        /// <summary>
        /// Gets performance statistics.
        /// Educational: Shows how to provide performance analytics.
        /// </summary>
        /// <returns>Performance statistics</returns>
        public PerformanceStats GetPerformanceStats()
        {
            return new PerformanceStats
            {
                CurrentFPS = currentFPS,
                CurrentFrameTime = currentFrameTime,
                CurrentMemoryUsage = currentMemoryUsage,
                CurrentDrawCalls = currentDrawCalls,
                CurrentTriangles = currentTriangles,
                TotalFrames = totalFrames,
                DroppedFrames = droppedFrames,
                PeakMemoryUsage = peakMemoryUsage,
                PerformanceWarnings = performanceWarnings,
                IsPerformanceGood = isPerformanceGood,
                AverageFPS = CalculateAverageFPS(),
                AverageFrameTime = CalculateAverageFrameTime(),
                AverageMemoryUsage = CalculateAverageMemoryUsage()
            };
        }
        
        /// <summary>
        /// Calculates average FPS.
        /// Educational: Shows how to calculate performance averages.
        /// </summary>
        /// <returns>Average FPS</returns>
        private float CalculateAverageFPS()
        {
            if (performanceSamples.Count == 0)
                return 0f;
            
            return performanceSamples.Average(s => s.fps);
        }
        
        /// <summary>
        /// Calculates average frame time.
        /// Educational: Shows how to calculate performance averages.
        /// </summary>
        /// <returns>Average frame time</returns>
        private float CalculateAverageFrameTime()
        {
            if (performanceSamples.Count == 0)
                return 0f;
            
            return performanceSamples.Average(s => s.frameTime);
        }
        
        /// <summary>
        /// Calculates average memory usage.
        /// Educational: Shows how to calculate performance averages.
        /// </summary>
        /// <returns>Average memory usage</returns>
        private float CalculateAverageMemoryUsage()
        {
            if (performanceSamples.Count == 0)
                return 0f;
            
            return performanceSamples.Average(s => s.memoryUsage);
        }
        
        /// <summary>
        /// Gets performance samples.
        /// Educational: Shows how to expose performance data.
        /// </summary>
        /// <returns>List of performance samples</returns>
        public List<PerformanceSample> GetPerformanceSamples()
        {
            return new List<PerformanceSample>(performanceSamples);
        }
        
        /// <summary>
        /// Gets performance metrics.
        /// Educational: Shows how to expose performance metrics.
        /// </summary>
        /// <returns>Dictionary of performance metrics</returns>
        public Dictionary<string, PerformanceMetric> GetPerformanceMetrics()
        {
            return new Dictionary<string, PerformanceMetric>(performanceMetrics);
        }
        
        /// <summary>
        /// Resets performance data.
        /// Educational: Shows how to reset performance tracking.
        /// </summary>
        public void ResetPerformanceData()
        {
            ResetPerformanceData();
            Log("Performance data reset");
        }
        
        /// <summary>
        /// Logs a message if debug logging is enabled.
        /// Educational: Shows how to implement conditional logging.
        /// </summary>
        /// <param name="message">Message to log</param>
        private void Log(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[PerformanceProfiler] {message}");
        }
    }
    
    /// <summary>
    /// Performance warning levels.
    /// Educational: Shows how to define warning levels.
    /// </summary>
    public enum PerformanceWarningLevel
    {
        Info,
        Warning,
        Critical
    }
    
    /// <summary>
    /// Performance sample data structure.
    /// Educational: Shows how to define performance data structures.
    /// </summary>
    [System.Serializable]
    public struct PerformanceSample
    {
        public float timestamp;
        public float fps;
        public float frameTime;
        public float memoryUsage;
        public int drawCalls;
        public int triangles;
        public bool isPerformanceGood;
    }
    
    /// <summary>
    /// Performance metric data structure.
    /// Educational: Shows how to define performance metrics.
    /// </summary>
    [System.Serializable]
    public struct PerformanceMetric
    {
        public string metricId;
        public string metricName;
        public float targetValue;
        public float warningThreshold;
        public float criticalThreshold;
        public bool isHigherBetter;
    }
    
    /// <summary>
    /// Performance warning data structure.
    /// Educational: Shows how to define performance warnings.
    /// </summary>
    [System.Serializable]
    public struct PerformanceWarning
    {
        public string metricId;
        public string metricName;
        public float currentValue;
        public float targetValue;
        public PerformanceWarningLevel warningLevel;
        public float timestamp;
    }
    
    /// <summary>
    /// Performance statistics.
    /// Educational: Shows how to create analytics data structures.
    /// </summary>
    [System.Serializable]
    public struct PerformanceStats
    {
        public float CurrentFPS;
        public float CurrentFrameTime;
        public float CurrentMemoryUsage;
        public int CurrentDrawCalls;
        public int CurrentTriangles;
        public int TotalFrames;
        public int DroppedFrames;
        public float PeakMemoryUsage;
        public int PerformanceWarnings;
        public bool IsPerformanceGood;
        public float AverageFPS;
        public float AverageFrameTime;
        public float AverageMemoryUsage;
    }
}
