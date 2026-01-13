using leditor.UI;
using SFML.Graphics;
using SFML.System;

namespace deadlauncher;

public sealed class HomeMenu : Menu
{
    private UIHost uiHost;

    public HomeMenu(UIHost host)
    {
        uiHost = host;
    }

    public override AUIElement GetRoot()
    {
        RenderWindow window = Application.Launcher.Window.RenderWindow;
        SingleBox firstButtonPlace = new SingleBox(uiHost);
        
        AnchorBox anchor = new AnchorBox(uiHost);
            
        UIButton versionButton = new UIButton(uiHost, "version! version!", new Vector2f(320, 50), VersionButton);
        UIButton changelogButton = new UIButton(uiHost, "changelog!", new Vector2f(320, 50), ChangelogButton);
        UIButton creditsButton = new UIButton(uiHost, "credits! credits!", new Vector2f(320, 50), CreditsButton);

        Anchor bottomAnchor = new Anchor(new FloatRect(-160, 0, window.Size.X, 0), new FloatRect(0.5f, 0.4f, 0, 0));
            
        anchor.AddChild(bottomAnchor,
            new AxisBox(uiHost, UIAxis.Vertical,
                new AxisBox(uiHost, UIAxis.Horizontal, firstButtonPlace, new UILabel(uiHost)),
                new AxisBox(uiHost, UIAxis.Horizontal, versionButton, new UILabel(uiHost)),
                new AxisBox(uiHost, UIAxis.Horizontal, changelogButton, new UILabel(uiHost)),
                new AxisBox(uiHost, UIAxis.Horizontal, creditsButton, new UILabel(uiHost))));
        
        if (Application.Launcher.Model.IsInstalled(Application.Launcher.Model.SelectedVersionID))
        {
            firstButtonPlace.Child = new UIButton(uiHost, "play! play! play!", new Vector2f(320, 50), LaunchSelectedVersion);
        }
        else
        {
            firstButtonPlace.Child = new UIButton(uiHost, "install! install!", new Vector2f(320, 50), InstallSelectedVersion);
        }
        
        Application.Launcher.Model.RunningLineText = Application.Launcher.Model.SelectedVersionID;
        return new StackBox(uiHost, [anchor]);
    }

    public override void Update(RenderWindow window) { }

    private void CreditsButton() => Application.Launcher.Window.OpenCreditsMenu();
    private void VersionButton() => Application.Launcher.Window.OpenVersionsMenu();
    private void ChangelogButton() => Application.Launcher.Window.OpenChangelogMenu();
    private void InstallSelectedVersion() => Application.Launcher.Window.OpenInstallMenu(Application.Launcher.Model.SelectedVersionID);
    private void LaunchSelectedVersion() => Application.Launcher.Runner.RunSelectedVersion();
}