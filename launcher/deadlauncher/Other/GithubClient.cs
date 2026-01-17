using System.Net;

namespace deadlauncher;

public class GithubClient
{
    private readonly string userID;
    private readonly string repoID;

    public string RepoURL => $"https://github.com/{userID}/{repoID}";
    public string TagsPageURL => RepoURL + "/tags";

    public GithubClient(string userId, string repoId)
    {
        userID = userId;
        repoID = repoId;
    }

    public string? GetDownloadOfAssetURL(string tag, string asset)
    {
        var url = RepoURL + $"/releases/download/{tag}/{asset}";

        if (IsValidURL(url))
        {
            return url;
        }
        else
        {
            return null;
        }
    }
    
    public async Task<string[]> GetReleaseTags()
    {
        WebClient webClient = new();
        string tagsPage = webClient.DownloadString(new Uri(TagsPageURL));

        string key = $"href=\"/{userID}/{repoID}/releases/tag/";

        HashSet<string> tags = new();
        
        while (tagsPage.Contains(key))
        {
            int startIndex = tagsPage.IndexOf(key);
            
            int endIndex = -1;

            string tag = "";
            
            for (int i = startIndex + key.Length; true; i++)
            {
                if (tagsPage[i] != '\"')
                {
                    tag += tagsPage[i];
                    endIndex = i;
                }
                else
                {
                    break;
                }
            }

            tagsPage = tagsPage.Remove(startIndex, endIndex - startIndex);
            
            tags.Add(tag);
        }
        
        return tags.ToArray();
    }

    public string[] GetAssetNamesOfRelease(string tag)
    {
        string releasePageURL = RepoURL + $"/releases/tag/{tag}";

        if (IsValidURL(releasePageURL))
        {
            WebClient webClient = new();

            string pageContent = webClient.DownloadString(releasePageURL);
            string key = $"href=\"/{userID}/{repoID}/releases/download/{tag}/";

            HashSet<string> assets = new();
            
            while (pageContent.Contains(key))
            {
                int startIndex = pageContent.IndexOf(key);
                
                int endIndex = -1;
    
                string assetID = "";
                
                for (int i = startIndex + key.Length; true; i++)
                {
                    if (pageContent[i] != '\"')
                    {
                        assetID += pageContent[i];
                        endIndex = i;
                    }
                    else
                    {
                        break;
                    }
                }
    
                pageContent = pageContent.Remove(startIndex, endIndex - startIndex);
                
                assets.Add(assetID);
                Console.WriteLine(assetID);
            }

            return assets.ToArray();
        }
        else
        {
            return new string[0];
        }
    }

    private static bool IsValidURL(string url)
    {
        try
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "HEAD";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                return response.StatusCode == HttpStatusCode.OK;
            }
        }   
        catch
        {
            Console.WriteLine(url);
            return false;
        }
    }
}