using leditor.UI;
using SFML.Graphics;
using SFML.System;

namespace deadlauncher;

public class InstallMenu : Menu
{
    private readonly UIHost host;
    private readonly RenderWindow window;

    public Action<string> ProgressCallback => TrackProgress;

    private RectangleShape progressBar;
    private RectangleShape textBackground;
    private Text progressBarText;

    public InstallMenu(UIHost host)
    {
        window = Application.Launcher.Window.RenderWindow;
        this.host = host;
    }

    public override AUIElement GetRoot()
    {
        progressBar = new RectangleShape();
        
        progressBar.Position = new Vector2f(60, 240);
        progressBar.Size     = new Vector2f(1, 40);
        progressBar.FillColor = host.Style.NormalButton.TopColor;

        Font font = new("C:\\Users\\destructive_crab\\dev\\buisnes\\OKNO\\deadlauncher\\deadlauncher\\assets\\Main.ttf");
        
        progressBarText = new Text();
        progressBarText.Font = font;
        progressBarText.CharacterSize = 32;
        progressBarText.Position = new Vector2f(280, 240);
        progressBarText.FillColor = host.Style.NormalButton.TopColor;
        progressBarText.DisplayedString = "XXX";

        textBackground = new RectangleShape();
        textBackground.FillColor = UIStyle.RectDefault;
        textBackground.Position = new(progressBarText.Position.X-4, 242);
        textBackground.Size = new(progressBarText.GetGlobalBounds().Size.X+4, 36);
        
        return new StackBox(host, [new UILabel(host, "installing!!!!   installing!!!!")]);
    }

    private int progress;
    private int barMax = 480;
    private void TrackProgress(string obj)
    {
        if (Int32.TryParse(obj, out int value))
        {
            progress = value;
        }
        else if(Single.TryParse(obj, out Single floatValue))
        {
            progress = (int)floatValue;
        }

        if (progress == 100)
        {
            Switch();
        }
    }

    private async void Switch()
    {
        progressBar.FillColor = UIStyle.ButtonBottom;
        textBackground.FillColor = UIStyle.ButtonTop;
        progressBarText.FillColor = UIStyle.RectDefault;
        await Task.Delay(1000);
        Application.Launcher.Window.OpenHomeMenu();
    }

    public override void Update(RenderWindow window)
    {
        progressBar.Size = new Vector2f(progress/100f*barMax, progressBar.Size.Y);

        string text = progress.ToString();
        if (text.Length == 1) text = $"  {text}";
        if (text.Length == 2) text = $" {text}";
        
        progressBarText.DisplayedString = text;
        
        window.Draw(progressBar);
        window.Draw(textBackground);
        window.Draw(progressBarText);
    }
}