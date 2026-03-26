namespace deUI;

public interface IUITree
{
    AUIElement? GetRoot();
    bool        AssertRoot(out AUIElement root);
    void        SetRoot(AUIElement newRoot);
}