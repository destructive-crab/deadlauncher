using System.Net;

namespace deadlauncher;

public class Downloader
{
    private Launcher l;

    public Downloader(Launcher l)
    {
        this.l = l;
    }

    public async Task PullVersions(string[] ignoreTagsThatContains)
    {
        GitHubClient client = new("destructive-crab", "deadlauncher");
        
        string[] allReleases = await client.GetReleaseTags();

        foreach (var tag in allReleases)
        {
            bool valid = true;
            {
                foreach (string ignore in ignoreTagsThatContains)
                {
                    if (tag.Contains(ignore))
                    {
                        valid = false;
                    }
                }    
            }
            
            if (!valid) continue;

            string assetName = "";
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))   { assetName = "deadays_windows.zip"; }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) { assetName = "deadays_linux.zip"; }
            
            string? downloadURL = client.GetAssetDownloadURL(tag, assetName);

            if (downloadURL != null)
            {
                l.Model.RegisterVersion(tag, downloadURL);    
            }
        }
    }

    public async void LoadLocalData()
    {
        Application.Launcher.FileManager.ValidateFolder(Application.Launcher.Model.LauncherFolder);
        Application.Launcher.FileManager.ValidateFolder(Application.Launcher.Model.DataFolder);
        Application.Launcher.FileManager.ValidateFolder(Application.Launcher.Model.VersionsFolder);

        //todo
        string[] subdirs = Directory.GetDirectories(Application.Launcher.Model.VersionsFolder);
        
        
        foreach (string s in subdirs)
        {
            string possibleID = Path.GetFileName(s);

            l.Model.RegisterVersionFolder(possibleID, s);
        }

        string? version = Application.Launcher.FileManager.ReadFile(Application.Launcher.Model.SelectedVersionFile);
        
        if (!string.IsNullOrEmpty(version))
        {
            if (!l.Model.SetVersion(version))
            {
                l.Model.SetVersion(l.Model.Available[0]);
            }
        }
        else
        {
            l.Model.SetVersion(l.Model.Available[0]);
        }
    }

    public async Task DownloadVersion(string id, Action<string> trackProgress)
    {
        if (!(l.Model.IsVersionValid(id) && !l.Model.IsInstalled(id))) return;
        
        WebClient webClient = new();
        string zipPath = Path.Combine(Application.Launcher.Model.VersionsFolder + l.Model.SelectedVersionID + ".zip");

        webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
        await webClient.DownloadFileTaskAsync(l.Model.DownloadLink(id), zipPath);
        trackProgress?.Invoke("101");

        string path = Path.Combine(Application.Launcher.Model.VersionsFolder, id);
        
        Application.Launcher.FileManager.ExtractZipTo(zipPath, path);
        Application.Launcher.FileManager.Delete(zipPath);

        Application.Launcher.FileManager.ValidateFolder(path);
        
        l.Model.RegisterVersionFolder(id, path);

        void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            float percent = (float)e.BytesReceived / e.TotalBytesToReceive * 100;
            string temp = percent.ToString();
            string res = temp.Substring(0, 3);
            trackProgress?.Invoke(res);
        }
    }

    public async Task DeleteVersion(string id)
    {
        string path = l.Model.ExecutableFolder(id);
        Application.Launcher.FileManager.Delete(path);
        l.Model.DeleteVersionFromDrive(id);
    }

    public async Task<string?> GetChangelog(string id)
    {
        GitHubClient client = new("destructive-crab", "deadlauncher");
        string downloadURL = client.GetAssetDownloadURL(id, "changelog.txt");
        
        WebClient webClient = new();

        try
        {
            string changelog = await webClient.DownloadStringTaskAsync(new Uri(downloadURL));
            return changelog;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}