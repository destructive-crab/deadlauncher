using System.Net;
using System.Threading.Tasks;
using Octokit;

namespace deadlauncher;

public class Starter
{
    public static async Task Main()
    {
        //Console.SetOut(TextWriter.Null);
        DeadaysLauncherWindow window = new();
        
        await Start(window);
    }

    private static async Task Start(DeadaysLauncherWindow window)
    {

        WebClient webClient = new();
        var s = webClient.DownloadString("https://github.com/destructive-crab/deadlauncher/releases/tag/v0.5");

        File.Create("C:\\Users\\destructive_crab\\Desktop\\dgt").Close();
        File.WriteAllText("C:\\Users\\destructive_crab\\Desktop\\dgt", s);
        
        return;
        
        
        await window.versionLogic.PullVersions();
        window.versionLogic.LoadLocalData();
        await window.Prepare();

        window.Loop();
    }
}