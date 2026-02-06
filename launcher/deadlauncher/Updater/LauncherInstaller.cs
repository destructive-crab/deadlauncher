using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using deadlauncher;
using leditor.UI;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class LauncherInstaller
{
    public struct InstallerLog
    {
        public enum InstallerResult
        {
            Install,
            Update,
            None
        }

        public InstallerResult Result;
    }
    
    public const string GithubUsername = "destructive-crab";
    public const string GithubRepositoryName = "deadlauncher";
    public const string GithubLauncherTag = "launcher";
    private const string GithubLauncherVersionAssetName = "version";
    
    public static string AssetName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "deadlauncher.exe";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))   return "deadlauncher";
        
        return "invalid";
    }

    public static readonly string LauncherFolderPath = Path.Combine("deadlauncher");
    public static readonly string DataFolderPath = Path.Combine(LauncherFolderPath, "data");
    
    public static readonly string LauncherVersionPath = Path.Combine(DataFolderPath, "launcher_version");

    public static readonly string ConfigPath = Path.Combine(DataFolderPath, "config.json");
    public string LauncherExecutablePath => Path.Combine(LauncherFolderPath, AssetName().Replace(".exe", $"_{GetActualLauncherVersion()}.exe"));
    
    //windows shortcuts
    public static readonly string StartMenuShortcutFullPath = GetRoamingRelatedPath(Path.Combine("Microsoft", "Windows", "Start Menu", "Programs", "Dead Launcher.lnk"));
    public static readonly string DesktopShortcutPath = (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"Microsoft/Windows/StartMenu/Programs"));

    public string LatestLauncherVersionID { get; private set; } = null;
    public string LocalLauncherVersionID { get; private set; } = null;
    
    private string? GetActualLauncherVersion() => LatestLauncherVersionID;

    public async Task ValidateData()
    { 
        Application.Launcher.FileManager.ValidateFolder(GetRoamingRelatedPath(LauncherFolderPath));
        Application.Launcher.FileManager.ValidateFolder(GetRoamingRelatedPath(DataFolderPath));
        
        LocalLauncherVersionID = Application.Launcher.FileManager.ReadFile(GetRoamingRelatedPath(LauncherVersionPath));
        LatestLauncherVersionID = PullLauncherVersionID();
    }
    
    public string? PullLauncherVersionID()
    {
        WebClient webClient = new();
        GitHubClient github = new GitHubClient(GithubUsername, GithubRepositoryName);
        
        return webClient.DownloadString(new Uri(github.GetAssetDownloadURL(GithubLauncherTag, "version")));
    }
    
    public static string GetRoamingRelatedPath(string path,         
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        if (path == null)
        {
            Console.WriteLine(Path.GetFileName(file) + $": {member}({line}): " +" INVALID CALL");
            return "";
        }
        
        string applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(applicationDataPath, path);
    }
    
    public async Task<bool> StartUpdater()
    {
        await ValidateData();
        
        Updater updater = new Updater(this);
        bool wasUpdated = await updater.TryUpdate();

        return wasUpdated;
    }
}

public class Updater
{
    private LauncherInstaller Installer;

    public Updater(LauncherInstaller installer)
    {
        Installer = installer;
    }

    public async Task<bool> TryUpdate()
    {
        bool isLauncherInstalled = Application.Launcher.FileManager.Exist(LauncherInstaller.GetRoamingRelatedPath(LauncherInstaller.LauncherVersionPath));
        string? installedVersionID = null;

        if (isLauncherInstalled)
        {
            installedVersionID = await File.ReadAllTextAsync(LauncherInstaller.GetRoamingRelatedPath(LauncherInstaller.LauncherVersionPath));
            
            if (!File.Exists(LauncherInstaller.GetRoamingRelatedPath(Installer.LauncherExecutablePath)))
            {
                isLauncherInstalled = false;
            }
        }
        
        string? latestVersionID = Installer.PullLauncherVersionID();
        
        if (isLauncherInstalled)
        {
            if (latestVersionID == installedVersionID)
            {
                string[] allExecutables = Application.Launcher.FileManager.PullFiles(LauncherInstaller.GetRoamingRelatedPath(LauncherInstaller.LauncherFolderPath), "*.exe");
                
                if (allExecutables.Length > 1)
                {
                    foreach (string executable in allExecutables)
                    {
                        if (executable != LauncherInstaller.GetRoamingRelatedPath(Installer.LauncherExecutablePath))
                        {
                            Application.Launcher.FileManager.Delete(executable);
                        }
                    }
                }
                
                return false;
            }
            else
            {
                await ReplaceLauncherWith(latestVersionID);
                BootInstalledLauncherAndShutdown();
                return true;
            }
        }
        else
        {
            await ReplaceLauncherWith(latestVersionID);
            return true;
        }
    }

    private async Task ReplaceLauncherWith(string versionID)
    {
        File.Create(LauncherInstaller.GetRoamingRelatedPath(LauncherInstaller.LauncherVersionPath)).Close();
        await File.WriteAllTextAsync(LauncherInstaller.GetRoamingRelatedPath(LauncherInstaller.LauncherVersionPath), versionID);
        
        GitHubClient client = new(LauncherInstaller.GithubUsername, LauncherInstaller.GithubRepositoryName);
        
        if (File.Exists(LauncherInstaller.GetRoamingRelatedPath(Installer.LauncherExecutablePath)))
        {
            File.Delete(LauncherInstaller.GetRoamingRelatedPath(Installer.LauncherExecutablePath));
        }

        string downloadLink = client.GetAssetDownloadURL(LauncherInstaller.GithubLauncherTag, LauncherInstaller.AssetName());
        
        WebClient webClient = new WebClient();
        ProgressWindow window = new(new UIStyle().Font);

        window.BindProgressWindow(webClient);
        await webClient.DownloadFileTaskAsync(downloadLink, LauncherInstaller.GetRoamingRelatedPath(Installer.LauncherExecutablePath));
        
        if (Application.Launcher.FileManager is WindowsFileManager windowsFileManager)
        {
            Application.Launcher.FileManager.Delete(LauncherInstaller.StartMenuShortcutFullPath);
            windowsFileManager.CreateShortcut(LauncherInstaller.StartMenuShortcutFullPath, LauncherInstaller.GetRoamingRelatedPath(Installer.LauncherExecutablePath), "Deadays Launcher by destructive_crab");
        }
        window.CloseWindow();
    }

    private void BootInstalledLauncherAndShutdown()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process process = new Process();
            process.StartInfo.FileName = LauncherInstaller.GetRoamingRelatedPath(Installer.LauncherExecutablePath);
            process.Start();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process givePermission = new Process();
            
            givePermission.StartInfo.FileName = "chmod";
            givePermission.StartInfo.Arguments = $"+x {LauncherInstaller.GetRoamingRelatedPath(Installer.LauncherExecutablePath)}";
            givePermission.Start();

            Process launch = new();
            launch.StartInfo.FileName = LauncherInstaller.GetRoamingRelatedPath(Installer.LauncherExecutablePath);
            launch.Start();
        }
        
        Process.GetCurrentProcess().Close();    
    }
}

public class ProgressWindow
{
    public string State = "Updating Launcher";
    private int progress;
    public Font Font;

    public ProgressWindow(Font font)
    {
        Font = font;
    }

    public void BindProgressWindow(WebClient webClient)
    {
        ThreadPool.QueueUserWorkItem(delegate { CreateWindow(); });
        webClient.DownloadProgressChanged += TrackProgress;
    }

    private void TrackProgress(object sender, DownloadProgressChangedEventArgs e)
    {
        progress = e.ProgressPercentage;
    }

    private bool WindowShouldClose = false;
    public void CreateWindow()
    {
        RenderWindow window = new(new VideoMode(300, 100),"Launcher Updater");
        window.Closed += (a, b) => WindowShouldClose = true;

        Text text = new Text();
        
        text.Position = new Vector2f(22, 10);
        text.CharacterSize = 21;
        text.FillColor = Color.Blue;
        
        RectangleShape progressBar = new();

        progressBar.Position = new(20, 50);
        progressBar.FillColor = Color.Blue;
        
        while (!WindowShouldClose)
        {
            int width = (int)((progress / 100f) * 260);
            window.Clear(Color.White);
            {
                text.DisplayedString = State + $" ({progress})";
                progressBar.Size = new Vector2f(width, 40);   
                
                window.Draw(text);
                window.Draw(progressBar);
            }
            window.Display();
        }
    }

    public void CloseWindow()
    {
        WindowShouldClose = true;
    }
}