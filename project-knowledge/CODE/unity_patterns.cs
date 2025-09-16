// SWITCH Unity Patterns - Reusable Code Patterns
// This file contains common Unity patterns used throughout the project

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SWITCH.Patterns
{
    /// <summary>
    /// Singleton pattern for managers and services
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        Debug.LogError($"No instance of {typeof(T)} found in scene!");
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Object pool pattern for performance optimization
    /// </summary>
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private Queue<T> pool = new Queue<T>();
        private T prefab;
        private Transform parent;

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;
            
            for (int i = 0; i < initialSize; i++)
            {
                T obj = UnityEngine.Object.Instantiate(prefab, parent);
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
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
            else
            {
                return UnityEngine.Object.Instantiate(prefab, parent);
            }
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// Event system for loose coupling
    /// </summary>
    public static class GameEvents
    {
        // Game state events
        public static event Action OnGameStart;
        public static event Action OnGameEnd;
        public static event Action OnGamePause;
        public static event Action OnGameResume;

        // Score events
        public static event Action<int> OnScoreChanged;
        public static event Action<int> OnHighScoreBeaten;

        // Match events
        public static event Action<List<Match>> OnMatchesFound;
        public static event Action<CascadeResult> OnCascadeComplete;

        // Power-up events
        public static event Action<PowerUpType> OnPowerUpActivated;
        public static event Action<PowerUpType> OnPowerUpEarned;

        // UI events
        public static event Action<Direction> OnDirectionSelected;
        public static event Action OnQueueUpdated;

        // Utility methods
        public static void TriggerGameStart() => OnGameStart?.Invoke();
        public static void TriggerGameEnd() => OnGameEnd?.Invoke();
        public static void TriggerScoreChanged(int score) => OnScoreChanged?.Invoke(score);
        public static void TriggerMatchesFound(List<Match> matches) => OnMatchesFound?.Invoke(matches);
    }

    /// <summary>
    /// Coroutine wrapper for async operations
    /// </summary>
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner instance;
        public static CoroutineRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("CoroutineRunner");
                    instance = go.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        public static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }
    }

    /// <summary>
    /// Generic state machine
    /// </summary>
    public class StateMachine<T> where T : Enum
    {
        private T currentState;
        private Dictionary<T, Action> stateEnterActions = new Dictionary<T, Action>();
        private Dictionary<T, Action> stateExitActions = new Dictionary<T, Action>();
        private Dictionary<T, Action> stateUpdateActions = new Dictionary<T, Action>();

        public T CurrentState => currentState;

        public void AddState(T state, Action onEnter = null, Action onUpdate = null, Action onExit = null)
        {
            if (onEnter != null) stateEnterActions[state] = onEnter;
            if (onUpdate != null) stateUpdateActions[state] = onUpdate;
            if (onExit != null) stateExitActions[state] = onExit;
        }

        public void ChangeState(T newState)
        {
            if (stateExitActions.ContainsKey(currentState))
                stateExitActions[currentState]?.Invoke();

            currentState = newState;

            if (stateEnterActions.ContainsKey(currentState))
                stateEnterActions[currentState]?.Invoke();
        }

        public void Update()
        {
            if (stateUpdateActions.ContainsKey(currentState))
                stateUpdateActions[currentState]?.Invoke();
        }
    }

    /// <summary>
    /// Generic data container for ScriptableObjects
    /// </summary>
    [CreateAssetMenu(fileName = "New Data", menuName = "SWITCH/Data")]
    public class GameData : ScriptableObject
    {
        [Header("Basic Info")]
        public string displayName;
        public string description;
        public Sprite icon;

        [Header("Configuration")]
        public bool isEnabled = true;
        public int priority = 0;

        public virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(displayName))
                displayName = name;
        }
    }

    /// <summary>
    /// Generic manager base class
    /// </summary>
    public abstract class Manager<T> : Singleton<T> where T : MonoBehaviour
    {
        [Header("Manager Configuration")]
        [SerializeField] protected bool isEnabled = true;
        [SerializeField] protected bool debugMode = false;

        protected virtual void Start()
        {
            if (isEnabled)
            {
                Initialize();
            }
        }

        protected abstract void Initialize();

        protected virtual void Log(string message)
        {
            if (debugMode)
            {
                Debug.Log($"[{typeof(T).Name}] {message}");
            }
        }

        protected virtual void LogWarning(string message)
        {
            if (debugMode)
            {
                Debug.LogWarning($"[{typeof(T).Name}] {message}");
            }
        }

        protected virtual void LogError(string message)
        {
            Debug.LogError($"[{typeof(T).Name}] {message}");
        }
    }
}
