using System.Net;

namespace deadlauncher;

public class Downloader
{
    private string launcherFolder;
    private string dataFolder;
    private string versionsFolder;

    private Launcher l;

    public Downloader(Launcher l)
    {
        this.l = l;
    }

    public async Task PullVersions()
    {
        GithubClient client = new("destructive-crab", "deadlauncher");
        
        string[] allReleases = await client.GetReleaseTags();

        foreach (var tag in allReleases)
        {
            if(tag== "launcher") continue;

            string downloadURL = client.GetDownloadOfAssetURL(tag, "deadays.zip");

            l.Model.RegisterVersion(tag, downloadURL);
        }
    }

    public async void LoadLocalData()
    {
        string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        
        launcherFolder = Path.Combine(roaming, "deadlauncher");
        dataFolder = Path.Combine(launcherFolder, "data");
        versionsFolder = Path.Combine(launcherFolder, "versions");

        Application.Launcher.FileManager.ValidateFolder(launcherFolder);
        Application.Launcher.FileManager.ValidateFolder(dataFolder);
        Application.Launcher.FileManager.ValidateFolder(versionsFolder);

        string[] subdirs = Directory.GetDirectories(versionsFolder);
        
        foreach (string s in subdirs)
        {
            string possibleID = Path.GetFileName(s);

            l.Model.RegisterVersionFolder(possibleID, s);
        }

        string selectedVersionFile = Path.Combine(dataFolder, "selected_version");

        string? version = Application.Launcher.FileManager.ReadFile(selectedVersionFile);
        
        if (!string.IsNullOrEmpty(version))
        {
            l.Model.SetVersion(version);
        }
        else
        {
            l.Model.SetVersion(l.Model.Available[0]);
            Application.Launcher.FileManager.WriteFile(selectedVersionFile, l.Model.Available[0]);
        }
    }

    public async Task DownloadVersion(string id, Action<string> trackProgress)
    {
        if (!(l.Model.IsVersionValid(id) && !l.Model.IsInstalled(id))) return;
        
        WebClient webClient = new();
        string zipPath = Path.Combine(versionsFolder + l.Model.SelectedVersionID + ".zip");

        webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
        await webClient.DownloadFileTaskAsync(l.Model.DownloadLink(id), zipPath);
        trackProgress?.Invoke("101");

        string path = Path.Combine(versionsFolder, id);
        
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

    public string? GetChangelog(string id)
    {
        GithubClient client = new("destructive-crab", "deadlauncher");
        string downloadURL = client.GetDownloadOfAssetURL(id, "changelog.txt");
        
        WebClient webClient = new();
        
        try
        {
            string changelog = webClient.DownloadString(new Uri(downloadURL));
            return changelog;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}