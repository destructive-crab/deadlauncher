using deadlauncher;
using SFML.Graphics;
using SFML.System;

namespace leditor.UI;

public class UIStyle()
{
    public static readonly int   BaseOutline = 5;
    
    public static readonly Color FirstBackgroundColor  = new(0x004671FF);
    public static readonly Color SecondBackgroundColor = new(0x00253cFF);
    public static readonly Color OutlineColor          = new(0xbbdde1FF);
    
    private static Font PrepareFont() => new(ResourcesHandler.Load("UI.ttf"));

    // Text
    public readonly Font Font = PrepareFont();
    public readonly uint FontSize = 24;
    
    // Rect
    public static readonly Color RectDefault = FirstBackgroundColor;
    
    // Label
    public static readonly Color LabelColor = new(0xbbdde1FF);
    
    // AxisBox
    public readonly int AxisBoxSpace = 5;
    
    // SplitBox
    public readonly int   SplitSeparatorThickness = 2;
    public readonly Color SplitSeparatorColor = new(0xeadb94FF);
    
    // UI Entry
    public readonly Color EntryBackgroundColor = new(0x240e1dFF);
    public readonly Color CursorColor = new(0xeadb94FF);
    public readonly int   CursorWidth = 4;
    public readonly int   BoxSizeX = 4;
    public readonly int   BoxSizeY = 12;
    
    // Button
    public readonly Vector2f ButtonSpace = new(8, 8);

    public static Color ButtonTop = new Color(0xbbdde1FF);
    public static Color ButtonBottom = new Color(0xdc82bbFF);
    
    public static Color ButtonPressed = new Color(0x516276FF);
    public static Color ButtonHovered = new Color(0x778e97FF);
    
    public readonly ButtonStateStyle NormalButton = new()
    {
        ContentOffset = new Vector2f(0, 0),
        
        TextColor     = RectDefault,
        
        TopColor      = ButtonTop,
        BottomColor   = ButtonBottom,
        
        Outline       = 0,
        
        BottomHeight  = 0
    };

    public readonly ButtonStateStyle HoveredButton = new()
    {
        ContentOffset = new Vector2f(0, 0),
        
        TextColor     = ButtonTop,
        
        TopColor      = ButtonHovered,
        BottomColor   = ButtonBottom,
        
        BottomHeight  = 0
    };

    public readonly ButtonStateStyle PressedButton = new()
    {
        ContentOffset = new Vector2f(0, 0),
        
        TextColor     = ButtonTop,
        
        TopColor      = ButtonPressed,
        BottomColor   = ButtonBottom,
        
        BottomHeight  = 0,
    };

    //ScrollBox
    public readonly float ScrollerThickness = 16;
    public readonly Color ScrollerColor = ButtonTop;
    public readonly Color ScrollerPressedColor = ButtonBottom;
    
    //TabBox
    public readonly int TabLineHeight = 30;
}