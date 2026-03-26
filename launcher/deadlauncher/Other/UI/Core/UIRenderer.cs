using SFML.Graphics;
using SFML.System;

namespace deUI;

public sealed class UIRenderer : IUIRenderer
{
    private readonly IUITree tree;

    public View View { get; private set; } = new();

    private readonly Stack<UIDrawCall> DrawStack = [];

    public UIRenderer(IUITree tree)
    {
        this.tree = tree;
    }

    public Vector2f GetSize()
    {
        return View.Size;
    }

    public void SetSize(Vector2f size)
    {
        View.Size = size;
        View.Center = size / 2;
        
        if (tree.AssertRoot(out AUIElement root))
        {
            root.SetRect(new FloatRect(new Vector2f(0,0), size));
            root.OnHostSizeChanged(size);
        }
    }
    
    public void Draw(RenderTarget target)
    {
        target.SetView(View);

        if (tree.AssertRoot(out AUIElement root))
        {
            root.Draw(target);
            
            while (DrawStack.TryPop(out UIDrawCall? drawCall))
            {
                drawCall.Invoke(target);
            }
        }
    }

    public void PushDrawCall(UIDrawCall call)
    {
        DrawStack.Push(call);
    }
}