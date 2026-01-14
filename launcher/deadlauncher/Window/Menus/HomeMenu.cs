using deadlauncher.Other.UI;
using leditor.UI;
using SFML.Graphics;
using SFML.System;

namespace deadlauncher;

public sealed class HomeMenu : Menu
{
    private UIHost host;

    public HomeMenu(UIHost host)
    {
        this.host = host;
    }

    public override AUIElement GetRoot(FloatRect rect)
    {
        RenderWindow window = Application.Launcher.Window.RenderWindow;
        SingleBox firstButtonPlace = new SingleBox(host);
        
        AnchorBox anchor = new AnchorBox(host);
            
        UIButton versionButton = new UIButton(host, "version! version!", new Vector2f(320, 50), VersionButton);
        UIButton changelogButton = new UIButton(host, "changelog!", new Vector2f(320, 50), ChangelogButton);
        UIButton creditsButton = new UIButton(host, "credits! credits!", new Vector2f(320, 50), CreditsButton);

        Anchor bottomAnchor = new Anchor(new FloatRect(-160, 0, window.Size.X, 0), new FloatRect(0.5f, 0.4f, 0, 0));
            
        anchor.AddChild(bottomAnchor,
            new AxisBox(host, UIAxis.Vertical,
                new AxisBox(host, UIAxis.Horizontal, firstButtonPlace, new UILabel(host)),
                new AxisBox(host, UIAxis.Horizontal, versionButton, new UILabel(host)),
                new AxisBox(host, UIAxis.Horizontal, changelogButton, new UILabel(host)),
                new AxisBox(host, UIAxis.Horizontal, creditsButton, new UILabel(host))));
        
        if (Application.Launcher.Model.IsInstalled(Application.Launcher.Model.SelectedVersionID))
        {
            firstButtonPlace.Child = new UIButton(host, "play! play! play!", new Vector2f(320, 50), LaunchSelectedVersion);
        }
        else
        {
            firstButtonPlace.Child = new UIButton(host, "install! install!", new Vector2f(320, 50), InstallSelectedVersion);
        }
        
        Application.Launcher.Model.RunningLineText = Application.Launcher.Model.SelectedVersionID;
        return new UIOutlineBox(host, new StackBox(host, [anchor]));
    }

    private void CreditsButton() => Application.Launcher.Window.OpenCreditsMenu();
    private void VersionButton() => Application.Launcher.Window.OpenVersionsMenu();
    private void ChangelogButton() => Application.Launcher.Window.OpenChangelogMenu();
    private void InstallSelectedVersion() => Application.Launcher.Window.OpenInstallMenu(Application.Launcher.Model.SelectedVersionID);
    private void LaunchSelectedVersion() => Application.Launcher.Runner.RunSelectedVersion();
}