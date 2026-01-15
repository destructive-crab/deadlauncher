using deadlauncher.Other.UI;
using leditor.UI;
using SFML.Graphics;
using SFML.System;

namespace deadlauncher;

public class VersionMenu : Menu
{
    private UIHost host;
    private Dictionary<string, SingleBox> actionButtonPlaces = new();

    private const int VERSION_BUTTON_WIDTH = 300;
    private const int VERSION_CONTEXT_ACTIONS_WIDTH = 300;

    public VersionMenu(UIHost host)
    {
        this.host = host;
    }

    public override AUIElement GetRoot(FloatRect rect)
    {
        actionButtonPlaces.Clear();
        
        RenderWindow window = Application.Launcher.Window.RenderWindow;
        Application.Launcher.Model.RunningLineText = Application.Launcher.Model.SelectedVersionID;
        
        UITabBox uiTabs = new UITabBox(host,
            new KeyValuePair<AUIElement, string>(BuildOfficialTab(), "Official"),
            new KeyValuePair<AUIElement, string>(BuildModsTab(),     "Mods"));
        
        Anchor anchorBack = new(new FloatRect(0, 10, 0, 40), new FloatRect(0, 1, 1, 0));
        AnchorBox backButton = new AnchorBox(host).AddChild(anchorBack, new UIButton(host, "\u2190", BackButton));
        
        StackBox root = new StackBox(host, [uiTabs, backButton]);
        return root;
    }

    private void InstallVersion(string id) => Application.Launcher.Window.OpenInstallMenu(id);

    private void OpenFolder(string id) { Application.Launcher.FileManager.OpenFolderInExplorer(Application.Launcher.Model.ExecutableFolder(id)); }

    private void VersionSelectButton(string id) => Application.Launcher.Model.SetVersion(id);

    private string GetButtonNameByVersionID(string id)
    {
        //slice
        var sliced = id.Split("_");
        
        //
        string result = "";

        for (var i = 0; i < sliced.Length; i++)
        {
            string s = sliced[i];
            
            if (i != 0)
            {
                result += " ";
            }

            s = s[0].ToString().ToUpper() + s.Substring(1);

            result += s;
        }

        return result;
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

    private AUIElement BuildVersionsList(string[] includeOnly, string[] excludeOnly)
    {
        UISelectionList versionList = new(host);
        AxisBox actionButtonsList   = new(host, UIAxis.Vertical);
        
        foreach (string id in Application.Launcher.Model.Available)
        {
            bool valid = true;
            {
                foreach (string include in includeOnly)
                {
                    valid = id.Contains(include);
                    if(!valid) break;
                }
                foreach (string exclude in excludeOnly)
                {
                    valid = !id.Contains(exclude);
                    if(!valid) break;
                }    
            }
            if(!valid) continue;
            
            SingleBox actionButtonsPlace = new(host);
            
            versionList.AddChild(new UIOption(host, GetButtonNameByVersionID(id), new Vector2f(100, 45), () => VersionSelectButton(id), Application.Launcher.Model.IsSelected(id)));

            actionButtonsList.AddChild(actionButtonsPlace);
            actionButtonsPlace.Child = BuildActionButtons(id);
            actionButtonPlaces.Add(id, actionButtonsPlace);
        }

        return new ScrollBox(host, new AxisBox(host, UIAxis.Horizontal, true, versionList, actionButtonsList));
    }

    private AUIElement BuildVersionsListIncludeOnly(params string[] includeOnly) => BuildVersionsList(includeOnly, []);
    private AUIElement BuildVersionsListExcludeOnly(params string[] excludeOnly) => BuildVersionsList([], excludeOnly);

    private AUIElement BuildOfficialTab() => BuildVersionsListExcludeOnly(LauncherModel.MODE_POSTFIX);
    private AUIElement BuildModsTab() => BuildVersionsListIncludeOnly(LauncherModel.MODE_POSTFIX);


    public AxisBox BuildActionButtons(string id)
    {
        AxisBox actionButtonsLine = new AxisBox(host, UIAxis.Horizontal, true);

        int segmentWidth = VERSION_CONTEXT_ACTIONS_WIDTH;
        
        if (Application.Launcher.Model.IsInstalled(id))
        {
            actionButtonsLine.AddChild(new UIButton(host, "Folder", new Vector2f(segmentWidth/2f - host.Style.AxisBoxSpace/2f, 45), () => OpenFolder(id)));
            actionButtonsLine.AddChild(new UIButton(host, "Delete", new Vector2f(segmentWidth/2f - host.Style.AxisBoxSpace/2f, 45), () => DeleteButton(id)));
        }
        else
        {
            actionButtonsLine.AddChild(new UIButton(host, "Install", new Vector2f(segmentWidth, 45), () => InstallVersion(id)));               
        }

        return actionButtonsLine;
    }
}