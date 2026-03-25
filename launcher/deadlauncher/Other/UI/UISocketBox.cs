using SFML.Graphics;

namespace deUI;

public class UISocketBox : AUIBox
{
    public AUIElement? Child { get; private set; }
    public bool Hidden { get; private set; }

    public UISocketBox(UIHost host, AUIElement? child = null, bool hidden = false) 
        : base(host, child?.MinimalSize ?? default)
    {
        SetChild(child);
        SetHidden(hidden);
    }

    public override AUIElement SetRect(FloatRect value)
    {
        Rect = value;
        Child?.SetRect(value);
        
        return this;
    }

    public UISocketBox SetHidden(bool value)
    {
        Hidden = value;
        UpdateMinimalSize();

        return this;
    }

    public UISocketBox Hide() => SetHidden(true);
    public UISocketBox Show() => SetHidden(false);
    
    public override IEnumerable<AUIElement> GetChildren() => Child == null ? [] : [Child];
    
    public void SetChild(AUIElement? value)
    {
        Child?.SetParent(null);
        
        value?.SetParent(this);
        
        Child = value;
        
        UpdateMinimalSize();
        UpdateLayout();
    }
    
    public override void RemoveChild(AUIElement child)
    {
        if (Child == child)
        {
            child.SetParent(null);
            SetChild(null);
        }
    }
    
    public override void UpdateLayout()
    {
        if (Child != null && !Hidden)
        {
            Child.SetRect(Rect);
        }
    }

    protected override void UpdateMinimalSize()
    {
        MinimalSize = Child?.MinimalSize ?? default;
    }
    
    public override void Draw(RenderTarget target)
    {
        if (Child != null && !Hidden)
        {
            Host.DrawStack.Push(Child.Draw);
        }
    }

    public override void ProcessClicks()
    {
        if (Child != null && !Hidden)
        {
            Host.ClickHandlersStack.Push(Child.ProcessClicks);
        }
    }
}