namespace deadlauncher;

public sealed class LauncherModel
{
    public const string NONE_SELECTED = "NONE";
    public const string MODE_POSTFIX  = "mode";
    
    public string RunningLineText = "";
    
    private string selectedVersionID = NONE_SELECTED;
    public string SelectedVersionID
    {
        get { return selectedVersionID; }
        set { if (IsVersionValid(value)) { SetVersion(value); } }
    }

    public string[] Installed => installedIDs.ToArray();
    public string[] Available => availableOnServerIDs.ToArray();

    private readonly List<string>               installedIDs = new();
    private readonly Dictionary<string, string> foldersWithExecutableMap = new();

    private readonly List<string>               availableOnServerIDs = new();
    private readonly Dictionary<string, string> downloadLinkMap = new();

    public event Action<string> OnVersionSelected;
    
    private Downloader logic => Application.Launcher.Downloader;
    
    public async Task<string?> Changelog(string id)
    {
        string? changelog = await Application.Launcher.Downloader.GetChangelog(id);

        string? folder = ExecutableFolder(id);
        string? cached = null;
        
        if (folder != null)
        {
            cached = Application.Launcher.FileManager.ReadFile(Path.Combine(folder, "changelog.txt"));
        }   
        
        if (changelog != null)
        {
            if (cached != null && cached != changelog)
            {
                Application.Launcher.FileManager.WriteFile(Path.Combine(folder, "changelog.txt"), changelog);
            }
            return changelog;
        }
        
        if(cached != null)
        {
            return cached;
        }

        return changelog;
    }

    public string? ExecutableFolder(string id)
    {
        if (!foldersWithExecutableMap.ContainsKey(id)) return null;
        return foldersWithExecutableMap[id];
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
    public bool IsVersionValid(string id) => availableOnServerIDs.Contains(id);

    public bool RegisterVersion(string id, string downloadLink)
    {
        if (availableOnServerIDs.Contains(id)) return false;
        
        availableOnServerIDs.Add(id);
        downloadLinkMap.Add(id, downloadLink);
        
        SortVersions();

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
        if (IsVersionValid(id) && selectedVersionID != id)
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

    private void SortVersions()
    {
        availableOnServerIDs.Sort(Comparator);

        int Comparator(string a, string b)
        {
            a = a.Replace("v", "");
            a = a.Replace("hotfix", "1");
            a = a.Replace("_", ".");

            b = b.Replace("v", "");
            b = b.Replace("hotfix", "1");
            b = b.Replace("_", ".");

            while (a.Length != b.Length)
            {
                if (a.Length > b.Length)
                {
                    b += ".0";
                }
                else
                {
                    a += ".0";
                }
            }
            
            
            string[] aDigits = a.Split(".", StringSplitOptions.RemoveEmptyEntries);
            string[] bDigits = b.Split(".", StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < aDigits.Length; i++)
            {
                if (!Int32.TryParse(aDigits[i], out int aDigit))
                {
                    return 0;
                }
                if (!Int32.TryParse(bDigits[i], out int bDigit))
                {
                    return 0;
                }
                
                if (aDigit > bDigit) return -1;
                if (aDigit < bDigit) return 1;
            }

            return 0;
        }
    }
}