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
    public string CurrentVersionExecutablePath => ExecutablePath(currentVersionID);
    
    private string currentVersionID;
    public string CurrentVersionID
    {
        get { return currentVersionID; }
        set { if (IsValid(value)) { currentVersionID = value; } }
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
        if (installedIDs.Contains(id)) return false;
        if (!availableOnServerIDs.Contains(id)) return false;
        
        installedIDs.Add(id);
        foldersWithExecutableMap.Add(id, executablePath);
        
        return true;
    }

    public bool SetVersion(string id)
    {
        if (IsValid(id) && currentVersionID != id)
        {
            currentVersionID = id;
            OnVersionSelected?.Invoke(id);
            
            return true;
        }
        return false;
    }
}