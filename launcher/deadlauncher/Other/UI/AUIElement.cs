using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace leditor.UI;

public abstract class AUIElement(UIHost host, Vector2f minimalSize)
{
    private AUIBox? parent;

    protected readonly UIHost Host = host;
    
    private Vector2f _minimalSize = minimalSize;

    public Vector2f MinimalSize
    {
        get => _minimalSize;
        protected set
        {
            _minimalSize = value;
            
            if (GetParent() != null)
            {
                Host.UpdateActionsQueue.Enqueue(GetParent().OnChildUpdate);
            }
        }
    }

    public  bool      InheritRect { get; private set; }
    private FloatRect rect;

    public FloatRect Rect
    {
        get
        {
            if (InheritRect && parent != null)
            {
                return parent.Rect;
            }

            return rect;
        }
        protected set => rect = value;
    }

    public AUIBox? GetParent() => parent;

    public void SetParent(AUIBox? value)
    {
        if (parent == value) return;
        
        parent = value;
        UpdateLayout();
    }

    public virtual AUIElement SetRect(FloatRect value)
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

    public AUIElement SetInheritRect(bool value)
    {
        if (InheritRect == value) return this;
        
        InheritRect = value;
        UpdateLayout();

        return this;
    }

    public abstract void UpdateLayout();

    public abstract void Draw(RenderTarget target);

    public virtual void ProcessClicks() {}
    
    public void Destroy()
    {
        GetParent()?.RemoveChild(this);
        SetParent(null);
    }

    public virtual void Deactivate() {}
    public virtual void OnTextEntered(string text) { }
    public virtual void OnKeyPressed(Keyboard.Key key) {}
    
    public virtual void OnMouseClick(Vector2f pos) {}

}