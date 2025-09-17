/******************************************************************************
 * SWITCH - AnimationSystem
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Central animation system for smooth transitions and effects
 * Dependencies: Unity Animation, DOTween (optional)
 * 
 * Educational Notes:
 * - Demonstrates centralized animation management
 * - Shows how to implement smooth game transitions
 * - Performance: Efficient animation pooling and management
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SWITCH.Animation
{
    /// <summary>
    /// Central animation system for smooth transitions and effects.
    /// Educational: This demonstrates centralized animation management.
    /// Performance: Efficient animation pooling and management with minimal overhead.
    /// </summary>
    public class AnimationSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private bool enableAnimations = true;
        [SerializeField] private int maxConcurrentAnimations = 50;
        
        [Header("Animation Settings")]
        [SerializeField] private float defaultAnimationDuration = 0.3f;
        [SerializeField] private AnimationCurve defaultEaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private bool enableAnimationPooling = true;
        
        [Header("Performance Settings")]
        [SerializeField] private bool enableAnimationCulling = true;
        [SerializeField] private float animationCullDistance = 100f;
        [SerializeField] private int maxAnimationsPerFrame = 10;
        
        // Animation state
        private List<AnimationData> activeAnimations = new List<AnimationData>();
        private Queue<AnimationData> animationPool = new Queue<AnimationData>();
        private Dictionary<string, AnimationPreset> animationPresets = new Dictionary<string, AnimationPreset>();
        
        // Performance tracking
        private int totalAnimationsPlayed = 0;
        private int totalAnimationsCompleted = 0;
        private int totalAnimationsCancelled = 0;
        private float totalAnimationTime = 0f;
        
        // Events
        public System.Action<AnimationData> OnAnimationStarted;
        public System.Action<AnimationData> OnAnimationCompleted;
        public System.Action<AnimationData> OnAnimationCancelled;
        
        // Singleton instance
        private static AnimationSystem instance;
        public static AnimationSystem Instance => instance;
        
        // Properties
        public int ActiveAnimationCount => activeAnimations.Count;
        public int TotalAnimationsPlayed => totalAnimationsPlayed;
        public int TotalAnimationsCompleted => totalAnimationsCompleted;
        public bool IsAnimationPlaying => activeAnimations.Count > 0;
        
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
            InitializeAnimationSystem();
        }
        
        private void Update()
        {
            if (enableAnimations)
            {
                UpdateAnimations();
            }
        }
        
        /// <summary>
        /// Initializes the animation system.
        /// Educational: Shows how to set up animation systems.
        /// </summary>
        private void InitializeAnimationSystem()
        {
            Log("Initializing Animation System");
            
            // Initialize animation presets
            InitializeAnimationPresets();
            
            // Initialize animation pool
            if (enableAnimationPooling)
            {
                InitializeAnimationPool();
            }
            
            Log("Animation System initialized");
        }
        
        /// <summary>
        /// Initializes animation presets.
        /// Educational: Shows how to define animation presets.
        /// </summary>
        private void InitializeAnimationPresets()
        {
            animationPresets.Clear();
            
            // Fade in preset
            animationPresets["fade_in"] = new AnimationPreset
            {
                presetId = "fade_in",
                presetName = "Fade In",
                animationType = AnimationType.Alpha,
                duration = 0.5f,
                easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                startValue = 0f,
                endValue = 1f
            };
            
            // Fade out preset
            animationPresets["fade_out"] = new AnimationPreset
            {
                presetId = "fade_out",
                presetName = "Fade Out",
                animationType = AnimationType.Alpha,
                duration = 0.5f,
                easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                startValue = 1f,
                endValue = 0f
            };
            
            // Scale in preset
            animationPresets["scale_in"] = new AnimationPreset
            {
                presetId = "scale_in",
                presetName = "Scale In",
                animationType = AnimationType.Scale,
                duration = 0.3f,
                easeCurve = AnimationCurve.EaseOutBack(0, 0, 1, 1),
                startValue = 0f,
                endValue = 1f
            };
            
            // Scale out preset
            animationPresets["scale_out"] = new AnimationPreset
            {
                presetId = "scale_out",
                presetName = "Scale Out",
                animationType = AnimationType.Scale,
                duration = 0.3f,
                easeCurve = AnimationCurve.EaseInBack(0, 0, 1, 1),
                startValue = 1f,
                endValue = 0f
            };
            
            // Slide in preset
            animationPresets["slide_in"] = new AnimationPreset
            {
                presetId = "slide_in",
                presetName = "Slide In",
                animationType = AnimationType.Position,
                duration = 0.4f,
                easeCurve = AnimationCurve.EaseOutCubic(0, 0, 1, 1),
                startValue = 0f,
                endValue = 1f
            };
            
            Log($"Initialized {animationPresets.Count} animation presets");
        }
        
        /// <summary>
        /// Initializes the animation pool.
        /// Educational: Shows how to implement object pooling for animations.
        /// </summary>
        private void InitializeAnimationPool()
        {
            animationPool.Clear();
            
            // Pre-allocate animation objects
            for (int i = 0; i < maxConcurrentAnimations; i++)
            {
                AnimationData animData = new AnimationData();
                animationPool.Enqueue(animData);
            }
            
            Log($"Initialized animation pool with {maxConcurrentAnimations} objects");
        }
        
        /// <summary>
        /// Updates all active animations.
        /// Educational: Shows how to update animation systems.
        /// </summary>
        private void UpdateAnimations()
        {
            if (activeAnimations.Count == 0)
                return;
            
            int animationsUpdated = 0;
            List<AnimationData> completedAnimations = new List<AnimationData>();
            
            foreach (var animation in activeAnimations)
            {
                if (animationsUpdated >= maxAnimationsPerFrame)
                    break;
                
                // Update animation
                bool isCompleted = UpdateAnimation(animation);
                
                if (isCompleted)
                {
                    completedAnimations.Add(animation);
                }
                
                animationsUpdated++;
            }
            
            // Complete finished animations
            foreach (var animation in completedAnimations)
            {
                CompleteAnimation(animation);
            }
        }
        
        /// <summary>
        /// Updates a single animation.
        /// Educational: Shows how to update individual animations.
        /// </summary>
        /// <param name="animation">Animation to update</param>
        /// <returns>True if animation is completed</returns>
        private bool UpdateAnimation(AnimationData animation)
        {
            if (animation.target == null)
            {
                return true; // Complete if target is destroyed
            }
            
            // Update animation time
            animation.currentTime += Time.deltaTime;
            float progress = Mathf.Clamp01(animation.currentTime / animation.duration);
            
            // Apply easing
            float easedProgress = animation.easeCurve.Evaluate(progress);
            
            // Update animation value
            float currentValue = Mathf.Lerp(animation.startValue, animation.endValue, easedProgress);
            
            // Apply animation to target
            ApplyAnimationToTarget(animation, currentValue);
            
            // Check if completed
            return progress >= 1f;
        }
        
        /// <summary>
        /// Applies animation value to target object.
        /// Educational: Shows how to apply animations to different target types.
        /// </summary>
        /// <param name="animation">Animation data</param>
        /// <param name="value">Current animation value</param>
        private void ApplyAnimationToTarget(AnimationData animation, float value)
        {
            switch (animation.animationType)
            {
                case AnimationType.Alpha:
                    ApplyAlphaAnimation(animation, value);
                    break;
                case AnimationType.Scale:
                    ApplyScaleAnimation(animation, value);
                    break;
                case AnimationType.Position:
                    ApplyPositionAnimation(animation, value);
                    break;
                case AnimationType.Rotation:
                    ApplyRotationAnimation(animation, value);
                    break;
                case AnimationType.Color:
                    ApplyColorAnimation(animation, value);
                    break;
            }
        }
        
        /// <summary>
        /// Applies alpha animation to target.
        /// Educational: Shows how to implement alpha animations.
        /// </summary>
        /// <param name="animation">Animation data</param>
        /// <param name="value">Current value</param>
        private void ApplyAlphaAnimation(AnimationData animation, float value)
        {
            if (animation.target is CanvasGroup canvasGroup)
            {
                canvasGroup.alpha = value;
            }
            else if (animation.target is Image image)
            {
                Color color = image.color;
                color.a = value;
                image.color = color;
            }
            else if (animation.target is SpriteRenderer spriteRenderer)
            {
                Color color = spriteRenderer.color;
                color.a = value;
                spriteRenderer.color = color;
            }
        }
        
        /// <summary>
        /// Applies scale animation to target.
        /// Educational: Shows how to implement scale animations.
        /// </summary>
        /// <param name="animation">Animation data</param>
        /// <param name="value">Current value</param>
        private void ApplyScaleAnimation(AnimationData animation, float value)
        {
            if (animation.target is Transform transform)
            {
                transform.localScale = Vector3.one * value;
            }
        }
        
        /// <summary>
        /// Applies position animation to target.
        /// Educational: Shows how to implement position animations.
        /// </summary>
        /// <param name="animation">Animation data</param>
        /// <param name="value">Current value</param>
        private void ApplyPositionAnimation(AnimationData animation, float value)
        {
            if (animation.target is Transform transform)
            {
                Vector3 startPos = animation.startVector;
                Vector3 endPos = animation.endVector;
                transform.position = Vector3.Lerp(startPos, endPos, value);
            }
        }
        
        /// <summary>
        /// Applies rotation animation to target.
        /// Educational: Shows how to implement rotation animations.
        /// </summary>
        /// <param name="animation">Animation data</param>
        /// <param name="value">Current value</param>
        private void ApplyRotationAnimation(AnimationData animation, float value)
        {
            if (animation.target is Transform transform)
            {
                Quaternion startRot = animation.startQuaternion;
                Quaternion endRot = animation.endQuaternion;
                transform.rotation = Quaternion.Lerp(startRot, endRot, value);
            }
        }
        
        /// <summary>
        /// Applies color animation to target.
        /// Educational: Shows how to implement color animations.
        /// </summary>
        /// <param name="animation">Animation data</param>
        /// <param name="value">Current value</param>
        private void ApplyColorAnimation(AnimationData animation, float value)
        {
            if (animation.target is Image image)
            {
                image.color = Color.Lerp(animation.startColor, animation.endColor, value);
            }
            else if (animation.target is SpriteRenderer spriteRenderer)
            {
                spriteRenderer.color = Color.Lerp(animation.startColor, animation.endColor, value);
            }
        }
        
        /// <summary>
        /// Completes an animation.
        /// Educational: Shows how to complete animations.
        /// </summary>
        /// <param name="animation">Animation to complete</param>
        private void CompleteAnimation(AnimationData animation)
        {
            // Remove from active animations
            activeAnimations.Remove(animation);
            
            // Fire completion event
            OnAnimationCompleted?.Invoke(animation);
            
            // Call completion callback
            animation.onCompleted?.Invoke();
            
            // Return to pool if pooling is enabled
            if (enableAnimationPooling)
            {
                animationPool.Enqueue(animation);
            }
            
            totalAnimationsCompleted++;
            totalAnimationTime += animation.duration;
            
            Log($"Animation completed: {animation.animationType} on {animation.target?.name}");
        }
        
        /// <summary>
        /// Plays an animation on a target object.
        /// Educational: Shows how to play animations.
        /// </summary>
        /// <param name="target">Target object to animate</param>
        /// <param name="animationType">Type of animation</param>
        /// <param name="duration">Animation duration</param>
        /// <param name="startValue">Start value</param>
        /// <param name="endValue">End value</param>
        /// <param name="easeCurve">Easing curve</param>
        /// <param name="onCompleted">Completion callback</param>
        /// <returns>Animation data</returns>
        public AnimationData PlayAnimation(object target, AnimationType animationType, float duration, 
            float startValue, float endValue, AnimationCurve easeCurve = null, System.Action onCompleted = null)
        {
            if (!enableAnimations || target == null)
                return null;
            
            // Get animation data from pool or create new
            AnimationData animData = GetAnimationData();
            
            // Set up animation
            animData.target = target;
            animData.animationType = animationType;
            animData.duration = duration;
            animData.startValue = startValue;
            animData.endValue = endValue;
            animData.easeCurve = easeCurve ?? defaultEaseCurve;
            animData.currentTime = 0f;
            animData.onCompleted = onCompleted;
            
            // Add to active animations
            activeAnimations.Add(animData);
            
            // Fire start event
            OnAnimationStarted?.Invoke(animData);
            
            totalAnimationsPlayed++;
            
            Log($"Animation started: {animationType} on {target} for {duration}s");
            
            return animData;
        }
        
        /// <summary>
        /// Plays an animation using a preset.
        /// Educational: Shows how to use animation presets.
        /// </summary>
        /// <param name="target">Target object to animate</param>
        /// <param name="presetId">Preset ID</param>
        /// <param name="onCompleted">Completion callback</param>
        /// <returns>Animation data</returns>
        public AnimationData PlayAnimationPreset(object target, string presetId, System.Action onCompleted = null)
        {
            if (!animationPresets.TryGetValue(presetId, out var preset))
            {
                LogError($"Animation preset not found: {presetId}");
                return null;
            }
            
            return PlayAnimation(target, preset.animationType, preset.duration, 
                preset.startValue, preset.endValue, preset.easeCurve, onCompleted);
        }
        
        /// <summary>
        /// Cancels an animation.
        /// Educational: Shows how to cancel animations.
        /// </summary>
        /// <param name="animation">Animation to cancel</param>
        public void CancelAnimation(AnimationData animation)
        {
            if (animation == null || !activeAnimations.Contains(animation))
                return;
            
            // Remove from active animations
            activeAnimations.Remove(animation);
            
            // Fire cancellation event
            OnAnimationCancelled?.Invoke(animation);
            
            // Return to pool if pooling is enabled
            if (enableAnimationPooling)
            {
                animationPool.Enqueue(animation);
            }
            
            totalAnimationsCancelled++;
            
            Log($"Animation cancelled: {animation.animationType} on {animation.target?.name}");
        }
        
        /// <summary>
        /// Gets animation data from pool or creates new.
        /// Educational: Shows how to implement object pooling.
        /// </summary>
        /// <returns>Animation data</returns>
        private AnimationData GetAnimationData()
        {
            if (enableAnimationPooling && animationPool.Count > 0)
            {
                return animationPool.Dequeue();
            }
            
            return new AnimationData();
        }
        
        /// <summary>
        /// Gets system statistics.
        /// Educational: Shows how to provide system analytics.
        /// </summary>
        /// <returns>System statistics</returns>
        public AnimationSystemStats GetSystemStats()
        {
            return new AnimationSystemStats
            {
                ActiveAnimationCount = activeAnimations.Count,
                TotalAnimationsPlayed = totalAnimationsPlayed,
                TotalAnimationsCompleted = totalAnimationsCompleted,
                TotalAnimationsCancelled = totalAnimationsCancelled,
                TotalAnimationTime = totalAnimationTime,
                PoolSize = animationPool.Count
            };
        }
        
        /// <summary>
        /// Logs a message if debug logging is enabled.
        /// Educational: Shows how to implement conditional logging.
        /// </summary>
        /// <param name="message">Message to log</param>
        private void Log(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[AnimationSystem] {message}");
        }
        
        /// <summary>
        /// Logs an error message.
        /// Educational: Shows how to implement error logging.
        /// </summary>
        /// <param name="message">Error message to log</param>
        private void LogError(string message)
        {
            Debug.LogError($"[AnimationSystem] {message}");
        }
    }
    
    /// <summary>
    /// Animation types.
    /// Educational: Shows how to define animation categories.
    /// </summary>
    public enum AnimationType
    {
        Alpha,
        Scale,
        Position,
        Rotation,
        Color
    }
    
    /// <summary>
    /// Animation data structure.
    /// Educational: Shows how to define animation data structures.
    /// </summary>
    [System.Serializable]
    public class AnimationData
    {
        public object target;
        public AnimationType animationType;
        public float duration;
        public float startValue;
        public float endValue;
        public Vector3 startVector;
        public Vector3 endVector;
        public Quaternion startQuaternion;
        public Quaternion endQuaternion;
        public Color startColor;
        public Color endColor;
        public AnimationCurve easeCurve;
        public float currentTime;
        public System.Action onCompleted;
    }
    
    /// <summary>
    /// Animation preset data structure.
    /// Educational: Shows how to define animation presets.
    /// </summary>
    [System.Serializable]
    public struct AnimationPreset
    {
        public string presetId;
        public string presetName;
        public AnimationType animationType;
        public float duration;
        public AnimationCurve easeCurve;
        public float startValue;
        public float endValue;
    }
    
    /// <summary>
    /// Animation system statistics.
    /// Educational: Shows how to create analytics data structures.
    /// </summary>
    [System.Serializable]
    public struct AnimationSystemStats
    {
        public int ActiveAnimationCount;
        public int TotalAnimationsPlayed;
        public int TotalAnimationsCompleted;
        public int TotalAnimationsCancelled;
        public float TotalAnimationTime;
        public int PoolSize;
    }
}
