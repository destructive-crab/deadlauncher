using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using Raylib_cs;

public class DeadLauncherBoot
{
    private const string GithubUsername = "destructive-crab";
    private const string GithubRepositoryName = "deadlauncher";
    private const string GithubLauncherTag = "launcher";
    public static string AssetName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "deadlauncher.exe";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "deadlauncher";
        }
        return "invalid";
    }

    public static readonly string LauncherFolderPath = Path.Combine("deadlauncher");
    public static readonly string DataFolderPath = Path.Combine(LauncherFolderPath, "data");
    
    public static readonly string ConfigPath = Path.Combine(DataFolderPath, "config.json");
    public static readonly string LauncherExecutablePath = Path.Combine(LauncherFolderPath, AssetName());
    public static readonly string LauncherVersionPath = Path.Combine(DataFolderPath, "launcher_version");

    public static string GetFullPath(string path)
    {
        string applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(applicationDataPath, path);
    }
    
    public static async Task Main()
    {
        Booter booter = new Booter();

        await booter.ValidateFolders();
        await booter.StartLauncher();
    }

    private class Booter
    {
        public async Task ValidateFolders()
        {
            if (!Directory.Exists(GetFullPath(LauncherFolderPath))) { Directory.CreateDirectory(GetFullPath(LauncherFolderPath)); }
            if (!Directory.Exists(GetFullPath(DataFolderPath)))     { Directory.CreateDirectory(GetFullPath(DataFolderPath)); }
        }

        public async Task StartLauncher()
        {
            string launcherPath = GetFullPath(LauncherExecutablePath);
            bool isLauncherInstalled = File.Exists(launcherPath);
            string versionID = null;

            if (isLauncherInstalled)
            {
                if (!File.Exists(GetFullPath(LauncherVersionPath)))
                {
                    File.Delete(launcherPath);
                    
                    isLauncherInstalled = true;
                }
                else
                {
                    versionID = await File.ReadAllTextAsync(GetFullPath(LauncherVersionPath));
                }
            }
            
            //update logic
            WebClient webClient = new();
            GitHubClient client = new GitHubClient(GithubUsername, GithubRepositoryName);
            
            string id = webClient.DownloadString(new Uri(client.GetAssetDownloadURL(GithubLauncherTag, "version")));

            if (isLauncherInstalled)
            {
                if (id == versionID)
                {
                    BootInstalledLauncherAndShutdown();
                    return;
                }
                else
                {
                    await ReplaceLauncherWith(id);
                }
            }
            else
            {
                await ReplaceLauncherWith(id);
            }
            
            BootInstalledLauncherAndShutdown();
        }

        private async Task ReplaceLauncherWith(string versionID)
        {
            GitHubClient client = new(GithubUsername, GithubRepositoryName);
            
            if (File.Exists(GetFullPath(LauncherExecutablePath)))
            {
                File.Delete(GetFullPath(LauncherExecutablePath));
            }

            string downloadLink = client.GetAssetDownloadURL(GithubLauncherTag, AssetName());
            
            WebClient webClient = new WebClient();
            ProgressWindow window = new();

            window.BindProgressWindow(webClient);
            await webClient.DownloadFileTaskAsync(downloadLink, GetFullPath(LauncherExecutablePath));
            
            File.Create(GetFullPath(LauncherVersionPath)).Close();
            await File.WriteAllTextAsync(GetFullPath(LauncherVersionPath), versionID);
        }

        private void BootInstalledLauncherAndShutdown()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process process = new Process();
                process.StartInfo.FileName = GetFullPath(LauncherExecutablePath);
                process.Start();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process givePermission = new Process();
                
                givePermission.StartInfo.FileName = "chmod";
                givePermission.StartInfo.Arguments = $"+x {GetFullPath(LauncherExecutablePath)}";
                givePermission.Start();

                Process launch = new();
                launch.StartInfo.FileName = GetFullPath(LauncherExecutablePath);
                launch.Start();
            }
            Process.GetCurrentProcess().Close();    
        }
    }
    private class ProgressWindow
    {
        public string State = "Updating Launcher";
        private int progress;

        public void BindProgressWindow(WebClient webClient)
        {
            ThreadPool.QueueUserWorkItem(delegate { CreateWindow(); });
            webClient.DownloadProgressChanged += TrackProgress;
        }

        private void TrackProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            progress = e.ProgressPercentage;
        }

        public void CreateWindow()
        {
            Raylib.InitWindow(300, 100, "Deadays Updater");
            while (!Raylib.WindowShouldClose())
            {
                int width = (int)((progress / 100f) * 260);
                Raylib.ClearBackground(Color.White);
                Raylib.BeginDrawing();
                {
                    Raylib.DrawText(State + $"({progress})", 22, 10, 21, Color.Blue);
                    Raylib.DrawRectangle(20, 50, width, 40, Color.Blue);   
                }
                Raylib.EndDrawing();
            }
        }
    }
}