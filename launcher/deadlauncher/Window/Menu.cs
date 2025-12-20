using leditor.UI;
using SFML.Graphics;

namespace deadlauncher;

public abstract class Menu
{
    public abstract AUIElement GetRoot();

    public abstract void Update(RenderWindow window);
}