using SFML.Graphics;
using SFML.System;

namespace deUI;

public enum UIAxis
{
    Vertical, Horizontal
}

public class AxisBox : AUIBox
{
    private static Vector2f CalculateSize(UIStyle style, UIAxis axis, IEnumerable<AUIElement> children)
    {
        var size = new Vector2f(0, 0);

        if (axis == UIAxis.Horizontal)
        {
            foreach (var child in children)
            {
                size.Y = float.Max(size.Y, child.MinimalSize.Y);
                size.X += child.MinimalSize.X + style.AxisBoxSpace;
            }
            
            size.X -= style.AxisBoxSpace;
        }
        else
        {
            foreach (var child in children)
            {
                size.X = float.Max(size.X, child.MinimalSize.X);
                size.Y += child.MinimalSize.Y + style.AxisBoxSpace;
            }
            size.Y -= style.AxisBoxSpace;
        }

        return size;
    }

    private readonly List<AUIElement> children = new();
    private UIAxis axis;

    private bool fitRect;
    private readonly List<AUIElement> useMinimalRectWhenFitFor = new();

    public AxisBox(UIHost host) : base(host) { }

    public override FloatRect GetRect()
    {
        if (Parent != null)
        {
            if (InheritRect) return Parent.GetRect();

            switch (axis)
            {
                case UIAxis.Vertical:   return new FloatRect(rect.Position, new(Parent.GetRect().Width, MinimalSize.Y));
                case UIAxis.Horizontal: return new FloatRect(rect.Position, new(MinimalSize.X, Parent.GetRect().Height));
            }    
        }

        return rect;
    }

    public AxisBox WithAxis(UIAxis value)
    {
        axis = value;
        return this;
    }
    
    public AxisBox FitRect(bool value)
    {
        fitRect = value;
        return this;
    }

    public AxisBox WithChildren(params AUIElement[] children)
    {
        foreach (AUIElement element in children)
        {
            AddChild(element);
        }

        return this;
    }
    
    public override IEnumerable<AUIElement> GetChildren()
        => children;

    public override void RemoveChild(AUIElement child)
    {
        if (useMinimalRectWhenFitFor.Contains(child)) useMinimalRectWhenFitFor.Remove(child);
        
        child.SetParent(null);
        children.Remove(child);
        
        UpdateMinimalSize();
    }

    /// <returns> returns a child </returns>
    public AUIElement AddChild(AUIElement child)
    {
        child.SetParent(this);
        children.Add(child);
        
        UpdateMinimalSize();
        
        return child;
    }

    public void UseMinimalSizeFor(AUIElement child)
    {
        if (!children.Contains(child) || useMinimalRectWhenFitFor.Contains(child)) return;
        
        useMinimalRectWhenFitFor.Add(child);
        UpdateLayoutIm();
    }
    
    protected override void UpdateLayoutIm()
    {
        if (children.Count == 0) return;
        
        if (children.Count == 1)
        {
            AUIElement child = children[0];
            child.SetRect(GetRect());

            return;
        }
        
        if (axis == UIAxis.Horizontal)
        {
            if (fitRect)
            {
                MakeChildrenFitRect((int)GetRect().Width, (int)GetRect().Height, 
                    (c)    => (int)c.MinimalSize.X, 
                    (c, v) => c.SetRect(new FloatRect(c.GetRect().Position, new Vector2f(v, GetRect().Height))));
            }
            else
            {
                foreach (AUIElement child in children)
                {
                    child.SetRect(new FloatRect(
                        default, default,
                        child.MinimalSize.X, GetRect().Height
                    ));
                }
            }
            
            UpdatePositions((c) => new(c.GetRect().Size.X, 0), () => new(Host.Style.AxisBoxSpace, 0));
        }
        else
        {
            if (fitRect)
            {
                MakeChildrenFitRect((int)GetRect().Height, (int)GetRect().Width, 
                    (c) => (int)c.MinimalSize.Y, 
                    (c, v) => c.SetRect(new FloatRect(c.GetRect().Position, new Vector2f(GetRect().Width, v))));
            }
            else
            {
                foreach (AUIElement child in children)
                {
                    child.SetRect(new FloatRect(
                        child.GetRect().Position.X, child.GetRect().Position.Y,
                        GetRect().Width, child.MinimalSize.Y
                    ));
                }
            }
            
            UpdatePositions((c) => new(0, c.GetRect().Size.Y), () => new(0, Host.Style.AxisBoxSpace));
        }
    }

    private void MakeChildrenFitRect(
        int originalToFit,
        int staticRectValue,
        Func<AUIElement, int> getMinimal, 
        Action<AUIElement, int> setSize)
    {
        if (children.Count == 0) return;
        
        List<AUIElement> doNotFit = new();
        
        int doNotFitValue = 0;
        foreach (AUIElement element in useMinimalRectWhenFitFor)
        {
            doNotFitValue += getMinimal(element);
            doNotFit.Add(element);
        }

        int axisBoxSpacesValue = Host.Style.AxisBoxSpace*(children.Count-1);
        int valueToFit = originalToFit - axisBoxSpacesValue - doNotFitValue;
        int optimalValue = valueToFit / (children.Count - doNotFit.Count);

        for (var i = 0; i < children.ToArray().Length; i++)
        {
            AUIElement child = children.ToArray()[i];

            if (doNotFit.Contains(child))
            {
                setSize(child, (int)getMinimal(child));   
                continue;
            }
                    
            if (getMinimal(child) > optimalValue)
            {
                valueToFit -= (int)getMinimal(child);
                doNotFit.Add(child);
                
                setSize(child, (int)getMinimal(child));
                
                if (doNotFit.Count == children.Count) break;
                        
                optimalValue = valueToFit / (children.Count - doNotFit.Count);
                i = -1; //restarting
            }
            else
            {
                setSize(child, optimalValue);
            }
        }   
    }

    private void UpdatePositions(Func<AUIElement, Vector2f> getPositionStep, Func<Vector2f> getAxisSpace)
    {
        Vector2f position = GetRect().Position;

        foreach (AUIElement child in children)
        {
            child.SetRect(new FloatRect(position, child.GetRect().Size));
            //Console.WriteLine($"Set Position For {child.GetType()} {position}");
            position += getPositionStep(child) + getAxisSpace();
        }
    }
    
    protected override void UpdateMinimalSize()
    {
        MinimalSize = CalculateSize(Host.Style, axis, children);
        UpdateLayoutIm();
    }

    public override void Draw(RenderTarget target)
    {
        foreach (AUIElement child in children)
        {
            Host.Renderer.PushDrawCallToStack(child.Draw);
        }
    }
}