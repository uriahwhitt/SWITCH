/******************************************************************************
 * SWITCH - PowerUpSlotUI
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Individual power-up slot UI component for inventory display
 * Dependencies: IPowerUp, PowerUpInventoryUI
 * 
 * Educational Notes:
 * - Demonstrates individual UI component design
 * - Shows how to create interactive power-up slots
 * - Performance: Efficient slot updates with minimal redraws
 *****************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SWITCH.PowerUps;

namespace SWITCH.UI
{
    /// <summary>
    /// Individual power-up slot UI component for inventory display.
    /// Educational: This demonstrates individual UI component design.
    /// Performance: Efficient slot updates with minimal redraws and smooth animations.
    /// </summary>
    public class PowerUpSlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button slotButton;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private Slider cooldownSlider;
        
        [Header("Visual Settings")]
        [SerializeField] private Color availableColor = Color.white;
        [SerializeField] private Color unavailableColor = Color.gray;
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private Color cooldownColor = Color.red;
        
        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private bool enableHoverEffects = true;
        [SerializeField] private bool enableClickEffects = true;
        
        // Slot state
        private int slotIndex = -1;
        private string powerUpId = null;
        private IPowerUp powerUp = null;
        private int powerUpCount = 0;
        private bool isSelected = false;
        private bool canUse = false;
        private float cooldown = 0f;
        private PowerUpInventoryUI parentUI = null;
        
        // Animation state
        private Coroutine scaleAnimationCoroutine = null;
        private Vector3 originalScale = Vector3.one;
        
        // Properties
        public int SlotIndex => slotIndex;
        public string PowerUpId => powerUpId;
        public IPowerUp PowerUp => powerUp;
        public int PowerUpCount => powerUpCount;
        public bool IsSelected => isSelected;
        public bool CanUse => canUse;
        public float Cooldown => cooldown;
        
        private void Awake()
        {
            // Store original scale for animations
            originalScale = transform.localScale;
            
            // Set up button listener
            if (slotButton != null)
                slotButton.onClick.AddListener(OnSlotClicked);
        }
        
        private void Start()
        {
            // Initialize slot appearance
            UpdateSlotAppearance();
        }
        
        /// <summary>
        /// Initializes the power-up slot.
        /// Educational: Shows how to initialize UI components.
        /// </summary>
        /// <param name="index">Slot index</param>
        /// <param name="parent">Parent inventory UI</param>
        public void Initialize(int index, PowerUpInventoryUI parent)
        {
            slotIndex = index;
            parentUI = parent;
            
            // Clear slot initially
            ClearSlot();
        }
        
        /// <summary>
        /// Updates the slot with power-up data.
        /// Educational: Shows how to update UI components with data.
        /// </summary>
        /// <param name="powerUp">Power-up instance</param>
        /// <param name="count">Power-up count</param>
        /// <param name="canUse">Whether power-up can be used</param>
        /// <param name="cooldown">Current cooldown</param>
        public void UpdateSlot(IPowerUp powerUp, int count, bool canUse, float cooldown)
        {
            this.powerUp = powerUp;
            this.powerUpId = powerUp?.PowerUpId;
            this.powerUpCount = count;
            this.canUse = canUse;
            this.cooldown = cooldown;
            
            UpdateSlotAppearance();
        }
        
        /// <summary>
        /// Clears the slot.
        /// Educational: Shows how to reset UI components.
        /// </summary>
        public void ClearSlot()
        {
            powerUp = null;
            powerUpId = null;
            powerUpCount = 0;
            canUse = false;
            cooldown = 0f;
            isSelected = false;
            
            UpdateSlotAppearance();
        }
        
        /// <summary>
        /// Sets the selected state of the slot.
        /// Educational: Shows how to manage selection state.
        /// </summary>
        /// <param name="selected">Whether slot is selected</param>
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            UpdateSlotAppearance();
        }
        
        /// <summary>
        /// Updates the cooldown display.
        /// Educational: Shows how to update cooldown visuals.
        /// </summary>
        /// <param name="cooldown">New cooldown value</param>
        public void UpdateCooldown(float cooldown)
        {
            this.cooldown = cooldown;
            UpdateCooldownDisplay();
        }
        
        /// <summary>
        /// Updates the slot appearance based on current state.
        /// Educational: Shows how to manage visual state.
        /// </summary>
        private void UpdateSlotAppearance()
        {
            // Update icon
            if (iconImage != null)
            {
                if (powerUp != null)
                {
                    iconImage.sprite = powerUp.Icon;
                    iconImage.color = GetIconColor();
                }
                else
                {
                    iconImage.sprite = null;
                    iconImage.color = Color.clear;
                }
            }
            
            // Update count text
            if (countText != null)
            {
                if (powerUpCount > 0)
                {
                    countText.text = powerUpCount.ToString();
                    countText.color = GetTextColor();
                }
                else
                {
                    countText.text = "";
                }
            }
            
            // Update name text
            if (nameText != null)
            {
                if (powerUp != null)
                {
                    nameText.text = powerUp.DisplayName;
                    nameText.color = GetTextColor();
                }
                else
                {
                    nameText.text = "";
                }
            }
            
            // Update background
            if (backgroundImage != null)
            {
                backgroundImage.color = GetBackgroundColor();
            }
            
            // Update cooldown display
            UpdateCooldownDisplay();
            
            // Update button interactability
            if (slotButton != null)
            {
                slotButton.interactable = powerUp != null && powerUpCount > 0;
            }
        }
        
        /// <summary>
        /// Updates the cooldown display.
        /// Educational: Shows how to implement cooldown visuals.
        /// </summary>
        private void UpdateCooldownDisplay()
        {
            // Update cooldown overlay
            if (cooldownOverlay != null)
            {
                if (cooldown > 0f)
                {
                    cooldownOverlay.gameObject.SetActive(true);
                    cooldownOverlay.color = new Color(cooldownColor.r, cooldownColor.g, cooldownColor.b, 0.5f);
                }
                else
                {
                    cooldownOverlay.gameObject.SetActive(false);
                }
            }
            
            // Update cooldown slider
            if (cooldownSlider != null)
            {
                if (cooldown > 0f && powerUp != null)
                {
                    cooldownSlider.gameObject.SetActive(true);
                    cooldownSlider.value = 1f - (cooldown / powerUp.CooldownTime);
                }
                else
                {
                    cooldownSlider.gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// Gets the appropriate icon color based on slot state.
        /// Educational: Shows how to determine visual state.
        /// </summary>
        /// <returns>Icon color</returns>
        private Color GetIconColor()
        {
            if (powerUp == null)
                return Color.clear;
            
            if (isSelected)
                return selectedColor;
            
            if (cooldown > 0f)
                return cooldownColor;
            
            if (canUse)
                return availableColor;
            
            return unavailableColor;
        }
        
        /// <summary>
        /// Gets the appropriate text color based on slot state.
        /// Educational: Shows how to determine visual state.
        /// </summary>
        /// <returns>Text color</returns>
        private Color GetTextColor()
        {
            if (powerUp == null)
                return Color.clear;
            
            if (isSelected)
                return selectedColor;
            
            if (cooldown > 0f)
                return cooldownColor;
            
            if (canUse)
                return availableColor;
            
            return unavailableColor;
        }
        
        /// <summary>
        /// Gets the appropriate background color based on slot state.
        /// Educational: Shows how to determine visual state.
        /// </summary>
        /// <returns>Background color</returns>
        private Color GetBackgroundColor()
        {
            if (powerUp == null)
                return Color.clear;
            
            if (isSelected)
                return new Color(selectedColor.r, selectedColor.g, selectedColor.b, 0.3f);
            
            if (cooldown > 0f)
                return new Color(cooldownColor.r, cooldownColor.g, cooldownColor.b, 0.1f);
            
            if (canUse)
                return new Color(availableColor.r, availableColor.g, availableColor.b, 0.1f);
            
            return new Color(unavailableColor.r, unavailableColor.g, unavailableColor.b, 0.1f);
        }
        
        /// <summary>
        /// Handles slot click events.
        /// Educational: Shows how to handle user interaction.
        /// </summary>
        private void OnSlotClicked()
        {
            if (powerUp == null || powerUpCount <= 0)
                return;
            
            // Select this slot
            if (parentUI != null)
            {
                parentUI.SelectPowerUp(powerUpId);
            }
            
            // Play click animation
            if (enableClickEffects)
            {
                PlayClickAnimation();
            }
        }
        
        /// <summary>
        /// Plays the click animation.
        /// Educational: Shows how to implement UI animations.
        /// </summary>
        private void PlayClickAnimation()
        {
            if (scaleAnimationCoroutine != null)
            {
                StopCoroutine(scaleAnimationCoroutine);
            }
            
            scaleAnimationCoroutine = StartCoroutine(ScaleAnimation(0.8f, 1.2f, 1f));
        }
        
        /// <summary>
        /// Plays a scale animation.
        /// Educational: Shows how to implement smooth UI animations.
        /// </summary>
        /// <param name="startScale">Starting scale</param>
        /// <param name="peakScale">Peak scale</param>
        /// <param name="endScale">Ending scale</param>
        /// <returns>Animation coroutine</returns>
        private System.Collections.IEnumerator ScaleAnimation(float startScale, float peakScale, float endScale)
        {
            float elapsed = 0f;
            
            // Scale down
            while (elapsed < animationDuration * 0.3f)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / (animationDuration * 0.3f);
                float scale = Mathf.Lerp(startScale, peakScale, scaleCurve.Evaluate(progress));
                transform.localScale = originalScale * scale;
                yield return null;
            }
            
            // Scale up
            elapsed = 0f;
            while (elapsed < animationDuration * 0.7f)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / (animationDuration * 0.7f);
                float scale = Mathf.Lerp(peakScale, endScale, scaleCurve.Evaluate(progress));
                transform.localScale = originalScale * scale;
                yield return null;
            }
            
            // Ensure final scale
            transform.localScale = originalScale * endScale;
            scaleAnimationCoroutine = null;
        }
        
        /// <summary>
        /// Shows tooltip information for this power-up.
        /// Educational: Shows how to implement tooltip systems.
        /// </summary>
        public void ShowTooltip()
        {
            if (powerUp == null)
                return;
            
            // TODO: Implement tooltip system
            Debug.Log($"[PowerUpSlotUI] Tooltip: {powerUp.DisplayName} - {powerUp.Description}");
        }
        
        /// <summary>
        /// Hides tooltip information.
        /// Educational: Shows how to implement tooltip systems.
        /// </summary>
        public void HideTooltip()
        {
            // TODO: Implement tooltip system
        }
    }
}
