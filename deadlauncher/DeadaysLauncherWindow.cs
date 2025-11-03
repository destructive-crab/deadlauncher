using System.Diagnostics;
using deGUISpace;
using Raylib_cs;

namespace deadlauncher;

public class DeadaysLauncherWindow
{
    private List<string> downloadedVersions = new();
    public VersionLogic versionLogic = new();

    private Button versionButton;
    private Button playButton;
    private Button backButton;

    private List<Button> versionButtons = new();
    private Dictionary<string, Button> versionButtonsMap = new();
    
    public async Task Prepare()
    {
        Raylib.InitWindow(deGUI.ORIGINAL_WIDTH, deGUI.ORIGINAL_HEIGHT, "Deadays Launcher");
        
        Color buttonColor = new Color(255, 217, 186, 255);
        Color buttonColorPressed = new Color(225, 160, 130, 255);
        
        versionButton = deGUI.PushButton(Anchor.LeftBottom, 150, -200, 200, 40, "VERSION", VersionButton, null);
        backButton = deGUI.PushButton(Anchor.CenterBottom, 0, -40, deGUI.ORIGINAL_WIDTH-100, 40, "BACK", BackButton, null);
        playButton = deGUI.PushButton(Anchor.CenterBottom, 0, -100, 200, 100, "PLAY", PlayButton, null);

        int offset = -(deGUI.ORIGINAL_HEIGHT - versionLogic.availableOnServerIDs.Count * 40) / 2;
        for (var i = 0; i < versionLogic.availableOnServerIDs.Count; i++)
        {
            string id = versionLogic.availableOnServerIDs[i];
            var button = deGUI.PushButton(Anchor.CenterBottom, 0, offset - 50 * i, 240, 40, id, () => VersionSelectButton(id), null);
            button.Label = id;
            button.Color = buttonColor;
            versionButtons.Add(button);
            versionButtons.Last().Hide();
            versionButtonsMap.Add(id, button);
        }

        versionButtons.Reverse();
        backButton.Hide();
    }

    private void BackButton()
    {
        foreach (Button button in versionButtons)
        {
            button.Hide();
        }
        playButton.Show();
        backButton.Hide();
        versionButton.Show();
    }

    private void VersionSelectButton(string id)
    {
        versionLogic.CurrentVersionId = id;
        BackButton();
    }
    
    private async void PlayButton()
    {
        if (versionLogic.IsSelectedVersionInstalled)
        {
            Process process = new Process();
            process.StartInfo.FileName = versionLogic.CurrentVersionExecutablePath;
            process.Start();
        }
        else
        {
            await versionLogic.DownloadSelectedVersion((progress) => playButton.Label = progress.ToString() + "%");
            playButton.Label = "PLAY";
        }
    }

    private void VersionButton()
    {
        playButton.Hide();

        foreach (Button button in versionButtons)
        {
            button.Show();
        }
        backButton.Show();
        versionButton.Hide();
    }

    public void Loop()
    {
        while (!Raylib.WindowShouldClose())
        {
            Color backgroundColor = new Color(65, 11, 21, 255);
            Raylib.ClearBackground(backgroundColor);
            
            Raylib.BeginDrawing();

            deGUI.Draw();
            versionButton.Label = $"VERSION - {versionLogic.CurrentVersionId}";
            
            foreach (KeyValuePair<string,Button> pair in versionButtonsMap)
            {
                pair.Value.Label = pair.Key;

                if (pair.Key == versionLogic.CurrentVersionId)
                {
                    pair.Value.Label = "> " + pair.Key + " <";
                }
            }
            
            if (playButton.Active)
            {
                if (!versionLogic.IsSelectedVersionInstalled && playButton.Label == "PLAY")
                {
                    playButton.Label = "DOWNLOAD";
                }

                if (versionLogic.IsSelectedVersionInstalled && playButton.Label == "DOWNLOAD")
                {
                    playButton.Label = "PLAY";
                  
                }
            }
            

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