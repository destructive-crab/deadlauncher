using System.Runtime.InteropServices;

namespace deadlauncher;

public sealed class Launcher
{
    public static readonly string VERSION = "4.0";
    
    //M
    public readonly LauncherModel  Model;
    //V
    public readonly LauncherWindow Window;
    //C
    public readonly Downloader     Downloader;
    public readonly Runner         Runner;
    public readonly FileManager    FileManager;

    public Launcher()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            FileManager = new WindowsFileManager();
            Downloader  = new Downloader(this);
            Runner      = new WindowsRunner(this);
            Model       = new LauncherModel();
            Window      = new LauncherWindow();
        }
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            FileManager = new LinuxFileManager();
            Downloader  = new Downloader(this);
            Runner      = new LinuxRunner(this);
            Model       = new LauncherModel();
            Window      = new LauncherWindow();
        }
    }
}