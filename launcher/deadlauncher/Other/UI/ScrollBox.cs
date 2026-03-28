using SFML.Graphics;
using SFML.System;

namespace deUI;

public sealed class ScrollBox : AUIBox
{
    sealed class ScrollBar
    {
        public UIAxis Axis;
        private Vector2i axis;
        public event Action<float> OnMoved;
        
        public float      CurrentScroll  { get; private set; }
        
        public Vector2f   StartPosition;
        public float      MoveLimit
        {
            get
            {
                switch (Axis)
                {
                    case UIAxis.Vertical:
                        return AttachedTo.GetRect().Size.Y - barShape.Size.Y;
                    case UIAxis.Horizontal:
                        return AttachedTo.GetRect().Size.X - barShape.Size.X;
                }

                return 0;
            }
        }

        private RectangleShape barShape;
        private RectangleShape backgroundShape;
        
        private readonly ScrollBox AttachedTo;

        private readonly UIStyle   Style;
        
        private readonly ClickArea ClickArea;
        
        private readonly ClickArea decreaseButton;
        private readonly ClickArea increaseButton;
        
        public ScrollBar(UIStyle style, ScrollBox box, UIAxis axis)
        {
            ClickArea = new ClickArea();
            ClickArea.Overlay = true;

            Axis      = axis;
            switch (Axis)
            {
                case UIAxis.Vertical:
                    this.axis = new(0, 1);
                    break;
                case UIAxis.Horizontal:
                    this.axis = new(1, 0);
                    break;
            }

            Style = style;
            AttachedTo = box;
            
            barShape           = new RectangleShape();
            barShape.FillColor = UIStyle.ScrollerColor;
            
            backgroundShape           = new RectangleShape();          
            backgroundShape.FillColor = new Color(0x00000030);
            
            ClickArea.OnMove = OnMove;
        }

        public float GetDelta(Vector2f oldPosition, Vector2f newPosition)
        {
            switch(Axis)
            {
                case UIAxis.Vertical:   return newPosition.Y -= oldPosition.Y;
                case UIAxis.Horizontal: return newPosition.X -= oldPosition.X;
            }

            return 0;
        }

        private void OnMove(Vector2f oldPosition, Vector2f newPosition)
        {
            float prevScroll = CurrentScroll;
            float delta      = GetDelta(oldPosition, newPosition);

            CurrentScroll += delta;
            
            if (CurrentScroll < 0)
            {
                if (prevScroll > 0)
                {
                    OnMoved.Invoke((0 - prevScroll)/ (AttachedTo.GetRect().Size.Y / AttachedTo.Child.GetRect().Size.Y));
                    CurrentScroll = 0;
                    UpdateLayout();
                }
                
                CurrentScroll = 0;
                
                return;
            }
            
            if (CurrentScroll > MoveLimit)
            {
                if (prevScroll < MoveLimit)
                {
                    OnMoved.Invoke((MoveLimit - prevScroll)/ (AttachedTo.GetRect().Size.Y / AttachedTo.Child.GetRect().Size.Y));
                    CurrentScroll = MoveLimit;         
                    UpdateLayout();
                }
                
                CurrentScroll = MoveLimit;         
                
                return;
            }

            OnMoved.Invoke(delta/ (AttachedTo.GetRect().Size.Y / AttachedTo.Child.GetRect().Size.Y));
            UpdateLayout();
        }

        private void Select()
        {
            barShape.FillColor = UIStyle.ScrollerPressedColor;
        }   

        private void Deselect()
        {
            barShape.FillColor = UIStyle.ScrollerColor;
        }
        
        public void UpdateLayout()
        {
            StartPosition = AttachedTo.GetRect().Position + new Vector2f(AttachedTo.GetRect().Size.X - Style.ScrollerThickness, 0);
            
            barShape.Size = new Vector2f(
                Style.ScrollerThickness, 
                AttachedTo.GetRect().Size.Y * AttachedTo.GetRect().Size.Y/AttachedTo.Child.GetRect().Size.Y);
            
            backgroundShape.Size      = new Vector2f(Style.ScrollerThickness, AttachedTo.GetRect().Size.Y);
            backgroundShape.Position  = StartPosition;
            
            switch (Axis)
            {
                case UIAxis.Vertical:
                    barShape.Position = StartPosition + new Vector2f(0, CurrentScroll);
                    break;
                case UIAxis.Horizontal:
                    barShape.Position = StartPosition + new Vector2f(CurrentScroll, 0);
                    break;
            }
            
            ClickArea.Rect = new FloatRect(barShape.Position, barShape.Size);
        }

        public void ProcessClicks(IInputsHandler handler)
        {
            handler.Areas.Process(ClickArea);
        }
        
        public void Draw(RenderTarget target)
        {
            if (ClickArea.IsGrabbed)
            {
                Select();
            }
            else
            {
                Deselect();
            }
            
            target.Draw(backgroundShape);
            target.Draw(barShape);
        }
    }

    public  AUIElement Child { get; private set; }
    private View       view;

    private IUIRenderer renderer;

    private ScrollBar yScroller;
    
    public ScrollBox(UIHost host, IUIRenderer renderer) : base(host)
    {
        this.renderer = renderer;
        yScroller = new ScrollBar(host.Style, this, UIAxis.Vertical);
        yScroller.OnMoved += YScrollerOnOnMoved;
    }

    private void YScrollerOnOnMoved(float delta)
    {
        view.Move(new Vector2f(0, delta));
    }

    public ScrollBox WithChild(AUIElement newChild)
    {
        Child = newChild;
        Child.SetParent(this);

        if (Child.InheritRect)
        {
            Child.SetInheritRect(false);
        }

        return this;
    }

    public override void ProcessClicks()
    {
        yScroller.ProcessClicks(Host.InputsHandler);
    }

    protected override void UpdateLayoutIm()
    {
        if (Child == null) return;
        
        Child.SetRect(new FloatRect(GetRect().Position, Child.GetRect().Size - new Vector2f(Host.Style.ScrollerThickness, 0)));

        view = new View();

        view.Size = Host.Renderer.View.Size;
        view.Center = Host.Renderer.View.Center;
        
        view.Center = GetRect().Position + GetRect().Size / 2;
        view.Size   = GetRect().Size;

        view.Viewport = new FloatRect(
            GetRect().Left   / renderer.GetSize().X,
            GetRect().Top    / renderer.GetSize().Y,
            GetRect().Width  / renderer.GetSize().X,
            GetRect().Height / renderer.GetSize().Y);
        
        Child?.UpdateLayout();
        yScroller.StartPosition = GetRect().Position;
        yScroller.UpdateLayout();
    }

    public override void Draw(RenderTarget target)
    {
        if (Child == null) return;
        
        target.SetView(view);

        renderer.PushDrawCallToStack(yScroller.Draw);
        renderer.PushDrawCallToStack(FinishScrollBox);
        renderer.PushDrawCallToStack(Child.Draw);
    }

    private void FinishScrollBox(RenderTarget target)
    {
        target.SetView(renderer.View);
    }

    public override IEnumerable<AUIElement> GetChildren()
    {
        if(Child != null) return [Child];
        else              return Array.Empty<AUIElement>();
    }

    public override void RemoveChild(AUIElement child)
    {
    }

    protected override void UpdateMinimalSize()
    {
    }
}