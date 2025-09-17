/******************************************************************************
 * SWITCH - PowerUpInventoryUI
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: UI system for displaying and managing power-up inventory
 * Dependencies: PowerUpInventory, PowerUpManager, IPowerUp
 * 
 * Educational Notes:
 * - Demonstrates UI integration with inventory systems
 * - Shows how to create responsive power-up displays
 * - Performance: Efficient UI updates with minimal redraws
 *****************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SWITCH.PowerUps;

namespace SWITCH.UI
{
    /// <summary>
    /// UI system for displaying and managing power-up inventory.
    /// Educational: This demonstrates UI integration with inventory systems.
    /// Performance: Efficient UI updates with minimal redraws and object pooling.
    /// </summary>
    public class PowerUpInventoryUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform inventoryContainer;
        [SerializeField] private GameObject powerUpSlotPrefab;
        [SerializeField] private ScrollRect inventoryScrollRect;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button clearAllButton;
        
        [Header("Display Settings")]
        [SerializeField] private int maxVisibleSlots = 5;
        [SerializeField] private float slotSpacing = 10f;
        [SerializeField] private bool showEmptySlots = false;
        [SerializeField] private bool enableDragAndDrop = true;
        
        [Header("Visual Settings")]
        [SerializeField] private Color availableColor = Color.white;
        [SerializeField] private Color unavailableColor = Color.gray;
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private Color cooldownColor = Color.red;
        
        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private bool enableSlotAnimations = true;
        
        [Header("References")]
        [SerializeField] private PowerUpInventory powerUpInventory;
        [SerializeField] private PowerUpManager powerUpManager;
        
        // UI state
        private List<PowerUpSlotUI> powerUpSlots = new List<PowerUpSlotUI>();
        private Dictionary<string, PowerUpSlotUI> slotMap = new Dictionary<string, PowerUpSlotUI>();
        private string selectedPowerUpId = null;
        private bool isInitialized = false;
        
        // Events
        public System.Action<string> OnPowerUpSelected;
        public System.Action<string> OnPowerUpUsed;
        public System.Action OnInventoryClosed;
        
        // Properties
        public bool IsOpen => gameObject.activeInHierarchy;
        public string SelectedPowerUpId => selectedPowerUpId;
        
        private void Awake()
        {
            // Get references if not assigned
            if (powerUpInventory == null)
                powerUpInventory = FindObjectOfType<PowerUpInventory>();
            if (powerUpManager == null)
                powerUpManager = FindObjectOfType<PowerUpManager>();
        }
        
        private void Start()
        {
            InitializeUI();
        }
        
        private void OnEnable()
        {
            SubscribeToEvents();
        }
        
        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }
        
        /// <summary>
        /// Initializes the power-up inventory UI.
        /// Educational: Shows how to set up complex UI systems.
        /// </summary>
        private void InitializeUI()
        {
            if (isInitialized)
                return;
            
            // Set up button listeners
            if (closeButton != null)
                closeButton.onClick.AddListener(CloseInventory);
            
            if (clearAllButton != null)
                clearAllButton.onClick.AddListener(ClearAllPowerUps);
            
            // Create initial slots
            CreatePowerUpSlots();
            
            // Update display
            UpdateInventoryDisplay();
            
            isInitialized = true;
            Debug.Log("[PowerUpInventoryUI] Initialized");
        }
        
        /// <summary>
        /// Creates power-up slots for the inventory display.
        /// Educational: Shows how to create dynamic UI elements.
        /// </summary>
        private void CreatePowerUpSlots()
        {
            if (powerUpSlotPrefab == null || inventoryContainer == null)
            {
                Debug.LogError("[PowerUpInventoryUI] Missing prefab or container reference");
                return;
            }
            
            // Clear existing slots
            ClearPowerUpSlots();
            
            // Create slots based on max visible slots
            for (int i = 0; i < maxVisibleSlots; i++)
            {
                GameObject slotObj = Instantiate(powerUpSlotPrefab, inventoryContainer);
                PowerUpSlotUI slotUI = slotObj.GetComponent<PowerUpSlotUI>();
                
                if (slotUI == null)
                {
                    Debug.LogError("[PowerUpInventoryUI] PowerUpSlotPrefab missing PowerUpSlotUI component");
                    Destroy(slotObj);
                    continue;
                }
                
                // Initialize slot
                slotUI.Initialize(i, this);
                powerUpSlots.Add(slotUI);
            }
            
            Debug.Log($"[PowerUpInventoryUI] Created {powerUpSlots.Count} power-up slots");
        }
        
        /// <summary>
        /// Clears all power-up slots.
        /// Educational: Shows how to clean up dynamic UI elements.
        /// </summary>
        private void ClearPowerUpSlots()
        {
            foreach (var slot in powerUpSlots)
            {
                if (slot != null)
                    Destroy(slot.gameObject);
            }
            
            powerUpSlots.Clear();
            slotMap.Clear();
        }
        
        /// <summary>
        /// Subscribes to inventory and manager events.
        /// Educational: Shows how to integrate with event systems.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (powerUpInventory != null)
            {
                powerUpInventory.OnPowerUpAdded += OnPowerUpAdded;
                powerUpInventory.OnPowerUpRemoved += OnPowerUpRemoved;
                powerUpInventory.OnPowerUpCountChanged += OnPowerUpCountChanged;
                powerUpInventory.OnInventoryChanged += OnInventoryChanged;
            }
            
            if (powerUpManager != null)
            {
                powerUpManager.OnPowerUpUsed += OnPowerUpUsed;
                powerUpManager.OnPowerUpCooldownChanged += OnPowerUpCooldownChanged;
            }
        }
        
        /// <summary>
        /// Unsubscribes from inventory and manager events.
        /// Educational: Shows how to properly clean up event subscriptions.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (powerUpInventory != null)
            {
                powerUpInventory.OnPowerUpAdded -= OnPowerUpAdded;
                powerUpInventory.OnPowerUpRemoved -= OnPowerUpRemoved;
                powerUpInventory.OnPowerUpCountChanged -= OnPowerUpCountChanged;
                powerUpInventory.OnInventoryChanged -= OnInventoryChanged;
            }
            
            if (powerUpManager != null)
            {
                powerUpManager.OnPowerUpUsed -= OnPowerUpUsed;
                powerUpManager.OnPowerUpCooldownChanged -= OnPowerUpCooldownChanged;
            }
        }
        
        /// <summary>
        /// Updates the entire inventory display.
        /// Educational: Shows how to efficiently update complex UI.
        /// </summary>
        public void UpdateInventoryDisplay()
        {
            if (!isInitialized)
                return;
            
            // Get current inventory
            var inventory = powerUpInventory?.GetAllPowerUps() ?? new Dictionary<string, int>();
            var inventoryOrder = powerUpInventory?.GetPowerUpOrder() ?? new List<string>();
            
            // Update slots
            for (int i = 0; i < powerUpSlots.Count; i++)
            {
                var slot = powerUpSlots[i];
                
                if (i < inventoryOrder.Count)
                {
                    string powerUpId = inventoryOrder[i];
                    int count = inventory[powerUpId];
                    
                    // Update slot with power-up data
                    UpdatePowerUpSlot(slot, powerUpId, count);
                }
                else
                {
                    // Clear slot
                    slot.ClearSlot();
                }
            }
        }
        
        /// <summary>
        /// Updates a specific power-up slot.
        /// Educational: Shows how to update individual UI elements.
        /// </summary>
        /// <param name="slot">Slot to update</param>
        /// <param name="powerUpId">Power-up ID</param>
        /// <param name="count">Power-up count</param>
        private void UpdatePowerUpSlot(PowerUpSlotUI slot, string powerUpId, int count)
        {
            if (slot == null)
                return;
            
            // Get power-up data
            var powerUp = powerUpManager?.GetPowerUp(powerUpId);
            if (powerUp == null)
            {
                slot.ClearSlot();
                return;
            }
            
            // Check if power-up can be used
            bool canUse = powerUpManager?.CanUsePowerUp(powerUpId) ?? false;
            float cooldown = powerUpManager?.GetPowerUpCooldown(powerUpId) ?? 0f;
            
            // Update slot
            slot.UpdateSlot(powerUp, count, canUse, cooldown);
            
            // Update slot map
            slotMap[powerUpId] = slot;
        }
        
        /// <summary>
        /// Opens the power-up inventory UI.
        /// Educational: Shows how to manage UI visibility.
        /// </summary>
        public void OpenInventory()
        {
            gameObject.SetActive(true);
            UpdateInventoryDisplay();
            Debug.Log("[PowerUpInventoryUI] Opened inventory");
        }
        
        /// <summary>
        /// Closes the power-up inventory UI.
        /// Educational: Shows how to manage UI visibility.
        /// </summary>
        public void CloseInventory()
        {
            gameObject.SetActive(false);
            selectedPowerUpId = null;
            OnInventoryClosed?.Invoke();
            Debug.Log("[PowerUpInventoryUI] Closed inventory");
        }
        
        /// <summary>
        /// Selects a power-up for use.
        /// Educational: Shows how to handle power-up selection.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to select</param>
        public void SelectPowerUp(string powerUpId)
        {
            if (string.IsNullOrEmpty(powerUpId))
                return;
            
            // Deselect previous power-up
            if (!string.IsNullOrEmpty(selectedPowerUpId) && slotMap.ContainsKey(selectedPowerUpId))
            {
                slotMap[selectedPowerUpId].SetSelected(false);
            }
            
            // Select new power-up
            selectedPowerUpId = powerUpId;
            if (slotMap.ContainsKey(powerUpId))
            {
                slotMap[powerUpId].SetSelected(true);
            }
            
            OnPowerUpSelected?.Invoke(powerUpId);
            Debug.Log($"[PowerUpInventoryUI] Selected power-up: {powerUpId}");
        }
        
        /// <summary>
        /// Uses the selected power-up.
        /// Educational: Shows how to handle power-up usage.
        /// </summary>
        public void UseSelectedPowerUp()
        {
            if (string.IsNullOrEmpty(selectedPowerUpId))
                return;
            
            if (powerUpManager?.CanUsePowerUp(selectedPowerUpId) == true)
            {
                powerUpManager.UsePowerUp(selectedPowerUpId);
                OnPowerUpUsed?.Invoke(selectedPowerUpId);
            }
        }
        
        /// <summary>
        /// Clears all power-ups from inventory.
        /// Educational: Shows how to handle bulk operations.
        /// </summary>
        private void ClearAllPowerUps()
        {
            powerUpInventory?.ClearInventory();
            Debug.Log("[PowerUpInventoryUI] Cleared all power-ups");
        }
        
        // Event handlers
        private void OnPowerUpAdded(string powerUpId, int count)
        {
            UpdateInventoryDisplay();
        }
        
        private void OnPowerUpRemoved(string powerUpId, int count)
        {
            UpdateInventoryDisplay();
        }
        
        private void OnPowerUpCountChanged(string powerUpId, int newCount)
        {
            if (slotMap.ContainsKey(powerUpId))
            {
                UpdatePowerUpSlot(slotMap[powerUpId], powerUpId, newCount);
            }
        }
        
        private void OnInventoryChanged()
        {
            UpdateInventoryDisplay();
        }
        
        private void OnPowerUpUsed(string powerUpId)
        {
            // Update display to reflect usage
            UpdateInventoryDisplay();
        }
        
        private void OnPowerUpCooldownChanged(string powerUpId, float cooldown)
        {
            if (slotMap.ContainsKey(powerUpId))
            {
                var slot = slotMap[powerUpId];
                slot.UpdateCooldown(cooldown);
            }
        }
    }
}
