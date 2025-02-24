namespace FABStudios.AdSystem
{
    public enum AdType
    {
        // Interstitial ads are full-screen ads that show between game levels
        // or during natural pause points in your game
        Interstitial,

        // App open ads are special ads that show when your game first starts
        // or returns to the foreground
        AppOpen,

        // Banner ads are great for constant monetization without interrupting
        // gameplay
        Banner
    }
}
