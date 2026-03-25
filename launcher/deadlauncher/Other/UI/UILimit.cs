using SFML.Graphics;
using SFML.System;

namespace deUI;

public class UILimit(UIHost host, Vector2f minimalSize) : AUIElement(host, minimalSize)
{
    public override void UpdateLayout() {}

    public override void Draw(RenderTarget target) {}
}