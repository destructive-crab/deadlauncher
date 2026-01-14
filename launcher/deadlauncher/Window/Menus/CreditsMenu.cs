using System.Diagnostics;
using deadlauncher.Other.UI;
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
        
    public override AUIElement GetRoot(FloatRect rect)
    {
        Anchor anchorBack = new(new FloatRect(0, 40, 500, -45), new FloatRect(0,1,0,0));

        //return new ScrollBox(host, new UITextBox(host, "1. Новый летающий враг - ледяной череп. Является модифицированной версией огненного черепа. Главное отличие - при атаке он накидывает дебафф обморожения\n\n2. Принципиально новая механика - баффы и дебаффы. Пока что в игре есть только один дебафф - обморожение, он замедляет игрока на время. Позже будут добавляться и другие эффекты\n\n3. Прыжок от стены - нововведение в передвижении игрока. Теперь вы после двойного прыжка можете еще раз отскочить от стены. Пригодится в режиме \"пол это лава\". Ой, его же еще нет в игре.\n\n4. Оптимизация - писал в посте выше."));
        
        return new UIOutlineBox(host, new AxisBox(host, UIAxis.Vertical,
            new UILabel(host, "~    credits!      credits!      credits!    ~"),
            new UILabel(host, "~                                            ~"),
            new AxisBox(host, UIAxis.Horizontal, true, new UILabel(host, " OKNO        "), new UIButton(host, "   TG Channel   ", () => OpenLink(OKNO))),
            new AxisBox(host, UIAxis.Horizontal, true, new UILabel(host, " Deadays     "), new UIButton(host, "     Itch.io    ", () => OpenLink(DDYS))),
            new AxisBox(host, UIAxis.Horizontal, true, new UILabel(host, " launcher by "), new UIButton(host, "destructive_crab", () => OpenLink(YOSH))),
            new AxisBox(host, UIAxis.Horizontal, true, new UILabel(host, " ui lib   by "), new UIButton(host, "   stop_mind    ", () => OpenLink(STMP))),
            new UILabel(host, "~                                            ~"),
            new UILabel(host, "\n"),
            new UIButton(host, "\u2190", new Vector2f(10, 30), BackButton)));
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