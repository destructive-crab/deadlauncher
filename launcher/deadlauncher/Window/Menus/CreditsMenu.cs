using System.Diagnostics;
using deUI;
using SFML.Graphics;

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

    public override bool HasBackButton() => true;

    public override AUIElement GetRoot(FloatRect rect)
    {
        return new AxisBox(host, UIAxis.Vertical,
                new UILabel(host, "~    credits!      credits!      credits!    ~"),
                new UILabel(host, "~                                            ~"),

                new AxisBox(host, UIAxis.Horizontal, true,
                    new UILabel(host, " OKNO        "), new UIButton(host, "   TG Channel   ", () => OpenLink(OKNO))),
                new AxisBox(host, UIAxis.Horizontal, true,
                    new UILabel(host, " Deadays     "), new UIButton(host, "     Itch.io    ", () => OpenLink(DDYS))),
                new AxisBox(host, UIAxis.Horizontal, true,
                    new UILabel(host, " launcher by "), new UIButton(host, "destructive_crab", () => OpenLink(YOSH))),
                new AxisBox(host, UIAxis.Horizontal, true,
                    new UILabel(host, " ui lib   by "), new UIButton(host, "   stop_mind    ", () => OpenLink(STMP))),

                new UILabel(host, "~                                            ~"));
    }

    private void BackButton()
    {
        Application.Launcher.Window.OpenHomeMenu();
    }

    private void OpenLink(string link)
    {
        Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
    }
}