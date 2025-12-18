using System.IO.Compression;
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

        if (!Path.Exists(launcherFolder)) Directory.CreateDirectory(launcherFolder);
        if (!Path.Exists(dataFolder))     Directory.CreateDirectory(dataFolder);
        if (!Path.Exists(versionsFolder)) Directory.CreateDirectory(versionsFolder);

        string[] subdirs = Directory.GetDirectories(versionsFolder);
        
        foreach (string s in subdirs)
        {
            string possibleID = Path.GetFileName(s);

            l.Model.RegisterVersionFolder(possibleID, s);
        }

        string selectedVersionFile = Path.Combine(dataFolder, "selected_version");
        
        if (Path.Exists(selectedVersionFile))
        {
            l.Model.SetVersion(File.ReadAllText(selectedVersionFile));
        }
        else
        {
            l.Model.SetVersion(l.Model.Available[0]);
            File.Create(selectedVersionFile).Close();
            File.WriteAllText(selectedVersionFile, l.Model.Available[0]);
        }
    }

    public async Task DownloadVersion(string id, Action<string> trackProgress)
    {
        if (!(l.Model.IsValid(id) && !l.Model.IsInstalled(id))) return;
        
        WebClient webClient = new();
        string zipPath = Path.Combine(versionsFolder + l.Model.SelectedVersionId + ".zip");

        webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
        await webClient.DownloadFileTaskAsync(l.Model.DownloadLink(id), zipPath);

        string path = Path.Combine(versionsFolder, id);
        Directory.CreateDirectory(path);
        
        ZipFile.ExtractToDirectory(zipPath, path);
        File.Delete(zipPath);

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
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            File.Delete(file);
        }
        Directory.Delete(path);
        l.Model.DeleteVersionFromDrive(id);
    }
}