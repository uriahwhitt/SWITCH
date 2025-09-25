using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Switch.Core
{
    /// <summary>
    /// Handles touch and mouse input for the game
    /// Uses Unity's new Input System for better mobile support
    /// </summary>
    public class TouchInputHandler : MonoBehaviour
    {
        [Header("Input Configuration")]
        [SerializeField] private bool enableTouchInput = true;
        [SerializeField] private bool enableMouseInput = true;
        [SerializeField] private float tapTimeThreshold = 0.3f;
        [SerializeField] private float tapDistanceThreshold = 50f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // Input state
        private Vector2 lastTouchPosition;
        private float touchStartTime;
        private bool isTouching = false;
        private Camera mainCamera;
        
        // Events
        public static event Action<Vector2> OnTap;
        public static event Action<Vector2> OnTouchStart;
        public static event Action<Vector2> OnTouchEnd;
        public static event Action<Vector2, Vector2> OnSwipe;
        public static event Action<Vector2> OnTouchMove;
        
        // Properties
        public bool IsTouching => isTouching;
        public Vector2 LastTouchPosition => lastTouchPosition;
        
        private void Awake()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
        }
        
        private void Start()
        {
            // Initialize input system
            InitializeInputSystem();
        }
        
        private void InitializeInputSystem()
        {
            // Check if Input System is available
            if (InputSystem.settings == null)
            {
                Debug.LogWarning("Input System not available. Falling back to legacy input.");
                return;
            }
            
            Debug.Log("Touch Input Handler initialized with Input System");
        }
        
        private void Update()
        {
            HandleInput();
        }
        
        /// <summary>
        /// Main input handling method
        /// </summary>
        private void HandleInput()
        {
            if (enableTouchInput)
            {
                HandleTouchInput();
            }
            
            if (enableMouseInput)
            {
                HandleMouseInput();
            }
        }
        
        /// <summary>
        /// Handles touch input for mobile devices
        /// </summary>
        private void HandleTouchInput()
        {
            // Use Input System for touch input
            var touchscreen = Touchscreen.current;
            if (touchscreen != null)
            {
                var touches = touchscreen.touches;
                if (touches.Count > 0)
                {
                    var touch = touches[0];
                    Vector2 touchPosition = touch.position.ReadValue();
                    
                    if (touch.press.wasPressedThisFrame)
                    {
                        HandleTouchStart(touchPosition);
                    }
                    else if (touch.press.isPressed)
                    {
                        HandleTouchMove(touchPosition);
                    }
                    else if (touch.press.wasReleasedThisFrame)
                    {
                        HandleTouchEnd(touchPosition);
                    }
                }
            }
        }
        
        /// <summary>
        /// Handles mouse input for desktop/editor testing
        /// </summary>
        private void HandleMouseInput()
        {
            // Use Input System for mouse input
            var mouse = Mouse.current;
            if (mouse != null)
            {
                Vector2 mousePosition = mouse.position.ReadValue();
                
                if (mouse.leftButton.wasPressedThisFrame)
                {
                    HandleTouchStart(mousePosition);
                }
                else if (mouse.leftButton.isPressed && isTouching)
                {
                    HandleTouchMove(mousePosition);
                }
                else if (mouse.leftButton.wasReleasedThisFrame && isTouching)
                {
                    HandleTouchEnd(mousePosition);
                }
            }
        }
        
        /// <summary>
        /// Handles the start of a touch/mouse press
        /// </summary>
        private void HandleTouchStart(Vector2 screenPosition)
        {
            isTouching = true;
            lastTouchPosition = screenPosition;
            touchStartTime = Time.time;
            
            OnTouchStart?.Invoke(screenPosition);
            
            if (showDebugInfo)
            {
                Debug.Log($"Touch started at: {screenPosition}");
            }
        }
        
        /// <summary>
        /// Handles touch/mouse movement
        /// </summary>
        private void HandleTouchMove(Vector2 screenPosition)
        {
            if (!isTouching) return;
            
            Vector2 deltaPosition = screenPosition - lastTouchPosition;
            lastTouchPosition = screenPosition;
            
            OnTouchMove?.Invoke(screenPosition);
            
            if (showDebugInfo && deltaPosition.magnitude > 5f)
            {
                Debug.Log($"Touch moved: {deltaPosition}");
            }
        }
        
        /// <summary>
        /// Handles the end of a touch/mouse press
        /// </summary>
        private void HandleTouchEnd(Vector2 screenPosition)
        {
            if (!isTouching) return;
            
            float touchDuration = Time.time - touchStartTime;
            float touchDistance = Vector2.Distance(screenPosition, lastTouchPosition);
            
            OnTouchEnd?.Invoke(screenPosition);
            
            // Determine if this was a tap or swipe
            if (touchDuration <= tapTimeThreshold && touchDistance <= tapDistanceThreshold)
            {
                HandleTap(screenPosition);
            }
            else if (touchDistance > tapDistanceThreshold)
            {
                HandleSwipe(lastTouchPosition, screenPosition);
            }
            
            isTouching = false;
            
            if (showDebugInfo)
            {
                Debug.Log($"Touch ended at: {screenPosition}, Duration: {touchDuration:F2}s, Distance: {touchDistance:F1}px");
            }
        }
        
        /// <summary>
        /// Handles a tap gesture
        /// </summary>
        private void HandleTap(Vector2 screenPosition)
        {
            Vector2 worldPosition = ScreenToWorldPosition(screenPosition);
            OnTap?.Invoke(worldPosition);
            
            if (showDebugInfo)
            {
                Debug.Log($"Tap detected at world position: {worldPosition}");
            }
        }
        
        /// <summary>
        /// Handles a swipe gesture
        /// </summary>
        private void HandleSwipe(Vector2 startPosition, Vector2 endPosition)
        {
            Vector2 startWorld = ScreenToWorldPosition(startPosition);
            Vector2 endWorld = ScreenToWorldPosition(endPosition);
            
            OnSwipe?.Invoke(startWorld, endWorld);
            
            if (showDebugInfo)
            {
                Vector2 swipeDirection = (endWorld - startWorld).normalized;
                Debug.Log($"Swipe detected: {swipeDirection} from {startWorld} to {endWorld}");
            }
        }
        
        /// <summary>
        /// Converts screen position to world position
        /// </summary>
        public Vector2 ScreenToWorldPosition(Vector2 screenPosition)
        {
            if (mainCamera != null)
            {
                return mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane));
            }
            return Vector2.zero;
        }
        
        /// <summary>
        /// Converts world position to screen position
        /// </summary>
        public Vector2 WorldToScreenPosition(Vector2 worldPosition)
        {
            if (mainCamera != null)
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(new Vector3(worldPosition.x, worldPosition.y, 0));
                return new Vector2(screenPos.x, screenPos.y);
            }
            return Vector2.zero;
        }
        
        /// <summary>
        /// Gets the current input position in world coordinates
        /// </summary>
        public Vector2 GetCurrentInputWorldPosition()
        {
            Vector2 inputPosition = Vector2.zero;
            
            if (enableTouchInput)
            {
                var touchscreen = Touchscreen.current;
                if (touchscreen != null && touchscreen.touches.Count > 0)
                {
                    inputPosition = touchscreen.touches[0].position.ReadValue();
                }
            }
            
            if (inputPosition == Vector2.zero && enableMouseInput)
            {
                var mouse = Mouse.current;
                if (mouse != null)
                {
                    inputPosition = mouse.position.ReadValue();
                }
            }
            
            return ScreenToWorldPosition(inputPosition);
        }
        
        /// <summary>
        /// Checks if input is currently active
        /// </summary>
        public bool IsInputActive()
        {
            if (enableTouchInput)
            {
                var touchscreen = Touchscreen.current;
                if (touchscreen != null && touchscreen.touches.Count > 0)
                {
                    return true;
                }
            }
            
            if (enableMouseInput)
            {
                var mouse = Mouse.current;
                if (mouse != null && mouse.leftButton.isPressed)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        #region Debug
        
        [ContextMenu("Test Tap")]
        private void DebugTestTap()
        {
            Vector2 testPosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            HandleTap(testPosition);
        }
        
        [ContextMenu("Test Swipe")]
        private void DebugTestSwipe()
        {
            Vector2 startPos = new Vector2(Screen.width * 0.3f, Screen.height * 0.5f);
            Vector2 endPos = new Vector2(Screen.width * 0.7f, Screen.height * 0.5f);
            HandleSwipe(startPos, endPos);
        }
        
        private void OnGUI()
        {
            if (showDebugInfo)
            {
                GUILayout.BeginArea(new Rect(10, 10, 300, 200));
                GUILayout.Label($"Touch Input Handler Debug");
                GUILayout.Label($"Is Touching: {isTouching}");
                GUILayout.Label($"Last Position: {lastTouchPosition}");
                
                // Use Input System methods
                var touchscreen = Touchscreen.current;
                var mouse = Mouse.current;
                GUILayout.Label($"Touch Count: {(touchscreen != null ? touchscreen.touches.Count : 0)}");
                GUILayout.Label($"Mouse Position: {(mouse != null ? mouse.position.ReadValue().ToString() : "N/A")}");
                
                GUILayout.EndArea();
            }
        }
        
        #endregion
    }
}
