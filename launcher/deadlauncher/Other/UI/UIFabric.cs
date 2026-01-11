using SFML.Graphics;
using SFML.System;

namespace leditor.UI;

public class UIFabric(UIHost host)
{
    private UIHost _host = host;

    public Text MakeText(string str)
    {
        var text = new Text(str, _host.Style.Font, _host.Style.FontSize)
        {
            Style = Text.Styles.Bold,
            FillColor = UIStyle.LabelColor
        };

        return text;
    }

    public RectangleShape MakeRect(Color color, Vector2f size = default)
        => new(size)
        {
            FillColor = color
        };

    public Vector2f MakeTextOut(string str, out Text text)
    {
        text = MakeText(str);
        return Utils.TextSize(text);
    }
}