using SFML.System;
using SFML.Window;

namespace deUI;

public sealed class InputsHandler : IInputsHandler
{
    private float mouseWheelDelta;
    private Window listeningTo;

    public ClickAreasController Areas { get; private set; }

    private readonly Stack<Action> processorsStack = [];

    public InputsHandler()
    {
        Areas = new();
    }

    public void ListenTo(Window window)
    {
        if (listeningTo == window || window == null) return;
        
        window.MouseWheelScrolled += OnMouseWheelScrolled;
        listeningTo = window;
    }

    public void PushClickProcessor(Action processor)
    {
        processorsStack.Push(processor);
    }

    public bool IsKeyPressed(Keyboard.Key key)
    {
        return Keyboard.IsKeyPressed(key);
    }

    public bool IsLeftMouseButtonReleased()
    {
        return Mouse.IsButtonPressed(Mouse.Button.Left);
    }

    public float MouseWheelDelta()
    {
        return mouseWheelDelta;
    }

    public Vector2f MousePosition()
    {
        return (Vector2f)Mouse.GetPosition(listeningTo);
    }

    private void OnMouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e)
    {
        mouseWheelDelta = e.Y;
    }

    public void Begin()
    {
        Areas.Begin(MousePosition());
    }

    public void End()
    {
        while (processorsStack.TryPop(out Action processor))
        {
            processor.Invoke();
        }

        Areas.End();
    }
}