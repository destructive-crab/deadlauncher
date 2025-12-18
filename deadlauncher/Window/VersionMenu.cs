using leditor.UI;
using SFML.Graphics;
using SFML.System;

namespace deadlauncher;

public class VersionMenu : Menu
{
    private UIHost host;
    
    public VersionMenu(UIHost host)
    {
        this.host = host;
    }

    private void InstallVersion(string id)
    {
        
    }
    private void OpenFolder(string id) { }
    private void VersionSelectButton(string id)
    {
        Application.Launcher.Model.SetVersion(id);
    }
    private void BackButton()
    {
        Application.Launcher.Window.OpenHomeMenu();
    }

    private void DeleteButton(string id)
    {
    }

    public override AUIElement GetRoot()
    {
        RenderWindow window = Application.Launcher.Window.RenderWindow;
        UISelectionList versionList = new(host);
        AxisBox actionButtonsList = new(host, UIAxis.Vertical);
        
        for(int i = 0; i < 2; i++)
        foreach (string id in Application.Launcher.Model.Available)
        {
            string state = "";
            
            if (Application.Launcher.Model.IsInstalled(id)) state = "installed!";
            else                                            state = "not installed!";
            
            versionList.AddChild(new UIOption(host, id, new Vector2f(220, 45), () => VersionSelectButton(id)));

            AxisBox actionButtonsLine = new AxisBox(host, UIAxis.Horizontal);

            if (state == "installed!")
            {
                actionButtonsLine.AddChild(new UIButton(host, "Folder", new Vector2f(120, 45), () => OpenFolder(id)));
                actionButtonsLine.AddChild(new UIButton(host, "Delete", new Vector2f(120, 45), () => DeleteButton(id)));               
            }
            else
            {
                actionButtonsLine.AddChild(new UIButton(host, "Install", new Vector2f(240, 45), () => InstallVersion(id)));               
            }
            actionButtonsList.AddChild(actionButtonsLine);
        }
        
        Anchor anchorBack = new(new FloatRect(0, 40, 500, -45), new FloatRect(0,1,0,0));
        
        AnchorBox anchorBox = new AnchorBox(host);
        
        return new StackBox(host, 
            [new ScrollBox(host, new AxisBox(host, UIAxis.Horizontal, versionList, actionButtonsList)),
             new AnchorBox(host).AddChild(anchorBack, new UIButton(host, "         \t\u2190", new Vector2f(10, 30), BackButton))]);
    }

    public override void Update(RenderWindow window)
    {
    }
}