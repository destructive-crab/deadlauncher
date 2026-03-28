using SFML.Graphics;

namespace deUI;

public struct Anchor(FloatRect baseRect, FloatRect relative)
{
    public FloatRect BaseRect = baseRect;
    public FloatRect Relative = relative;
}

public class AnchorBox : AUIBox
{
    private readonly List<(Anchor, AUIElement)> children = [];

    public AnchorBox(UIHost host) : base(host) { }

    protected override void UpdateLayoutIm()
    {
        foreach (var (anchor, child) in children)
        {
            var rect = anchor.BaseRect;
            rect.Left += GetRect().Left + anchor.Relative.Left * GetRect().Width;
            rect.Top += GetRect().Top + anchor.Relative.Top * GetRect().Height;
            rect.Width += anchor.Relative.Width * GetRect().Width;
            rect.Height += anchor.Relative.Height * GetRect().Height;
            child.SetRect(rect);
        }
    }
    
    public override IEnumerable<AUIElement> GetChildren()
        => children
            .Select(tuple => tuple.Item2)
            .AsEnumerable();

    public AnchorBox WithChild(Anchor anchor, AUIElement child)
    {
        child.SetParent(this);
        children.Add((anchor, child));
        
        UpdateLayout();
        return this;
    }

    public override void RemoveChild(AUIElement child)
    {
        child.SetParent(null);
        (Anchor, AUIElement) match = children.FirstOrDefault(tuple => tuple.Item2 == child);
        if (match.Item2 != null)
        {
            children.Remove(match);
        }
        UpdateLayoutIm();
    }

    protected override void UpdateMinimalSize() { }

    public override void Draw(RenderTarget target)
    {
        foreach (var child in children.AsEnumerable().Reverse())
        {
            child.Item2.Draw(target);
        }
    }
}