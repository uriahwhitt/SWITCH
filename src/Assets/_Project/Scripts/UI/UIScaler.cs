using UnityEngine;
using UnityEngine.UI;

namespace SWITCH.UI
{
    /// <summary>
    /// Handles responsive scaling and device-specific UI adjustments
    /// </summary>
    public class UIScaler : MonoBehaviour
    {
        [Header("Target Aspect Ratios")]
        [SerializeField] private float targetAspectRatio = 9f / 16f; // Standard mobile portrait
        [SerializeField] private float tabletAspectRatio = 3f / 4f;  // Tablet portrait
        
        [Header("Device Detection")]
        [SerializeField] private float tabletThreshold = 0.75f;
        [SerializeField] private float phoneThreshold = 0.6f;
        
        [Header("Scaling Settings")]
        [SerializeField] private float baseScale = 1f;
        [SerializeField] private float tabletScale = 1.2f;
        [SerializeField] private float phoneScale = 0.9f;
        
        [Header("Safe Area Settings")]
        [SerializeField] private bool handleSafeArea = true;
        [SerializeField] private float safeAreaPadding = 20f;
        
        [Header("UI References")]
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private CanvasScaler canvasScaler;
        [SerializeField] private RectTransform safeAreaTransform;
        
        // Device info
        private DeviceType currentDeviceType;
        private float currentAspectRatio;
        private bool hasNotch;
        
        // Events
        public System.Action<DeviceType> OnDeviceTypeChanged;
        public System.Action<float> OnAspectRatioChanged;
        
        public enum DeviceType
        {
            Phone,
            Tablet,
            Unknown
        }
        
        private void Awake()
        {
            // Get references if not assigned
            if (mainCanvas == null)
                mainCanvas = GetComponent<Canvas>();
            
            if (canvasScaler == null)
                canvasScaler = GetComponent<CanvasScaler>();
            
            if (safeAreaTransform == null)
                safeAreaTransform = GetComponent<RectTransform>();
        }
        
        private void Start()
        {
            DetectDevice();
            AdjustForDevice();
        }
        
        private void Update()
        {
            // Check for orientation changes
            float newAspectRatio = (float)Screen.width / Screen.height;
            if (Mathf.Abs(newAspectRatio - currentAspectRatio) > 0.01f)
            {
                currentAspectRatio = newAspectRatio;
                OnAspectRatioChanged?.Invoke(currentAspectRatio);
                AdjustForDevice();
            }
        }
        
        /// <summary>
        /// Detects the current device type and characteristics
        /// </summary>
        private void DetectDevice()
        {
            currentAspectRatio = (float)Screen.width / Screen.height;
            
            // Detect device type based on aspect ratio
            if (currentAspectRatio >= tabletThreshold)
            {
                currentDeviceType = DeviceType.Tablet;
            }
            else if (currentAspectRatio >= phoneThreshold)
            {
                currentDeviceType = DeviceType.Phone;
            }
            else
            {
                currentDeviceType = DeviceType.Unknown;
            }
            
            // Detect notch (simplified detection)
            hasNotch = DetectNotch();
            
            OnDeviceTypeChanged?.Invoke(currentDeviceType);
        }
        
        /// <summary>
        /// Detects if the device has a notch (simplified)
        /// </summary>
        /// <returns>True if device likely has a notch</returns>
        private bool DetectNotch()
        {
            // This is a simplified detection
            // In a real implementation, you'd use Screen.safeArea
            return Screen.safeArea.y > 0 || Screen.safeArea.height < Screen.height;
        }
        
        /// <summary>
        /// Adjusts UI elements for the current device
        /// </summary>
        public void AdjustForDevice()
        {
            AdjustCanvasScaler();
            AdjustSafeArea();
            AdjustUIElements();
        }
        
        /// <summary>
        /// Adjusts the canvas scaler based on device type
        /// </summary>
        private void AdjustCanvasScaler()
        {
            if (canvasScaler == null) return;
            
            switch (currentDeviceType)
            {
                case DeviceType.Tablet:
                    canvasScaler.scaleFactor = tabletScale;
                    break;
                case DeviceType.Phone:
                    canvasScaler.scaleFactor = phoneScale;
                    break;
                default:
                    canvasScaler.scaleFactor = baseScale;
                    break;
            }
        }
        
        /// <summary>
        /// Adjusts safe area handling
        /// </summary>
        private void AdjustSafeArea()
        {
            if (!handleSafeArea || safeAreaTransform == null) return;
            
            Rect safeArea = Screen.safeArea;
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            
            // Convert to normalized coordinates
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            
            // Apply safe area
            safeAreaTransform.anchorMin = anchorMin;
            safeAreaTransform.anchorMax = anchorMax;
            
            // Add padding if needed
            if (hasNotch)
            {
                safeAreaTransform.offsetMin = new Vector2(safeAreaPadding, safeAreaPadding);
                safeAreaTransform.offsetMax = new Vector2(-safeAreaPadding, -safeAreaPadding);
            }
        }
        
        /// <summary>
        /// Adjusts individual UI elements based on device type
        /// </summary>
        private void AdjustUIElements()
        {
            // Adjust touch targets for different devices
            AdjustTouchTargets();
            
            // Adjust font sizes if needed
            AdjustFontSizes();
            
            // Adjust spacing and padding
            AdjustSpacing();
        }
        
        /// <summary>
        /// Adjusts touch targets for different devices
        /// </summary>
        private void AdjustTouchTargets()
        {
            // Find all buttons and adjust their size
            Button[] buttons = FindObjectsOfType<Button>();
            float minTouchSize = 44f; // Minimum touch target size in points
            
            foreach (Button button in buttons)
            {
                RectTransform rectTransform = button.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    // Ensure minimum touch target size
                    if (rectTransform.sizeDelta.x < minTouchSize)
                    {
                        rectTransform.sizeDelta = new Vector2(minTouchSize, rectTransform.sizeDelta.y);
                    }
                    
                    if (rectTransform.sizeDelta.y < minTouchSize)
                    {
                        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, minTouchSize);
                    }
                }
            }
        }
        
        /// <summary>
        /// Adjusts font sizes based on device type
        /// </summary>
        private void AdjustFontSizes()
        {
            // This would adjust font sizes for better readability on different devices
            // Implementation depends on your text components
        }
        
        /// <summary>
        /// Adjusts spacing and padding based on device type
        /// </summary>
        private void AdjustSpacing()
        {
            // This would adjust spacing between UI elements
            // Implementation depends on your layout system
        }
        
        /// <summary>
        /// Gets the current device type
        /// </summary>
        /// <returns>Current device type</returns>
        public DeviceType GetDeviceType()
        {
            return currentDeviceType;
        }
        
        /// <summary>
        /// Gets the current aspect ratio
        /// </summary>
        /// <returns>Current aspect ratio</returns>
        public float GetAspectRatio()
        {
            return currentAspectRatio;
        }
        
        /// <summary>
        /// Checks if the device has a notch
        /// </summary>
        /// <returns>True if device has a notch</returns>
        public bool HasNotch()
        {
            return hasNotch;
        }
        
        /// <summary>
        /// Checks if the current device is a tablet
        /// </summary>
        /// <returns>True if device is a tablet</returns>
        public bool IsTablet()
        {
            return currentDeviceType == DeviceType.Tablet;
        }
        
        /// <summary>
        /// Checks if the current device is a phone
        /// </summary>
        /// <returns>True if device is a phone</returns>
        public bool IsPhone()
        {
            return currentDeviceType == DeviceType.Phone;
        }
        
        /// <summary>
        /// Forces a device adjustment
        /// </summary>
        public void ForceAdjustment()
        {
            DetectDevice();
            AdjustForDevice();
        }
        
        /// <summary>
        /// Sets the target aspect ratio
        /// </summary>
        /// <param name="aspectRatio">Target aspect ratio</param>
        public void SetTargetAspectRatio(float aspectRatio)
        {
            targetAspectRatio = aspectRatio;
            AdjustForDevice();
        }
        
        /// <summary>
        /// Sets the device type manually
        /// </summary>
        /// <param name="deviceType">Device type to set</param>
        public void SetDeviceType(DeviceType deviceType)
        {
            currentDeviceType = deviceType;
            AdjustForDevice();
        }
        
        #region Editor Debugging
        #if UNITY_EDITOR
        [Header("Editor Debug Controls")]
        [SerializeField] private bool showDebugControls = false;
        
        private void OnGUI()
        {
            if (!showDebugControls) return;
            
            GUILayout.BeginArea(new Rect(10, 400, 300, 200));
            GUILayout.Label("UI Scaler Debug");
            GUILayout.Label($"Device Type: {currentDeviceType}");
            GUILayout.Label($"Aspect Ratio: {currentAspectRatio:F3}");
            GUILayout.Label($"Has Notch: {hasNotch}");
            GUILayout.Label($"Screen Size: {Screen.width}x{Screen.height}");
            GUILayout.Label($"Safe Area: {Screen.safeArea}");
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Force Adjustment"))
                ForceAdjustment();
            if (GUILayout.Button("Set Phone"))
                SetDeviceType(DeviceType.Phone);
            if (GUILayout.Button("Set Tablet"))
                SetDeviceType(DeviceType.Tablet);
            
            GUILayout.EndArea();
        }
        #endif
        #endregion
    }
}
