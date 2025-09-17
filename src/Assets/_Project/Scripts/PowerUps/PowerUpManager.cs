/******************************************************************************
 * SWITCH - PowerUpManager
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Central manager for all power-up systems
 * Dependencies: IPowerUp, PowerUpContext, GameManager
 * 
 * Educational Notes:
 * - Demonstrates manager pattern for power-up coordination
 * - Shows how to integrate power-ups with game systems
 * - Performance: Efficient power-up registration and execution
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SWITCH.Core;
using SWITCH.Data;

namespace SWITCH.PowerUps
{
    /// <summary>
    /// Central manager for all power-up systems.
    /// Educational: This demonstrates the manager pattern for power-up coordination.
    /// Performance: Efficient power-up registration and execution with minimal overhead.
    /// </summary>
    public class PowerUpManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private float globalCooldownTime = 0.5f;
        
        [Header("Power-up Inventory")]
        [SerializeField] private int maxInventorySize = 10;
        [SerializeField] private bool allowDuplicatePowerUps = true;
        
        [Header("References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private BoardController boardController;
        
        // Power-up registry
        private Dictionary<string, IPowerUp> registeredPowerUps = new Dictionary<string, IPowerUp>();
        private Dictionary<string, float> powerUpCooldowns = new Dictionary<string, float>();
        private Dictionary<string, int> powerUpInventory = new Dictionary<string, int>();
        
        // Execution state
        private bool isExecutingPowerUp = false;
        private float lastPowerUpTime = 0f;
        private string currentExecutingPowerUp = null;
        
        // Events
        public event Action<string> OnPowerUpRegistered;
        public event Action<string> OnPowerUpUsed;
        public event Action<string, int> OnPowerUpAdded;
        public event Action<string> OnPowerUpExecutionStarted;
        public event Action<string, bool> OnPowerUpExecutionCompleted;
        public event Action<string, float> OnPowerUpCooldownChanged;
        
        // Properties
        public bool IsExecutingPowerUp => isExecutingPowerUp;
        public int InventoryCount => powerUpInventory.Values.Sum();
        public bool IsInventoryFull => InventoryCount >= maxInventorySize;
        
        private void Awake()
        {
            // Get references if not assigned
            if (gameManager == null)
                gameManager = FindObjectOfType<GameManager>();
            if (boardController == null)
                boardController = FindObjectOfType<BoardController>();
        }
        
        private void Start()
        {
            // Initialize power-up system
            InitializePowerUpSystem();
        }
        
        private void Update()
        {
            // Update cooldowns
            UpdateCooldowns();
        }
        
        /// <summary>
        /// Initializes the power-up system.
        /// Educational: Shows how to set up a manager system.
        /// </summary>
        private void InitializePowerUpSystem()
        {
            Log("Initializing Power-Up Manager");
            
            // Clear any existing data
            registeredPowerUps.Clear();
            powerUpCooldowns.Clear();
            powerUpInventory.Clear();
            
            // Register default power-ups
            RegisterDefaultPowerUps();
            
            Log($"Power-Up Manager initialized with {registeredPowerUps.Count} power-ups");
        }
        
        /// <summary>
        /// Registers default power-ups with the system.
        /// Educational: Shows how to register power-ups programmatically.
        /// </summary>
        private void RegisterDefaultPowerUps()
        {
            // Register basic power-ups
            RegisterPowerUp(new ColorBombPowerUp());
            RegisterPowerUp(new LineClearPowerUp());
            RegisterPowerUp(new AreaClearPowerUp());
            RegisterPowerUp(new TimeFreezePowerUp());
            RegisterPowerUp(new ScoreMultiplierPowerUp());
        }
        
        /// <summary>
        /// Registers a power-up with the manager.
        /// Educational: Demonstrates how to add power-ups to the system.
        /// </summary>
        /// <param name="powerUp">Power-up to register</param>
        public void RegisterPowerUp(IPowerUp powerUp)
        {
            if (powerUp == null)
            {
                LogError("Cannot register null power-up");
                return;
            }
            
            string id = powerUp.PowerUpId;
            if (registeredPowerUps.ContainsKey(id))
            {
                LogWarning($"Power-up {id} is already registered");
                return;
            }
            
            registeredPowerUps[id] = powerUp;
            powerUpCooldowns[id] = 0f;
            powerUpInventory[id] = 0;
            
            Log($"Registered power-up: {id}");
            OnPowerUpRegistered?.Invoke(id);
        }
        
        /// <summary>
        /// Unregisters a power-up from the manager.
        /// Educational: Shows how to remove power-ups from the system.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to unregister</param>
        public void UnregisterPowerUp(string powerUpId)
        {
            if (!registeredPowerUps.ContainsKey(powerUpId))
            {
                LogWarning($"Power-up {powerUpId} is not registered");
                return;
            }
            
            registeredPowerUps.Remove(powerUpId);
            powerUpCooldowns.Remove(powerUpId);
            powerUpInventory.Remove(powerUpId);
            
            Log($"Unregistered power-up: {powerUpId}");
        }
        
        /// <summary>
        /// Gets a registered power-up by ID.
        /// Educational: Shows how to retrieve power-ups safely.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to get</param>
        /// <returns>Power-up instance or null if not found</returns>
        public IPowerUp GetPowerUp(string powerUpId)
        {
            return registeredPowerUps.TryGetValue(powerUpId, out var powerUp) ? powerUp : null;
        }
        
        /// <summary>
        /// Gets all registered power-ups.
        /// Educational: Demonstrates how to enumerate power-ups.
        /// </summary>
        /// <returns>Collection of all registered power-ups</returns>
        public IEnumerable<IPowerUp> GetAllPowerUps()
        {
            return registeredPowerUps.Values;
        }
        
        /// <summary>
        /// Checks if a power-up can be used.
        /// Educational: Shows how to validate power-up usage.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to check</param>
        /// <returns>True if power-up can be used</returns>
        public bool CanUsePowerUp(string powerUpId)
        {
            if (!registeredPowerUps.TryGetValue(powerUpId, out var powerUp))
                return false;
            
            if (isExecutingPowerUp)
                return false;
            
            if (powerUpCooldowns[powerUpId] > 0f)
                return false;
            
            if (powerUpInventory[powerUpId] <= 0)
                return false;
            
            if (!powerUp.CanUse)
                return false;
            
            return true;
        }
        
        /// <summary>
        /// Uses a power-up with the given context.
        /// Educational: Demonstrates power-up execution with context.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to use</param>
        /// <param name="context">Context for power-up execution</param>
        /// <returns>True if power-up was successfully used</returns>
        public bool UsePowerUp(string powerUpId, PowerUpContext context = null)
        {
            if (!CanUsePowerUp(powerUpId))
            {
                LogWarning($"Cannot use power-up {powerUpId}");
                return false;
            }
            
            var powerUp = registeredPowerUps[powerUpId];
            
            // Create context if not provided
            if (context == null)
            {
                context = CreateDefaultContext();
            }
            
            // Validate context
            if (!context.IsValid())
            {
                LogError($"Invalid context for power-up {powerUpId}");
                return false;
            }
            
            // Validate power-up can execute
            if (!powerUp.CanExecute(context))
            {
                LogWarning($"Power-up {powerUpId} cannot execute in current context");
                return false;
            }
            
            // Execute power-up
            return ExecutePowerUp(powerUpId, powerUp, context);
        }
        
        /// <summary>
        /// Executes a power-up with proper state management.
        /// Educational: Shows how to manage power-up execution state.
        /// </summary>
        /// <param name="powerUpId">ID of power-up</param>
        /// <param name="powerUp">Power-up instance</param>
        /// <param name="context">Execution context</param>
        /// <returns>True if execution was successful</returns>
        private bool ExecutePowerUp(string powerUpId, IPowerUp powerUp, PowerUpContext context)
        {
            isExecutingPowerUp = true;
            currentExecutingPowerUp = powerUpId;
            lastPowerUpTime = Time.time;
            
            Log($"Executing power-up: {powerUpId}");
            OnPowerUpExecutionStarted?.Invoke(powerUpId);
            
            try
            {
                // Activate power-up
                powerUp.OnActivated();
                
                // Execute power-up
                bool success = powerUp.Execute(context);
                
                // Handle execution result
                if (success)
                {
                    // Consume power-up from inventory
                    powerUpInventory[powerUpId]--;
                    
                    // Set cooldown
                    powerUpCooldowns[powerUpId] = powerUp.CooldownTime;
                    
                    // Fire events
                    OnPowerUpUsed?.Invoke(powerUpId);
                    OnPowerUpCooldownChanged?.Invoke(powerUpId, powerUp.CooldownTime);
                }
                
                // Notify completion
                powerUp.OnExecuted(success);
                OnPowerUpExecutionCompleted?.Invoke(powerUpId, success);
                
                Log($"Power-up {powerUpId} execution {(success ? "succeeded" : "failed")}");
                return success;
            }
            catch (Exception ex)
            {
                LogError($"Error executing power-up {powerUpId}: {ex.Message}");
                powerUp.OnExecuted(false);
                OnPowerUpExecutionCompleted?.Invoke(powerUpId, false);
                return false;
            }
            finally
            {
                isExecutingPowerUp = false;
                currentExecutingPowerUp = null;
            }
        }
        
        /// <summary>
        /// Adds a power-up to the inventory.
        /// Educational: Shows how to manage power-up inventory.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to add</param>
        /// <param name="count">Number to add</param>
        public void AddPowerUp(string powerUpId, int count = 1)
        {
            if (!registeredPowerUps.ContainsKey(powerUpId))
            {
                LogWarning($"Cannot add unregistered power-up: {powerUpId}");
                return;
            }
            
            if (!allowDuplicatePowerUps && powerUpInventory[powerUpId] > 0)
            {
                LogWarning($"Duplicate power-ups not allowed: {powerUpId}");
                return;
            }
            
            powerUpInventory[powerUpId] += count;
            Log($"Added {count} {powerUpId} to inventory (total: {powerUpInventory[powerUpId]})");
            OnPowerUpAdded?.Invoke(powerUpId, count);
        }
        
        /// <summary>
        /// Removes a power-up from the inventory.
        /// Educational: Shows how to remove power-ups from inventory.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to remove</param>
        /// <param name="count">Number to remove</param>
        public void RemovePowerUp(string powerUpId, int count = 1)
        {
            if (!powerUpInventory.ContainsKey(powerUpId))
                return;
            
            powerUpInventory[powerUpId] = Mathf.Max(0, powerUpInventory[powerUpId] - count);
            Log($"Removed {count} {powerUpId} from inventory (remaining: {powerUpInventory[powerUpId]})");
        }
        
        /// <summary>
        /// Gets the count of a power-up in inventory.
        /// Educational: Shows how to query inventory state.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to check</param>
        /// <returns>Count in inventory</returns>
        public int GetPowerUpCount(string powerUpId)
        {
            return powerUpInventory.TryGetValue(powerUpId, out var count) ? count : 0;
        }
        
        /// <summary>
        /// Gets the remaining cooldown for a power-up.
        /// Educational: Shows how to query cooldown state.
        /// </summary>
        /// <param name="powerUpId">ID of power-up to check</param>
        /// <returns>Remaining cooldown in seconds</returns>
        public float GetPowerUpCooldown(string powerUpId)
        {
            return powerUpCooldowns.TryGetValue(powerUpId, out var cooldown) ? cooldown : 0f;
        }
        
        /// <summary>
        /// Creates a default context for power-up execution.
        /// Educational: Shows how to create execution contexts.
        /// </summary>
        /// <returns>Default power-up context</returns>
        private PowerUpContext CreateDefaultContext()
        {
            var context = new PowerUpContext(gameManager, boardController, null);
            context.CurrentScore = gameManager?.CurrentScore ?? 0;
            context.CurrentMomentum = 0f; // TODO: Get from momentum system
            return context;
        }
        
        /// <summary>
        /// Updates cooldowns for all power-ups.
        /// Educational: Shows how to manage cooldown timers.
        /// </summary>
        private void UpdateCooldowns()
        {
            var keys = powerUpCooldowns.Keys.ToList();
            foreach (var key in keys)
            {
                if (powerUpCooldowns[key] > 0f)
                {
                    powerUpCooldowns[key] -= Time.deltaTime;
                    if (powerUpCooldowns[key] <= 0f)
                    {
                        powerUpCooldowns[key] = 0f;
                        OnPowerUpCooldownChanged?.Invoke(key, 0f);
                    }
                }
            }
        }
        
        /// <summary>
        /// Clears all power-ups from inventory.
        /// Educational: Shows how to reset inventory state.
        /// </summary>
        public void ClearInventory()
        {
            powerUpInventory.Clear();
            foreach (var powerUp in registeredPowerUps.Keys)
            {
                powerUpInventory[powerUp] = 0;
            }
            Log("Cleared power-up inventory");
        }
        
        /// <summary>
        /// Resets all cooldowns.
        /// Educational: Shows how to reset cooldown state.
        /// </summary>
        public void ResetCooldowns()
        {
            foreach (var key in powerUpCooldowns.Keys.ToList())
            {
                powerUpCooldowns[key] = 0f;
                OnPowerUpCooldownChanged?.Invoke(key, 0f);
            }
            Log("Reset all power-up cooldowns");
        }
        
        /// <summary>
        /// Logs a message if debug logging is enabled.
        /// Educational: Shows how to implement conditional logging.
        /// </summary>
        /// <param name="message">Message to log</param>
        private void Log(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[PowerUpManager] {message}");
        }
        
        /// <summary>
        /// Logs a warning message.
        /// Educational: Shows how to implement warning logging.
        /// </summary>
        /// <param name="message">Warning message to log</param>
        private void LogWarning(string message)
        {
            Debug.LogWarning($"[PowerUpManager] {message}");
        }
        
        /// <summary>
        /// Logs an error message.
        /// Educational: Shows how to implement error logging.
        /// </summary>
        /// <param name="message">Error message to log</param>
        private void LogError(string message)
        {
            Debug.LogError($"[PowerUpManager] {message}");
        }
    }
}
