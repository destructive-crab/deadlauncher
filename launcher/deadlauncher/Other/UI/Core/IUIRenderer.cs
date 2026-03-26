using SFML.Graphics;
using SFML.System;

namespace deUI;

public delegate void UIDrawCall(RenderTarget target);

public interface IUIRenderer
{

    Vector2f GetSize     ();
    void     SetSize     (Vector2f size);
    
    void     Draw        (RenderTarget target);
    
    void     PushDrawCall(UIDrawCall call);
    View View { get; }
}