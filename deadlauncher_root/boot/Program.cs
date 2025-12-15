using System.Diagnostics;
using System.Net;
using Octokit;

public class DeadLauncherBoot
{
    public const string GithubUsername = "destructive-crab";
    public const string GithubRepositoryName = "deadlauncher";
    public const string GithubLauncherTag = "launcher";

    public static readonly string ConfigPath = Path.Combine("deadlauncher", "config.json");
    public static readonly string LauncherPath = Path.Combine("deadlauncher", "deadlauncher.exe");
    public static readonly string LauncherVersionPath = Path.Combine("deadlauncher", "data", "launcher_version");

    public static string GetPath(string path)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), path);
    }
    
    public static async Task Main()
    {
        Booter booter = new Booter();
        await booter.StartLauncher();
    }

    class Booter
    {
        public async Task StartLauncher()
        {
            string launcherPath = GetPath(LauncherPath);
            bool isLauncherInstalled = File.Exists(launcherPath);
            string versionID = null;

            if (isLauncherInstalled)
            {
                if (!File.Exists(GetPath(LauncherVersionPath)))
                {
                    File.Delete(launcherPath);
                    
                    isLauncherInstalled = true;
                }
                else
                {
                    versionID = await File.ReadAllTextAsync(GetPath(LauncherVersionPath));
                }
            }
            
            //update logic
            GitHubClient client = new GitHubClient(new ProductHeaderValue("Deadays-Launcher"));
            
            Task<Release>? releaseTask = client.Repository.Release.Get(GithubUsername, GithubRepositoryName, GithubLauncherTag);
            await releaseTask;
            var release = releaseTask.Result;

            if (release == null)
            {
                //throw message 
                return;
            }
            
            if (isLauncherInstalled)
            {
                if (release.Name == versionID)
                {
                    BootInstalledLauncherAndShutdown();
                    return;
                }
                else
                {
                    await ReplaceLauncherWith(release);
                }
            }
            else
            {
                await ReplaceLauncherWith(release);
            }
            
            BootInstalledLauncherAndShutdown();
        }

        private async Task ReplaceLauncherWith(Release release)
        {
            if (File.Exists(GetPath(LauncherPath)))
            {
                File.Delete(GetPath(LauncherPath));
            }

            string downloadLink = $"https://github.com/destructive-crab/deadlauncher/releases/download/{release.TagName}/{release.Assets[0].Name}";

            WebClient webClient = new WebClient();
            await webClient.DownloadFileTaskAsync(downloadLink, GetPath(LauncherPath));
            File.Create(GetPath(LauncherVersionPath)).Close();
            await File.WriteAllTextAsync(GetPath(LauncherVersionPath), release.Name);
        }

        private void BootInstalledLauncherAndShutdown()
        {
            Process process = new Process();
            process.StartInfo.FileName = GetPath(LauncherPath);
            process.Start();
            Process.GetCurrentProcess().Close();
        }
    }
}