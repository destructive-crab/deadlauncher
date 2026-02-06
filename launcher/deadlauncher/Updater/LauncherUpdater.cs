using System.Net;
using deadlauncher;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class LauncherUpdater
{
    #region CONSTS
    public const string GithubUsername = "destructive-crab";
    public const string GithubRepositoryName = "deadlauncher";
    public const string GithubLauncherTag = "launcher";
    public const string GithubLauncherVersionAssetName = "version";
    
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

    //windows shortcuts
    public static readonly string StartMenuShortcutFullPath = FullPath(Path.Combine("Microsoft", "Windows", "Start Menu", "Programs", "Dead Launcher.lnk"));
    #endregion
    
    public string LauncherExecutablePath
    {
        get
        {
            string assetName = AssetName();
            string idAddition = $"_{GetActualLauncherVersion()}";

            if (assetName.Contains(".exe"))
            {
                assetName = assetName.Replace(".exe", idAddition+".exe");
            }
            else
            {
                assetName = assetName += idAddition;
            }

            return Path.Combine(LauncherFolderPath, assetName);
        }
    }
    
    public string LatestLauncherVersionID { get; private set; } = null;
    public string LocalLauncherVersionID { get; private set; } = null;
    
    private string? GetActualLauncherVersion() => LatestLauncherVersionID;

    public static async Task<string?> PullLauncherVersionID()
    {
        WebClient webClient = new();
        GitHubClient github = new GitHubClient(GithubUsername, GithubRepositoryName);

        return await webClient.DownloadStringTaskAsync(new Uri(github.GetAssetDownloadURL(GithubLauncherTag, "version")));
    }

    public static string FullPath(string path)
    {
        string applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(applicationDataPath, path);
    }

    public async Task<InstallerContext> StartUpdater()
    {
        await ValidateData();
        
        InstallerContext.InstallerResult result = await TryUpdate();

        return new InstallerContext(result, FullPath(LauncherExecutablePath));
    }

    public async Task ValidateData()
    { 
        Application.Launcher.FileManager.ValidateFolder(FullPath(LauncherFolderPath));
        Application.Launcher.FileManager.ValidateFolder(FullPath(DataFolderPath));
        
        LocalLauncherVersionID = Application.Launcher.FileManager.ReadFile(FullPath(LauncherVersionPath));
        LatestLauncherVersionID = await PullLauncherVersionID();
    }

    public ProgressWindow Window { get; private set; }
    
    public async Task<InstallerContext.InstallerResult> TryUpdate()
    {
        try
        {
            InstallerContext.InstallerResult result;

            bool isLatestLauncherInstalled = File.Exists(LauncherUpdater.FullPath(LauncherExecutablePath));
        
            string? latestVersionID = await PullLauncherVersionID();
        
            if (!isLatestLauncherInstalled)
            {
                Window = new ProgressWindow();
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
                string[] allExecutables = Application.Launcher.FileManager.PullFiles(FullPath(LauncherFolderPath), "*.exe");
                
                if (allExecutables.Length > 1)
                {
                    foreach (string executable in allExecutables)
                    {
                        if (executable != FullPath(LauncherExecutablePath) || Environment.ProcessPath != executable)
                        {
                            Application.Launcher.FileManager.Delete(executable);
                        }
                    }
                }
                result = LauncherUpdater.InstallerContext.InstallerResult.None;
            }
        
            if (Application.ProcessDirectory != FullPath(LauncherFolderPath))
            {
                if (Window == null)
                {
                    Window = new ProgressWindow();
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
        File.Create(LauncherUpdater.FullPath(LauncherUpdater.LauncherVersionPath)).Close();
        await File.WriteAllTextAsync(LauncherUpdater.FullPath(LauncherUpdater.LauncherVersionPath), versionID);
        
        GitHubClient client = new(LauncherUpdater.GithubUsername, LauncherUpdater.GithubRepositoryName);
        
        if (File.Exists(LauncherUpdater.FullPath(LauncherExecutablePath)))
        {
            File.Delete(LauncherUpdater.FullPath(LauncherExecutablePath));
        }

        string downloadLink = client.GetAssetDownloadURL(LauncherUpdater.GithubLauncherTag, LauncherUpdater.AssetName());
        
        WebClient webClient = new WebClient();
        
        window.BindProgressWindow(webClient);
        await webClient.DownloadFileTaskAsync(downloadLink, LauncherUpdater.FullPath(LauncherExecutablePath));
        
        if (Application.Launcher.FileManager is WindowsFileManager windowsFileManager)
        {
            Application.Launcher.FileManager.Delete(LauncherUpdater.StartMenuShortcutFullPath);
            windowsFileManager.CreateShortcut(LauncherUpdater.StartMenuShortcutFullPath, LauncherUpdater.FullPath(LauncherExecutablePath), "Deadays Launcher by destructive_crab");
        }
    }
    
    public struct InstallerContext
    {
        public enum InstallerResult
        {
            Install,
            Update,
            None
        }

        public readonly InstallerResult Result;
        public readonly string LauncherExecutablePath;

        public InstallerContext(InstallerResult result, string launcherExecutablePath)
        {
            Result = result;
            LauncherExecutablePath = launcherExecutablePath;
        }
    }
}

public class ProgressWindow
{
    public string State = "Updating Launcher";
    public int Progress;
    
    public readonly Font Font;

    private WebClient boundWith;

    public ProgressWindow() => Font = new(ResourcesHandler.Load("UI.ttf"));

    ~ProgressWindow()
    {
        Unbind();
    }
    
    public void BindProgressWindow(WebClient webClient)
    {
        boundWith = webClient;

        Unbind();
        
        webClient.DownloadProgressChanged += TrackProgress;
    }

    private void Unbind()
    {
        if (boundWith != null)
        {
            boundWith.DownloadProgressChanged -= TrackProgress;
        }
    }

    private void TrackProgress(object sender, DownloadProgressChangedEventArgs e)
    {
        Progress = e.ProgressPercentage;
    }

    
    public void CreateWindow()
    {
        ThreadPool.QueueUserWorkItem(delegate { WindowLoop(); });
    }

    public bool WindowShouldClose = false;
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

        progressBar.Position = new Vector2f(20, 50);
        progressBar.FillColor = Color.Blue;
        
        while (!WindowShouldClose)
        {
            int width = (int)((Progress / 100f) * (window.Size.X-40));
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