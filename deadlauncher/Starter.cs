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
        await window.versionLogic.PullVersions();
        window.versionLogic.LoadLocalData();
        await window.Prepare();

        window.Loop();
    }
}