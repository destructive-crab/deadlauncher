using deadlauncher.Other.UI;
using deUI;
using SFML.Graphics;

namespace deadlauncher;

public class ChangelogMenu : Menu
{
    private readonly string versionID;
    private readonly UIHost host;

    private UITextBox textBox;

    public ChangelogMenu(UIHost host, string versionID)
    {
        this.host = host;
        this.versionID = versionID;
    }

    public override bool HasBackButton() => true;

    public override AUIElement GetRoot(FloatRect rect)
    {
        string text = "... wait... wait... wait...";

        textBox = new UITextBox(host, text);
        
        Anchor anchorBack = new(new FloatRect(0, 10, 0, 40), new FloatRect(0, 1, 1, 0));
        AnchorBox backButtonAnchorBox = new AnchorBox(host);
        backButtonAnchorBox.SetInheritRect(true);
        backButtonAnchorBox.AddChild(anchorBack, new UIButton(host, "\u2190", BackButton));

        UIOutlineBox root = new UIOutlineBox(host, 
                                new StackBox(host, 
                                    [
                                        new ScrollBox(host, textBox).SetInheritRect(true), 
                                        backButtonAnchorBox         
                                    ]).SetInheritRect(true));
        root.SetInheritRect(true);

        DisplayChangelog();
        
        return root;
    }

    private async void DisplayChangelog()
    {
        if (Application.Launcher.Model.IsVersionValid(versionID))
        {
            var changelog = await Application.Launcher.Model.Changelog(versionID);
            if (changelog != null)
            {
                textBox.Text = changelog;
            }
            else
            {
                textBox.Text = "No changelog added to this version";
            }
        }
    }

    private void BackButton()
    {
        Application.Launcher.Window.OpenHomeMenu();
    }
}