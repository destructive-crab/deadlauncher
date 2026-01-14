using leditor.UI;
using SFML.Graphics;
using SFML.System;

namespace deadlauncher;

public abstract class Menu
{
    public abstract AUIElement GetRoot(FloatRect rect);

    public virtual void Update(RenderWindow window, FloatRect rect) {}
}