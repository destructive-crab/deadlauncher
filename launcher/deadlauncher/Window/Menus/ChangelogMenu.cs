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

        textBox = host.New<UITextBox>().WithText(text);
        
        Anchor anchorBack = new(new FloatRect(0, 10, 0, 40), new FloatRect(0, 1, 1, 0));
        
        AnchorBox backButtonAnchorBox = host.New<AnchorBox>();
        
        backButtonAnchorBox.SetInheritRect(true);
        backButtonAnchorBox.WithChild(anchorBack, host.New<UIButton>().WithText("\u2190").OnClick(BackButton));

        UIOutlineBox root = host.New<UIOutlineBox>().WithChild( 
                                host.New<StackBox>().WithChildren( 
                                    [
                                        host.New<ScrollBox>().WithChild(textBox).SetInheritRect(true), 
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