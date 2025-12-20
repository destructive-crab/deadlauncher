using System.Reflection;

namespace deadlauncher;

public static class Application
{
    public static Launcher Launcher = new();
    
    public static async Task Main()
    {
        //Console.SetOut(TextWriter.Null);
        await Start(Launcher.Window);
    }

    private static async Task Start(LauncherWindow window)
    {
        
        
        await Launcher.Downloader.PullVersions();
        
        Launcher.Downloader.LoadLocalData();
        
        await window.Prepare();

        window.Loop();
    }
}