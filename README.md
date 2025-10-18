# FAB Studios Ad System

A complete, production-ready Google AdMob integration for Unity games with support for banner, interstitial, and app open ads.

## Features

- ✅ **Banner Ads** - Configurable position and size
- ✅ **Interstitial Ads** - Full-screen ads for natural breaks
- ✅ **App Open Ads** - Monetize app launches
- ✅ **Test Mode** - Built-in test ad support for development
- ✅ **Auto-Retry** - Automatic SDK initialization retry logic
- ✅ **Error Handling** - Comprehensive error logging
- ✅ **Events** - Unity events for ad lifecycle callbacks
- ✅ **Pause Support** - Optional game pause during ads
- ✅ **Debug Mode** - Detailed console logging

## Requirements

- Unity 2020.3 or newer
- Google Mobile Ads Unity Plugin 9.0.0+
- External Dependency Manager for Unity

## Installation

### Method 1: Unity Package Manager (Recommended)

1. Open Unity Package Manager (`Window > Package Manager`)
2. Click the `+` button and select `Add package from git URL`
3. Enter: `https://github.com/yomaine/FABStudios.AdSystem.git`
4. Click `Add`

### Method 2: Manual Installation

1. Download the latest release from GitHub
2. Extract to your project's `Packages` folder
3. Unity will automatically detect and import the package

## Prerequisites Setup

### 1. Install Google Mobile Ads SDK

1. Download from: https://github.com/googleads/googleads-mobile-unity/releases
2. Import the `.unitypackage` file
3. Follow the Google Mobile Ads setup wizard

### 2. Install External Dependency Manager

1. Download from: https://github.com/googlesamples/unity-jar-resolver/releases
2. Import the `external-dependency-manager-latest.unitypackage`
3. Resolve dependencies: `Assets > External Dependency Manager > Android Resolver > Force Resolve`

### 3. Configure AdMob App ID

Create or edit `Assets/Plugins/Android/AndroidManifest.xml`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <application>
        <meta-data
            android:name="com.google.android.gms.ads.APPLICATION_ID"
            android:value="ca-app-pub-YOUR_APP_ID_HERE"/>
    </application>
</manifest>
```

Replace `YOUR_APP_ID_HERE` with your actual AdMob App ID.

## Quick Start

### 1. Create Ad Configuration

1. Right-click in Project window
2. Select `Create > FABStudios > Ad Configuration`
3. Name it `DefaultAdConfig`
4. Configure your ad unit IDs in the Inspector

### 2. Add Ad Manager to Scene

1. Create an empty GameObject in your scene
2. Rename it to `AdManager`
3. Add the `AdManagerNew` component
4. Drag your `DefaultAdConfig` asset into the Ad Config field

### 3. Show Ads

```csharp
// Show banner ad
AdManagerNew.Instance.ShowBannerAd();

// Show interstitial ad
AdManagerNew.Instance.ShowInterstitialAd();

// Show app open ad
AdManagerNew.Instance.ShowAppOpenAd();

// Hide banner ad
AdManagerNew.Instance.HideBannerAd();

// Toggle banner visibility
AdManagerNew.Instance.ToggleBannerAd();
```

## Configuration Options

### Ad Unit IDs
- **Interstitial Ad Unit ID** - Your AdMob interstitial ad ID
- **App Open Ad Unit ID** - Your AdMob app open ad ID
- **Banner Ad Unit ID** - Your AdMob banner ad ID

### Ad Behavior
- **Show Initial App Open Ad** - Display app open ad on game start
- **Initial App Open Ad Delay** - Delay before showing (0-10 seconds)
- **Pause On Ad** - Pause game (Time.timeScale = 0) during ads

### Banner Settings
- **Show Banner On Start** - Automatically show banner when game starts
- **Banner Position** - Top, Bottom, TopLeft, TopRight, BottomLeft, BottomRight, Center
- **Banner Size** - Standard banner (320x50) or other sizes
- **Hide Banner During Interstitial** - Automatically hide banner during full-screen ads

### Development Settings
- **Use Test Ads** - Use Google's test ad unit IDs (always enabled in Editor)
- **Debug Mode** - Show detailed logs in console

### Events
- **On Ad Displayed** - Called when an ad starts showing
- **On Ad Closed** - Called when an ad closes
- **On Ad Failed** - Called if ad fails to load/show

## Example: Game Over Interstitial

```csharp
public class GameManager : MonoBehaviour
{
    void OnGameOver()
    {
        // Show interstitial ad when player loses
        if (AdManagerNew.Instance != null)
        {
            AdManagerNew.Instance.ShowInterstitialAd();
        }
    }
}
```

## Example: Persistent Banner

```csharp
public class MainMenu : MonoBehaviour
{
    void Start()
    {
        // Show banner on main menu
        if (AdManagerNew.Instance != null)
        {
            AdManagerNew.Instance.ShowBannerAd();
        }
    }

    void OnDestroy()
    {
        // Hide banner when leaving menu
        if (AdManagerNew.Instance != null)
        {
            AdManagerNew.Instance.HideBannerAd();
        }
    }
}
```

## Testing

The system automatically uses test ads when:
- Running in Unity Editor
- `useTestAds` is enabled in AdConfig

Test ad unit IDs are provided by Google and won't generate revenue.

## Best Practices

1. **Always test with test ads** during development
2. **Switch to production ads** only when publishing
3. **Don't click your own ads** - this can get your AdMob account banned
4. **Show interstitials at natural breaks** - game over, level complete, between rounds
5. **Respect user experience** - don't show ads too frequently
6. **Handle ad failures gracefully** - ads may not always be available

## Troubleshooting

### Ads not showing
- Verify AdMob App ID is in AndroidManifest.xml
- Check that External Dependency Manager resolved dependencies
- Ensure ad unit IDs are correct (or use test mode)
- Look for errors in console with Debug Mode enabled

### Build errors
- Make sure External Dependency Manager is installed
- Force resolve Android dependencies
- Check minimum Android SDK version is 21 or higher

### App crashes on device
- Verify AndroidManifest.xml has correct AdMob App ID
- Check that Google Play Services are up to date on device
- Review device logs for specific error messages

## API Reference

### AdManagerNew

#### Properties
- `Instance` - Singleton instance
- `IsShowingAd` - Whether an ad is currently being displayed
- `IsInitialized` - Whether Mobile Ads SDK is initialized
- `IsBannerShowing` - Whether banner is currently visible

#### Methods
- `ShowInterstitialAd()` - Display interstitial ad
- `ShowAppOpenAd()` - Display app open ad
- `ShowBannerAd()` - Show banner ad
- `HideBannerAd()` - Hide banner ad
- `ToggleBannerAd()` - Toggle banner visibility
- `DestroyBannerAd()` - Completely destroy banner

## Support

- **Issues**: https://github.com/yomaine/FABStudios.AdSystem/issues
- **Documentation**: https://github.com/yomaine/FABStudios.AdSystem
- **Email**: support@fabstudios.com

## License

MIT License - See LICENSE file for details

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history.

---

Made with ❤️ by FAB Studios