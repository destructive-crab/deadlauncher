using Raylib_cs;

namespace deGUISpace;

public class Button : GUIElement
{
    public Anchor Anchor;
    
    public int OffsetX;
    public int OffsetY; 
    
    public int ScaleX;
    public int ScaleY;
    
    //contents
    public string Label = "";
    
    public Texture2D Texture;
    public Color Color = Color.White;

    //interactions
    private Action RightCallbacks;
    private Action LeftCallbacks;

    public void ApplyArea(AreaManager.GUIArea area)
    {
        GUIArea = area as AreaManager.RectGUIArea;
    }
    
    public Button(int offsetX, int offsetY, int scaleX, int scaleY, string label)
    {
        OffsetX = offsetX;
        OffsetY = offsetY;
        ScaleX = scaleX;
        ScaleY = scaleY;
        Label = label;
    }

    public Button(Anchor anchor, int offsetX, int offsetY, int scaleX, int scaleY, string label = "")
    {
        Anchor = anchor;
        OffsetX = offsetX;
        OffsetY = offsetY;
        ScaleX = scaleX;
        ScaleY = scaleY;
        Label = label;
    }

    public void AddCallback(Action right, Action left)
    {
        if(right != null) RightCallbacks += right;
        if(left != null) LeftCallbacks += left;
    }

    public void InvokeRightClick()
    {
        RightCallbacks?.Invoke();
    }

    public void InvokeLeftClick()
    {
        LeftCallbacks?.Invoke();
    }

    public GUIElement Parent { get; set; }
    public bool Active { get; private set; }
    public void Hide() => Active = false;
    public void Show() => Active = true;
}