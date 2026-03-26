using SFML.System;
using SFML.Window;

namespace deUI;

public interface IInputsHandler
{
    void     ListenTo                  (Window window);
    bool     IsKeyPressed              (Keyboard.Key key);
    bool     IsLeftMouseButtonReleased ();
    float    MouseWheelDelta           ();
    Vector2f MousePosition             ();
    
    void     PushClickProcessor        (Action processor);
    
    ClickAreasController Areas { get; }


    void Begin();
    void End();
}