using deGUISpace;
using Raylib_cs;
using Octokit;

namespace deadlauncher;

public class DeadaysLauncherWindow
{
    private string Version = "";

    private List<string> downloadedVersions = new();
    
    public async Task Prepare()
    {
        Raylib.InitWindow(500, 400, "Deadays Launcher");
        
        deGUI.PushButton(Anchor.CenterTop, 0, 40, 160, 40, "VERSION", VersionButton, null);
        var button = deGUI.PushButton(Anchor.CenterBottom, 0, -40, 160, 40, "PLAY", PlayButton, null);

        GitHubClient client = new GitHubClient(new ProductHeaderValue("Deadays-Launcher"));
        Task<IReadOnlyList<Release>> repo = client.Repository.Release.GetAll("destructive-crab", "deadlauncher");
        await repo;

        repo.Result[0];
        // button.Label = user.Name;
    }

    private void PlayButton()
    {
    }

    private void VersionButton()
    {
    }

    public void Loop()
    {
        while (!Raylib.WindowShouldClose())
        {
            Raylib.ClearBackground(Color.White);
            
            Raylib.BeginDrawing();
            
            
            
            
            
            deGUI.Draw();

            Raylib.EndDrawing();
        }
    }

    public void Shutdown()
    {
        Raylib.CloseWindow();
    }

    private void LaunchSelectedVersion()
    {
        
    }
}