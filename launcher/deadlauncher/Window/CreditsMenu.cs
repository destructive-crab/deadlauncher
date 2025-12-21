using System.Diagnostics;
using leditor.UI;
using SFML.Graphics;
using SFML.System;

namespace deadlauncher;

public sealed class CreditsMenu : Menu
{
    private readonly UIHost host;

    public CreditsMenu(UIHost host)
    {
        this.host = host;
    }

    private const string OKNO = "https://t.me/oknogmdv";
    private const string DDYS = "https://oknogmdv.itch.io/deadays";
    private const string YOSH = "https://github.com/destructive-crab";
    private const string STMP = "https://github.com/stopmind";
        
    public override AUIElement GetRoot()
    {
        Anchor anchorBack = new(new FloatRect(0, 40, 500, -45), new FloatRect(0,1,0,0));
        return new AxisBox(host, UIAxis.Vertical,
            new UILabel(host, "~ credits! credits! credits!  ~"),
            new UILabel(host, "~                             ~"),
            new AxisBox(host, UIAxis.Horizontal, new UILabel(host, " OKNO        "), new UIButton(host, "   TG Channel   ", () => OpenLink(OKNO))),
            new AxisBox(host, UIAxis.Horizontal, new UILabel(host, " Deadays     "), new UIButton(host, "     Itch.io    ", () => OpenLink(DDYS))),
            new AxisBox(host, UIAxis.Horizontal, new UILabel(host, " launcher by "), new UIButton(host, "destructive_crab", () => OpenLink(YOSH))),
            new AxisBox(host, UIAxis.Horizontal, new UILabel(host, " ui lib   by "), new UIButton(host, "   stop_mind    ", () => OpenLink(STMP))),
            new UILabel(host, "~                             ~"),
            new UILabel(host, "\n"),
            new UIButton(host, "         \t\u2190", new Vector2f(10, 30), BackButton));
    }

    private void OpenLink(string link)
    {
        Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
    }

    private void BackButton()
    {
        Application.Launcher.Window.OpenHomeMenu();
    }

    public override void Update(RenderWindow window) { }
}