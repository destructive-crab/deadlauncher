using SFML.Graphics;
using SFML.System;

namespace deUI;

public sealed class UITree : IUITree
{
    private readonly UIHost Host;
    private AUIElement? root;

    public UITree(UIHost host)
    {
        Host = host;
    }

    public AUIElement? GetRoot()
    {
        return root;
    }

    public bool AssertRoot(out AUIElement root)
    {
        root = this.root;
        return this.root != null;
    }

    public void SetRoot(AUIElement newRoot)
    {
        root = newRoot;

        root.SetRect(new FloatRect(new Vector2f(0,0), Host.Renderer.GetSize()));
    }
}