using SFML.Graphics;
using SFML.System;

namespace deUI;

class Scroller
{
    public readonly UIHost host;
    
    public readonly ClickArea Area;
    
    private readonly RectangleShape shape;
    private readonly RectangleShape backgroundShape;

    public FloatRect Limits { get; private set; }
    
    public Action<Vector2f> OnUpdate;
    
    public bool Selected { get; private set; }

    public Scroller(UIHost host, Action<Vector2f> onUpdate, FloatRect limits, Vector2f size)
    {
        this.host = host;
        
        shape = new RectangleShape
        {
            FillColor = UIStyle.ScrollerColor,
            Size = size
        };
        
        backgroundShape = new RectangleShape
        {
            FillColor = UIStyle.SecondBackgroundColor,
            Position = Limits.Position,
            Size = Limits.Size,  
        };
        
        Area = new ClickArea(new FloatRect(limits.Left, limits.Top, size.X, size.Y))
        {
            OnMove = OnMove,
            OnRightMouseButtonClick    = Select,
            OnRightMouseButtonReleased = Deselect,
        };
        
        SetLimits(limits);
        OnUpdate = onUpdate;
    }

    public void Deselect()
    {
        shape.FillColor = UIStyle.ScrollerColor;
        Selected = false;
    }

    public void Select()
    {
        shape.FillColor = UIStyle.ScrollerPressedColor;
        Selected = true;
    }

    private void OnMove(Vector2f oldPos, Vector2f newPos)
    {
        if (!Selected) return;
        
        Vector2f pos = new Vector2f(
            float.Clamp(shape.Position.X + (newPos.X - oldPos.X), Limits.Left, Limits.Left + Limits.Width),
            float.Clamp(shape.Position.Y + (newPos.Y - oldPos.Y), Limits.Top, Limits.Top + Limits.Height)
        );

        shape.Position = pos;
        Area.Rect.Left = pos.X;
        Area.Rect.Top = pos.Y;

        shape.FillColor = UIStyle.ScrollerPressedColor;
        
        if (Limits.Width != 0)
        {
            pos.X -= Limits.Left;
            pos.X /= Limits.Width;
        }
        else pos.X = 0;
        
        if (Limits.Height != 0)
        {
            pos.Y -= Limits.Top;
            pos.Y /= Limits.Height;
        }
        else pos.Y = 0;

        OnUpdate(pos);
    }

    public void SetPosition(Vector2f pos)
    {
        shape.Position = pos;
        Area.Rect.Left = pos.X;
        Area.Rect.Top = pos.Y;
    }
    
    public void SetSize(Vector2f size, Vector2f background)
    {
        shape.Size = size;
        
        Area.Rect.Width = size.X;
        Area.Rect.Height = size.Y;
        
        backgroundShape.Size = background;
    }

    public void SetLimits(FloatRect limits)
    {
        Limits = limits;
        backgroundShape.Position = Limits.Position;
    }
    
    public void Draw(RenderTarget target)
    {
        target.Draw(backgroundShape);
        target.Draw(shape);
    }
}

public class ScrollBox : AUIBox
{
    private readonly Scroller scrollerX;
    private readonly Scroller scrollerY;

    private readonly View view = new();

    private Vector2f difference;
    private Vector2f scroll;
    
    private FloatRect clickView;
    private ClickArea scrollArea;
    
    private AUIElement? child;
    public AUIElement? Child
    {
        get => child;
        set
        {
            child = value;
            if (child == null) return;

            child.SetParent(this);
            child.SetRect(Rect);
        }
    }

    public ScrollBox(UIHost host) : base(host)
    {
        MinimalSize = new Vector2f(host.Style.ScrollerThickness * 2, host.Style.ScrollerThickness * 2);

        scrollArea = new ClickArea(default, false);
        
        scrollerX = new Scroller(host, OnScrollX, new FloatRect(), new Vector2f(host.Style.ScrollerThickness, host.Style.ScrollerThickness));
        scrollerY = new Scroller(host, OnScrollY, new FloatRect(), new Vector2f(host.Style.ScrollerThickness, host.Style.ScrollerThickness));
    }

    public ScrollBox WithChild(AUIElement element)
    {
        Child = element;
        return this;
    }
    
    public override void ProcessClicks()
    {
        if (child != null)
        {
         //   Host.ClickHandlersStack.Push(
         //       () => Host.Areas.PopViewport()
         //   );
         //   
         //   Host.ClickHandlersStack.Push(child.ProcessClicks);
         //   
         //   Host.ClickHandlersStack.Push(
         //       () => Host.Areas.SetViewport(clickView)
         //   );
        }
            
        Host.InputsHandler.Areas.Process(scrollerX.Area);
        Host.InputsHandler.Areas.Process(scrollerY.Area);
        
        Host.InputsHandler.Areas.Process(scrollArea);

        float mouseWheelDelta = Host.InputsHandler.MouseWheelDelta();
        if (scrollArea.IsHovered && Rect.Contains(Host.InputsHandler.MousePosition()) && mouseWheelDelta != 0)
        {
            OnScrollY(new(scroll.X, scroll.Y - mouseWheelDelta/20));
        }

        if (scrollerX.Selected && Host.InputsHandler.IsLeftMouseButtonReleased())
        {
            scrollerX.Deselect();   
        }
        
        if (scrollerY.Selected && Host.InputsHandler.IsLeftMouseButtonReleased())
        {
            scrollerY.Deselect();
        }
    }

    private void OnScrollX(Vector2f vec)
    {
        SetScroll(new Vector2f(vec.X, scroll.Y));
    }
    
    private void OnScrollY(Vector2f vec)
    {
        SetScroll(new Vector2f(scroll.X, vec.Y));
    }

    private void SetScroll(Vector2f scroll)
    {
        this.scroll = new Vector2f(
            float.Clamp(scroll.X, 0, 1),
            float.Clamp(scroll.Y, 0, 1)
        );

        if (Child != null)
            Child.SetRect(new FloatRect (
                Rect.Left - this.scroll.X * difference.X , 
                Rect.Top - this.scroll.Y * difference.Y,
                Rect.Width - Host.Style.ScrollerThickness - 20,
                Rect.Height - Host.Style.ScrollerThickness
            ));
    }

    public override IEnumerable<AUIElement> GetChildren()
        => child != null ? [child] : [];

    public override void RemoveChild(AUIElement child)
    {
        if (this.child == child)
        {
            this.child.SetParent(null);
            this.child = null;
        }
    }

    protected override void UpdateMinimalSize() { }

    protected override void UpdateLayoutIm()
    {
        if (child == null) return;

        scrollArea.Rect = Rect;
        Vector2f size = new Vector2f(Rect.Width - Host.Style.ScrollerThickness - 20, Rect.Height - Host.Style.ScrollerThickness);
        
        child.SetRect(new FloatRect (
            Rect.Left - scroll.X * difference.X, 
            Rect.Top - scroll.Y * difference.Y,
            size.X, size.Y
        ));
        
        difference = new Vector2f(
            float.Max(0, child.Rect.Width - size.X),
            float.Max(0, child.Rect.Height - size.Y)
        );

        clickView = Rect;
        
        UpdateScrollerXLayout();
        UpdateScrollerYLayout();
    }

    protected override void OnHostSizeChangedIm(Vector2f newSize)
    {
        view.Size = Rect.Size;
        view.Center = Rect.Position + Rect.Size / 2;
        view.Viewport = new FloatRect(
            Rect.Left   / Host.Renderer.GetSize().X,
            Rect.Top    / Host.Renderer.GetSize().Y,
            Rect.Width  / Host.Renderer.GetSize().X,
            Rect.Height / Host.Renderer.GetSize().Y
        );
    }

    private void UpdateScrollerXLayout()
    {
        Vector2f inner = Rect.Size - new Vector2f(Host.Style.ScrollerThickness, Host.Style.ScrollerThickness);

        float len;
        FloatRect limits;
        len = float.Min(1, inner.X / child.Rect.Width) * inner.X;
        limits = new FloatRect(
            Rect.Left,
            Rect.Top + Rect.Height - Host.Style.ScrollerThickness,
            inner.X - len, 
            0
        );
        scrollerX.SetPosition(new Vector2f(limits.Left + scroll.X * limits.Width, limits.Top));
        scrollerX.SetSize(new Vector2f(len, Host.Style.ScrollerThickness), new Vector2f(Rect.Width, Host.Style.ScrollerThickness));
        scrollerX.SetLimits(limits);
    }

    private void UpdateScrollerYLayout()
    {
        Vector2f inner = Rect.Size - new Vector2f(Host.Style.ScrollerThickness, Host.Style.ScrollerThickness);

        float len = float.Min(1, inner.Y / child.Rect.Height) * inner.Y;
        FloatRect limits = new FloatRect(
            Rect.Left + Rect.Width - Host.Style.ScrollerThickness,
            Rect.Top,
            0,
            inner.Y - len
        );
        
        scrollerY.SetPosition(new Vector2f(limits.Left, limits.Top + scroll.Y * limits.Height));
        scrollerY.SetSize(new Vector2f(Host.Style.ScrollerThickness, len), new Vector2f(Host.Style.ScrollerThickness, Rect.Height));
        scrollerY.SetLimits(limits);
    }

    private void FinishDraw(RenderTarget target)
    {
        scrollerX.Draw(target);
        scrollerY.Draw(target);
        
        target.SetView(Host.Renderer.View);
    }
    
    public override void Draw(RenderTarget target)
    {
        if (child == null) return;
        
        target.SetView(view);
        
        Host.Renderer.PushDrawCall(FinishDraw);
        Host.Renderer.PushDrawCall(child.Draw);
    }
    
}