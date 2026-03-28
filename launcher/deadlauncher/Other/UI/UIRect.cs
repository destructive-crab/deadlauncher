using SFML.Graphics;
using SFML.System;

namespace deUI;

public class UIRect : AUIElement
{
    private readonly RectangleShape shape;
    private readonly RectangleShape outlineShape;
    
    private          ClickArea      area;
    
    private          Vector2f       outline;

    public Color Color
    {
        get => shape.FillColor;
        set => shape.FillColor = value;
    }

    public UIRect(UIHost host) : base(host)
    {
        shape = new RectangleShape
        {
            FillColor = UIStyle.RectDefault
        };
        
        outlineShape = new RectangleShape();
        outlineShape.FillColor = UIStyle.RectDefault;
    }

    public UIRect WithColor(Color color)
    {
        Color = color;
        return this;
    }

    public UIRect WithOutline(int value)
    {
        return WithOutline(new Vector2f(value, value));
    }
    
    public UIRect WithOutline(Vector2f value)
    {
        outline = value;
        UpdateLayoutIm();
        return this;
    }

    public UIRect WithBlockClicks(bool value)
    {
        
        area = new ClickArea(GetRect(), true);
        return this;
    }
    
    public override void ProcessClicks()
    {
        if (area == null) return;
        
        base.ProcessClicks();
        Host.InputsHandler.Areas.Process(area);
    }
    
    protected override void UpdateLayoutIm()
    {
        outlineShape.Position = GetRect().Position;
        outlineShape.Size = GetRect().Size + 2 * outline;
        
        shape.Position = GetRect().Position + outline;
        shape.Size = GetRect().Size;
        
        if(area != null) area.Rect = GetRect();
    }

    public override void Draw(RenderTarget target)
    {
        if(outline.X != 0 || outline.Y != 0) target.Draw(outlineShape);
        target.Draw(shape);
    }
}