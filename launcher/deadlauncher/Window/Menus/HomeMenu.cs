using deUI;
using SFML.Graphics;

namespace deadlauncher;

public sealed class HomeMenu : Menu
{
    private UIHost host;

    public HomeMenu(UIHost host)
    {
        this.host = host;
    }

    public override bool HasBackButton() => false;

    public override AUIElement GetRoot(FloatRect rect)
    {
        IUIFactory f = host.Factory;
        
        RenderWindow window = Application.Launcher.Window.RenderWindow;
        UISocketBox firstButtonPlace = new UISocketBox(host);
        
        AnchorBox anchorBox = new AnchorBox(host);
            
        UIButton versionButton   = f.New<UIButton>().WithText("version! version!").OnClick(VersionButton);
        UIButton changelogButton = f.New<UIButton>().WithText("changelog!"       ).OnClick(ChangelogButton);
        UIButton creditsButton   = f.New<UIButton>().WithText("credits! credits!").OnClick(CreditsButton);

        Anchor bottomAnchor = new Anchor(new FloatRect(0, -75, 0, 0), new FloatRect(0, 0.5f, 1, 0));
            
        anchorBox.WithChild(bottomAnchor,
            f.New<AxisBox>().WithAxis(UIAxis.Vertical).WithChildren(                
                f.New<AxisBox>().WithAxis(UIAxis.Horizontal).WithChildren(firstButtonPlace),
                f.New<AxisBox>().WithAxis(UIAxis.Horizontal).WithChildren(versionButton),
                f.New<AxisBox>().WithAxis(UIAxis.Horizontal).WithChildren(changelogButton),
                f.New<AxisBox>().WithAxis(UIAxis.Horizontal).WithChildren(creditsButton)))
            .SetInheritRect(true);
        
        if (Application.Launcher.Model.IsInstalled(Application.Launcher.Model.SelectedVersionID))
        {
            firstButtonPlace.WithChild(f.New<UIButton>().WithText("play! play! play!").OnClick(LaunchSelectedVersion));
        }
        else
        {
            firstButtonPlace.WithChild(f.New<UIButton>().WithText("install! install!").OnClick(InstallSelectedVersion));
        }
        
        Application.Launcher.Model.RunningLineText = Application.Launcher.Model.SelectedVersionID;

        return anchorBox;
    }

    private void CreditsButton() => Application.Launcher.Window.OpenCreditsMenu();
    private void VersionButton() => Application.Launcher.Window.OpenVersionsMenu();
    private void ChangelogButton() => Application.Launcher.Window.OpenChangelogMenu();
    private void InstallSelectedVersion() => Application.Launcher.Window.OpenInstallMenu(Application.Launcher.Model.SelectedVersionID);
    private void LaunchSelectedVersion() => Application.Launcher.Runner.RunSelectedVersion();
}