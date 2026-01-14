using deadlauncher.Other.UI;
using leditor.UI;
using SFML.Graphics;
using SFML.System;

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

    public override AUIElement GetRoot(FloatRect rect)
    {
        string text = "";
        if (Application.Launcher.Model.IsVersionValid(versionID))
        {
            var changelog = Application.Launcher.Model.Changelog(versionID);
            if (changelog != null) text = changelog;
        }

        Anchor anchorBack = new(new FloatRect(0, 10, 0, 40), new FloatRect(0, 1, 1, 0));
        AnchorBox backButton = new AnchorBox(host).AddChild(anchorBack, new UIButton(host, "\u2190", BackButton));
        
        var root = new UIOutlineBox(host, new StackBox(host, [new ScrollBox(host, new UITextBox(host, text)), backButton]));
        return root;
    }
    
    private void BackButton()
    {
        Application.Launcher.Window.OpenHomeMenu();
    }
}