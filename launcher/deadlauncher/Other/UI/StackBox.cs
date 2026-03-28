using SFML.Graphics;
using SFML.System;

namespace deUI;

public class StackBox : AUIBox
{
    private static Vector2f GetMinSize(IEnumerable<AUIElement> children, UIPadding padding)
    {
        var size = new Vector2f();
        foreach (var child in children)
        {
            size.X = float.Max(size.X, child.MinimalSize.X);
            size.Y = float.Max(size.Y, child.MinimalSize.Y);
        }

        size.X += padding.Left + padding.Right;
        size.Y += padding.Top + padding.Bottom;
        
        return size;
    }

    private readonly List<AUIElement> _children = new();

    private UIPadding _padding;

    public UIPadding Padding
    {
        get => _padding;
        set
        {
            _padding = value;
            MinimalSize = new Vector2f(
                MinimalSize.X + _padding.Left + _padding.Right - value.Left - value.Right,
                MinimalSize.Y + _padding.Top + _padding.Bottom - value.Top - value.Bottom
            );
        }
    }

    public StackBox(UIHost host) : base(host) { }

    public StackBox WithChildren(params AUIElement[] elements)
    {
        _children.Clear();
        AddChildren(elements);
        
        return this;
    }
    
    public override IEnumerable<AUIElement> GetChildren()
        => _children;

    public override void RemoveChild(AUIElement child)
    {
        child.SetParent(null);
        _children.Remove(child);
    }

    public StackBox AddChildren(AUIElement[] children)
    {
        foreach (AUIElement element in children)
        {
            AddChild(element);
        }

        return this;
    }
    
    public void AddChild(AUIElement child)
    {
        child.SetParent(this);
        
        _children.Add(child);
        
        MinimalSize = new Vector2f(
            float.Max(MinimalSize.X, child.MinimalSize.X),
            float.Max(MinimalSize.Y, child.MinimalSize.Y)
        );
    }
    
    protected override void UpdateMinimalSize()
        => MinimalSize = GetMinSize(_children, _padding);

    protected override void UpdateLayoutIm()
    {
        foreach (AUIElement element in _children)
        {
            element.UpdateLayout();
        }
        
        //TODO REFACTOR STACK BOX
        return;
        var baseRect = new FloatRect(
            GetRect().Left   + Padding.Left, 
            GetRect().Top    + Padding.Top,
            GetRect().Width  - Padding.Left - Padding.Right, 
            GetRect().Height - Padding.Bottom - Padding.Top
        );

        baseRect.Left += baseRect.Width / 2;
        baseRect.Top += baseRect.Height / 2;

        foreach (var child in _children)
        {
            if (_centerX)
                baseRect.Width = child.MinimalSize.X;
            if (_centerY)
                baseRect.Height = child.MinimalSize.Y;

            child.SetRect(new FloatRect(
                baseRect.Left - baseRect.Width / 2,
                baseRect.Top - baseRect.Height / 2,
                baseRect.Width,
                baseRect.Height
            ));
        }
    }

    public override void Draw(RenderTarget target)
    {
        foreach (var child in _children.AsEnumerable().Reverse())
            Host.Renderer.PushDrawCallToStack(child.Draw);
    }

    private bool _centerX;
    private bool _centerY;

    public bool CenterX
    {
        get => _centerX;
        set
        {
            _centerX = value;
            UpdateLayout();
        }
    }
    
    public bool CenterY
    {
        get => _centerY;
        set
        {
            _centerY = value;
            UpdateLayout();
        }
    }
}