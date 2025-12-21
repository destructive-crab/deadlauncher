namespace deadlauncher;

public sealed class Launcher
{
    //M
    public readonly LauncherModel  Model;
    //V
    public readonly LauncherWindow Window;
    //C
    public readonly Downloader     Downloader;
    public readonly Runner         Runner;
    public readonly FileManager    FileManager = new WindowsFileManager();

    public Launcher()
    {
        Downloader = new Downloader(this);
        Runner     = new Runner(this);
        Model      = new LauncherModel();
        Window     = new LauncherWindow();
    }
}

public sealed class LauncherModel
{
    public string RunningLineText;
    public string CurrentVersionExecutablePath => ExecutablePath(selectedVersionID);
    
    private string selectedVersionID;
    public string SelectedVersionID
    {
        get { return selectedVersionID; }
        set { if (IsValid(value)) { selectedVersionID = value; } }
    }

    public string[] Installed => installedIDs.ToArray();
    public string[] Available => availableOnServerIDs.ToArray();

    private readonly List<string>               installedIDs = new();
    private readonly Dictionary<string, string> foldersWithExecutableMap = new();

    private readonly List<string>               availableOnServerIDs = new();
    private readonly Dictionary<string, string> downloadLinkMap = new();

    public event Action<string> OnVersionSelected;
    
    private Downloader logic => Application.Launcher.Downloader;

    public string? ExecutablePath(string id)
    {
        if (!foldersWithExecutableMap.ContainsKey(id)) return null;
        
        string path = Path.Combine(foldersWithExecutableMap[id], "Deadays.exe");

        return path;
    }

    public string ExecutableFolder(string id) => foldersWithExecutableMap[id];

    public string? DownloadLink(string id)
    {
        if (!downloadLinkMap.ContainsKey(id))
        {
            return null;
        }

        return downloadLinkMap[id];
    }
    
    public bool IsInstalled(string id) => foldersWithExecutableMap.ContainsKey(id);
    public bool IsValid(string id) => availableOnServerIDs.Contains(id);

    public bool RegisterVersion(string id, string downloadLink)
    {
        if (availableOnServerIDs.Contains(id)) return false;
        
        availableOnServerIDs.Add(id);
        downloadLinkMap.Add(id, downloadLink);

        return true;
    }
    public bool RegisterVersionFolder(string id, string executablePath)
    {
        if (installedIDs.Contains(id))          return false;
        if (!availableOnServerIDs.Contains(id)) return false;
        
        installedIDs.Add(id);
        foldersWithExecutableMap.Add(id, executablePath);
        
        return true;
    }

    public void DeleteVersionFromDrive(string id)
    {
        installedIDs.Remove(id);
        foldersWithExecutableMap.Remove(id);
    }

    public bool SetVersion(string id)
    {
        if (IsValid(id) && selectedVersionID != id)
        {
            selectedVersionID = id;
            OnVersionSelected?.Invoke(id);
            
            return true;
        }
        return false;
    }

    public bool IsSelected(string id)
    {
        return selectedVersionID == id;
    }
}