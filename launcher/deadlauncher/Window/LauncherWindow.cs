using deadlauncher.Other.UI;
using deUI;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace deadlauncher;

public class LauncherWindow
{
    public RenderWindow RenderWindow { get; private set; }
    public UIHost       UIHost       { get; private set; }

    private StackBox  rootElement;
    
    private UIOutlineBox menuLayer;
    private UISocketBox    returnButton;
    
    private UISocketBox    popupLayer;
    
    private Menu previousMenu;
    private Menu currentMenu;

    private MessageBox? messageBox;
    
    private WindowBackgroundGraphics backgroundGraphics;

    //CONSTS
    public const string WindowLabel  = "Deadays Launcher";
    public const int    WindowWidth  = 800;
    public const int    WindowHeight = 500;

    public readonly FloatRect MenuRect = new(25, 124, WindowWidth - 50, WindowHeight - 200); 
    
    public async Task Prepare()
    {
        backgroundGraphics = new WindowBackgroundGraphics(WindowWidth, WindowHeight);
        
        UIHost = new UIHost(new UIStyle(), new Vector2f(WindowWidth, WindowHeight));
        
        menuLayer  = new UIOutlineBox(UIHost, null, MenuRect);
        popupLayer = new UISocketBox(UIHost);

        popupLayer.SetInheritRect(true);
        
        rootElement = new StackBox(UIHost, 
        [
            new AxisBox(UIHost, UIAxis.Vertical, 
                menuLayer,
                new UISocketBox(UIHost, new UIButton(UIHost, "Back", BackToPrevious))).SetRect(MenuRect),
            
            popupLayer
        ]);
        
        UIHost.SetRoot(rootElement);
        
        Application.Launcher.Model.OnVersionSelected += OnVersionSelected;
    }

    private void OnVersionSelected(string obj)
    {
        Application.Launcher.Model.RunningLineText = obj;
    }

    private void RenderWindowOnResized(object? sender, SizeEventArgs e) => UIHost.SetSize(new Vector2f(e.Width, e.Height));
    private void RenderWindowOnClosed(object? sender, EventArgs e) => Shutdown();

    public void Loop()
    {
        RenderWindow = new RenderWindow(new VideoMode(WindowWidth, WindowHeight), WindowLabel, Styles.Titlebar | Styles.Close);

        RenderWindow.Closed  += RenderWindowOnClosed;
        RenderWindow.Resized += RenderWindowOnResized;
        
        OpenHomeMenu();
        
        while (RenderWindow.IsOpen)
        {
            RenderWindow.DispatchEvents();
            
            if(!RenderWindow.HasFocus())
            {
                continue;
            }
            
            UIHost.Update(RenderWindow);
            
            RenderWindow.Clear(UIStyle.SecondBackgroundColor);
            {
                backgroundGraphics.Draw(RenderWindow);
                currentMenu       ?.Update(RenderWindow, MenuRect);
                UIHost            .Draw(RenderWindow);
            }            
            RenderWindow.Display();
        }
    }

    public void Shutdown()
    {
        RenderWindow.Close();
    }

    public void OpenMessageBox(string message, params Tuple<string, Action>[] buttons)
    {
        return;
        popupLayer.SetChild(new MessageBox(message, buttons));
    }

    public void OpenInstallMenu(string id)
    {
        InstallMenu menu = new InstallMenu(UIHost);
        SwitchTo(menu);
        Application.Launcher.Downloader.DownloadVersion(id, menu.ProgressCallback);
    }

    public void OpenHomeMenu()      => SwitchTo(new HomeMenu(UIHost));
    public void OpenVersionsMenu()  => SwitchTo(new VersionMenu(UIHost));
    public void OpenCreditsMenu()   => SwitchTo(new CreditsMenu(UIHost));
    public void OpenChangelogMenu() => SwitchTo(new ChangelogMenu(UIHost, Application.Launcher.Model.SelectedVersionID));

    private void SwitchTo(Menu menu)
    {
        if (menu == null) return;
        
        previousMenu = currentMenu;
        currentMenu = menu;
        
        SetMenuElement(currentMenu.GetRoot(MenuRect));
    }

    public void BackToPrevious() => SwitchTo(previousMenu);

    private void SetMenuElement(AUIElement menu)
    {
        menuLayer.SetChild(menu);
        
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
            
            background = new Drawable[] { /*menuOutline, menuBackground*/ };
           
            //deadays line
            Texture labelTex = new(ResourcesHandler.Load(LabelAssetName));
            
            Sprite sprite = new Sprite(labelTex);
            sprite.Scale = new Vector2f(1.65f, 1.65f);
            deadaysLine.Build(sprite, WindowWidth, 0, 10, (int)sprite.GetGlobalBounds().Size.X, (el) => new Sprite(el));
            
            //version line
            Font    font       = new(ResourcesHandler.Load(FontAssetName));
         
            Text version       = new Text("v1.5 ", font, labelTex.Size.Y/4);
            Text versionShadow = new Text(version);
            
            version      .FillColor = new Color(0xbbdde1FF);
            versionShadow.FillColor = new Color(0xdc82bbFF); 
     
            versionLine.Build(version, WindowWidth, 0, 70, (int)version.GetGlobalBounds().Size.X, (el) => new(el));
            versionLineShadow.Build(versionShadow, WindowWidth, 2, 73, (int)version.GetGlobalBounds().Size.X, (el) => new(el));
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
            
            deadaysLine      .Draw(target);
            versionLine      .Draw(target);
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