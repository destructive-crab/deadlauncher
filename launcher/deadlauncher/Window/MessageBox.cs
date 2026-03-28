using deUI;
using SFML.Graphics;
using SFML.System;

namespace deadlauncher;

public sealed class MessageBox : AUIElement
{
    private UIRect background;
    private StackBox stackBox;

    private UITextBox textBox;
    private UIButton[] buttons;

    public MessageBox(UIHost host) : base(host)
    {
        MinimalSize = new Vector2f(
            Application.Launcher.Window.UIHost.Renderer.GetSize().X / 1.5f,
            Application.Launcher.Window.UIHost.Renderer.GetSize().Y / 3f);
    }

    public MessageBox WithMessage(string message)
    {
        textBox = Host.New<UITextBox>().WithText(message).WithAlignCenter(true);
        textBox.SetInheritRect(true);

        return this;
    }

    public MessageBox WithButtons(Tuple<string, Action>[] tuples)
    {
        buttons = new UIButton[tuples.Length];
        
        for (var i = 0; i < tuples.Length; i++)
        {
            buttons[i] = Host.New<UIButton>().WithText(tuples[i].Item1).OnClick(tuples[i].Item2);
        }

        return this;
    }

    public MessageBox FinishConfiguration()
    {
        Anchor buttonsLineAnchor = new Anchor(new FloatRect(20, -50, -40, 0), new FloatRect(0, 1, 1, 0));

        var boxBackground = Host
            .New<UIRect>()
            .WithOutline(2)
            .WithBlockClicks(true)
            .SetInheritRect(true);
        
        var buttonsLine = Host
            .New<AxisBox>()
            .WithAxis(UIAxis.Horizontal)
            .FitRect(true)
            .WithChildren(buttons);
        
        stackBox = Host.New<StackBox>().WithChildren(
        [
            boxBackground,
            textBox,
            Host.New<AnchorBox>().WithChild(buttonsLineAnchor, buttonsLine).SetInheritRect(true),
        ]);

        background = Host.New<UIRect>().WithColor(new Color(0x00000080)).WithBlockClicks(true);

        UpdateLayout();
        
        return this;
    }
    
    public override void ProcessClicks()
    {
        Host.InputsHandler.PushClickProcessor(background.ProcessClicks);
        Host.InputsHandler.PushClickProcessor(stackBox.ProcessClicks);
    }

    protected override void UpdateLayoutIm()
    {
        Vector2f boxPosition = GetRect().Position + GetRect().Size/2f - stackBox.GetRect().Size/2f;
        
        stackBox.SetRect(new FloatRect(boxPosition,  MinimalSize));
        background.SetRect(GetRect());
        
        background.UpdateLayout();
        stackBox.UpdateLayout();
    }

    public override void Draw(RenderTarget target)
    {
        background.Draw(target);
        stackBox.Draw(target);
    }
}