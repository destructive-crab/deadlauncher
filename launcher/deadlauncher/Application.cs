using System.Diagnostics;

namespace deadlauncher;

public static class Application
{
    public static string ProcessDirectory => Path.GetDirectoryName(Environment.ProcessPath);
    public static string ProcessPath => Environment.ProcessPath;
    
    public static readonly Launcher Launcher = new();
    
    public static async Task Main() => await Start();

    private static async Task Start()
    {
        LauncherUpdater.InstallerContext context = await StartUpdater();
        
        if(context.LauncherExecutablePath == ProcessPath)
        {
            await StartLauncher();
        }
        else if(Path.GetDirectoryName(context.LauncherExecutablePath) == Path.GetDirectoryName(ProcessPath))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process process = new Process();
                process.StartInfo.FileName = LauncherUpdater.GetRoamingRelatedPath(context.LauncherExecutablePath);
                process.Start();
            }
            
            Process.GetCurrentProcess().Close();    
        }
    }

    private static async Task<LauncherUpdater.InstallerContext> StartUpdater()
    {
        LauncherUpdater updater = new();
        LauncherUpdater.InstallerContext context = (await updater.StartUpdater());

        return context;
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