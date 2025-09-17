using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SWITCH.UI
{
    /// <summary>
    /// Manages the ad banner UI for monetization
    /// </summary>
    public class AdBannerUI : MonoBehaviour
    {
        [Header("Ad Banner Settings")]
        [SerializeField] private const int BANNER_WIDTH = 320;
        [SerializeField] private const int BANNER_HEIGHT = 50;
        
        [Header("UI Components")]
        [SerializeField] private GameObject adContainer;
        [SerializeField] private GameObject alternativeContent;
        [SerializeField] private TextMeshProUGUI alternativeText;
        [SerializeField] private Image bannerBackground;
        
        [Header("Visual Settings")]
        [SerializeField] private Color adBackgroundColor = UIColors.adBackground;
        [SerializeField] private Color adTextColor = UIColors.adText;
        [SerializeField] private string[] gameTips = {
            "Tip: Build heat for higher multipliers!",
            "Tip: Use power-ups strategically!",
            "Tip: Plan your moves ahead!",
            "Tip: Cascades give bonus points!",
            "Tip: Watch the heat meter!"
        };
        
        [Header("Ad Settings")]
        [SerializeField] private bool premiumUser = false;
        [SerializeField] private bool adsEnabled = true;
        [SerializeField] private float adRefreshInterval = 30f;
        
        // State
        private bool adLoaded = false;
        private float lastAdRefresh = 0f;
        private int currentTipIndex = 0;
        
        // Events
        public System.Action OnAdLoaded;
        public System.Action OnAdFailed;
        public System.Action OnAdClicked;
        
        private void Start()
        {
            InitializeUI();
            SetupAdBanner();
        }
        
        private void Update()
        {
            // Check if we need to refresh the ad
            if (adsEnabled && !premiumUser && Time.time - lastAdRefresh > adRefreshInterval)
            {
                RefreshAd();
            }
        }
        
        /// <summary>
        /// Initializes the UI components
        /// </summary>
        private void InitializeUI()
        {
            // Initialize banner background
            if (bannerBackground != null)
            {
                bannerBackground.color = adBackgroundColor;
            }
            
            // Initialize alternative content
            if (alternativeText != null)
            {
                alternativeText.color = adTextColor;
                alternativeText.text = GetRandomTip();
            }
        }
        
        /// <summary>
        /// Sets up the ad banner based on user status
        /// </summary>
        private void SetupAdBanner()
        {
            if (premiumUser)
            {
                ShowAlternativeContent();
            }
            else if (adsEnabled)
            {
                LoadAd();
            }
            else
            {
                ShowAlternativeContent();
            }
        }
        
        /// <summary>
        /// Loads an ad (placeholder implementation)
        /// </summary>
        private void LoadAd()
        {
            // This would integrate with actual ad SDK
            // For now, simulate ad loading
            StartCoroutine(SimulateAdLoad());
        }
        
        /// <summary>
        /// Simulates ad loading (placeholder)
        /// </summary>
        /// <returns>Coroutine</returns>
        private System.Collections.IEnumerator SimulateAdLoad()
        {
            yield return new WaitForSeconds(1f);
            
            // Simulate random success/failure
            if (Random.Range(0f, 1f) > 0.2f) // 80% success rate
            {
                ShowAd();
            }
            else
            {
                ShowAlternativeContent();
            }
        }
        
        /// <summary>
        /// Shows the ad
        /// </summary>
        private void ShowAd()
        {
            if (adContainer != null)
            {
                adContainer.SetActive(true);
            }
            
            if (alternativeContent != null)
            {
                alternativeContent.SetActive(false);
            }
            
            adLoaded = true;
            lastAdRefresh = Time.time;
            OnAdLoaded?.Invoke();
        }
        
        /// <summary>
        /// Shows alternative content (tips or stats)
        /// </summary>
        private void ShowAlternativeContent()
        {
            if (adContainer != null)
            {
                adContainer.SetActive(false);
            }
            
            if (alternativeContent != null)
            {
                alternativeContent.SetActive(true);
                
                if (alternativeText != null)
                {
                    alternativeText.text = GetRandomTip();
                }
            }
            
            adLoaded = false;
        }
        
        /// <summary>
        /// Refreshes the ad
        /// </summary>
        private void RefreshAd()
        {
            if (adsEnabled && !premiumUser)
            {
                LoadAd();
            }
        }
        
        /// <summary>
        /// Gets a random game tip
        /// </summary>
        /// <returns>Random tip string</returns>
        private string GetRandomTip()
        {
            if (gameTips.Length > 0)
            {
                currentTipIndex = (currentTipIndex + 1) % gameTips.Length;
                return gameTips[currentTipIndex];
            }
            return "Tip: Have fun playing SWITCH!";
        }
        
        /// <summary>
        /// Called when ad is clicked
        /// </summary>
        public void OnAdClick()
        {
            OnAdClicked?.Invoke();
            
            // This would open the ad or handle the click
            // For now, just log it
            Debug.Log("Ad clicked!");
        }
        
        /// <summary>
        /// Sets the premium user status
        /// </summary>
        /// <param name="isPremium">Whether user is premium</param>
        public void SetPremiumUser(bool isPremium)
        {
            premiumUser = isPremium;
            SetupAdBanner();
        }
        
        /// <summary>
        /// Sets whether ads are enabled
        /// </summary>
        /// <param name="enabled">Whether ads are enabled</param>
        public void SetAdsEnabled(bool enabled)
        {
            adsEnabled = enabled;
            SetupAdBanner();
        }
        
        /// <summary>
        /// Gets the current ad status
        /// </summary>
        /// <returns>True if ad is loaded and visible</returns>
        public bool IsAdLoaded()
        {
            return adLoaded && !premiumUser;
        }
        
        /// <summary>
        /// Gets the banner dimensions
        /// </summary>
        /// <returns>Banner dimensions as Vector2</returns>
        public Vector2 GetBannerDimensions()
        {
            return new Vector2(BANNER_WIDTH, BANNER_HEIGHT);
        }
        
        /// <summary>
        /// Updates the alternative content text
        /// </summary>
        /// <param name="text">New text to display</param>
        public void UpdateAlternativeContent(string text)
        {
            if (alternativeText != null)
            {
                alternativeText.text = text;
            }
        }
        
        /// <summary>
        /// Cycles to the next tip
        /// </summary>
        public void NextTip()
        {
            if (alternativeText != null)
            {
                alternativeText.text = GetRandomTip();
            }
        }
        
        #region Editor Debugging
        #if UNITY_EDITOR
        [Header("Editor Debug Controls")]
        [SerializeField] private bool showDebugControls = false;
        
        private void OnGUI()
        {
            if (!showDebugControls) return;
            
            GUILayout.BeginArea(new Rect(640, 10, 200, 200));
            GUILayout.Label("Ad Banner UI Debug");
            GUILayout.Label($"Premium User: {premiumUser}");
            GUILayout.Label($"Ads Enabled: {adsEnabled}");
            GUILayout.Label($"Ad Loaded: {adLoaded}");
            GUILayout.Label($"Last Refresh: {lastAdRefresh:F1}s ago");
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Toggle Premium"))
                SetPremiumUser(!premiumUser);
            if (GUILayout.Button("Toggle Ads"))
                SetAdsEnabled(!adsEnabled);
            if (GUILayout.Button("Refresh Ad"))
                RefreshAd();
            if (GUILayout.Button("Next Tip"))
                NextTip();
            if (GUILayout.Button("Simulate Ad Click"))
                OnAdClick();
            
            GUILayout.EndArea();
        }
        #endif
        #endregion
    }
}
