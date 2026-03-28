using System.Runtime.CompilerServices;
using deUI;
using SFML.Graphics;
using SFML.System;

namespace deadlauncher.Other.UI;

public sealed class UIOutlineBox : AUIBox
{
    public AUIElement? Child { get; private set; }

    private readonly RectangleShape line = new RectangleShape();

    public UIOutlineBox(UIHost host) : base(host)
    {
        line.FillColor = Host.Style.OutlineColor;
    }

    public UIOutlineBox WithChild(AUIElement child)
    {
        SetChild(child);
        return this;
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

    public override IEnumerable<AUIElement> GetChildren()
    {
        if(Child != null) return [Child];
        else              return new AUIElement[] {};
    }

    public override void RemoveChild(AUIElement child)
    {
        if (Child == child)
        {
            Child = null;
        }
    }

    public override AUIElement SetRect(FloatRect value,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        return base.SetRect(value);
    }

    protected override void UpdateMinimalSize() { }

    protected override void UpdateLayoutIm()
    {
        if (Child == null) return;
        
        Child.SetRect(GetRect());
    }

    public override void Draw(RenderTarget target)
    {
        if (Child == null) return;
        
        Host.Renderer.PushDrawCallToStack(Child.Draw);
        Host.Renderer.PushDrawCallToStack(DrawOutline);
    }

    private void DrawOutline(RenderTarget target)
    {
        var outline = Host.Style.BaseOutline;

        //horizontal
        line.Position = GetRect().Position - new Vector2f(outline, outline);
        line.Size     = new Vector2f(GetRect().Size.X + outline * 2, outline);
       
        target.Draw(line);

        line.Position += new Vector2f(0, GetRect().Size.Y + outline);
        
        target.Draw(line);
        
        //vertical
        
        line.Position = GetRect().Position - new Vector2f(outline, outline);
        line.Size     = new Vector2f(outline, GetRect().Size.Y + outline * 2);

        target.Draw(line);

        line.Position += new Vector2f(GetRect().Size.X + outline, 0);
        
        target.Draw(line);
    }
}