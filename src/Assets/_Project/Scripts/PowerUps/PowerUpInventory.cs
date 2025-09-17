/******************************************************************************
 * SWITCH - PowerUpInventory
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Power-up inventory system for managing player's power-up collection
 * Dependencies: IPowerUp, PowerUpManager
 * 
 * Educational Notes:
 * - Demonstrates inventory management system design
 * - Shows how to integrate with power-up manager
 * - Performance: Efficient inventory operations with minimal allocations
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SWITCH.PowerUps
{
    /// <summary>
    /// Power-up inventory system for managing player's power-up collection.
    /// Educational: This demonstrates inventory management system design.
    /// Performance: Efficient inventory operations with minimal allocations.
    /// </summary>
    public class PowerUpInventory : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private int maxInventorySize = 10;
        [SerializeField] private bool allowDuplicatePowerUps = true;
        [SerializeField] private bool enableDebugLogs = false;
        
        [Header("References")]
        [SerializeField] private PowerUpManager powerUpManager;
        
        // Inventory data
        private Dictionary<string, int> inventory = new Dictionary<string, int>();
        private List<string> inventoryOrder = new List<string>();
        
        // Events
        public event Action<string, int> OnPowerUpAdded;
        public event Action<string, int> OnPowerUpRemoved;
        public event Action<string, int> OnPowerUpCountChanged;
        public event Action OnInventoryChanged;
        public event Action OnInventoryFull;
        public event Action OnInventoryEmpty;
        
        // Properties
        public int MaxInventorySize => maxInventorySize;
        public int CurrentInventorySize => inventory.Values.Sum();
        public bool IsFull => CurrentInventorySize >= maxInventorySize;
        public bool IsEmpty => CurrentInventorySize == 0;
        public int UniquePowerUpCount => inventory.Count;
        
        private void Awake()
        {
            // Get references if not assigned
            if (powerUpManager == null)
                powerUpManager = FindObjectOfType<PowerUpManager>();
        }
        
        private void Start()
        {
            InitializeInventory();
        }
        
        /// <summary>
        /// Initializes the inventory system.
        /// Educational: Shows how to set up inventory management.
        /// </summary>
        private void InitializeInventory()
        {
            Log("Initializing Power-Up Inventory");
            
            // Clear any existing data
            inventory.Clear();
            inventoryOrder.Clear();
            
            Log($"Power-Up Inventory initialized with max size: {maxInventorySize}");
        }
        
        /// <summary>
        /// Adds a power-up to the inventory.
        /// Educational: Shows how to implement inventory addition with validation.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to add</param>
        /// <param name="count">Number to add</param>
        /// <returns>True if power-up was successfully added</returns>
        public bool AddPowerUp(string powerUpId, int count = 1)
        {
            if (string.IsNullOrEmpty(powerUpId))
            {
                LogError("Cannot add power-up with null or empty ID");
                return false;
            }
            
            if (count <= 0)
            {
                LogError($"Cannot add {count} power-ups (must be positive)");
                return false;
            }
            
            // Check if power-up is registered
            if (powerUpManager != null && powerUpManager.GetPowerUp(powerUpId) == null)
            {
                LogError($"Cannot add unregistered power-up: {powerUpId}");
                return false;
            }
            
            // Check inventory capacity
            if (IsFull && !inventory.ContainsKey(powerUpId))
            {
                LogWarning($"Cannot add {powerUpId}: inventory is full");
                OnInventoryFull?.Invoke();
                return false;
            }
            
            // Check duplicate policy
            if (!allowDuplicatePowerUps && inventory.ContainsKey(powerUpId) && inventory[powerUpId] > 0)
            {
                LogWarning($"Cannot add duplicate power-up: {powerUpId}");
                return false;
            }
            
            // Add to inventory
            if (!inventory.ContainsKey(powerUpId))
            {
                inventory[powerUpId] = 0;
                inventoryOrder.Add(powerUpId);
            }
            
            int oldCount = inventory[powerUpId];
            inventory[powerUpId] += count;
            
            Log($"Added {count} {powerUpId} to inventory (total: {inventory[powerUpId]})");
            
            // Fire events
            OnPowerUpAdded?.Invoke(powerUpId, count);
            OnPowerUpCountChanged?.Invoke(powerUpId, inventory[powerUpId]);
            OnInventoryChanged?.Invoke();
            
            return true;
        }
        
        /// <summary>
        /// Removes a power-up from the inventory.
        /// Educational: Shows how to implement inventory removal with validation.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to remove</param>
        /// <param name="count">Number to remove</param>
        /// <returns>True if power-up was successfully removed</returns>
        public bool RemovePowerUp(string powerUpId, int count = 1)
        {
            if (string.IsNullOrEmpty(powerUpId))
            {
                LogError("Cannot remove power-up with null or empty ID");
                return false;
            }
            
            if (count <= 0)
            {
                LogError($"Cannot remove {count} power-ups (must be positive)");
                return false;
            }
            
            if (!inventory.ContainsKey(powerUpId))
            {
                LogWarning($"Cannot remove {powerUpId}: not in inventory");
                return false;
            }
            
            int currentCount = inventory[powerUpId];
            if (currentCount < count)
            {
                LogWarning($"Cannot remove {count} {powerUpId}: only {currentCount} available");
                return false;
            }
            
            // Remove from inventory
            inventory[powerUpId] -= count;
            
            // Remove from order if count reaches zero
            if (inventory[powerUpId] <= 0)
            {
                inventory[powerUpId] = 0;
                inventoryOrder.Remove(powerUpId);
            }
            
            Log($"Removed {count} {powerUpId} from inventory (remaining: {inventory[powerUpId]})");
            
            // Fire events
            OnPowerUpRemoved?.Invoke(powerUpId, count);
            OnPowerUpCountChanged?.Invoke(powerUpId, inventory[powerUpId]);
            OnInventoryChanged?.Invoke();
            
            // Check if inventory is now empty
            if (IsEmpty)
            {
                OnInventoryEmpty?.Invoke();
            }
            
            return true;
        }
        
        /// <summary>
        /// Gets the count of a power-up in the inventory.
        /// Educational: Shows how to query inventory state safely.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to check</param>
        /// <returns>Count in inventory</returns>
        public int GetPowerUpCount(string powerUpId)
        {
            if (string.IsNullOrEmpty(powerUpId))
                return 0;
            
            return inventory.TryGetValue(powerUpId, out var count) ? count : 0;
        }
        
        /// <summary>
        /// Checks if a power-up is in the inventory.
        /// Educational: Shows how to check inventory membership.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to check</param>
        /// <returns>True if power-up is in inventory</returns>
        public bool HasPowerUp(string powerUpId)
        {
            return GetPowerUpCount(powerUpId) > 0;
        }
        
        /// <summary>
        /// Gets all power-ups in the inventory.
        /// Educational: Shows how to enumerate inventory contents.
        /// </summary>
        /// <returns>Dictionary of power-up IDs and their counts</returns>
        public Dictionary<string, int> GetAllPowerUps()
        {
            return new Dictionary<string, int>(inventory);
        }
        
        /// <summary>
        /// Gets all power-ups in inventory order.
        /// Educational: Shows how to maintain inventory order.
        /// </summary>
        /// <returns>List of power-up IDs in order</returns>
        public List<string> GetPowerUpOrder()
        {
            return new List<string>(inventoryOrder);
        }
        
        /// <summary>
        /// Gets the first power-up in the inventory.
        /// Educational: Shows how to get inventory items by order.
        /// </summary>
        /// <returns>First power-up ID or null if empty</returns>
        public string GetFirstPowerUp()
        {
            return inventoryOrder.Count > 0 ? inventoryOrder[0] : null;
        }
        
        /// <summary>
        /// Gets the last power-up in the inventory.
        /// Educational: Shows how to get inventory items by order.
        /// </summary>
        /// <returns>Last power-up ID or null if empty</returns>
        public string GetLastPowerUp()
        {
            return inventoryOrder.Count > 0 ? inventoryOrder[inventoryOrder.Count - 1] : null;
        }
        
        /// <summary>
        /// Moves a power-up to the front of the inventory order.
        /// Educational: Shows how to manipulate inventory order.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to move</param>
        /// <returns>True if power-up was moved</returns>
        public bool MoveToFront(string powerUpId)
        {
            if (!inventoryOrder.Contains(powerUpId))
                return false;
            
            inventoryOrder.Remove(powerUpId);
            inventoryOrder.Insert(0, powerUpId);
            
            Log($"Moved {powerUpId} to front of inventory");
            OnInventoryChanged?.Invoke();
            
            return true;
        }
        
        /// <summary>
        /// Moves a power-up to the back of the inventory order.
        /// Educational: Shows how to manipulate inventory order.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to move</param>
        /// <returns>True if power-up was moved</returns>
        public bool MoveToBack(string powerUpId)
        {
            if (!inventoryOrder.Contains(powerUpId))
                return false;
            
            inventoryOrder.Remove(powerUpId);
            inventoryOrder.Add(powerUpId);
            
            Log($"Moved {powerUpId} to back of inventory");
            OnInventoryChanged?.Invoke();
            
            return true;
        }
        
        /// <summary>
        /// Clears all power-ups from the inventory.
        /// Educational: Shows how to reset inventory state.
        /// </summary>
        public void ClearInventory()
        {
            inventory.Clear();
            inventoryOrder.Clear();
            
            Log("Cleared power-up inventory");
            OnInventoryChanged?.Invoke();
            OnInventoryEmpty?.Invoke();
        }
        
        /// <summary>
        /// Sets the maximum inventory size.
        /// Educational: Shows how to modify inventory configuration.
        /// </summary>
        /// <param name="newMaxSize">New maximum size</param>
        public void SetMaxInventorySize(int newMaxSize)
        {
            if (newMaxSize < 0)
            {
                LogError($"Cannot set negative inventory size: {newMaxSize}");
                return;
            }
            
            maxInventorySize = newMaxSize;
            Log($"Set max inventory size to {maxInventorySize}");
        }
        
        /// <summary>
        /// Sets whether duplicate power-ups are allowed.
        /// Educational: Shows how to modify inventory policy.
        /// </summary>
        /// <param name="allowDuplicates">Whether to allow duplicates</param>
        public void SetAllowDuplicates(bool allowDuplicates)
        {
            allowDuplicatePowerUps = allowDuplicates;
            Log($"Set allow duplicates to {allowDuplicatePowerUps}");
        }
        
        /// <summary>
        /// Gets inventory statistics.
        /// Educational: Shows how to provide inventory analytics.
        /// </summary>
        /// <returns>Inventory statistics</returns>
        public InventoryStats GetInventoryStats()
        {
            return new InventoryStats
            {
                TotalPowerUps = CurrentInventorySize,
                UniquePowerUps = UniquePowerUpCount,
                MaxSize = maxInventorySize,
                IsFull = IsFull,
                IsEmpty = IsEmpty,
                MostCommonPowerUp = GetMostCommonPowerUp(),
                LeastCommonPowerUp = GetLeastCommonPowerUp()
            };
        }
        
        /// <summary>
        /// Gets the most common power-up in the inventory.
        /// Educational: Shows how to analyze inventory contents.
        /// </summary>
        /// <returns>Most common power-up ID or null if empty</returns>
        private string GetMostCommonPowerUp()
        {
            if (inventory.Count == 0)
                return null;
            
            return inventory.OrderByDescending(kvp => kvp.Value).First().Key;
        }
        
        /// <summary>
        /// Gets the least common power-up in the inventory.
        /// Educational: Shows how to analyze inventory contents.
        /// </summary>
        /// <returns>Least common power-up ID or null if empty</returns>
        private string GetLeastCommonPowerUp()
        {
            if (inventory.Count == 0)
                return null;
            
            return inventory.OrderBy(kvp => kvp.Value).First().Key;
        }
        
        /// <summary>
        /// Logs a message if debug logging is enabled.
        /// Educational: Shows how to implement conditional logging.
        /// </summary>
        /// <param name="message">Message to log</param>
        private void Log(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[PowerUpInventory] {message}");
        }
        
        /// <summary>
        /// Logs a warning message.
        /// Educational: Shows how to implement warning logging.
        /// </summary>
        /// <param name="message">Warning message to log</param>
        private void LogWarning(string message)
        {
            Debug.LogWarning($"[PowerUpInventory] {message}");
        }
        
        /// <summary>
        /// Logs an error message.
        /// Educational: Shows how to implement error logging.
        /// </summary>
        /// <param name="message">Error message to log</param>
        private void LogError(string message)
        {
            Debug.LogError($"[PowerUpInventory] {message}");
        }
    }
    
    /// <summary>
    /// Statistics about the power-up inventory.
    /// Educational: Shows how to create data structures for analytics.
    /// </summary>
    [System.Serializable]
    public struct InventoryStats
    {
        public int TotalPowerUps;
        public int UniquePowerUps;
        public int MaxSize;
        public bool IsFull;
        public bool IsEmpty;
        public string MostCommonPowerUp;
        public string LeastCommonPowerUp;
    }
}
