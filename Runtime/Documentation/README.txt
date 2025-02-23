# FABStudios Ad System
A flexible and reusable ad system for Unity mobile games, supporting both interstitial and app open ads.

## Overview
This package provides a complete solution for implementing Google AdMob advertisements in Unity games. It features:
- Easy setup with minimal configuration
- Support for interstitial and app open ads
- Test mode for development
- Built-in testing interface
- Automatic ad lifecycle management

## Installation

1. Copy the FABStudios.AdSystem folder into your Unity project's Assets folder
2. Add the AdSystem prefab to your initial scene
3. Configure your ad unit IDs in the DefaultAdConfig asset

## Setting Up the Ad System

### Basic Setup

1. In your starting scene (usually the Main Menu):
   - Create an empty GameObject named "AdSystem"
   - Add the AdManagerNew component to it
   - Assign the DefaultAdConfig asset to the "Ad Config" field

2. For development and testing:
   - Create another empty GameObject named "AdTestUI"
   - Add the AdSystemTestUI component to it
   - This will create a test panel with buttons to trigger ads

### Configuring Ad IDs

1. Locate the DefaultAdConfig asset in:
2. In the Inspector:
- During development, keep "Use Test Ads" checked
- For production, uncheck "Use Test Ads" and set your real ad unit IDs:
  - Interstitial Ad Unit ID
  - App Open Ad Unit ID

## Using the Ad System

### Showing Interstitial Ads

```csharp
// Show an interstitial ad from anywhere in your code
AdManagerNew.Instance.ShowInterstitialAd();

// Example: Show ad after completing a level
public void OnLevelComplete()
{
 // Save progress first
 SaveProgress();
 
 // Show the ad
 AdManagerNew.Instance.ShowInterstitialAd();
}

AdManagerNew.Instance.ShowAppOpenAd();

void Start()
{
    // Get reference to AdConfig
    AdConfig adConfig = Resources.Load<AdConfig>("DefaultAdConfig");

    // Subscribe to events
    adConfig.onAdDisplayed.AddListener(OnAdDisplayed);
    adConfig.onAdClosed.AddListener(OnAdClosed);
    adConfig.onAdFailed.AddListener(OnAdFailed);
}

void OnAdDisplayed(AdType adType)
{
    Debug.Log($"Ad displayed: {adType}");
}

void OnAdClosed(AdType adType)
{
    Debug.Log($"Ad closed: {adType}");
}

void OnAdFailed(AdType adType, string error)
{
    Debug.Log($"Ad failed: {adType}, Error: {error}");
}

--------------------------------------------------------------------------------------------
Configuration Options
In the DefaultAdConfig asset:

Ad Unit IDs

Interstitial Ad Unit ID: Your AdMob interstitial ad ID
App Open Ad Unit ID: Your AdMob app open ad ID


App Open Ad Settings

Show Initial App Open Ad: Whether to show an app open ad when game starts
Initial App Open Ad Delay: How long to wait before showing the first ad


General Settings

Use Test Ads: Use Google's test ad IDs (enable during development)
Debug Mode: Show detailed logs in the console
Pause On Ad: Pause the game while ads are showing



Testing

In the Unity Editor:

Make sure "Use Test Ads" is checked in DefaultAdConfig
The AdTestUI will provide buttons to test both ad types
Check the Console window for detailed logs


On Device:

Build to a device with test ads enabled first
Verify ads are showing correctly
Check timing and placement in your game flow



Troubleshooting
Common issues and solutions:

Ads not showing:

Verify Ad Unit IDs are correct
Check if ads are still loading
Look for error messages in the Console
Ensure device has internet connection


Test UI not working:

Verify AdTestUI is in the scene
Check console for initialization messages
Ensure no other UI is blocking input


Build issues:

Verify Google Mobile Ads package is imported
Check minimum Android/iOS version settings
Ensure correct platform is selected



Best Practices

Ad Placement:

Show interstitial ads at natural break points
Don't interrupt intense gameplay
Give players time to process achievements


Testing:

Always test with test ads during development
Test on multiple devices
Verify ad behavior with different internet conditions


Production:

Double-check ad unit IDs before release
Test a production build before publishing
Monitor ad performance after release



Support
For issues or questions:

Check the Console window for error messages
Verify against this documentation
Review the AdMob documentation for specific ad-related issues

Version History

1.0.0: Initial release

Interstitial and app open ad support
Test UI implementation
Automatic ad lifecycle management
------------------------------------------------------------------------------------------