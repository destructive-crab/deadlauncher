using leditor.UI;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace deadlauncher;

public class LauncherWindow
{
    public RenderWindow RenderWindow { get; private set; }
    private UIHost host;

    private Menu previousMenu;
    private Menu currentMenu;

    private WindowBackgroundGraphics backgroundGraphics;

    //CONSTS
    private const string WindowLabel       = "Deadays Launcher";
    private const int    WindowWidth       = 600;
    private const int    WindowHeight      = 500;
    
    public async Task Prepare()
    {
        RenderWindow = new RenderWindow(new VideoMode(WindowWidth, WindowHeight), WindowLabel, Styles.Titlebar | Styles.Close);
        
        host = new UIHost(new UIStyle(), new Vector2f(RenderWindow.Size.X, RenderWindow.Size.Y));

        backgroundGraphics = new WindowBackgroundGraphics(WindowWidth, WindowHeight);
        
        RenderWindow.Closed  += RenderWindowOnClosed;
        RenderWindow.Resized += RenderWindowOnResized;
        
        Application.Launcher.Model.OnVersionSelected += OnVersionSelected;
        
        OpenHomeMenu();
    }

    private void OnVersionSelected(string obj)
    {
        Application.Launcher.Model.RunningLineText = obj;
    }

    private void RenderWindowOnResized(object? sender, SizeEventArgs e) => host.SetSize(new Vector2f(e.Width, e.Height));
    private void RenderWindowOnClosed(object? sender, EventArgs e) => Shutdown();

    public void Loop()
    {
        while (RenderWindow.IsOpen )
        {
            RenderWindow.DispatchEvents();
            if(!RenderWindow.HasFocus())
            {
                continue;
            }
            
            host.Update(RenderWindow);
            
            RenderWindow.Clear(new Color(0x00363cFF));
            {
                backgroundGraphics.Draw(RenderWindow);
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

    public void OpenHomeMenu()
    {
        previousMenu = currentMenu;
        currentMenu = new HomeMenu(host);
        host.SetRoot(currentMenu.GetRoot());
    }

    public void OpenInstallMenu(string id)
    {
        InstallMenu menu = new InstallMenu(host);
        SwitchTo(menu);
        Application.Launcher.Downloader.DownloadVersion(id, menu.ProgressCallback);
    }

    public void OpenVersionsMenu()
    {
        SwitchTo(new VersionMenu(host));
    }

    public void OpenCreditsMenu()
    {
        SwitchTo(new CreditsMenu(host));
    }

    private void SwitchTo(Menu menu)
    {
        previousMenu = currentMenu;
        currentMenu = menu;
        SetRoot(currentMenu.GetRoot());
    }

    public void BackToPrevious()
    {
        SwitchTo(previousMenu);
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

    class WindowBackgroundGraphics
    {
        private string currentRunningLineText;
        private Drawable[] background;
        
        private readonly RunningLine<Sprite> deadaysLine = new();
        
        private readonly RunningLine<Text> versionLine = new();
        private readonly RunningLine<Text> versionLineShadow = new();
        
        //consts
        private const string FontAssetName     = "Line.ttf";
        private const string LabelAssetName    = "dd_label.png";
         
        private const float  ddLineXDelta      = 0.07f;
        private const float  versionLineXDelta = 0.03f;
        private const float  ddLogoScale       = 1.65f;
        private const int    versionLineY      = 70;
        
        public WindowBackgroundGraphics(int windowWidth, int windowHeight)
        {
            //background
            RectangleShape menuOutline = new RectangleShape();
            menuOutline.Size     = new Vector2f(windowWidth - 80, windowHeight - 180);
            menuOutline.Position = new Vector2f(40, 120);
    
            menuOutline.FillColor = new UIStyle().ScrollerColor;
            
            RectangleShape menuBackground = new RectangleShape();
            menuBackground.Size     = new Vector2f(windowWidth - 88, windowHeight - 188);
            menuBackground.Position = new Vector2f(44, 124);
    
            menuBackground.FillColor = UIStyle.RectDefault;
            
            background = new Drawable[] { menuOutline, menuBackground };
           
            //deadays line
            Texture labelTex = new(ResourcesHandler.Load(LabelAssetName));
            
            Sprite sprite = new Sprite(labelTex);
            sprite.Scale = new Vector2f(1.65f, 1.65f);
            deadaysLine.Build(sprite, 600, 0, 10, (int)sprite.GetGlobalBounds().Size.X, (el) => new Sprite(el));
            
            //version line
            Font    font     = new(ResourcesHandler.Load(FontAssetName));
         
            Text version       = new Text("v1.5 ", font, labelTex.Size.Y/4);
            Text versionShadow = new Text(version);
            
            version      .FillColor = new Color(0xbbdde1FF);
            versionShadow.FillColor = new Color(0xdc82bbFF); 
     
            versionLine.Build(version, 600, 0, 70, (int)version.GetGlobalBounds().Size.X, (el) => new(el));
            versionLineShadow.Build(versionShadow, 600, 2, 73, (int)version.GetGlobalBounds().Size.X, (el) => new(el));
        }
    
        public void Draw(RenderTarget target)
        {
            deadaysLine.MovePositions(ddLineXDelta);

            if (Application.Launcher.Model.RunningLineText != currentRunningLineText)
            {
                SetLineText(Application.Launcher.Model.RunningLineText);
            }
            
            versionLine      .MovePositions(versionLineXDelta);
            versionLineShadow.MovePositions(versionLineXDelta);
            
            foreach (Drawable drawable in background)
            {
                target.Draw(drawable);
            }
            
            deadaysLine.Draw(target);
            versionLine.Draw(target);
            versionLineShadow.Draw(target);
        }

        public void SetLineText(string text)
        {
            text = $" {text} ";
            Text e = versionLine.BaseElementShared;
            e.DisplayedString = text;
            
            versionLine.SetElementWidth((int)e.GetGlobalBounds().Width);
            versionLine.Foreach((e) => e.DisplayedString = text);
            
            versionLineShadow.SetElementWidth((int)e.GetGlobalBounds().Width);
            versionLineShadow.Foreach((e) => e.DisplayedString = text);
            
            currentRunningLineText = text;
        }
    }
}