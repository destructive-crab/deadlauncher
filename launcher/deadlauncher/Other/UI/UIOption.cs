namespace deUI;

public sealed class UIOption : UIButton
{
    private bool isSelected;

    public event Action<UIOption> SelectionChange;
    public bool IsSelected
    {
        get
        {
            return isSelected;
        }
        set
        {
            isSelected = value;
            
            if (isSelected)
            {
                base.ApplyStyle(Host.Style.PressedButton);
            }
            else
            {
                base.ApplyStyle(Host.Style.NormalButton);
            }
            
            SelectionChange?.Invoke(this);
        }
    }

    public bool IsLocked;
    
    public UIOption(UIHost host) : base(host)
    {
        ApplyStyle(host.Style.NormalButton);
    }

    public UIOption SetState(bool state)
    {
        IsSelected = state;
        return this;
    }
    
    protected override void OnReleased()
    {
        if(IsLocked) return;
        
        base.OnReleased();
        IsSelected = !IsSelected;
    }

    protected override void ApplyStyle(ButtonStateStyle style)
    {
        if (IsSelected)
        {
            base.ApplyStyle(appliedStyle);
        }
        else
        {
            base.ApplyStyle(style);
        }
    }
}