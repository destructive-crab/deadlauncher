namespace deGUISpace;

public abstract class GUIElement
{
    public AreaManager.RectGUIArea GUIArea { get; set; }
    public GUIElement Parent { get; }
    public bool Active { get; private set; } = true;

    public virtual void Hide()
    {
        Active = false;
    }

    public virtual void Show()
    {
        Active = true;
    }
}