using UnityEngine;
using System.Collections;

namespace SWITCH.PowerUps
{
    /// <summary>
    /// Power orb that spawns in center and moves toward target edge.
    /// Provides instant maximum heat when collected at correct edge.
    /// </summary>
    public class PowerOrb : MonoBehaviour, ITile
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer glowRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private Collider2D orbCollider;
        
        [Header("Data")]
        [SerializeField] private PowerOrbData data;
        
        [Header("State")]
        [SerializeField] private Vector2Int boardPosition;
        [SerializeField] private Vector2Int targetEdge;
        [SerializeField] private int age = 0;
        [SerializeField] private bool isActive = false;
        [SerializeField] private bool hasReachedEdge = false;
        
        [Header("Visual Effects")]
        [SerializeField] private ParticleSystem glowParticles;
        [SerializeField] private Light orbLight;
        
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float moveDelay = 1f;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        
        private Coroutine glowCoroutine;
        private Coroutine movementCoroutine;
        private Coroutine ageCoroutine;
        
        // Events
        public System.Action<PowerOrb> OnOrbCollected;
        public System.Action<PowerOrb> OnOrbLost;
        
        // Properties
        public Vector2Int BoardPosition => boardPosition;
        public Vector2Int TargetEdge => targetEdge;
        public int Age => age;
        public bool IsActive => isActive;
        public bool HasReachedEdge => hasReachedEdge;
        public PowerOrbData Data => data;
        
        /// <summary>
        /// Initializes the power orb with data and position
        /// </summary>
        /// <param name="orbData">Power orb configuration data</param>
        /// <param name="position">Board position to spawn at</param>
        public void Initialize(PowerOrbData orbData, Vector2Int position)
        {
            data = orbData;
            boardPosition = position;
            targetEdge = orbData.targetEdge;
            age = 0;
            isActive = true;
            hasReachedEdge = false;
            
            // Set visual properties
            if (spriteRenderer != null)
            {
                spriteRenderer.color = GetOrbColor(orbData.color);
            }
            
            if (glowRenderer != null)
            {
                glowRenderer.color = orbData.glowColor;
            }
            
            // Start visual effects
            StartGlowEffect();
            StartAgeTracking();
            
            // Start movement toward target edge
            StartMovement();
            
            LogDebug($"Power Orb initialized at {position} targeting edge {targetEdge}");
        }
        
        /// <summary>
        /// Moves the orb one step toward its target edge
        /// </summary>
        public void MoveTowardEdge()
        {
            if (!isActive || hasReachedEdge) return;
            
            Vector2Int direction = GetMovementDirection();
            Vector2Int newPosition = boardPosition + direction;
            
            // Check if we've reached the target edge
            if (IsAtTargetEdge(newPosition))
            {
                ReachTargetEdge();
            }
            else
            {
                boardPosition = newPosition;
                UpdateVisualPosition();
                LogDebug($"Power Orb moved to {boardPosition}");
            }
        }
        
        /// <summary>
        /// Called when orb reaches its target edge
        /// </summary>
        public void ReachTargetEdge()
        {
            hasReachedEdge = true;
            isActive = false;
            
            // Trigger collection effects
            StartCoroutine(CollectionEffect());
            
            // Notify collection
            OnOrbCollected?.Invoke(this);
            
            LogDebug($"Power Orb reached target edge {targetEdge}!");
        }
        
        /// <summary>
        /// Called when orb reaches wrong edge or is destroyed
        /// </summary>
        public void LoseOrb()
        {
            isActive = false;
            hasReachedEdge = false;
            
            // Trigger loss effects
            StartCoroutine(LossEffect());
            
            // Notify loss
            OnOrbLost?.Invoke(this);
            
            LogDebug("Power Orb lost!");
        }
        
        /// <summary>
        /// Gets the score value for this orb
        /// </summary>
        /// <returns>Score value based on age and base score</returns>
        public int GetScoreValue()
        {
            if (data == null) return 0;
            return data.CalculateScore(age);
        }
        
        /// <summary>
        /// Checks if orb has reached the correct edge
        /// </summary>
        /// <returns>True if at correct edge</returns>
        public bool ReachedCorrectEdge()
        {
            return hasReachedEdge && IsAtTargetEdge(boardPosition);
        }
        
        /// <summary>
        /// Gets the movement direction toward target edge
        /// </summary>
        /// <returns>Direction vector</returns>
        private Vector2Int GetMovementDirection()
        {
            Vector2Int direction = Vector2Int.zero;
            
            // Move toward target edge
            if (targetEdge.x == 0) // Left edge
            {
                direction.x = -1;
            }
            else if (targetEdge.x == 7) // Right edge
            {
                direction.x = 1;
            }
            
            if (targetEdge.y == 0) // Bottom edge
            {
                direction.y = -1;
            }
            else if (targetEdge.y == 7) // Top edge
            {
                direction.y = 1;
            }
            
            return direction;
        }
        
        /// <summary>
        /// Checks if position is at the target edge
        /// </summary>
        /// <param name="position">Position to check</param>
        /// <returns>True if at target edge</returns>
        private bool IsAtTargetEdge(Vector2Int position)
        {
            return position.x == targetEdge.x || position.y == targetEdge.y;
        }
        
        /// <summary>
        /// Gets the color for the orb based on its color type
        /// </summary>
        /// <param name="orbColor">Orb color type</param>
        /// <returns>Color value</returns>
        private Color GetOrbColor(OrbColor orbColor)
        {
            switch (orbColor)
            {
                case OrbColor.Red: return Color.red;
                case OrbColor.Blue: return Color.blue;
                case OrbColor.Green: return Color.green;
                case OrbColor.Yellow: return Color.yellow;
                case OrbColor.Purple: return Color.magenta;
                case OrbColor.Orange: return new Color(1f, 0.5f, 0f);
                default: return Color.white;
            }
        }
        
        /// <summary>
        /// Starts the glow effect
        /// </summary>
        private void StartGlowEffect()
        {
            if (glowCoroutine != null)
            {
                StopCoroutine(glowCoroutine);
            }
            
            glowCoroutine = StartCoroutine(GlowEffectCoroutine());
        }
        
        /// <summary>
        /// Glow effect coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator GlowEffectCoroutine()
        {
            while (isActive)
            {
                float pulse = Mathf.Sin(Time.time * data.pulseSpeed) * 0.5f + 0.5f;
                float intensity = data.glowIntensity * pulse;
                
                if (glowRenderer != null)
                {
                    Color color = glowRenderer.color;
                    color.a = intensity;
                    glowRenderer.color = color;
                }
                
                if (orbLight != null)
                {
                    orbLight.intensity = intensity;
                }
                
                yield return null;
            }
        }
        
        /// <summary>
        /// Starts age tracking
        /// </summary>
        private void StartAgeTracking()
        {
            if (ageCoroutine != null)
            {
                StopCoroutine(ageCoroutine);
            }
            
            ageCoroutine = StartCoroutine(AgeTrackingCoroutine());
        }
        
        /// <summary>
        /// Age tracking coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator AgeTrackingCoroutine()
        {
            while (isActive)
            {
                yield return new WaitForSeconds(1f); // Age every second
                age++;
            }
        }
        
        /// <summary>
        /// Starts movement toward target edge
        /// </summary>
        private void StartMovement()
        {
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
            }
            
            movementCoroutine = StartCoroutine(MovementCoroutine());
        }
        
        /// <summary>
        /// Movement coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator MovementCoroutine()
        {
            while (isActive && !hasReachedEdge)
            {
                yield return new WaitForSeconds(moveDelay);
                MoveTowardEdge();
            }
        }
        
        /// <summary>
        /// Updates visual position based on board position
        /// </summary>
        private void UpdateVisualPosition()
        {
            // Convert board position to world position
            Vector3 worldPos = new Vector3(boardPosition.x, boardPosition.y, 0f);
            transform.position = worldPos;
        }
        
        /// <summary>
        /// Collection effect coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator CollectionEffect()
        {
            // Play collection animation
            if (animator != null)
            {
                animator.SetTrigger("Collect");
            }
            
            // Play particles
            if (glowParticles != null)
            {
                glowParticles.Play();
            }
            
            // Wait for effect to complete
            yield return new WaitForSeconds(1f);
            
            // Deactivate
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Loss effect coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator LossEffect()
        {
            // Play loss animation
            if (animator != null)
            {
                animator.SetTrigger("Lost");
            }
            
            // Wait for effect to complete
            yield return new WaitForSeconds(0.5f);
            
            // Deactivate
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Debug logging helper
        /// </summary>
        /// <param name="message">Message to log</param>
        private void LogDebug(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[PowerOrb] {message}");
            }
        }
        
        #region ITile Implementation
        public Vector2Int Position => boardPosition;
        public TileType Type => TileType.PowerOrb;
        public bool IsMovable => false;
        public bool CanMatch => false;
        #endregion
        
        #region Editor Debugging
        #if UNITY_EDITOR
        [Header("Editor Debug Controls")]
        [SerializeField] private bool showDebugControls = false;
        
        private void OnGUI()
        {
            if (!showDebugControls || !isActive) return;
            
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            if (screenPos.z > 0)
            {
                GUILayout.BeginArea(new Rect(screenPos.x - 50, Screen.height - screenPos.y - 50, 100, 100));
                GUILayout.Label($"Age: {age}");
                GUILayout.Label($"Target: {targetEdge}");
                GUILayout.Label($"Score: {GetScoreValue()}");
                GUILayout.EndArea();
            }
        }
        #endif
        #endregion
    }
    
    /// <summary>
    /// Power orb color types
    /// </summary>
    public enum OrbColor
    {
        Red,
        Blue,
        Green,
        Yellow,
        Purple,
        Orange
    }
    
    /// <summary>
    /// Tile type enumeration
    /// </summary>
    public enum TileType
    {
        Normal,
        PowerOrb,
        BlockingBlock
    }
    
    /// <summary>
    /// Interface for all tile types
    /// </summary>
    public interface ITile
    {
        Vector2Int Position { get; }
        TileType Type { get; }
        bool IsMovable { get; }
        bool CanMatch { get; }
    }
}
