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
        IUIFactory f = host.Factory;

        return f.New<AxisBox>().WithAxis(UIAxis.Vertical).WithChildren(
            f.New<UILabel>().WithText("~    credits!      credits!      credits!    ~"),
            f.New<UILabel>().WithText("~                                            ~"),
            f.New<AxisBox>().WithAxis(UIAxis.Horizontal).FitRect(true).WithChildren(
                f.New<UILabel>().WithText(" OKNO        "), f.New<UIButton>().WithText("   TG Channel   ").OnClick(() => OpenLink(OKNO))),
            f.New<AxisBox>().WithAxis(UIAxis.Horizontal).FitRect(true).WithChildren(
                f.New<UILabel>().WithText(" Deadays     "), f.New<UIButton>().WithText("     Itch.io    ").OnClick(() => OpenLink(DDYS))),
            f.New<AxisBox>().WithAxis(UIAxis.Horizontal).FitRect(true).WithChildren(
                f.New<UILabel>().WithText(" launcher by "), f.New<UIButton>().WithText("destructive_crab").OnClick(() => OpenLink(YOSH))),
            f.New<AxisBox>().WithAxis(UIAxis.Horizontal).FitRect(true).WithChildren(
                f.New<UILabel>().WithText(" ui lib   by "), f.New<UIButton>().WithText("   stop_mind    ").OnClick(() => OpenLink(STMP))),
            f.New<UILabel>().WithText("~                                            ~"))
        ;
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