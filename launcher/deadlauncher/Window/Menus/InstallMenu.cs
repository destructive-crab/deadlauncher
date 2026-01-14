using deadlauncher.Other.UI;
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

    public override AUIElement GetRoot(FloatRect rect)
    {
        progressBar = new RectangleShape();
        
        progressBar.Position = rect.Position + new Vector2f(30, rect.Size.Y/2-20);
        progressBar.Size     = new Vector2f(1, 40);
        progressBar.FillColor = host.Style.NormalButton.TopColor;

        Font font = new(ResourcesHandler.Load("Line.ttf"));
        
        progressBarText = new Text();
        progressBarText.Font = font;
        progressBarText.CharacterSize = 32;
        progressBarText.DisplayedString = "XXX";
        progressBarText.Position = new Vector2f(rect.Position.X + rect.Size.X/2, progressBar.Position.Y + progressBar.Size.Y/2 - 10) - progressBarText.GetGlobalBounds().Size/2;
        progressBarText.FillColor = host.Style.NormalButton.TopColor;

        textBackground = new RectangleShape();
        textBackground.FillColor = UIStyle.SecondBackgroundColor;
        textBackground.Size      = new Vector2f(progressBarText.GetGlobalBounds().Size.X + 6, progressBar.GetGlobalBounds().Size.Y - 6);
        textBackground.Position  = new Vector2f(progressBar.Position.X + GetBarMaxWidth(rect)/2 - textBackground.Size.X/2 - 3,progressBar.Position.Y+3);

        return new UIOutlineBox(host, new StackBox(host, [new UITextBox(host, "installing!!!!   installing!!!!", true)]));
    }

    private int progress;

    private int GetBarMaxWidth(FloatRect menuRect)
    {
        return (int)(menuRect.Size.X - 60);
    }
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

        if (progress == 101)
        {
            progress = 100;
            Switch();
        }
    }

    private async void Switch()
    {
        progressBar.FillColor = UIStyle.ButtonBottom;
        textBackground.FillColor = UIStyle.ButtonTop;
        progressBarText.FillColor = UIStyle.RectDefault;
        
        await Task.Delay(1000);
        Application.Launcher.Window.BackToPrevious();
    }

    public override void Update(RenderWindow window, FloatRect menuRect)
    {
        progressBar.Size = new Vector2f(progress/100f*GetBarMaxWidth(menuRect), progressBar.Size.Y);

        string text = progress.ToString();
        if (text.Length == 1) text = $"  {text}";
        if (text.Length == 2) text = $" {text}";
        
        progressBarText.DisplayedString = text;
        Application.Launcher.Model.RunningLineText = text+" %";
        
        window.Draw(progressBar);
        window.Draw(textBackground);
        window.Draw(progressBarText);
    }
}