using SFML.Graphics;
using SFML.System;

namespace leditor.UI;

public class UIStyle()
{
    private static Font PrepareFont()
    {
        Font font 
        = new("C:\\Users\\destructive_crab\\dev\\band-bang\\leditor\\leditor\\assets\\Main.ttf");

        return font;
    }
    
    // Text
    public readonly Font Font = PrepareFont();
    public readonly uint FontSize = 24;
    
    // Rect
    public static readonly Color RectDefault = new(0x00363cFF);
    
    // Label
    public readonly Color LabelColor = new(0xbbdde1FF);
    
    // AxisBox
    public readonly int AxisBoxSpace = 5;
    
    // SplitBox
    public readonly int SplitSeparatorThickness = 2;
    public readonly Color SplitSeparatorColor = new(0xeadb94FF);
    
    // UI Entry
    public readonly Color EntryBackgroundColor = new(0x240e1dFF);
    public readonly Color CursorColor = new(0xeadb94FF);
    public readonly int CursorWidth = 4;
    public readonly int BoxSizeX = 4;
    public readonly int BoxSizeY = 12;
    
    // Button
    public readonly Vector2f ButtonSpace = new(8, 8);

    public static Color ButtonTop = new Color(0xbbdde1FF);
    public static Color ButtonBottom = new Color(0xdc82bbFF);
    
    public readonly ButtonStateStyle NormalButton = new()
    {
        ContentOffset = new Vector2f(4, 4),
        
        TextColor     = RectDefault,
        
        TopColor      = ButtonTop,
        BottomColor   = ButtonBottom,
        
        Outline       = 2,
        
        BottomHeight  = 10
    };

    public readonly ButtonStateStyle HoveredButton = new()
    {
        ContentOffset = new Vector2f(4, 4),
        
        TextColor     = RectDefault,
        
        TopColor      = ButtonTop,
        BottomColor   = ButtonBottom,
        
        OutlineColor  = new Color(0xfdea70FF),
        Outline       = 2,
        
        BottomHeight  = 10

    };

    public readonly ButtonStateStyle PressedButton = new()
    {
        ContentOffset = new Vector2f(4, 4),
        
        TextColor     = RectDefault,
        
        TopColor      = ButtonTop,
        BottomColor   = ButtonBottom,
        
        BottomHeight  = 2,
        
        Outline       = 2,
        OutlineColor =  new Color(0xfdea70FF),
    };

    //ScrollBox
    public readonly float ScrollerThickness = 16;
    public readonly Color ScrollerColor = ButtonTop;
    public readonly Color ScrollerPressedColor = ButtonBottom;
}