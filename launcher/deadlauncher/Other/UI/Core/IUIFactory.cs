namespace deUI;

public interface IUIFactory
{
    TElement New<TElement>() where TElement : AUIElement;
}