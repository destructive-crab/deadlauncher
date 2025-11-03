using System.IO.Compression;
using System.Net;
using Octokit;

namespace deadlauncher;

public class VersionLogic
{
    public string CurrentVersionId
    {
        get
        {
            return currentVersionID;
        }
        set
        {
            currentVersionID = value;
            
            if(executableMap.ContainsKey(currentVersionID))
            {
                CurrentVersionExecutablePath = Path.Combine(executableMap[CurrentVersionId], "Deadays.exe");
            }
        }
    }

    private string currentVersionID;
    public string CurrentVersionExecutablePath;
    
    public readonly List<string> localVersionsIDs = new();
    public readonly Dictionary<string, string> executableMap = new();
    
    public readonly List<string> availableOnServerIDs = new();
    public readonly Dictionary<string, string> downloadLinkMap = new();
    private string launcherFolder;
    private string dataFolder;
    private string versionsFolder;

    public bool IsSelectedVersionInstalled => executableMap.ContainsKey(CurrentVersionId);
    
    public async Task PullVersions()
    {
        availableOnServerIDs.Clear();
        downloadLinkMap.Clear();
        
        GitHubClient client = new GitHubClient(new ProductHeaderValue("Deadays-Launcher"));
        Task<IReadOnlyList<Release>> releaseTask = client.Repository.Release.GetAll("destructive-crab", "deadlauncher");
        await releaseTask;

        IReadOnlyList<Release> allReleases = releaseTask.Result;

        foreach (Release release in allReleases)
        {
            string downloadURL 
                = $"https://github.com/destructive-crab/deadlauncher/releases/download/{release.TagName}/{release.Assets[0].Name}";
            
            availableOnServerIDs.Add(release.TagName);
            downloadLinkMap.Add(release.TagName, downloadURL);
        }
    }

    public async void LoadLocalData()
    {
        string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        
        launcherFolder = Path.Combine(roaming, "deadlauncher");
        dataFolder = Path.Combine(launcherFolder, "data");
        versionsFolder = Path.Combine(launcherFolder, "versions");

        if (!Path.Exists(launcherFolder)) Directory.CreateDirectory(launcherFolder);
        if (!Path.Exists(dataFolder)) Directory.CreateDirectory(dataFolder);
        if (!Path.Exists(versionsFolder)) Directory.CreateDirectory(versionsFolder);

        string[] subdirs = Directory.GetDirectories(versionsFolder);
        
        foreach (string s in subdirs)
        {
            string possibleID = Path.GetFileName(s);
            
            if (availableOnServerIDs.Contains(possibleID))
            {
                localVersionsIDs.Add(possibleID);
                executableMap.Add(possibleID, s);
            }
        }

        string selectedVersionFile = Path.Combine(dataFolder, "selected_version");
        
        if (Path.Exists(selectedVersionFile))
        {
            CurrentVersionId = File.ReadAllText(selectedVersionFile);
        }
        else
        {
            CurrentVersionId = availableOnServerIDs[0];
        }

        if (executableMap.ContainsKey(CurrentVersionId))
        {
            CurrentVersionExecutablePath = Path.Combine(executableMap[CurrentVersionId], "Deadays.exe");
        }
    }

    public async Task DownloadSelectedVersion(Action<string> trackProgress)
    {
        WebClient webClient = new WebClient();
        string fileName = Path.Combine(versionsFolder + CurrentVersionId + ".zip");

        webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
        await webClient.DownloadFileTaskAsync(downloadLinkMap[CurrentVersionId], fileName);

        string path = Path.Combine(versionsFolder, CurrentVersionId);
        Directory.CreateDirectory(path);
        
        ZipFile.ExtractToDirectory(fileName, path);
        executableMap.Add(CurrentVersionId, path);
        File.Delete(fileName);
        
        CurrentVersionExecutablePath = Path.Combine(executableMap[CurrentVersionId], "Deadays.exe");

        void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            float percent = (float)e.BytesReceived / e.TotalBytesToReceive * 100;
            string temp = percent.ToString();
            string res = temp.Substring(0, 3);
            trackProgress?.Invoke(res);
        }
    }
}