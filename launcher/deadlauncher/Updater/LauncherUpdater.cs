using System.Net;
using deadlauncher;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class LauncherUpdater
{
    public struct InstallerContext
    {
        public enum InstallerResult
        {
            Install,
            Update,
            None
        }

        public readonly InstallerResult Result;
        public readonly bool StartedFromDifferentDirectory;
        public readonly string LauncherExecutablePath;

        public InstallerContext(bool startedFromDifferentDirectory, InstallerResult result, string launcherExecutablePath)
        {
            StartedFromDifferentDirectory = startedFromDifferentDirectory;
            Result = result;
            LauncherExecutablePath = launcherExecutablePath;
        }
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

    public async Task<InstallerContext> StartUpdater()
    {
        await ValidateData();
        
        InstallerContext.InstallerResult result = await TryUpdate();

        return new InstallerContext(Environment.ProcessPath != GetRoamingRelatedPath(LauncherFolderPath), result, GetRoamingRelatedPath(LauncherExecutablePath));
    }

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

    public static string GetRoamingRelatedPath(string path)
    {
        string applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(applicationDataPath, path);
    }
    
    public ProgressWindow Window { get; private set; }
    
    public async Task<InstallerContext.InstallerResult> TryUpdate()
    {
        try
        {
            InstallerContext.InstallerResult result;

            bool isLatestLauncherInstalled = File.Exists(LauncherUpdater.GetRoamingRelatedPath(LauncherExecutablePath));
        
            string? latestVersionID = PullLauncherVersionID();
        
            if (!isLatestLauncherInstalled)
            {
                Window = new();
                Window.CreateWindow();
                if(LocalLauncherVersionID == null)
                {
                    Window.State = "Installing Launcher *";
                    result = LauncherUpdater.InstallerContext.InstallerResult.Install;
                }
                else
                {
                    Window.State = "Updating Launcher *";
                    result = LauncherUpdater.InstallerContext.InstallerResult.Update;
                }
            
                await ReplaceLauncherWith(latestVersionID, Window);
            }
            else
            {
                string[] allExecutables = Application.Launcher.FileManager.PullFiles(GetRoamingRelatedPath(LauncherFolderPath), "*.exe");
                
                if (allExecutables.Length > 1)
                {
                    foreach (string executable in allExecutables)
                    {
                        if (executable != GetRoamingRelatedPath(LauncherExecutablePath) || Environment.ProcessPath != executable)
                        {
                            Application.Launcher.FileManager.Delete(executable);
                        }
                    }
                }
                result = LauncherUpdater.InstallerContext.InstallerResult.None;
            }
        
            if (Application.ProcessDirectory != GetRoamingRelatedPath(LauncherFolderPath))
            {
                if (Window == null)
                {
                    Window = new();
                    Window.CreateWindow();
                }
                Window.State = "Launcher Is Installed!\nNow you can start it from Start Menu\nThis executable can be deleted";
                Window.Progress = 0;

                while (!Window.WindowShouldClose)
                {
                    await Task.Yield();
                }
            }
            
            Window?.CloseWindow();
        
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return InstallerContext.InstallerResult.None;
        }
    }

    private async Task ReplaceLauncherWith(string versionID, ProgressWindow window)
    {
        File.Create(LauncherUpdater.GetRoamingRelatedPath(LauncherUpdater.LauncherVersionPath)).Close();
        await File.WriteAllTextAsync(LauncherUpdater.GetRoamingRelatedPath(LauncherUpdater.LauncherVersionPath), versionID);
        
        GitHubClient client = new(LauncherUpdater.GithubUsername, LauncherUpdater.GithubRepositoryName);
        
        if (File.Exists(LauncherUpdater.GetRoamingRelatedPath(LauncherExecutablePath)))
        {
            File.Delete(LauncherUpdater.GetRoamingRelatedPath(LauncherExecutablePath));
        }

        string downloadLink = client.GetAssetDownloadURL(LauncherUpdater.GithubLauncherTag, LauncherUpdater.AssetName());
        
        WebClient webClient = new WebClient();
        
        window.BindProgressWindow(webClient);
        await webClient.DownloadFileTaskAsync(downloadLink, LauncherUpdater.GetRoamingRelatedPath(LauncherExecutablePath));
        
        if (Application.Launcher.FileManager is WindowsFileManager windowsFileManager)
        {
            Application.Launcher.FileManager.Delete(LauncherUpdater.StartMenuShortcutFullPath);
            windowsFileManager.CreateShortcut(LauncherUpdater.StartMenuShortcutFullPath, LauncherUpdater.GetRoamingRelatedPath(LauncherExecutablePath), "Deadays Launcher by destructive_crab");
        }
    }
}

public class ProgressWindow
{
    public string State = "Updating Launcher";
    public int Progress;
    public Font Font;

    public ProgressWindow()
    {
        Font = new(ResourcesHandler.Load("UI.ttf"));
    }

    public void BindProgressWindow(WebClient webClient)
    {
        webClient.DownloadProgressChanged += TrackProgress;
    }

    private void TrackProgress(object sender, DownloadProgressChangedEventArgs e)
    {
        Progress = e.ProgressPercentage;
    }

    public bool WindowShouldClose = false;
    
    public void CreateWindow()
    {
        ThreadPool.QueueUserWorkItem(delegate { WindowLoop(); });
    }

    private void WindowLoop()
    {
        RenderWindow window = new(new VideoMode(300, 100), "Launcher Updater", Styles.Close);
        window.Closed += (a, b) => WindowShouldClose = true;

        Text text = new Text();
        
        text.Position = new Vector2f(22, 10);
        text.CharacterSize = 18;
        text.FillColor = Color.Blue;
        text.Font = Font;
        
        RectangleShape progressBar = new();

        progressBar.Position = new(20, 50);
        progressBar.FillColor = Color.Blue;
        
        while (!WindowShouldClose)
        {
            int width = (int)((Progress / 100f) * 260);
            window.DispatchEvents();
            window.Clear(Color.White);
            {
                text.DisplayedString = State.Replace("*", $"({Progress})");
                progressBar.Size = new Vector2f(width, 40);

                if (window.Size.X < text.GetGlobalBounds().Size.X + 40)
                {
                    window.Size = new Vector2u((uint)(text.GetGlobalBounds().Size.X + 40), window.Size.Y); window.Draw(text);
                    window.SetView(new View(new FloatRect(0,0,window.Size.X, window.Size.Y)));   
                }
                
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