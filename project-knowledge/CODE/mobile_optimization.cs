// SWITCH Mobile Optimization - Performance Code Patterns
// This file contains mobile-specific optimization patterns

using System.Collections;
using UnityEngine;

namespace SWITCH.Optimization
{
    /// <summary>
    /// Mobile performance monitor
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        [Header("Performance Settings")]
        [SerializeField] private float updateInterval = 1.0f;
        [SerializeField] private int targetFPS = 60;
        [SerializeField] private float memoryWarningThreshold = 150f; // MB

        private float fps;
        private float memoryUsage;
        private int frameCount;
        private float timeAccumulator;

        public float FPS => fps;
        public float MemoryUsage => memoryUsage;
        public bool IsPerformanceGood => fps >= targetFPS && memoryUsage < memoryWarningThreshold;

        private void Start()
        {
            StartCoroutine(MonitorPerformance());
        }

        private IEnumerator MonitorPerformance()
        {
            while (true)
            {
                yield return new WaitForSeconds(updateInterval);
                
                // Calculate FPS
                fps = frameCount / timeAccumulator;
                frameCount = 0;
                timeAccumulator = 0f;

                // Calculate memory usage
                memoryUsage = (float)System.GC.GetTotalMemory(false) / (1024f * 1024f);

                // Log performance warnings
                if (fps < targetFPS)
                {
                    Debug.LogWarning($"Low FPS: {fps:F1} (Target: {targetFPS})");
                }

                if (memoryUsage > memoryWarningThreshold)
                {
                    Debug.LogWarning($"High Memory Usage: {memoryUsage:F1}MB (Threshold: {memoryWarningThreshold}MB)");
                }
            }
        }

        private void Update()
        {
            frameCount++;
            timeAccumulator += Time.unscaledDeltaTime;
        }
    }

    /// <summary>
    /// Optimized object pool with memory management
    /// </summary>
    public class OptimizedObjectPool<T> where T : MonoBehaviour
    {
        private Queue<T> pool = new Queue<T>();
        private T prefab;
        private Transform parent;
        private int maxPoolSize;
        private int currentSize;

        public OptimizedObjectPool(T prefab, int initialSize, int maxSize, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;
            this.maxPoolSize = maxSize;
            this.currentSize = 0;

            // Pre-allocate objects
            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
        }

        public T Get()
        {
            if (pool.Count > 0)
            {
                T obj = pool.Dequeue();
                obj.gameObject.SetActive(true);
                return obj;
            }
            else if (currentSize < maxPoolSize)
            {
                return CreateNewObject();
            }
            else
            {
                // Pool is full, create temporary object
                return UnityEngine.Object.Instantiate(prefab, parent);
            }
        }

        public void Return(T obj)
        {
            if (obj == null) return;

            obj.gameObject.SetActive(false);
            
            if (currentSize <= maxPoolSize)
            {
                pool.Enqueue(obj);
            }
            else
            {
                // Pool is over capacity, destroy object
                UnityEngine.Object.Destroy(obj.gameObject);
                currentSize--;
            }
        }

        private T CreateNewObject()
        {
            T obj = UnityEngine.Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
            currentSize++;
            return obj;
        }

        public void Clear()
        {
            while (pool.Count > 0)
            {
                T obj = pool.Dequeue();
                if (obj != null)
                {
                    UnityEngine.Object.Destroy(obj.gameObject);
                }
            }
            currentSize = 0;
        }
    }

    /// <summary>
    /// Mobile-optimized coroutine manager
    /// </summary>
    public class MobileCoroutineManager : MonoBehaviour
    {
        private static MobileCoroutineManager instance;
        public static MobileCoroutineManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("MobileCoroutineManager");
                    instance = go.AddComponent<MobileCoroutineManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();
        private int maxConcurrentCoroutines = 5;
        private int currentCoroutines = 0;

        public void StartCoroutineOptimized(IEnumerator coroutine)
        {
            if (currentCoroutines < maxConcurrentCoroutines)
            {
                StartCoroutine(ExecuteCoroutine(coroutine));
            }
            else
            {
                coroutineQueue.Enqueue(coroutine);
            }
        }

        private IEnumerator ExecuteCoroutine(IEnumerator coroutine)
        {
            currentCoroutines++;
            yield return StartCoroutine(coroutine);
            currentCoroutines--;

            // Process queued coroutines
            if (coroutineQueue.Count > 0)
            {
                StartCoroutine(ExecuteCoroutine(coroutineQueue.Dequeue()));
            }
        }
    }

    /// <summary>
    /// Mobile-optimized texture manager
    /// </summary>
    public class MobileTextureManager : MonoBehaviour
    {
        private static MobileTextureManager instance;
        public static MobileTextureManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("MobileTextureManager");
                    instance = go.AddComponent<MobileTextureManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
        private int maxCacheSize = 50;
        private Queue<string> cacheOrder = new Queue<string>();

        public Texture2D GetTexture(string path)
        {
            if (textureCache.ContainsKey(path))
            {
                return textureCache[path];
            }

            // Load texture
            Texture2D texture = Resources.Load<Texture2D>(path);
            if (texture != null)
            {
                CacheTexture(path, texture);
            }

            return texture;
        }

        private void CacheTexture(string path, Texture2D texture)
        {
            if (textureCache.Count >= maxCacheSize)
            {
                // Remove oldest texture
                string oldestPath = cacheOrder.Dequeue();
                if (textureCache.ContainsKey(oldestPath))
                {
                    textureCache.Remove(oldestPath);
                }
            }

            textureCache[path] = texture;
            cacheOrder.Enqueue(path);
        }

        public void ClearCache()
        {
            textureCache.Clear();
            cacheOrder.Clear();
        }
    }

    /// <summary>
    /// Mobile-optimized audio manager
    /// </summary>
    public class MobileAudioManager : MonoBehaviour
    {
        private static MobileAudioManager instance;
        public static MobileAudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("MobileAudioManager");
                    instance = go.AddComponent<MobileAudioManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private Dictionary<string, AudioClip> audioCache = new Dictionary<string, AudioClip>();
        private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
        private int maxAudioSources = 10;

        private void Start()
        {
            // Pre-allocate audio sources
            for (int i = 0; i < maxAudioSources; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                audioSourcePool.Enqueue(source);
            }
        }

        public void PlaySound(string clipName, float volume = 1f)
        {
            AudioClip clip = GetAudioClip(clipName);
            if (clip != null)
            {
                AudioSource source = GetAudioSource();
                if (source != null)
                {
                    source.clip = clip;
                    source.volume = volume;
                    source.Play();
                    StartCoroutine(ReturnAudioSource(source, clip.length));
                }
            }
        }

        private AudioClip GetAudioClip(string clipName)
        {
            if (audioCache.ContainsKey(clipName))
            {
                return audioCache[clipName];
            }

            AudioClip clip = Resources.Load<AudioClip>(clipName);
            if (clip != null)
            {
                audioCache[clipName] = clip;
            }

            return clip;
        }

        private AudioSource GetAudioSource()
        {
            if (audioSourcePool.Count > 0)
            {
                return audioSourcePool.Dequeue();
            }
            return null;
        }

        private IEnumerator ReturnAudioSource(AudioSource source, float duration)
        {
            yield return new WaitForSeconds(duration);
            audioSourcePool.Enqueue(source);
        }
    }
}
