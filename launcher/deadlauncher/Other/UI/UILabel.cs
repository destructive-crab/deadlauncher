using SFML.Graphics;

namespace deUI;

public class UILabel : AUIElement
{
    public UILabel(UIHost host) : base(host)
    {
        textObject = Host.Fabric.MakeText("X");
    }

    public UILabel WithText(string text)
    {
        Text = text;
        return this;
    }
    
    private Text textObject;
    public string Text
    {
        get => textObject.DisplayedString;
        set
        {
            textObject.DisplayedString = value;
            
            MinimalSize = Utils.TextSize(textObject);
            SetRect(new FloatRect(GetRect().Position, MinimalSize));
        }
    }

    protected override void UpdateLayoutIm()
    {
        textObject.Position = GetRect().Position;
    }

    public override void Draw(RenderTarget target)
    {
        if(textObject != null)
        {
            target.Draw(textObject);
        }
    }
}