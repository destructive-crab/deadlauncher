using System.Diagnostics.CodeAnalysis;
using System.Net;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace deadlauncher;

public class Downloader
{
    private Launcher l;

    private const string SERVER_URL = "https://oknoweb.ru";
    
    public Downloader(Launcher l)
    {
        this.l = l;
    }

    public string GetDownloadLink(string id) => SERVER_URL + $"/api/versions/download/{id}";
    
    public async Task PullVersions(string[] ignoreTagsThatContains)
    {
        try
        {
            HttpClient client = new()
            {
                BaseAddress = new Uri(SERVER_URL)
            };

            var response = await client.GetAsync("api/versions/");

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            VersionInfo[]? infos = JsonSerializer.Deserialize<VersionInfo[]>(jsonResponse);

            foreach (var info in infos)
            {
                Console.WriteLine(info.id + " " + info.tag);
                l.Model.RegisterVersion(info.id, info);
                Console.WriteLine(SERVER_URL + $"/api/download/{info.id}");
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
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
        try
        {
            if (!(l.Model.IsVersionValid(id) && !l.Model.IsInstalled(id))) return;
        
            WebClient webClient = new();
            string zipPath = Path.Combine(Application.Launcher.Model.VersionsFolder + l.Model.SelectedVersionID + ".zip");

            webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
            await webClient.DownloadFileTaskAsync(SERVER_URL+$"/api/versions/files/{id}", zipPath);
            trackProgress?.Invoke("101");

            string path = Path.Combine(Application.Launcher.Model.VersionsFolder, id);
        
            Application.Launcher.FileManager.ExtractZipTo(zipPath, path);
            Application.Launcher.FileManager.Delete(zipPath);

            Application.Launcher.FileManager.ValidateFolder(path);
        
            l.Model.RegisterVersionFolder(id, path);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

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
        try
        {
            HttpClient client = new()
            {
                BaseAddress = new Uri(SERVER_URL)
            };

            HttpResponseMessage response = await client.GetAsync($"api/versions/{id}");

            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();

            VersionInfo info = JsonSerializer.Deserialize<VersionInfo>(jsonResponse);
            return info.changelog;
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            return "No changelog found";
        }
    }
}