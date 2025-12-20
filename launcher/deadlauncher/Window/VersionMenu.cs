using leditor.UI;
using SFML.Graphics;
using SFML.System;

namespace deadlauncher;

public class VersionMenu : Menu
{
    private UIHost host;
    private Dictionary<string, SingleBox> actionButtonPlaces = new();
    
    public VersionMenu(UIHost host)
    {
        this.host = host;
    }

    private void InstallVersion(string id)
    {
        Application.Launcher.Window.OpenInstallMenu(id);
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
        Application.Launcher.Downloader.DeleteVersion(id);
        actionButtonPlaces[id].Child = BuildActionButtons(id);
    }

    public override AUIElement GetRoot()
    {
        actionButtonPlaces.Clear();
        RenderWindow window = Application.Launcher.Window.RenderWindow;
        UISelectionList versionList = new(host);
        AxisBox actionButtonsList = new(host, UIAxis.Vertical);
        
        foreach (string id in Application.Launcher.Model.Available)
        {
            SingleBox actionButtonsPlace = new(host);
            
            versionList.AddChild(new UIOption(host, id, new Vector2f(220, 45), () => VersionSelectButton(id), Application.Launcher.Model.IsSelected(id)));

            actionButtonsList.AddChild(actionButtonsPlace);
            actionButtonsPlace.Child = BuildActionButtons(id);
            actionButtonPlaces.Add(id, actionButtonsPlace);
        }
        
        Anchor anchorBack = new(new FloatRect(0, 40, 500, -45), new FloatRect(0,1,0,0));
        
        return new StackBox(host, 
            [new ScrollBox(host, new AxisBox(host, UIAxis.Horizontal, versionList, actionButtonsList)),
             new AnchorBox(host).AddChild(anchorBack, new UIButton(host, "         \t\u2190", new Vector2f(10, 30), BackButton))]);
    }
    
    public AxisBox BuildActionButtons(string id)
    {
        AxisBox actionButtonsLine = new AxisBox(host, UIAxis.Horizontal);
            
        if (Application.Launcher.Model.IsInstalled(id))
        {
            actionButtonsLine.AddChild(new UIButton(host, "Folder", new Vector2f(120, 45), () => OpenFolder(id)));
            actionButtonsLine.AddChild(new UIButton(host, "Delete", new Vector2f(120, 45), () => DeleteButton(id)));
        }
        else
        {
            actionButtonsLine.AddChild(new UIButton(host, "Install", new Vector2f(240, 45), () => InstallVersion(id)));               
        }

        return actionButtonsLine;
    }
    
    public override void Update(RenderWindow window)
    {
    }
}