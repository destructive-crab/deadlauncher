using System.Diagnostics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace deadlauncher;

public static class Application
{
    public static string ProcessDirectory => Path.GetDirectoryName(Environment.ProcessPath);
    public static string ProcessPath        => Environment.ProcessPath;
    
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

    static void ViewportTest()
    {
        RenderWindow window = new(VideoMode.DesktopMode, "SASD");

        var tex = new Texture("C:\\Users\\destructive_crab\\Documents\\2026-03-28_13-05_1.png");
        var sp  = new Sprite(tex);
        sp.Position = new Vector2f(0, 0);
        
        View view = new();
        view.Center = (Vector2f)window.Size/4f;
        view.Size = (Vector2f)window.Size/2;
        view.Viewport = new FloatRect((view.Center.X - view.Size.X/2f)/window.Size.X, (view.Center.Y - view.Size.Y/2f)/window.Size.Y, view.Size.X/window.Size.X, view.Size.Y/window.Size.Y);
        
        while (window.IsOpen)
        {
            window.DispatchEvents();
          
            if(Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                window.SetView(view);
            }

            if(Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                window.SetView(window.DefaultView);
            }
            
            window.Clear(Color.Green);
            {
                window.Draw(sp);
                window.Draw(new RectangleShape(new Vector2f(200, 200)) { FillColor = Color.Red, Position = new(200, 200)});
                window.Draw(new RectangleShape(new Vector2f(200, 200)) { FillColor = Color.Red, Position = new(300, 400)});
                window.Draw(new CircleShape(200) { FillColor = Color.Red, Position = new(300, 400)});
            }
            window.Display();
        }
    }
    
    private static async Task Start()
    {
        await StartLauncher();

        return;
        
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
        Launcher.Downloader.LoadLocalData();
        
        await Launcher.Window.Prepare();
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            await Launcher.Downloader.PullVersions(["launcher", "linux"]);   
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            await Launcher.Downloader.PullVersions(["launcher", "windows"]);
        }
        
        Launcher.Window.Loop();
    }

    public static void Quit()
    {
        
    }
}