using SFML.Graphics;
using SFML.System;

namespace leditor.UI;

public class UIRect : AUIElement
{
    private readonly RectangleShape shape;
    private readonly RectangleShape outlineShape;

    private          Vector2f       outline;
    
    private          ClickArea      area;
    
    public UIRect(UIHost host, Color? color = null, Vector2f outline = default) : base(host, default)
    {
        shape = new RectangleShape
        {
            FillColor = color ?? UIStyle.RectDefault
        };
        
        this.outline = outline;
        
        outlineShape = new RectangleShape();
        outlineShape.FillColor = UIStyle.OutlineColor;
    }

    public Color Color
    {
        get => shape.FillColor;
        set => shape.FillColor = value;
    }

    public UIRect BlockClicks()
    {
        area = new ClickArea(Rect, true);
        return this;
    }

    public override void ProcessClicks()
    {
        if (area == null) return;
        
        base.ProcessClicks();
        Host.Areas.Process(area);
    }

    public override void UpdateLayout()
    {
        outlineShape.Position = Rect.Position;
        outlineShape.Size = Rect.Size + 2 * outline;
        
        shape.Position = Rect.Position + outline;
        shape.Size = Rect.Size;

        if(area != null) area.Rect = Rect;
    }

    public override void Draw(RenderTarget target)
    {
        target.Draw(outlineShape);
        target.Draw(shape);
    }
}