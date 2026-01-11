using deadlauncher.Other.UI;
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
    private void OpenFolder(string id) { Application.Launcher.FileManager.OpenFolderInExplorer(Application.Launcher.Model.ExecutableFolder(id)); }
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
        
        Application.Launcher.Model.RunningLineText = Application.Launcher.Model.SelectedVersionID;
        var tabs = new TabBox(host,
            new KeyValuePair<AUIElement, string>(
                new ScrollBox(host, new AxisBox(host, UIAxis.Horizontal, versionList, actionButtonsList)),
                "Official"),
            new KeyValuePair<AUIElement, string>(
                new AxisBox(host, UIAxis.Vertical, new UILabel(host, "sdfkjjskfjskfk"),
                    new UILabel(host, "sdfkjjskfjskfk"), new UILabel(host, "sdfkjjskfjskfk")),
                "Modes"),
            new KeyValuePair<AUIElement, string>(
                new AxisBox(host, UIAxis.Vertical, new UILabel(host, "sdfkjjskfjskfk"),
                    new UILabel(host, "sdfkjjskfjskfk"), new UILabel(host, "sdfkjjskfjskfk")),
                "Modes 2"));
        
        Anchor anchorBack = new(new FloatRect(0, 10, 0, 40), new FloatRect(0, 1, 1, 0));
        
        var backButton = new AnchorBox(host).AddChild(anchorBack, new UIButton(host, "\u2190", BackButton));
        
        var root = new StackBox(host, [tabs, backButton]);
        return root;
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
    
    public override void Update(RenderWindow window) { }
}