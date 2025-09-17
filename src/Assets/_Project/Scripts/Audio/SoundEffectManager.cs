/******************************************************************************
 * SWITCH - SoundEffectManager
 * Sprint: 2
 * Author: Implementation Team
 * Date: January 2025
 * 
 * Description: Central sound effects manager for all game audio
 * Dependencies: Unity AudioSource, AudioClip
 * 
 * Educational Notes:
 * - Demonstrates centralized audio management
 * - Shows how to implement efficient sound effect systems
 * - Performance: Efficient audio pooling and management
 *****************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SWITCH.Audio
{
    /// <summary>
    /// Central sound effects manager for all game audio.
    /// Educational: This demonstrates centralized audio management.
    /// Performance: Efficient audio pooling and management with minimal overhead.
    /// </summary>
    public class SoundEffectManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private bool enableSoundEffects = true;
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private int maxConcurrentSounds = 20;
        
        [Header("Audio Settings")]
        [SerializeField] private bool enableAudioPooling = true;
        [SerializeField] private float audioFadeTime = 0.5f;
        [SerializeField] private bool enableSpatialAudio = false;
        [SerializeField] private float spatialAudioRange = 10f;
        
        [Header("Sound Effect Categories")]
        [SerializeField] private float uiVolume = 1f;
        [SerializeField] private float gameplayVolume = 1f;
        [SerializeField] private float powerUpVolume = 1f;
        [SerializeField] private float ambientVolume = 1f;
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource uiAudioSource;
        [SerializeField] private AudioSource gameplayAudioSource;
        [SerializeField] private AudioSource powerUpAudioSource;
        [SerializeField] private AudioSource ambientAudioSource;
        
        // Sound effect registry
        private Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>();
        private Dictionary<string, SoundEffectData> soundEffectData = new Dictionary<string, SoundEffectData>();
        private List<AudioSource> audioSourcePool = new List<AudioSource>();
        
        // Performance tracking
        private int totalSoundsPlayed = 0;
        private int totalSoundsCompleted = 0;
        private int totalSoundsCancelled = 0;
        private float totalAudioTime = 0f;
        
        // Events
        public System.Action<string> OnSoundEffectPlayed;
        public System.Action<string> OnSoundEffectCompleted;
        public System.Action<string> OnSoundEffectCancelled;
        public System.Action<float> OnMasterVolumeChanged;
        
        // Singleton instance
        private static SoundEffectManager instance;
        public static SoundEffectManager Instance => instance;
        
        // Properties
        public bool EnableSoundEffects => enableSoundEffects;
        public float MasterVolume => masterVolume;
        public int ActiveSoundCount => audioSourcePool.Count(s => s.isPlaying);
        public int TotalSoundsPlayed => totalSoundsPlayed;
        
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
            InitializeSoundEffectManager();
        }
        
        /// <summary>
        /// Initializes the sound effect manager.
        /// Educational: Shows how to set up audio systems.
        /// </summary>
        private void InitializeSoundEffectManager()
        {
            Log("Initializing Sound Effect Manager");
            
            // Initialize audio sources
            InitializeAudioSources();
            
            // Initialize sound effect registry
            InitializeSoundEffectRegistry();
            
            // Initialize audio pool
            if (enableAudioPooling)
            {
                InitializeAudioPool();
            }
            
            Log("Sound Effect Manager initialized");
        }
        
        /// <summary>
        /// Initializes audio sources.
        /// Educational: Shows how to set up audio sources.
        /// </summary>
        private void InitializeAudioSources()
        {
            // Create audio sources if not assigned
            if (uiAudioSource == null)
            {
                GameObject uiObj = new GameObject("UI Audio Source");
                uiObj.transform.SetParent(transform);
                uiAudioSource = uiObj.AddComponent<AudioSource>();
                uiAudioSource.playOnAwake = false;
            }
            
            if (gameplayAudioSource == null)
            {
                GameObject gameplayObj = new GameObject("Gameplay Audio Source");
                gameplayObj.transform.SetParent(transform);
                gameplayAudioSource = gameplayObj.AddComponent<AudioSource>();
                gameplayAudioSource.playOnAwake = false;
            }
            
            if (powerUpAudioSource == null)
            {
                GameObject powerUpObj = new GameObject("PowerUp Audio Source");
                powerUpObj.transform.SetParent(transform);
                powerUpAudioSource = powerUpObj.AddComponent<AudioSource>();
                powerUpAudioSource.playOnAwake = false;
            }
            
            if (ambientAudioSource == null)
            {
                GameObject ambientObj = new GameObject("Ambient Audio Source");
                ambientObj.transform.SetParent(transform);
                ambientAudioSource = ambientObj.AddComponent<AudioSource>();
                ambientAudioSource.playOnAwake = false;
                ambientAudioSource.loop = true;
            }
        }
        
        /// <summary>
        /// Initializes the sound effect registry.
        /// Educational: Shows how to set up sound effect registries.
        /// </summary>
        private void InitializeSoundEffectRegistry()
        {
            soundEffects.Clear();
            soundEffectData.Clear();
            
            // TODO: Load sound effects from Resources or AssetDatabase
            // This would typically load audio clips and their metadata
            
            Log("Sound effect registry initialized");
        }
        
        /// <summary>
        /// Initializes the audio source pool.
        /// Educational: Shows how to implement audio pooling.
        /// </summary>
        private void InitializeAudioPool()
        {
            audioSourcePool.Clear();
            
            // Pre-allocate audio sources
            for (int i = 0; i < maxConcurrentSounds; i++)
            {
                GameObject poolObj = new GameObject($"Pooled Audio Source {i}");
                poolObj.transform.SetParent(transform);
                AudioSource audioSource = poolObj.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSourcePool.Add(audioSource);
            }
            
            Log($"Initialized audio pool with {maxConcurrentSounds} sources");
        }
        
        /// <summary>
        /// Registers a sound effect.
        /// Educational: Shows how to register sound effects.
        /// </summary>
        /// <param name="soundId">Sound effect ID</param>
        /// <param name="audioClip">Audio clip</param>
        /// <param name="category">Sound category</param>
        /// <param name="volume">Volume level</param>
        public void RegisterSoundEffect(string soundId, AudioClip audioClip, SoundCategory category = SoundCategory.Gameplay, float volume = 1f)
        {
            if (string.IsNullOrEmpty(soundId) || audioClip == null)
            {
                LogError($"Cannot register sound effect: {soundId}");
                return;
            }
            
            soundEffects[soundId] = audioClip;
            soundEffectData[soundId] = new SoundEffectData
            {
                soundId = soundId,
                audioClip = audioClip,
                category = category,
                volume = volume,
                isRegistered = true
            };
            
            Log($"Registered sound effect: {soundId} ({category})");
        }
        
        /// <summary>
        /// Plays a sound effect.
        /// Educational: Shows how to play sound effects.
        /// </summary>
        /// <param name="soundId">Sound effect ID</param>
        /// <param name="volume">Volume override</param>
        /// <param name="pitch">Pitch override</param>
        /// <returns>Audio source used for playback</returns>
        public AudioSource PlaySoundEffect(string soundId, float volume = -1f, float pitch = 1f)
        {
            if (!enableSoundEffects || !soundEffects.TryGetValue(soundId, out var audioClip))
            {
                LogWarning($"Cannot play sound effect: {soundId}");
                return null;
            }
            
            // Get audio source
            AudioSource audioSource = GetAudioSource(soundId);
            if (audioSource == null)
            {
                LogError($"No available audio source for: {soundId}");
                return null;
            }
            
            // Set up audio source
            audioSource.clip = audioClip;
            audioSource.volume = CalculateVolume(soundId, volume);
            audioSource.pitch = pitch;
            audioSource.spatialBlend = enableSpatialAudio ? 1f : 0f;
            audioSource.maxDistance = spatialAudioRange;
            
            // Play sound
            audioSource.Play();
            
            // Update tracking
            totalSoundsPlayed++;
            totalAudioTime += audioClip.length;
            
            // Fire events
            OnSoundEffectPlayed?.Invoke(soundId);
            
            Log($"Playing sound effect: {soundId} at volume {audioSource.volume}");
            
            return audioSource;
        }
        
        /// <summary>
        /// Plays a sound effect at a specific position.
        /// Educational: Shows how to play spatial audio.
        /// </summary>
        /// <param name="soundId">Sound effect ID</param>
        /// <param name="position">World position</param>
        /// <param name="volume">Volume override</param>
        /// <param name="pitch">Pitch override</param>
        /// <returns>Audio source used for playback</returns>
        public AudioSource PlaySoundEffectAtPosition(string soundId, Vector3 position, float volume = -1f, float pitch = 1f)
        {
            AudioSource audioSource = PlaySoundEffect(soundId, volume, pitch);
            
            if (audioSource != null)
            {
                audioSource.transform.position = position;
                audioSource.spatialBlend = 1f; // Force 3D audio
            }
            
            return audioSource;
        }
        
        /// <summary>
        /// Stops a sound effect.
        /// Educational: Shows how to stop sound effects.
        /// </summary>
        /// <param name="soundId">Sound effect ID</param>
        public void StopSoundEffect(string soundId)
        {
            // Find and stop audio source playing this sound
            foreach (var audioSource in audioSourcePool)
            {
                if (audioSource.clip != null && audioSource.clip.name == soundId && audioSource.isPlaying)
                {
                    audioSource.Stop();
                    OnSoundEffectCancelled?.Invoke(soundId);
                    totalSoundsCancelled++;
                    Log($"Stopped sound effect: {soundId}");
                    return;
                }
            }
        }
        
        /// <summary>
        /// Stops all sound effects.
        /// Educational: Shows how to stop all audio.
        /// </summary>
        public void StopAllSoundEffects()
        {
            foreach (var audioSource in audioSourcePool)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
            
            Log("Stopped all sound effects");
        }
        
        /// <summary>
        /// Sets the master volume.
        /// Educational: Shows how to control audio volume.
        /// </summary>
        /// <param name="volume">New master volume</param>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            
            // Update all audio sources
            foreach (var audioSource in audioSourcePool)
            {
                if (audioSource.clip != null && soundEffectData.TryGetValue(audioSource.clip.name, out var data))
                {
                    audioSource.volume = CalculateVolume(audioSource.clip.name, -1f);
                }
            }
            
            OnMasterVolumeChanged?.Invoke(masterVolume);
            Log($"Master volume set to: {masterVolume}");
        }
        
        /// <summary>
        /// Sets the volume for a specific category.
        /// Educational: Shows how to control category volumes.
        /// </summary>
        /// <param name="category">Sound category</param>
        /// <param name="volume">New volume</param>
        public void SetCategoryVolume(SoundCategory category, float volume)
        {
            volume = Mathf.Clamp01(volume);
            
            switch (category)
            {
                case SoundCategory.UI:
                    uiVolume = volume;
                    break;
                case SoundCategory.Gameplay:
                    gameplayVolume = volume;
                    break;
                case SoundCategory.PowerUp:
                    powerUpVolume = volume;
                    break;
                case SoundCategory.Ambient:
                    ambientVolume = volume;
                    break;
            }
            
            Log($"Category volume set: {category} = {volume}");
        }
        
        /// <summary>
        /// Gets an available audio source.
        /// Educational: Shows how to implement audio source pooling.
        /// </summary>
        /// <param name="soundId">Sound effect ID</param>
        /// <returns>Available audio source</returns>
        private AudioSource GetAudioSource(string soundId)
        {
            // Try to get a pooled audio source
            if (enableAudioPooling)
            {
                foreach (var audioSource in audioSourcePool)
                {
                    if (!audioSource.isPlaying)
                    {
                        return audioSource;
                    }
                }
            }
            
            // Fallback to category-specific audio source
            if (soundEffectData.TryGetValue(soundId, out var data))
            {
                switch (data.category)
                {
                    case SoundCategory.UI:
                        return uiAudioSource;
                    case SoundCategory.Gameplay:
                        return gameplayAudioSource;
                    case SoundCategory.PowerUp:
                        return powerUpAudioSource;
                    case SoundCategory.Ambient:
                        return ambientAudioSource;
                }
            }
            
            return gameplayAudioSource; // Default fallback
        }
        
        /// <summary>
        /// Calculates the final volume for a sound effect.
        /// Educational: Shows how to calculate audio volumes.
        /// </summary>
        /// <param name="soundId">Sound effect ID</param>
        /// <param name="volumeOverride">Volume override</param>
        /// <returns>Final volume</returns>
        private float CalculateVolume(string soundId, float volumeOverride)
        {
            float baseVolume = volumeOverride >= 0f ? volumeOverride : 1f;
            
            if (soundEffectData.TryGetValue(soundId, out var data))
            {
                baseVolume *= data.volume;
                
                switch (data.category)
                {
                    case SoundCategory.UI:
                        baseVolume *= uiVolume;
                        break;
                    case SoundCategory.Gameplay:
                        baseVolume *= gameplayVolume;
                        break;
                    case SoundCategory.PowerUp:
                        baseVolume *= powerUpVolume;
                        break;
                    case SoundCategory.Ambient:
                        baseVolume *= ambientVolume;
                        break;
                }
            }
            
            return baseVolume * masterVolume;
        }
        
        /// <summary>
        /// Gets system statistics.
        /// Educational: Shows how to provide system analytics.
        /// </summary>
        /// <returns>System statistics</returns>
        public SoundEffectManagerStats GetSystemStats()
        {
            return new SoundEffectManagerStats
            {
                ActiveSoundCount = ActiveSoundCount,
                TotalSoundsPlayed = totalSoundsPlayed,
                TotalSoundsCompleted = totalSoundsCompleted,
                TotalSoundsCancelled = totalSoundsCancelled,
                TotalAudioTime = totalAudioTime,
                MasterVolume = masterVolume,
                RegisteredSoundCount = soundEffects.Count
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
                Debug.Log($"[SoundEffectManager] {message}");
        }
        
        /// <summary>
        /// Logs a warning message.
        /// Educational: Shows how to implement warning logging.
        /// </summary>
        /// <param name="message">Warning message to log</param>
        private void LogWarning(string message)
        {
            Debug.LogWarning($"[SoundEffectManager] {message}");
        }
        
        /// <summary>
        /// Logs an error message.
        /// Educational: Shows how to implement error logging.
        /// </summary>
        /// <param name="message">Error message to log</param>
        private void LogError(string message)
        {
            Debug.LogError($"[SoundEffectManager] {message}");
        }
    }
    
    /// <summary>
    /// Sound effect categories.
    /// Educational: Shows how to define audio categories.
    /// </summary>
    public enum SoundCategory
    {
        UI,
        Gameplay,
        PowerUp,
        Ambient
    }
    
    /// <summary>
    /// Sound effect data structure.
    /// Educational: Shows how to define sound effect data.
    /// </summary>
    [System.Serializable]
    public struct SoundEffectData
    {
        public string soundId;
        public AudioClip audioClip;
        public SoundCategory category;
        public float volume;
        public bool isRegistered;
    }
    
    /// <summary>
    /// Sound effect manager statistics.
    /// Educational: Shows how to create analytics data structures.
    /// </summary>
    [System.Serializable]
    public struct SoundEffectManagerStats
    {
        public int ActiveSoundCount;
        public int TotalSoundsPlayed;
        public int TotalSoundsCompleted;
        public int TotalSoundsCancelled;
        public float TotalAudioTime;
        public float MasterVolume;
        public int RegisteredSoundCount;
    }
}
