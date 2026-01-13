using leditor.UI;
using SFML.Graphics;

namespace deadlauncher;

public class ChangelogMenu : Menu
{
    private readonly string versionID;
    private readonly UIHost host;

    public ChangelogMenu(UIHost host, string versionID)
    {
        this.host = host;
        this.versionID = versionID;
    }

    public override AUIElement GetRoot()
    {
        string text = "";
        if (Application.Launcher.Model.IsVersionValid(versionID))
        {
            var changelog = Application.Launcher.Model.Changelog(versionID);
            if (changelog != null) text = changelog;
        }

        return new ScrollBox(host, new UITextBox(host, text));
    }

    public override void Update(RenderWindow window)
    {
    }
}