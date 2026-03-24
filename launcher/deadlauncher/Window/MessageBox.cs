using leditor.UI;
using SFML.Graphics;
using SFML.System;

namespace deadlauncher;

public sealed class MessageBox : AUIElement
{
    private readonly UIButton[] buttons;
    private readonly StackBox   stackBox;
    
    public MessageBox(string message, Tuple<string, Action>[] tuples) 
        : base(Application.Launcher.Window.UIHost, new Vector2f(Application.Launcher.Window.UIHost.Size.X / 1.5f, Application.Launcher.Window.UIHost.Size.Y / 3f))
    {
        buttons = new UIButton[tuples.Length];
        
        for (var i = 0; i < tuples.Length; i++)
        {
            buttons[i] = new UIButton(Application.Launcher.Window.UIHost, tuples[i].Item1, tuples[i].Item2);
        }

        UIHost h = Application.Launcher.Window.UIHost;
        
        stackBox = new StackBox(h,
        [
            new UIRect (h, null, new Vector2f(2,2)).BlockClicks().SetInheritRect(true),
            new UITextBox(h, message).SetAlignCenter(true).SetInheritRect(true),
            new AnchorBox(h).AddChild(new Anchor(new FloatRect(20, -50, -40, 0), new FloatRect(0, 1, 1, 0)), 
                new AxisBox  (h, UIAxis.Horizontal, true, buttons)).SetInheritRect(true),
        ]);
        stackBox.SetInheritRect(true);

        SetInheritRect(true);
    }

    public override void ProcessClicks()
    {
        base.ProcessClicks();
        stackBox.ProcessClicks();
    }

    public override void UpdateLayout()
    {
        var boxPosition = Rect.Position + Rect.Size/2f - stackBox.Rect.Size/2f;
        
        stackBox.SetRect(new FloatRect(boxPosition,  MinimalSize));
        stackBox.UpdateLayout();
    }

    public override void Draw(RenderTarget target)
    {
        stackBox.Draw(target);
    }
}