/******************************************************************************
 * SWITCH - DoubleTapDetector
 * Sprint: 1
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Double-tap input detection with <100ms latency
 * Dependencies: Unity Input System
 * 
 * Educational Notes:
 * - Demonstrates input buffering and timing-based detection
 * - Uses efficient timing checks to minimize input lag
 * - Performance: Minimal allocation, uses cached values
 *****************************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

namespace SWITCH.Core
{
    /// <summary>
    /// Double-tap detection implementation for tile selection.
    /// Educational: This demonstrates input buffering and timing-based detection.
    /// Performance: Uses efficient timing checks to minimize input lag.
    /// </summary>
    public class DoubleTapDetector : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float maxDoubleTapTime = 0.3f;
        [SerializeField] private float maxDoubleTapDistance = 50f;
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        
        // Input state
        private Vector2 firstTapPosition;
        private float firstTapTime;
        private bool waitingForSecondTap;
        
        // Events
        public System.Action<Vector2> OnDoubleTapDetected;
        public System.Action<Vector2> OnSingleTapDetected;
        
        /// <summary>
        /// Detects double-tap input with configurable timing and distance.
        /// Educational: Shows how to implement precise input detection.
        /// Performance: Minimal allocation, uses cached values.
        /// </summary>
        public bool DetectDoubleTap(Vector2 position, float time)
        {
            if (waitingForSecondTap)
            {
                float timeDiff = time - firstTapTime;
                float distance = Vector2.Distance(position, firstTapPosition);
                
                if (timeDiff <= maxDoubleTapTime && distance <= maxDoubleTapDistance)
                {
                    waitingForSecondTap = false;
                    OnDoubleTapDetected?.Invoke(position);
                    Log($"Double tap detected at {position} (time: {timeDiff:F3}s, distance: {distance:F1}px)");
                    return true;
                }
                else
                {
                    // First tap was too long ago or too far away, treat as single tap
                    OnSingleTapDetected?.Invoke(firstTapPosition);
                    ResetDoubleTap();
                }
            }
            else
            {
                firstTapPosition = position;
                firstTapTime = time;
                waitingForSecondTap = true;
                Log($"First tap registered at {position}");
            }
            return false;
        }
        
        /// <summary>
        /// Resets the double-tap detection state.
        /// Educational: Shows proper state management for input systems.
        /// </summary>
        public void ResetDoubleTap()
        {
            waitingForSecondTap = false;
            firstTapPosition = Vector2.zero;
            firstTapTime = 0f;
        }
        
        /// <summary>
        /// Handles touch input for mobile devices.
        /// Educational: Demonstrates cross-platform input handling.
        /// </summary>
        public void OnTouchInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                Vector2 touchPosition = context.ReadValue<Vector2>();
                DetectDoubleTap(touchPosition, Time.time);
            }
        }
        
        /// <summary>
        /// Handles mouse input for desktop testing.
        /// Educational: Shows how to support multiple input methods.
        /// </summary>
        public void OnMouseInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                DetectDoubleTap(mousePosition, Time.time);
            }
        }
        
        /// <summary>
        /// Updates the double-tap detection state.
        /// Educational: Shows how to handle timeout scenarios.
        /// </summary>
        private void Update()
        {
            if (waitingForSecondTap)
            {
                float timeSinceFirstTap = Time.time - firstTapTime;
                if (timeSinceFirstTap > maxDoubleTapTime)
                {
                    // Timeout - treat as single tap
                    OnSingleTapDetected?.Invoke(firstTapPosition);
                    ResetDoubleTap();
                    Log("Double tap timeout - treating as single tap");
                }
            }
        }
        
        private void Log(string message)
        {
            if (debugMode)
            {
                Debug.Log($"[DoubleTapDetector] {message}");
            }
        }
        
        private void OnDrawGizmos()
        {
            if (waitingForSecondTap)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(firstTapPosition, maxDoubleTapDistance);
            }
        }
    }
}
