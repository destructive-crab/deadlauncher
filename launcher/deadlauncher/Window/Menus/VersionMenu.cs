using deadlauncher.Other.UI;
using deUI;
using SFML.Graphics;
using SFML.System;

namespace deadlauncher;

public class VersionMenu : Menu
{
    private UIHost host;
    private Dictionary<string, UISocketBox> actionButtonPlaces = new();

    private const int VERSION_BUTTON_WIDTH = 300;
    private const int VERSION_CONTEXT_ACTIONS_WIDTH = 300;

    public VersionMenu(UIHost host)
    {
        this.host = host;
    }

    public override bool HasBackButton() => true;

    public override AUIElement GetRoot(FloatRect rect)
    {
        actionButtonPlaces.Clear();
        
        RenderWindow window = Application.Launcher.Window.RenderWindow;
        Application.Launcher.Model.RunningLineText = Application.Launcher.Model.SelectedVersionID;
        
        UITabBox uiTabs = host.New<UITabBox>().WithTabs(
            new KeyValuePair<AUIElement, string>(BuildOfficialTab(), "Official"),
            new KeyValuePair<AUIElement, string>(BuildModsTab(),     "Mods"));
        
        return uiTabs;
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
        actionButtonPlaces[id].WithChild(BuildActionButtons(id));
    }

    private AUIElement BuildVersionsList(string[] includeOnly, string[] excludeOnly)
    {
        UISelectionList versionList       = host.New<UISelectionList>();
        AxisBox         actionButtonsList = host.New<AxisBox>().WithAxis(UIAxis.Vertical);
        
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
            
            UISocketBox actionButtonsPlace = host.New<UISocketBox>();
            
            versionList.AddChild(host.New<UIOption>().SetState(Application.Launcher.Model.IsSelected(id)).WithText(GetButtonNameByVersionID(id)).OnClick(() => VersionSelectButton(id)) as UIOption);

            actionButtonsList.AddChild(actionButtonsPlace);
            actionButtonsPlace.WithChild(BuildActionButtons(id));
            actionButtonPlaces.Add(id, actionButtonsPlace);
        }

        return host.New<ScrollBox>().WithChild(host.New<AxisBox>().WithAxis(UIAxis.Horizontal).FitRect(true).WithChildren(versionList, actionButtonsList));
    }

    private AUIElement BuildVersionsListIncludeOnly(params string[] includeOnly) => BuildVersionsList(includeOnly, []);
    private AUIElement BuildVersionsListExcludeOnly(params string[] excludeOnly) => BuildVersionsList([], excludeOnly);

    private AUIElement BuildOfficialTab() => BuildVersionsListExcludeOnly(LauncherModel.MODE_POSTFIX);
    private AUIElement BuildModsTab()     => BuildVersionsListIncludeOnly(LauncherModel.MODE_POSTFIX);


    public AxisBox BuildActionButtons(string id)
    {
        AxisBox actionButtonsLine = host.New<AxisBox>().WithAxis(UIAxis.Horizontal).FitRect(true);

        int segmentWidth = VERSION_CONTEXT_ACTIONS_WIDTH;
        
        if (Application.Launcher.Model.IsInstalled(id))
        {
            actionButtonsLine.AddChild(host.New<UIButton>().WithText("Folder").OnClick(() => OpenFolder(id)));
            actionButtonsLine.AddChild(host.New<UIButton>().WithText("Delete").OnClick(() => DeleteButton(id)));
        }
        else
        {
            actionButtonsLine.AddChild(host.New<UIButton>().WithText("Install").OnClick(() => InstallVersion(id)));               
        }

        return actionButtonsLine;
    }
}