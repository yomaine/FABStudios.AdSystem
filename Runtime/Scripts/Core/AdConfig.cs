using UnityEngine;
using UnityEngine.Events;
using GoogleMobileAds.Api;  // Needed for AdSize conversions

namespace FABStudios.AdSystem
{
    [CreateAssetMenu(fileName = "AdConfig", menuName = "FABStudios/Ad Configuration")]
    public class AdConfig : ScriptableObject
    {
        // Test ad unit IDs
        public const string TEST_INTERSTITIAL_ANDROID = "ca-app-pub-3940256099942544/1033173712";
        public const string TEST_APP_OPEN_ANDROID = "ca-app-pub-3940256099942544/3419835294";
        public const string TEST_BANNER_ANDROID = "ca-app-pub-3940256099942544/6300978111";

        #region Inspector Fields
        [Header("Ad Unit IDs")]
        [Tooltip("Your AdMob interstitial ad ID")]
        public string interstitialAdUnitId = "YOUR_INTERSTITIAL_AD_UNIT_ID";

        [Tooltip("Your AdMob app open ad ID")]
        public string appOpenAdUnitId = "YOUR_APP_OPEN_AD_UNIT_ID";

        [Tooltip("Your AdMob banner ad ID")]
        public string bannerAdUnitId = "YOUR_BANNER_AD_UNIT_ID";

        [Header("Ad Behavior Settings")]
        [Tooltip("Show an app open ad when game starts?")]
        public bool showInitialAppOpenAd = true;

        [Tooltip("Delay before showing initial app open ad")]
        [Range(0f, 10f)]
        public float initialAppOpenAdDelay = 3f;

        [Tooltip("Pause game while ads are showing?")]
        public bool pauseOnAd = true;

        [Header("Banner Ad Settings")]
        [Tooltip("Show banner ad when game starts?")]
        public bool showBannerOnStart = false;

        [Tooltip("Where to position the banner on screen")]
        public BannerPosition bannerPosition = BannerPosition.Bottom;

        [Tooltip("Standard banner is 320x50")]
        public AdSize bannerSize = AdSize.Banner;

        [Tooltip("Hide banner during interstitial ads?")]
        public bool hideBannerDuringInterstitial = true;

        [Header("Development Settings")]
        [Tooltip("Use test ads during development")]
        public bool useTestAds = true;

        [Tooltip("Show debug logs in console")]
        public bool debugMode = true;

        [Header("Ad Events")]
        [Tooltip("Called when an ad starts displaying")]
        public UnityEvent<AdType> onAdDisplayed;

        [Tooltip("Called when an ad finishes and closes")]
        public UnityEvent<AdType> onAdClosed;

        [Tooltip("Called if there's an error showing an ad")]
        public UnityEvent<AdType, string> onAdFailed;
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the appropriate banner ad unit ID based on configuration
        /// </summary>
        public string GetBannerAdUnitId()
        {
            if (useTestAds)
            {
#if UNITY_ANDROID
                return TEST_BANNER_ANDROID;
#else
                    return bannerAdUnitId; // Replace with iOS test ID if needed
#endif
            }
            return bannerAdUnitId;
        }

        /// <summary>
        /// Gets the appropriate Google Ad Position based on our banner position
        /// </summary>
        internal AdPosition GetGoogleAdPosition()
        {
            switch (bannerPosition)
            {
                case BannerPosition.Top:
                    return AdPosition.Top;
                case BannerPosition.Bottom:
                    return AdPosition.Bottom;
                case BannerPosition.TopLeft:
                    return AdPosition.TopLeft;
                case BannerPosition.TopRight:
                    return AdPosition.TopRight;
                case BannerPosition.BottomLeft:
                    return AdPosition.BottomLeft;
                case BannerPosition.BottomRight:
                    return AdPosition.BottomRight;
                case BannerPosition.Center:
                    return AdPosition.Center;
                default:
                    return AdPosition.Bottom;
            }
        }

        /// <summary>
        /// Validates that all required ad unit IDs are properly set
        /// </summary>
        public bool IsValid()
        {
            if (useTestAds) return true;

            bool validInterstitial = !string.IsNullOrEmpty(interstitialAdUnitId)
                && interstitialAdUnitId != "YOUR_INTERSTITIAL_AD_UNIT_ID";

            bool validAppOpen = !string.IsNullOrEmpty(appOpenAdUnitId)
                && appOpenAdUnitId != "YOUR_APP_OPEN_AD_UNIT_ID";

            bool validBanner = !string.IsNullOrEmpty(bannerAdUnitId)
                && bannerAdUnitId != "YOUR_BANNER_AD_UNIT_ID";

            return validInterstitial && validAppOpen && validBanner;
        }
        #endregion
    }
}