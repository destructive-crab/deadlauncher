using deadlauncher;
using SFML.Graphics;
using SFML.System;

namespace deUI;

public class UIStyle
{
    public static readonly Color FirstBackgroundColor   = new(0x004671FF);
    public static readonly Color SecondBackgroundColor  = new(0x00253cFF);
    
    public readonly int          BaseOutline            = 5;
    public Color OutlineColor           = new(0xbbdde1FF);

    private static Font PrepareFont() => new(ResourcesHandler.Load("UI.ttf"));

    // Text
    public Font Font      = PrepareFont();
    public uint FontSize  = 16;
    
    // Rect
    public static readonly Color RectDefault  = FirstBackgroundColor ;
    
    //Label
    public readonly Color LabelColor  = new(0xbbdde1FF);
    
    //AxisBox
    public readonly int AxisBoxSpace  = 8;
    
    //SplitBox
    public readonly int SplitSeparatorThickness  = 2;
    public readonly Color SplitSeparatorColor  = new(0xeadb94FF);
    
    //Entry
    public readonly Color EntryBackgroundColor  = SecondBackgroundColor ;
    public readonly Color CursorColor  = new(0xeadb94FF);
    public readonly int CursorWidth  = 4;
    public readonly int BoxSizeX  = 4;
    public readonly int BoxSizeY  = 12;
    
    //Button
    public readonly Vector2f ButtonSpace = new(8, 8);
    
    public static readonly Color ButtonTop  = new Color(0xbbdde1FF);
    public static readonly Color ButtonBottom  = new Color(0xdc82bbFF);
    
    public static readonly Color ButtonPressed  = new Color(0x516276FF);
    public static readonly Color ButtonHovered  = new Color(0x778e97FF);
    
    public readonly ButtonStateStyle NormalButton  = new ()
    {
        ContentOffset = new Vector2f(0, 0),
        
        TextColor     = RectDefault ,
        
        TopColor      = ButtonTop ,
        BottomColor   = ButtonBottom ,
        
        Outline       = 0,
        
        BottomHeight  = 0
    };

    public readonly ButtonStateStyle HoveredButton  = new ()
    {
        ContentOffset = new Vector2f(0, 0),
        
        TextColor     = ButtonTop ,
        
        TopColor      = ButtonHovered ,
        BottomColor   = ButtonBottom ,
        
        BottomHeight  = 0
    };

    public readonly ButtonStateStyle PressedButton = new ()
    {
        ContentOffset = new Vector2f(0, 0),
        
        TextColor     = ButtonTop,
        
        TopColor      = ButtonPressed,
        BottomColor   = ButtonBottom,
        
        BottomHeight  = 0,
    };

    //ScrollBox
    public readonly float ScrollerThickness  = 16;
    public static readonly Color ScrollerColor  = ButtonTop ;
    public static readonly Color ScrollerPressedColor  = ButtonBottom ;
    
    //TabBox
    public readonly int TabLineHeight  = 30;
}