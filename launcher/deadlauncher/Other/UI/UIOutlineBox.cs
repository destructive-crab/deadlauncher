using deUI;
using SFML.Graphics;
using SFML.System;

namespace deadlauncher.Other.UI;

public sealed class UIOutlineBox : AUIBox
{
    public AUIElement? Child { get; private set; }

    private readonly RectangleShape line = new RectangleShape();

    public UIOutlineBox(UIHost host, AUIElement child, FloatRect rect = default) : base(host, rect.Size + new Vector2f(UIStyle.BaseOutline, UIStyle.BaseOutline) * 2)
    {
        line.FillColor = UIStyle.OutlineColor;
        Child = child;
        SetRect(rect);
    }

    public override void Draw(RenderTarget target)
    {
        if (Child == null) return;
        
        Child.Draw(target);
        
        var outline = UIStyle.BaseOutline;

        //horizontal
        line.Position = Rect.Position - new Vector2f(outline, outline);
        line.Size     = new Vector2f(Rect.Size.X + outline * 2, outline);
       
        target.Draw(line);

        line.Position += new Vector2f(0, Rect.Size.Y + outline);
        
        target.Draw(line);
        
        //vertical
        line.Position = Rect.Position - new Vector2f(outline, outline);
        line.Size     = new Vector2f(outline, Rect.Size.Y + outline * 2);

        target.Draw(line);

        line.Position += new Vector2f(Rect.Size.X + outline, 0);
        
        target.Draw(line);
    }

    public override IEnumerable<AUIElement> GetChildren()
    {
        return [Child];
    }

    public override void UpdateLayout()
    {
        if (Child == null) return;
        
        Child.SetRect(Rect);
    }

    public override AUIElement SetRect(FloatRect value)
    {
        Rect = new FloatRect(value.Position - new Vector2f(UIStyle.BaseOutline, UIStyle.BaseOutline), value.Size + new Vector2f(UIStyle.BaseOutline, UIStyle.BaseOutline));
        return this;
    }

    public override void RemoveChild(AUIElement child)
    {
        if (Child == child)
        {
            Child = null;
        }
    }

    public void SetChild(AUIElement child)
    {
        if (Child == child) return;

        if (Child != null)
        {
            Child.SetParent(null);
        }

        Child = child;
        child.SetParent(this);
        
        UpdateLayout();
    }

    protected override void UpdateMinimalSize() { }
}