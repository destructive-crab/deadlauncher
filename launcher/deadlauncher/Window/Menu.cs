using deUI;
using SFML.Graphics;

namespace deadlauncher;

public abstract class Menu
{
    public abstract bool HasBackButton();
    
    public abstract AUIElement GetRoot(FloatRect rect);

    public virtual void Update(RenderWindow window, FloatRect rect) {}
}