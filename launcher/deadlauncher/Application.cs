namespace deadlauncher;

public static class Application
{
    public static readonly Launcher Launcher = new();
    
    public static async Task Main() => await Start();

    private static async Task Start()
    {
        await Launcher.Downloader.PullVersions();
        Launcher.Downloader.LoadLocalData();
        
        await Launcher.Window.Prepare();
        Launcher.Window.Loop();
    }
}