using UnityEngine;
using System.Collections;

namespace SWITCH.Services
{
    /// <summary>
    /// Manages dynamic audio system that responds to heat levels.
    /// Provides layered music, tempo changes, and sound effects based on momentum.
    /// </summary>
    public class HeatAudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource baseLayer;      // Always playing
        [SerializeField] private AudioSource rhythmLayer;    // Fades in at 3+ heat
        [SerializeField] private AudioSource melodyLayer;    // Fades in at 6+ heat
        [SerializeField] private AudioSource climaxLayer;    // Fades in at 9+ heat
        
        [Header("Audio Clips")]
        [SerializeField] private AudioClip[] heatUpSounds;   // Pitched up with heat level
        [SerializeField] private AudioClip[] coolDownSounds; // Descending tone
        [SerializeField] private AudioClip infernoBurst;     // Plays at max heat
        [SerializeField] private AudioClip powerOrbExplosion; // Power orb collection
        
        [Header("Tempo Configuration")]
        [SerializeField] private float baseBPM = 120f;
        [SerializeField] private float maxBPM = 180f;
        [SerializeField] private float tempoChangeSpeed = 2f;
        
        [Header("Fade Configuration")]
        [SerializeField] private float fadeInSpeed = 1f;
        [SerializeField] private float fadeOutSpeed = 2f;
        
        [Header("Heartbeat Effect")]
        [SerializeField] private bool enableHeartbeat = true;
        [SerializeField] private AudioClip heartbeatSound;
        [SerializeField] private float baseHeartbeatInterval = 1f;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        
        // References
        private MomentumSystem momentumSystem;
        private Coroutine heartbeatCoroutine;
        private Coroutine tempoChangeCoroutine;
        
        // Current state
        private float currentBPM;
        private MomentumSystem.HeatLevel lastHeatLevel;
        private bool isHeartbeatActive = false;
        
        private void Awake()
        {
            momentumSystem = FindObjectOfType<MomentumSystem>();
            if (momentumSystem == null)
            {
                Debug.LogError("[HeatAudioManager] MomentumSystem not found!");
                return;
            }
            
            // Subscribe to momentum events
            momentumSystem.OnHeatLevelChanged += OnHeatLevelChanged;
            momentumSystem.OnMaxHeatReached += OnMaxHeatReached;
            momentumSystem.OnHeatDecay += OnHeatDecay;
        }
        
        private void Start()
        {
            InitializeAudioLayers();
            currentBPM = baseBPM;
            lastHeatLevel = momentumSystem.CurrentHeatLevel;
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (momentumSystem != null)
            {
                momentumSystem.OnHeatLevelChanged -= OnHeatLevelChanged;
                momentumSystem.OnMaxHeatReached -= OnMaxHeatReached;
                momentumSystem.OnHeatDecay -= OnHeatDecay;
            }
        }
        
        /// <summary>
        /// Initializes audio layers with proper settings
        /// </summary>
        private void InitializeAudioLayers()
        {
            // Configure base layer (always playing)
            if (baseLayer != null)
            {
                baseLayer.volume = 1f;
                baseLayer.loop = true;
                baseLayer.Play();
            }
            
            // Configure other layers (start muted)
            ConfigureAudioLayer(rhythmLayer, 0f, true);
            ConfigureAudioLayer(melodyLayer, 0f, true);
            ConfigureAudioLayer(climaxLayer, 0f, true);
        }
        
        /// <summary>
        /// Configures an audio layer with volume and loop settings
        /// </summary>
        /// <param name="audioSource">Audio source to configure</param>
        /// <param name="volume">Initial volume</param>
        /// <param name="loop">Should loop</param>
        private void ConfigureAudioLayer(AudioSource audioSource, float volume, bool loop)
        {
            if (audioSource != null)
            {
                audioSource.volume = volume;
                audioSource.loop = loop;
                if (volume > 0f)
                {
                    audioSource.Play();
                }
            }
        }
        
        /// <summary>
        /// Called when heat level changes
        /// </summary>
        /// <param name="newHeat">New heat value</param>
        private void OnHeatLevelChanged(float newHeat)
        {
            var newHeatLevel = momentumSystem.GetHeatLevel(newHeat);
            
            if (newHeatLevel != lastHeatLevel)
            {
                UpdateAudioForHeatLevel(newHeatLevel);
                lastHeatLevel = newHeatLevel;
            }
            
            // Update tempo based on heat
            UpdateTempo(newHeat);
            
            // Play heat up sound
            PlayHeatUpSound(newHeatLevel);
        }
        
        /// <summary>
        /// Called when maximum heat is reached (Power Orb)
        /// </summary>
        private void OnMaxHeatReached()
        {
            PlayPowerOrbExplosion();
            UpdateAudioForHeatLevel(MomentumSystem.HeatLevel.Inferno);
        }
        
        /// <summary>
        /// Called when heat decays
        /// </summary>
        private void OnHeatDecay()
        {
            PlayCoolDownSound();
        }
        
        /// <summary>
        /// Updates audio layers based on heat level
        /// </summary>
        /// <param name="heatLevel">Current heat level</param>
        private void UpdateAudioForHeatLevel(MomentumSystem.HeatLevel heatLevel)
        {
            switch (heatLevel)
            {
                case MomentumSystem.HeatLevel.Cold:
                    FadeOutLayer(rhythmLayer);
                    FadeOutLayer(melodyLayer);
                    FadeOutLayer(climaxLayer);
                    StopHeartbeat();
                    break;
                    
                case MomentumSystem.HeatLevel.Warm:
                    FadeInLayer(rhythmLayer);
                    FadeOutLayer(melodyLayer);
                    FadeOutLayer(climaxLayer);
                    StopHeartbeat();
                    break;
                    
                case MomentumSystem.HeatLevel.Hot:
                    FadeInLayer(rhythmLayer);
                    FadeInLayer(melodyLayer);
                    FadeOutLayer(climaxLayer);
                    StartHeartbeat();
                    break;
                    
                case MomentumSystem.HeatLevel.Blazing:
                    FadeInLayer(rhythmLayer);
                    FadeInLayer(melodyLayer);
                    FadeInLayer(climaxLayer);
                    StartHeartbeat();
                    break;
                    
                case MomentumSystem.HeatLevel.Inferno:
                    FadeInLayer(rhythmLayer);
                    FadeInLayer(melodyLayer);
                    FadeInLayer(climaxLayer);
                    StartHeartbeat();
                    break;
            }
        }
        
        /// <summary>
        /// Updates tempo based on current heat
        /// </summary>
        /// <param name="heat">Current heat value</param>
        private void UpdateTempo(float heat)
        {
            float targetBPM = baseBPM + (heat * (maxBPM - baseBPM) / 10f);
            
            if (tempoChangeCoroutine != null)
            {
                StopCoroutine(tempoChangeCoroutine);
            }
            
            tempoChangeCoroutine = StartCoroutine(ChangeTempoSmoothly(targetBPM));
        }
        
        /// <summary>
        /// Smoothly changes tempo over time
        /// </summary>
        /// <param name="targetBPM">Target BPM</param>
        /// <returns>Coroutine</returns>
        private IEnumerator ChangeTempoSmoothly(float targetBPM)
        {
            float startBPM = currentBPM;
            float elapsed = 0f;
            
            while (elapsed < tempoChangeSpeed)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / tempoChangeSpeed;
                currentBPM = Mathf.Lerp(startBPM, targetBPM, t);
                
                // Apply tempo to all audio sources
                ApplyTempoToAllLayers();
                
                yield return null;
            }
            
            currentBPM = targetBPM;
            ApplyTempoToAllLayers();
        }
        
        /// <summary>
        /// Applies current tempo to all audio layers
        /// </summary>
        private void ApplyTempoToAllLayers()
        {
            float pitch = currentBPM / baseBPM;
            
            SetPitch(baseLayer, pitch);
            SetPitch(rhythmLayer, pitch);
            SetPitch(melodyLayer, pitch);
            SetPitch(climaxLayer, pitch);
        }
        
        /// <summary>
        /// Sets pitch for an audio source
        /// </summary>
        /// <param name="audioSource">Audio source</param>
        /// <param name="pitch">Pitch value</param>
        private void SetPitch(AudioSource audioSource, float pitch)
        {
            if (audioSource != null)
            {
                audioSource.pitch = pitch;
            }
        }
        
        /// <summary>
        /// Fades in an audio layer
        /// </summary>
        /// <param name="audioSource">Audio source to fade in</param>
        private void FadeInLayer(AudioSource audioSource)
        {
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
            
            StartCoroutine(FadeAudio(audioSource, 1f, fadeInSpeed));
        }
        
        /// <summary>
        /// Fades out an audio layer
        /// </summary>
        /// <param name="audioSource">Audio source to fade out</param>
        private void FadeOutLayer(AudioSource audioSource)
        {
            StartCoroutine(FadeAudio(audioSource, 0f, fadeOutSpeed));
        }
        
        /// <summary>
        /// Fades audio volume over time
        /// </summary>
        /// <param name="audioSource">Audio source</param>
        /// <param name="targetVolume">Target volume</param>
        /// <param name="fadeSpeed">Fade speed</param>
        /// <returns>Coroutine</returns>
        private IEnumerator FadeAudio(AudioSource audioSource, float targetVolume, float fadeSpeed)
        {
            if (audioSource == null) yield break;
            
            float startVolume = audioSource.volume;
            float elapsed = 0f;
            
            while (elapsed < fadeSpeed)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeSpeed;
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
                yield return null;
            }
            
            audioSource.volume = targetVolume;
            
            // Stop audio if volume is 0
            if (targetVolume <= 0f && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
        
        /// <summary>
        /// Starts heartbeat effect
        /// </summary>
        private void StartHeartbeat()
        {
            if (!enableHeartbeat || heartbeatSound == null || isHeartbeatActive) return;
            
            isHeartbeatActive = true;
            heartbeatCoroutine = StartCoroutine(HeartbeatLoop());
        }
        
        /// <summary>
        /// Stops heartbeat effect
        /// </summary>
        private void StopHeartbeat()
        {
            if (heartbeatCoroutine != null)
            {
                StopCoroutine(heartbeatCoroutine);
                heartbeatCoroutine = null;
            }
            isHeartbeatActive = false;
        }
        
        /// <summary>
        /// Heartbeat loop coroutine
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator HeartbeatLoop()
        {
            while (isHeartbeatActive)
            {
                float heat = momentumSystem.CurrentMomentum;
                float heartbeatInterval = baseHeartbeatInterval - (heat * 0.08f);
                heartbeatInterval = Mathf.Clamp(heartbeatInterval, 0.2f, 1f);
                
                // Play heartbeat sound
                if (baseLayer != null)
                {
                    baseLayer.PlayOneShot(heartbeatSound, 0.3f);
                }
                
                yield return new WaitForSeconds(heartbeatInterval);
            }
        }
        
        /// <summary>
        /// Plays heat up sound effect
        /// </summary>
        /// <param name="heatLevel">Current heat level</param>
        private void PlayHeatUpSound(MomentumSystem.HeatLevel heatLevel)
        {
            if (heatUpSounds == null || heatUpSounds.Length == 0) return;
            
            int soundIndex = (int)heatLevel;
            if (soundIndex < heatUpSounds.Length && heatUpSounds[soundIndex] != null)
            {
                baseLayer.PlayOneShot(heatUpSounds[soundIndex], 0.5f);
            }
        }
        
        /// <summary>
        /// Plays cool down sound effect
        /// </summary>
        private void PlayCoolDownSound()
        {
            if (coolDownSounds == null || coolDownSounds.Length == 0) return;
            
            int randomIndex = Random.Range(0, coolDownSounds.Length);
            if (coolDownSounds[randomIndex] != null)
            {
                baseLayer.PlayOneShot(coolDownSounds[randomIndex], 0.4f);
            }
        }
        
        /// <summary>
        /// Plays power orb explosion sound
        /// </summary>
        private void PlayPowerOrbExplosion()
        {
            if (powerOrbExplosion != null)
            {
                baseLayer.PlayOneShot(powerOrbExplosion, 0.8f);
            }
        }
        
        /// <summary>
        /// Plays inferno burst sound at max heat
        /// </summary>
        private void PlayInfernoBurst()
        {
            if (infernoBurst != null)
            {
                baseLayer.PlayOneShot(infernoBurst, 1f);
            }
        }
        
        /// <summary>
        /// Debug logging helper
        /// </summary>
        /// <param name="message">Message to log</param>
        private void LogDebug(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[HeatAudioManager] {message}");
            }
        }
        
        #region Editor Debugging
        #if UNITY_EDITOR
        [Header("Editor Debug Controls")]
        [SerializeField] private bool showDebugControls = false;
        
        private void OnGUI()
        {
            if (!showDebugControls) return;
            
            GUILayout.BeginArea(new Rect(10, 220, 300, 150));
            GUILayout.Label("Heat Audio Manager Debug");
            GUILayout.Label($"Current BPM: {currentBPM:F1}");
            GUILayout.Label($"Heat Level: {lastHeatLevel}");
            GUILayout.Label($"Heartbeat Active: {isHeartbeatActive}");
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Test Heat Up Sound"))
                PlayHeatUpSound(lastHeatLevel);
            if (GUILayout.Button("Test Cool Down Sound"))
                PlayCoolDownSound();
            if (GUILayout.Button("Test Power Orb Explosion"))
                PlayPowerOrbExplosion();
            if (GUILayout.Button("Test Inferno Burst"))
                PlayInfernoBurst();
            
            GUILayout.EndArea();
        }
        #endif
        #endregion
    }
}
