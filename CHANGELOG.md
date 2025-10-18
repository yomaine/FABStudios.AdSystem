# Changelog

All notable changes to FAB Studios Ad System will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-10-14

### Added
- Initial release of FAB Studios Ad System
- Banner ad support with configurable positions
- Interstitial ad support with automatic reloading
- App open ad support with configurable delay
- Automatic SDK initialization with retry logic
- Test ad support for development
- Comprehensive error handling and logging
- Unity events for ad lifecycle callbacks
- Optional game pause during ad display
- Singleton pattern for easy global access
- ScriptableObject-based configuration system
- Automatic banner hiding during interstitial ads
- Debug mode for development troubleshooting

### Features
- Support for 7 banner positions (Top, Bottom, corners, Center)
- Configurable banner sizes via AdSize
- Auto-retry SDK initialization (up to 3 attempts)
- Test environment detection (Editor vs Build)
- DontDestroyOnLoad for persistent ad management
- Memory-safe ad destruction on scene changes

### Documentation
- Complete README with installation instructions
- API reference documentation
- Quick start guide
- Example code snippets
- Troubleshooting section
- Best practices guide

## [Unreleased]

### Planned Features
- Rewarded video ad support
- Rewarded interstitial ad support
- Native ad support
- Ad frequency capping
- Analytics integration
- GDPR consent management
- iOS support improvements
- Custom event system for advanced tracking

---

For older versions or detailed commit history, see the [GitHub repository](https://github.com/yomaine/FABStudios.AdSystem).