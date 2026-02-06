using System.Runtime.InteropServices;

namespace deadlauncher;

public static class Application
{
    public static readonly Launcher Launcher = new();
    
    public static async Task Main() => await Start();

    private static async Task Start()
    {
        bool moveNext = await StartUpdater();
        if(moveNext) await StartLauncher();
    }

    private static async Task<bool> StartUpdater()
    {
        LauncherInstaller installer = new();
        return !(await installer.StartUpdater());
    }

    private static async Task StartLauncher()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            await Launcher.Downloader.PullVersions(["launcher", "linux"]);   
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            await Launcher.Downloader.PullVersions(["launcher", "windows"]);
        }
        
        Launcher.Downloader.LoadLocalData();
        
        await Launcher.Window.Prepare();
        Launcher.Window.Loop();
    }
}