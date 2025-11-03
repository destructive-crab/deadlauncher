using System.Numerics;
using Raylib_cs;

namespace deGUISpace;

public enum Anchor
{
    Center,
    
    CenterTop,
    CenterBottom,
    RightCenter,
    LeftCenter,
    
    LeftTop,
    LeftBottom,
    RightTop,
    RightBottom,
}

public static class deGUI
{
    public const int ORIGINAL_WIDTH = 300;
    public const int ORIGINAL_HEIGHT = 600;

    public static int ScreenWidth { get; private set; } = ORIGINAL_WIDTH;
    public static int ScreenHeight { get; private set; } = ORIGINAL_HEIGHT;

    public static readonly AreaManager Areas = new();
    public static readonly ButtonManager ButtonManager = new();

    public static GUIElement[] Elements => elements.ToArray();
    private static readonly List<GUIElement> elements = new();
    
    public static float WCF;
    public static float HCF;

    public static Button PushButton(Anchor anchor, int xOffset, int yOffset, int xScale, int yScale, string label, Action left, Action right)
    {
        Button button = new Button(anchor, xOffset, yOffset, xScale, yScale, label);
        
        elements.Add(button);
        AreaManager.RectGUIArea guiArea = Areas.BindRect(anchor, button.OffsetX, button.OffsetY, button.ScaleX, button.ScaleY, button.Label);
        button.AddCallback(left, right);
        button.ApplyArea(guiArea);

        return button;
    }

    public static GUIImage PushImage(Texture2D texture, Anchor anchor, int xOffset, int yOffset, int width, int height)
    {
        GUIImage image = new GUIImage(new AreaManager.RectGUIArea(anchor, xOffset, yOffset, width, height, ""), texture);
        
        elements.Add(image);

        return image;
    }
    
    public static void Draw()
    {
        WCF = (float)Raylib.GetScreenWidth() / ORIGINAL_WIDTH;
        HCF = (float)Raylib.GetScreenHeight() / ORIGINAL_HEIGHT;

        ScreenWidth = Raylib.GetScreenWidth();
        ScreenHeight = Raylib.GetScreenHeight();
        
        if (MathF.Abs(1 - WCF) > MathF.Abs(1 - HCF)) HCF = WCF;
        else WCF = HCF;
        
        Areas.Process();

        ButtonManager.ProcessInteractions();
        
        foreach (GUIElement element in elements)
        {
            if(!element.Active) continue;
            
            DrawElement(element);
        }
        
        //Areas.DrawDebugLayout();
    }

    private static void DrawElement(GUIElement element)
    {
        switch (element)
        {
            case Button button:
                DrawButton(button);
                break;
            case GUIImage image:
                DrawImage(image);
                break;
            case GUIText text:
                break;
        }
    }

    private static void DrawImage(GUIImage image)
    {
        Vector2 anchoredPosition = GUIUtil.AnchorPosition(image.GUIArea);
        Vector2 scale = new Vector2(GUIUtil.AdaptX(image.GUIArea.Width), GUIUtil.AdaptY(image.GUIArea.Height));
        
        Raylib.DrawTextureEx(image.Texture, anchoredPosition, 0, scale.X, Color.White);
    }

    private static void DrawButton(Button button)
    {
        Vector2 anchoredPosition = GUIUtil.AnchorPosition(button);
        Vector2 scale = new Vector2(GUIUtil.AdaptX(button.ScaleX), GUIUtil.AdaptY(button.ScaleY));
        
        Raylib.DrawRectangle((int)anchoredPosition.X-3, (int)anchoredPosition.Y-3, (int)(scale.X + 6), (int)(scale.Y + 6), Color.Black);
        Raylib.DrawRectangle((int)anchoredPosition.X, (int)anchoredPosition.Y, (int)scale.X, (int)scale.Y, button.Color);
        
        int count = button.Label.Length;

        int fontSize = (int)(scale.Y - 10);

        if (Raylib.MeasureText(button.Label, fontSize) >= scale.X)
        {
            fontSize = (int)(scale.X / count);

            if (Raylib.MeasureText(button.Label, fontSize) >= scale.X)
            {
                return;
            }
            
            if (fontSize >= scale.Y)
            {
                fontSize = (int)(scale.Y - 4);
            }           
        }

        int posX = (int)anchoredPosition.X + (int)((scale.X - Raylib.MeasureText(button.Label, fontSize)) / 2);
        int posY = (int)anchoredPosition.Y + (int)(scale.Y - fontSize) / 2;

        Raylib.DrawText(button.Label, posX, posY, fontSize, Color.Black);
    }
}