using System.Runtime.CompilerServices;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace deUI;

public abstract class AUIElement
{
    public AUIBox? Parent { get; private set; }

    protected UIHost Host;
    
    private Vector2f _minimalSize;

    public Vector2f MinimalSize
    {
        get => _minimalSize;
        protected set
        {
            _minimalSize = value;
            
            if (Parent != null)
            {
                Host.UpdateActionsQueue.Enqueue(Parent.OnChildUpdate);
            }
        }
    }

    public  bool      InheritRect { get; private set; }
    private FloatRect rect;

    protected AUIElement(UIHost host)
    {
        Host = host;
    }

    public FloatRect Rect
    {
        get
        {
            if (InheritRect && Parent != null)
            {
                return Parent.Rect;
            }

            return rect;
        }
        protected set => rect = value;
    }

    public virtual AUIElement SetRect(FloatRect value ,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        if (Rect == value) return this;

        Rect = new FloatRect(
            value.Left, value.Top,
            float.Max(MinimalSize.X, value.Width),
            float.Max(MinimalSize.Y, value.Height)
        );

        Host.UpdateActionsQueue.Enqueue(UpdateLayout);
        return this;
    }

    public void SetParent(AUIBox? value)
    {
        if (Parent == value) return;
        
        Parent = value;
        UpdateLayout();
    }

    public AUIElement SetInheritRect(bool value)
    {
        if (InheritRect == value) return this;
        
        InheritRect = value;
        UpdateLayout();

        return this;
    }

    public void UpdateLayout()
    {
        //Console.WriteLine($"UPDATE LAYOUT: {this.GetType()}; {Rect}; {Parent?.GetType().Name}");
        UpdateLayoutIm();
    }

    protected abstract void UpdateLayoutIm();

    public abstract void Draw(RenderTarget target);

    public virtual void ProcessClicks() {}
    
    public void Destroy()
    {
        Parent?.RemoveChild(this);
        SetParent(null);
    }

    public virtual void Deactivate() {}
    public virtual void OnTextEntered(string text) { }
    public virtual void OnKeyPressed(Keyboard.Key key) {}
    
    public virtual void OnMouseClick(Vector2f pos) {}

    public void OnHostSizeChanged(Vector2f newSize)
    {
        OnHostSizeChangedIm(newSize);
        if (this is AUIBox box)
        {
            foreach (AUIElement child in box.GetChildren())
            {
                child.OnHostSizeChanged(newSize);
            }
        }
    }

    protected virtual void OnHostSizeChangedIm(Vector2f newSize)
    {
        
    }
}