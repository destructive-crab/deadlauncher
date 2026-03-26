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
        
        area = new ClickArea(Rect, true);
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
        outlineShape.Position = Rect.Position;
        outlineShape.Size = Rect.Size + 2 * outline;
        
        shape.Position = Rect.Position + outline;
        shape.Size = Rect.Size;
        
        if(area != null) area.Rect = Rect;
    }

    public override void Draw(RenderTarget target)
    {
        if(outline.X != 0 || outline.Y != 0) target.Draw(outlineShape);
        target.Draw(shape);
    }
}