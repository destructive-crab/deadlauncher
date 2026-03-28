using System.Runtime.CompilerServices;
using SFML.Graphics;

namespace deUI;

public class UISocketBox : AUIBox
{
    public AUIElement? Child { get; private set; }
    public bool Hidden { get; private set; }

    public UISocketBox(UIHost host) : base(host) { }

    public override AUIElement SetRect(FloatRect value ,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        rect = value;
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
    
    public UISocketBox WithChild(AUIElement? value)
    {
        Child?.SetParent(null);
        
        value?.SetParent(this);
        
        Child = value;
        
        UpdateMinimalSize();
        UpdateLayout();
        return this;
    }
    
    public override void RemoveChild(AUIElement child)
    {
        if (Child == child)
        {
            child.SetParent(null);
            WithChild(null);
        }
    }
    
    protected override void UpdateLayoutIm()
    {
        if (Child != null && !Hidden)
        {
            Child.SetRect(GetRect());
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
            Host.Renderer.PushDrawCallToStack(Child.Draw);
        }
    }

    public override void ProcessClicks()
    {
        if (Child != null && !Hidden)
        {
            Host.InputsHandler.PushClickProcessor(Child.ProcessClicks);
        }
    }
}