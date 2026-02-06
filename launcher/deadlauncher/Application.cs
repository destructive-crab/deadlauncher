using System.Diagnostics;

namespace deadlauncher;

public static class Application
{
    public static string ProcessDirectory => Path.GetDirectoryName(Environment.ProcessPath);
    public static string ProcessPath      => Environment.ProcessPath;
    
    public static readonly Launcher Launcher = new();
    
    private static bool IsExecutableRunningFromLauncherFolder(LauncherUpdater.InstallerContext context)
    {
        return Path.GetDirectoryName(context.LauncherExecutablePath) == Path.GetDirectoryName(ProcessPath);
    }

    private static bool WasExecutableChanged(LauncherUpdater.InstallerContext context)
    {
        return context.LauncherExecutablePath == ProcessPath;
    }
    
    public static async Task Main() => await Start();

    private static async Task Start()
    {
        LauncherUpdater.InstallerContext context = await StartUpdater();
        
        if(WasExecutableChanged(context))
        {
            await StartLauncher();
        }
        else if(IsExecutableRunningFromLauncherFolder(context))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process process = new Process();
                process.StartInfo.FileName = LauncherUpdater.FullPath(context.LauncherExecutablePath);
                process.Start();
            }
            
            Process.GetCurrentProcess().Close();    
        }
    }

    private static async Task<LauncherUpdater.InstallerContext> StartUpdater()
    {
        LauncherUpdater updater = new();
        return await updater.StartUpdater();
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