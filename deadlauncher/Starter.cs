using Octokit;

namespace deadlauncher;

public class Starter
{
    public static async Task Main()
    {
        
        DeadaysLauncherWindow window = new();
        await Start(window);
    }

    private static async Task Start(DeadaysLauncherWindow window)
    {
        await window.Prepare();
        //window.Loop();
    }
}