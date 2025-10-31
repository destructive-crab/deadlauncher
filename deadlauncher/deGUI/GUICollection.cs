namespace deGUISpace;

public class GUICollection : GUIElement
{
    public GUIElement Parent { get; set; }
    public bool Active { get; private set; }
    
    public void Hide()
    {
        Active = false;
    }

    public void Show()
    {
        Active = true;
    }
}