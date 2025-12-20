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
            
        UIButton versionButton = new UIButton(uiHost, " version! version!", new Vector2f(320, 50), VersionButton);
        
        Anchor bottomAnchor = new Anchor(new FloatRect(-160, 0, window.Size.X, 40), new FloatRect(0.5f, 0.5f, 0, 0));
            
        anchor.AddChild(bottomAnchor,
            new AxisBox(uiHost, UIAxis.Vertical,
                new AxisBox(uiHost, UIAxis.Horizontal, firstButtonPlace, new UILabel(uiHost)),
                new AxisBox(uiHost, UIAxis.Horizontal, versionButton, new UILabel(uiHost))));
        
        if (Application.Launcher.Model.IsInstalled(Application.Launcher.Model.SelectedVersionId))
        {
            firstButtonPlace.Child = new UIButton(uiHost, " play! play! play!", new Vector2f(320, 50), LaunchSelectedVersion);
        }
        else
        {
            firstButtonPlace.Child = new UIButton(uiHost, " install! install!", new Vector2f(320, 50), InstallSelectedVersion);
        }
        
        return new StackBox(uiHost, [anchor]);
    }

    public override void Update(RenderWindow window) { }

    private void VersionButton() => Application.Launcher.Window.OpenVersionsMenu();
    private void InstallSelectedVersion() => Application.Launcher.Window.OpenInstallMenu(Application.Launcher.Model.SelectedVersionId);
    private void LaunchSelectedVersion() => Application.Launcher.Runner.RunSelectedVersion();
}