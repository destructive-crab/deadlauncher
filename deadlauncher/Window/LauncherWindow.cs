using System.Diagnostics;
using leditor.UI;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace deadlauncher;

public class LauncherWindow
{
    public RenderWindow RenderWindow { get; private set; }
    private UIHost host;

    private List<Sprite> label = new List<Sprite>();
    private List<Text> versions = new();
    private List<Text> versionsBack = new();

    private Drawable[] background;
    
    private RunningLine<Sprite> deadaysLine = new();
    
    private RunningLine<Text> versionLine = new();
    private RunningLine<Text> versionLineShadow = new();

    private Menu currentMenu;
    private string currentRunningLineText;

    public async Task Prepare()
    {
        RenderWindow = new RenderWindow(new VideoMode(600, 500), "Deadays Launcher", Styles.Titlebar | Styles.Close);
        
        RenderWindow.Closed += RenderWindowOnClosed;

        RectangleShape menuOutline = new RectangleShape();
        menuOutline.Size     = new Vector2f(RenderWindow.Size.X - 80, RenderWindow.Size.Y - 180);
        menuOutline.Position = new Vector2f(40, 120);

        menuOutline.FillColor = new UIStyle().ScrollerColor;
        
        RectangleShape menuBackground = new RectangleShape();
        menuBackground.Size     = new Vector2f(RenderWindow.Size.X - 88, RenderWindow.Size.Y - 188);
        menuBackground.Position = new Vector2f(44, 124);

        menuBackground.FillColor = UIStyle.RectDefault;
        
        background = new[] { menuOutline, menuBackground };
        
        Texture labelTex = new("C:\\Users\\destructive_crab\\dev\\buisnes\\OKNO\\deadlauncher\\deadlauncher\\assets\\dd_label.png");
        Font font = new("C:\\Users\\destructive_crab\\dev\\buisnes\\OKNO\\deadlauncher\\deadlauncher\\assets\\Main.ttf");
        
        Text version = new Text("v1.5 ", font, labelTex.Size.Y/4);
        version.FillColor = new Color(0xbbdde1FF);
        
        Text versionShadow = new Text("v1.5 ", font, labelTex.Size.Y/4);
        versionShadow.FillColor = new Color(0xdc82bbFF); 
        
        host = new UIHost(new UIStyle(), new Vector2f(RenderWindow.Size.X, RenderWindow.Size.Y));

        deadaysLine.Build(new Sprite(labelTex), 600, 0, 10, (int)labelTex.Size.X, (el) => new Sprite(el));
        
        versionLine.Build(version, 600, 0, 50, (int)version.GetGlobalBounds().Size.X, (el) => new(el));
        versionLineShadow.Build(versionShadow, 600, 4, 53, (int)version.GetGlobalBounds().Size.X, (el) => new(el));
        
        RenderWindow.Resized += RenderWindowOnResized;
        Application.Launcher.Model.OnVersionSelected += ModelOnOnVersionSelected;
        
        OpenHomeMenu();
    }

    private void ModelOnOnVersionSelected(string obj)
    {
        SetLineText(obj);
    }

    private void RenderWindowOnResized(object? sender, SizeEventArgs e)
    {
        host.SetSize(new(e.Width, e.Height));
    }

    private void RenderWindowOnClosed(object? sender, EventArgs e)
    {
        Shutdown();
    }

    public void Loop()
    {
        while (RenderWindow.IsOpen)
        {
            RenderWindow.DispatchEvents();
            
            deadaysLine.MovePositions(0.05f);

            if (Application.Launcher.Model.RunningLineText != currentRunningLineText)
            {
                currentRunningLineText = Application.Launcher.Model.RunningLineText;
                SetLineText(currentRunningLineText);
            }
            
            versionLine.MovePositions(0.03f);
            versionLineShadow.MovePositions(0.03f);
            
            host.Update(RenderWindow);
            
            RenderWindow.Clear(new Color(0x00363cFF));
            {
                deadaysLine.Draw(RenderWindow);
                versionLine.Draw(RenderWindow);
                versionLineShadow.Draw(RenderWindow);
                
                foreach (Drawable drawable in background)
                {
                    RenderWindow.Draw(drawable);
                }
    
                currentMenu?.Update(RenderWindow);
                host.Draw(RenderWindow);
            }            
            RenderWindow.Display();
        }
    }

    public void Shutdown()
    {
        RenderWindow.Close();
    }

    private void SetLineText(string text)
    {
        text = $" {text} ";
        var e = versionLine.BaseElementShared;
        e.DisplayedString = text;
        
        versionLine.SetElementWidth((int)e.GetGlobalBounds().Width);
        versionLine.Foreach((e) => e.DisplayedString = text);
        
        versionLineShadow.SetElementWidth((int)e.GetGlobalBounds().Width);
        versionLineShadow.Foreach((e) => e.DisplayedString = text);
    }

    public void OpenHomeMenu()
    {
        currentMenu = new HomeMenu(host);
        host.SetRoot(currentMenu.GetRoot());
    }

    public void OpenInstallMenu(string id)
    {
        var menu = new InstallMenu(host);
        SetRoot(menu.GetRoot());
        Application.Launcher.Downloader.DownloadVersion(id, menu.ProgressCallback);
        currentMenu = menu;
    }

    public void OpenVersionsMenu()
    {
        currentMenu = new VersionMenu(host);
        SetRoot(currentMenu.GetRoot());
    }

    private void SetRoot(AUIElement menu)
    {
        Anchor anchor = new Anchor(
            new FloatRect(50, 124, RenderWindow.Size.X - 100, RenderWindow.Size.Y - 200),
            new FloatRect(0, 0, 0, 0));
        
        AnchorBox anchorBox = new AnchorBox(host);
        anchorBox.AddChild(anchor, menu);
        
        host.SetRoot(anchorBox);
    }
}