using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using Unity.VisualScripting.FullSerializer;

namespace FABStudios.AdSystem
{
    /// <summary>
    /// Manages mobile ads for the game, handling both interstitial and app open ads.
    /// This script should be placed on a GameObject in your starting scene.
    /// </summary>
    public class AdManagerNew : MonoBehaviour
    {
        // Singleton pattern for easy access from other scripts
        public static AdManagerNew Instance { get; private set; }

        private bool isTestEnvironment =
#if UNITY_EDITOR
        true;
#else
        false;
#endif

        [Header("Configuration")]
        [SerializeField]
        [Tooltip("Assign your AdConfig asset here")]
        private AdConfig adConfig;

        // Ad instances that will hold our loaded advertisements
        private InterstitialAd interstitialAd;
        private AppOpenAd appOpenAd;
        private BannerView bannerView;

        // State tracking with properties for better encapsulation
        public bool IsShowingAd => isShowingAd;
        public bool IsInitialized => isInitialized;
        public bool IsBannerShowing => isBannerShowing;
        private bool isShowingAd = false;
        private bool isInitialized = false;
        private bool isBannerShowing = false;

        // Constants for initialization retry logic
        private const int MAX_INIT_RETRIES = 3;
        private int initRetryCount = 0;
        private const float RETRY_DELAY = 2f;

        #region Initialization
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                DebugLog("AdManagerNew initialized");
                ValidateAndSetupConfiguration();
            }
            else
            {
                DebugLog("Duplicate instance destroyed");
                Destroy(gameObject);
            }
        }

        private void ValidateAndSetupConfiguration()
        {
            if (adConfig == null)
            {
                Debug.LogError("AdManagerNew: AdConfig not assigned! Please assign an AdConfig asset in the Inspector.");
                return;
            }

            // Set up test ads if enabled
            if (adConfig.useTestAds)
            {
                adConfig.interstitialAdUnitId = AdConfig.TEST_INTERSTITIAL_ANDROID;
                adConfig.appOpenAdUnitId = AdConfig.TEST_APP_OPEN_ANDROID;
                DebugLog("Using test ad unit IDs");
            }
            else
            {
                // Validate production ad IDs
                if (string.IsNullOrEmpty(adConfig.interstitialAdUnitId) ||
                    string.IsNullOrEmpty(adConfig.appOpenAdUnitId))
                {
                    Debug.LogError("AdManagerNew: Production ad unit IDs are not set in AdConfig!");
                }
            }
        }

        private void Start()
        {
            if (adConfig != null)
            {
                StartCoroutine(InitializeWithRetry());
            }
        }

        private IEnumerator SimulateAdClose(float delay = 3f)
        {
            if (!isTestEnvironment) yield break;

            yield return new WaitForSecondsRealtime(delay);
            DebugLog("Test environment: Simulating ad close");

            // Create a temporary event handler to simulate the close sequence
            if (appOpenAd != null && isShowingAd)
            {
                // We'll manually trigger the same actions that would happen when an ad closes
                isShowingAd = false;
                DebugLog("App open ad closed (simulated)");
                if (adConfig.pauseOnAd)
                {
                    Time.timeScale = 1;
                    DebugLog("Restoring time scale to 1");
                }
                appOpenAd.Destroy();
                appOpenAd = null;
                LoadAppOpenAd();
                adConfig.onAdClosed?.Invoke(AdType.AppOpen);
            }
            else if (interstitialAd != null && isShowingAd)
            {
                // Same sequence for interstitial ads
                isShowingAd = false;
                DebugLog("Interstitial ad closed (simulated)");
                if (adConfig.pauseOnAd)
                {
                    Time.timeScale = 1;
                    DebugLog("Restoring time scale to 1");
                }
                interstitialAd.Destroy();
                interstitialAd = null;
                LoadInterstitialAd();
                adConfig.onAdClosed?.Invoke(AdType.Interstitial);
            }
        }

        private IEnumerator InitializeWithRetry()
        {
            while (!isInitialized && initRetryCount < MAX_INIT_RETRIES)
            {
                initRetryCount++;
                DebugLog($"Attempting to initialize Mobile Ads SDK (Attempt {initRetryCount}/{MAX_INIT_RETRIES})");

                try
                {
                    MobileAds.Initialize(OnSDKInitialized);
                }
                catch (Exception e)
                {
                    Debug.LogError($"AdManagerNew: Failed to initialize Mobile Ads SDK: {e.Message}");
                }

                yield return new WaitForSeconds(RETRY_DELAY);
            }

            if (!isInitialized)
            {
                Debug.LogError("AdManagerNew: Failed to initialize after maximum retries");
            }
        }

        private void OnSDKInitialized(InitializationStatus initStatus)
        {
            isInitialized = true;
            DebugLog("Mobile Ads SDK initialized successfully");
            LoadAds();
        }

        private void LoadAds()
        {
            LoadInterstitialAd();
            LoadAppOpenAd();

            if (adConfig.showBannerOnStart)
            {
                LoadBannerAd();
            }

            if (adConfig.showInitialAppOpenAd)
            {
                StartCoroutine(ShowInitialAppOpenAd());
            }
        }
        #endregion

        #region Interstitial Ads
        private void LoadInterstitialAd()
        {
            if (!isInitialized) return;

            DebugLog("Loading interstitial ad");

            // Clean up existing ad before loading new one
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
                interstitialAd = null;
            }

            var adRequest = new AdRequest();
            DebugLog($"Requesting interstitial ad with ID: {adConfig.interstitialAdUnitId}");

            InterstitialAd.Load(adConfig.interstitialAdUnitId, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    if (error != null)
                    {
                        DebugLog($"Interstitial ad failed to load: {error.GetMessage()}");
                        return;
                    }

                    interstitialAd = ad;
                    DebugLog("Interstitial ad loaded successfully");
                    RegisterInterstitialEvents();
                });
        }

        private void RegisterInterstitialEvents()
        {
            if (interstitialAd == null) return;

            interstitialAd.OnAdImpressionRecorded += () =>
            {
                DebugLog("Interstitial ad recorded an impression");
            };

            interstitialAd.OnAdFullScreenContentOpened += () =>
            {
                isShowingAd = true;
                DebugLog("Interstitial ad opening");
                if (adConfig.pauseOnAd) Time.timeScale = 0;
                adConfig.onAdDisplayed?.Invoke(AdType.Interstitial);
                if (adConfig.hideBannerDuringInterstitial && isBannerShowing)
                {
                    HideBannerAd();
                }
            };

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                isShowingAd = false;
                DebugLog("Interstitial ad closed");
                if (adConfig.pauseOnAd) Time.timeScale = 1;
                LoadInterstitialAd();
                adConfig.onAdClosed?.Invoke(AdType.Interstitial);
                if (adConfig.hideBannerDuringInterstitial && !isBannerShowing)
                {
                    ShowBannerAd();
                }
            };

            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                DebugLog($"Interstitial ad failed to show: {error.GetMessage()}");
                isShowingAd = false;
                if (adConfig.pauseOnAd) Time.timeScale = 1;
                LoadInterstitialAd();
                adConfig.onAdFailed?.Invoke(AdType.Interstitial, error.GetMessage());
            };
        }

        public void ShowInterstitialAd()
        {
            DebugLog("Show interstitial ad requested");

            if (!isInitialized)
            {
                DebugLog("Mobile Ads SDK not yet initialized");
                return;
            }

            if (isShowingAd)
            {
                DebugLog("An ad is already showing");
                return;
            }

            if (interstitialAd == null || !interstitialAd.CanShowAd())
            {
                DebugLog("Interstitial ad not ready - loading new one");
                LoadInterstitialAd();
                return;
            }

            try
            {
                DebugLog("Showing interstitial ad");
                interstitialAd.Show();

                // Start both the safety timeout and the test close simulation
                StartCoroutine(SafetyTimeout());
                if (isTestEnvironment)
                {
                    StartCoroutine(SimulateAdClose());
                }
            }
            catch (Exception e)
            {
                DebugLog($"Error showing interstitial ad: {e.Message}");
                isShowingAd = false;
                if (adConfig.pauseOnAd) Time.timeScale = 1;
                LoadInterstitialAd();
            }
        }
        #endregion

        #region App Open Ads
        private void LoadAppOpenAd()
        {
            if (!isInitialized) return;

            DebugLog("Loading app open ad");

            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
                appOpenAd = null;
            }

            var adRequest = new AdRequest();

            AppOpenAd.Load(adConfig.appOpenAdUnitId, adRequest,
                (AppOpenAd ad, LoadAdError error) =>
                {
                    if (error != null)
                    {
                        DebugLog($"App open ad failed to load: {error.GetMessage()}");
                        return;
                    }

                    appOpenAd = ad;
                    DebugLog("App open ad loaded successfully");
                    RegisterAppOpenEvents();
                });
        }

        private void RegisterAppOpenEvents()
        {
            if (appOpenAd == null)
            {
                DebugLog("Cannot register events for null app open ad");
                return;
            }

            // When the ad opens
            appOpenAd.OnAdFullScreenContentOpened += () =>
            {
                isShowingAd = true;
                DebugLog("App open ad opened");
                if (adConfig.pauseOnAd)
                {
                    Time.timeScale = 0;
                    DebugLog("Setting time scale to 0");
                }
                adConfig.onAdDisplayed?.Invoke(AdType.AppOpen);
            };

            // When the ad closes
            appOpenAd.OnAdFullScreenContentClosed += () =>
            {
                isShowingAd = false;
                DebugLog("App open ad closed");
                if (adConfig.pauseOnAd)
                {
                    Time.timeScale = 1;
                    DebugLog("Restoring time scale to 1");
                }
                // Clean up and reload
                appOpenAd.Destroy();
                appOpenAd = null;
                LoadAppOpenAd();
                adConfig.onAdClosed?.Invoke(AdType.AppOpen);
            };

            // Error handling
            appOpenAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                string errorMessage = $"App open ad failed to show: {error.GetMessage()}";
                DebugLog(errorMessage);
                isShowingAd = false;
                if (adConfig.pauseOnAd)
                {
                    Time.timeScale = 1;
                    DebugLog("Restoring time scale after error");
                }
                // Clean up and reload
                appOpenAd.Destroy();
                appOpenAd = null;
                LoadAppOpenAd();
                adConfig.onAdFailed?.Invoke(AdType.AppOpen, errorMessage);
            };

            // Impression tracking
            appOpenAd.OnAdImpressionRecorded += () =>
            {
                DebugLog("App open ad recorded an impression");
            };
        }

        public void ShowAppOpenAd()
        {
            if (!isInitialized || isShowingAd)
            {
                DebugLog($"Cannot show app open ad - {(!isInitialized ? "not initialized" : "ad already showing")}");
                return;
            }

            if (appOpenAd == null || !appOpenAd.CanShowAd())
            {
                DebugLog("App open ad not ready - loading new one");
                LoadAppOpenAd();
                return;
            }

            try
            {
                DebugLog("Showing app open ad");
                appOpenAd.Show();

                // Start both the safety timeout and the test close simulation
                StartCoroutine(SafetyTimeout());
                if (isTestEnvironment)
                {
                    StartCoroutine(SimulateAdClose());
                }
            }
            catch (Exception e)
            {
                DebugLog($"Error showing app open ad: {e.Message}");
                isShowingAd = false;
                if (adConfig.pauseOnAd) Time.timeScale = 1;
                LoadAppOpenAd();
            }
        }

        private IEnumerator SafetyTimeout()
        {
            // Wait longer than the simulated close to act as a true safety net
            yield return new WaitForSecondsRealtime(8f);

            if (isShowingAd)
            {
                DebugLog("Ad timeout - forcing cleanup");
                isShowingAd = false;
                if (adConfig.pauseOnAd) Time.timeScale = 1;

                if (appOpenAd != null)
                {
                    appOpenAd.Destroy();
                    appOpenAd = null;
                    LoadAppOpenAd();
                }

                if (interstitialAd != null)
                {
                    interstitialAd.Destroy();
                    interstitialAd = null;
                    LoadInterstitialAd();
                }
            }
        }

        private IEnumerator ShowInitialAppOpenAd()
        {
            yield return new WaitForSeconds(adConfig.initialAppOpenAdDelay);

            if (!isShowingAd)
            {
                ShowAppOpenAd();
            }
        }
        #endregion

        #region Banner Ads
        private void LoadBannerAd()
        {
            if (!isInitialized) return;

            DebugLog("Loading banner ad");

            if (bannerView != null)
            {
                bannerView.Destroy();
                bannerView = null;
            }

            // Create a banner view using configuration
            bannerView = new BannerView(
                adConfig.GetBannerAdUnitId(),
                adConfig.bannerSize,
                adConfig.GetGoogleAdPosition() // This converts our position to Google's
            );

            RegisterBannerEvents();

            var adRequest = new AdRequest();
            bannerView.LoadAd(adRequest);
        }

        private void RegisterBannerEvents()
        {
            if (bannerView == null) return;

            bannerView.OnBannerAdLoaded += () =>
            {
                DebugLog("Banner ad loaded successfully");
                if (adConfig.showBannerOnStart && !isBannerShowing && !isShowingAd)
                {
                    ShowBannerAd();
                }
            };

            bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                DebugLog($"Banner ad failed to load: {error.GetMessage()}");
                adConfig.onAdFailed?.Invoke(AdType.Banner, error.GetMessage());
            };

            bannerView.OnAdImpressionRecorded += () =>
            {
                DebugLog("Banner ad recorded an impression");
            };
        }

        public void ShowBannerAd()
        {
            if (!isInitialized)
            {
                DebugLog("Cannot show banner ad - not initialized");
                return;
            }

            if (bannerView == null)
            {
                DebugLog("Banner ad not ready - loading new one");
                LoadBannerAd();
                return;
            }

            DebugLog("Showing banner ad");
            bannerView.Show();
            isBannerShowing = true;
            adConfig.onAdDisplayed?.Invoke(AdType.Banner);
        }

        public void HideBannerAd()
        {
            if (bannerView != null && isBannerShowing)
            {
                DebugLog("Hiding banner ad");
                bannerView.Hide();
                isBannerShowing = false;
                adConfig.onAdClosed?.Invoke(AdType.Banner);
            }
        }
        #endregion

        private void DebugLog(string message)
        {
            if (adConfig != null && adConfig.debugMode)
            {
                Debug.Log($"AdManagerNew: {message}");
            }
        }

        private void OnDestroy()
        {
            DebugLog("OnDestroy called - cleaning up ad resources");
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
            }
            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
            }

            if (bannerView != null)
            {
                bannerView.Destroy();
            }
        }
    }
}