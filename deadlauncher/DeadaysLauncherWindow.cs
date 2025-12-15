using System.Diagnostics;
using leditor.UI;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace deadlauncher;

public class DeadaysLauncherWindow
{
    private List<string> downloadedVersions = new();
    public VersionLogic versionLogic = new();

    private RenderWindow window;
    private UIHost uiHost;

    private List<Sprite> label = new List<Sprite>();
    private List<Text> versions = new();
    private List<Text> versionsBack = new();

    private RunningLine<Sprite> deadaysLine = new();
    
    private RunningLine<Text> versionLine = new();
    private RunningLine<Text> versionLineShadow = new();
    
    public async Task Prepare()
    {
        window = new RenderWindow(new VideoMode(600, 500), "DDLAUNCHER", Styles.Titlebar | Styles.Close);
        
        window.Closed += WindowOnClosed;

        Texture labelTex = new("C:\\Users\\destructive_crab\\dev\\buisnes\\OKNO\\deadlauncher\\deadlauncher\\assets\\dd_label.png");
        Font font = new("C:\\Users\\destructive_crab\\dev\\buisnes\\OKNO\\deadlauncher\\deadlauncher\\assets\\Main.ttf");
        
        Text version = new Text("v1.5 ", font, labelTex.Size.Y/4);
        version.FillColor = new Color(0xbbdde1FF);
        
        Text versionShadow = new Text("v1.5 ", font, labelTex.Size.Y/4);
        versionShadow.FillColor = new Color(0xdc82bbFF); 
        
        uiHost = new UIHost(new UIStyle(), new Vector2f(window.Size.X, window.Size.Y));

        deadaysLine.Build(new Sprite(labelTex), 600, 0, 10, (int)labelTex.Size.X, (el) => new Sprite(el));
        
        versionLine.Build(version, 600, 0, 50, (int)version.GetGlobalBounds().Size.X, (el) => new(el));
        versionLineShadow.Build(versionShadow, 600, 5, 55, (int)version.GetGlobalBounds().Size.X, (el) => new(el));
        
        uiHost.SetRoot(GetMainMenu());

        window.Resized += WindowOnResized;
    }

    private void WindowOnResized(object? sender, SizeEventArgs e)
    {
        uiHost.SetSize(new(e.Width, e.Height));
    }

    private void WindowOnClosed(object? sender, EventArgs e)
    {
        Shutdown();
    }

    public void Loop()
    {
        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Clear(new Color(0x00363cFF));

            deadaysLine.MovePositions(0.05f);
            deadaysLine.Draw(window);
            
            versionLine.MovePositions(0.03f);
            versionLine.Draw(window);
            
            versionLineShadow.MovePositions(0.03f);
            versionLineShadow.Draw(window);
            
            uiHost.Update(window);
            uiHost.Draw(window);
            window.Display();
        }
    }

    public void Shutdown()
    {
        window.Close();
    }

    private void BackButton()
    {
   
    }

    private void VersionSelectButton(string id)
    {
        versionLogic.CurrentVersionId = id;

        var e = versionLine.BaseElementShared;
        e.DisplayedString = id;
        
        versionLine.SetElementWidth((int)e.GetGlobalBounds().Width);
        versionLine.Foreach((e) => e.DisplayedString = id);
        
        versionLineShadow.SetElementWidth((int)e.GetGlobalBounds().Width);
        versionLineShadow.Foreach((e) => e.DisplayedString = id);
        
        BackButton();
    }
    
    private async void PlayButton()
    {

    }

    private void VersionButton()
    {
        SwitchToVersionMenu();
    }
    private void LaunchSelectedVersion()
    {
        
    }

    private void SwitchToVersionMenu()
    {
        uiHost.SetRoot(GetVersionSelectionMenu());
    }

    private void SwitchToMainMenu()
    {
        uiHost.SetRoot(GetMainMenu());
    }
    
    private AUIElement GetVersionSelectionMenu()
    {
        AxisBox versionList = new(uiHost, UIAxis.Vertical);
        foreach (string id in versionLogic.availableOnServerIDs)
        {
            string state = "";
            
            if (versionLogic.localVersionsIDs.Contains(id)) state = "installed!";
            if (!versionLogic.localVersionsIDs.Contains(id)) state = "not installed!";
            
            versionList.AddChild(new AxisBox(uiHost, UIAxis.Horizontal, new UIButton(uiHost, id, new Vector2f(220, 45), () => VersionSelectButton(id)), new UIButton(uiHost, "F"), new UILabel(uiHost, state)));
        }

        versionList.AddChild(new UILabel(uiHost, "\n"));
        
        Anchor listAnchor = new(new FloatRect(40, 100, window.Size.X - 50, window.Size.Y - 100 - 45), new FloatRect(0,0,0,0));
        Anchor anchorBack = new(new FloatRect(0, 0, window.Size.X-80, -45), new FloatRect(0,1,0,0));
        
        AnchorBox anchorBox = new AnchorBox(uiHost);
        
        anchorBox.AddChild(listAnchor, new StackBox(uiHost, 
            [new UIRect(uiHost), 
             new ScrollBox(uiHost, versionList),
             new AnchorBox(uiHost).AddChild(anchorBack, new UIButton(uiHost, "           \t\u2190", new Vector2f(10, 45), SwitchToMainMenu))]));
        

        return anchorBox;
    }

    private AUIElement GetMainMenu()
    {
        AnchorBox anchor = new AnchorBox(uiHost);
        
        UIButton playButton = new UIButton(uiHost, " play! play! play!", new Vector2f(320, 50), LaunchSelectedVersion);
        UIButton versionButton = new UIButton(uiHost, " version! version!", new Vector2f(320, 50), VersionButton);
        
        Anchor bottomAnchor = new Anchor(new FloatRect(-160, 0, window.Size.X, 40), new FloatRect(0.5f, 0.5f, 0, 0));
        
        anchor.AddChild(bottomAnchor, new AxisBox(uiHost, UIAxis.Vertical, new AxisBox(uiHost, UIAxis.Horizontal, playButton, new UILabel(uiHost)), new AxisBox(uiHost, UIAxis.Horizontal, versionButton, new UILabel(uiHost))));
        return new StackBox(uiHost, [anchor]);
    }

    class RunningLine<TElement>
        where TElement : Transformable, Drawable
    {
        public TElement BaseElementShared => clone(baseElement);
        
        private TElement baseElement;
        
        private int lineLength;
        private int elementWidth;

        private Func<TElement, TElement> clone;
        
        private readonly List<TElement> line = new();

        public void SetElementWidth(int width)
        {
            elementWidth = width;
        }
        
        public void Clear()
        {
            line.Clear();
            baseElement = null;
            lineLength = 0;
            elementWidth = 0;
            clone = null;
        }
        
        public void Build(TElement drawable, int lineWidth, int x, int y, int elementWidth, Func<TElement, TElement> clone)
        {
            Clear();
            
            baseElement = drawable;
            this.lineLength = lineWidth;
            this.elementWidth = elementWidth;
            this.clone = clone;

            int elementsCount = (lineLength / elementWidth) + 2;
            for (int i = 0; i < elementsCount; i++)
            {
                line.Add(clone(baseElement));
            }
            
            UpdatePositions(new Vector2f(x, y));
        }

        public void Foreach(Action<TElement> each)
        {
            foreach (TElement element in line)
            {
                each.Invoke(element);
            }
        }

        public void UpdatePositions(Vector2f leadPosition)
        {
            line[0].Position = leadPosition;
            TElement prev = null;
            
            foreach (TElement element in line.ToArray())
            {
                if(prev == null)
                {
                    prev = element;
                    continue;
                }

                element.Position = new Vector2f(prev.Position.X - elementWidth, prev.Position.Y);
                prev = element;
            }
        }
        
        public void MovePositions(float delta)
        {
            line[0].Position = new Vector2f(line[0].Position.X + delta, line[0].Position.Y);

            if (line[0].Position.X > lineLength)
            {
                TElement tmp = line[0];
                line.RemoveAt(0);
                line.Add(tmp);
            }
            
            line[0].Position = new Vector2f(line[0].Position.X + delta, line[0].Position.Y);
            
            TElement prev = null;
            
            foreach (TElement element in line.ToArray())
            {
                if(prev == null)
                {
                    prev = element;
                    continue;
                }

                element.Position = new Vector2f(prev.Position.X - elementWidth, prev.Position.Y);
                prev = element;
            }
        }

        public void Draw(RenderTarget target)
        {
            foreach (TElement drawable in line)
            {
                target.Draw(drawable);
            }
        }
    }
}