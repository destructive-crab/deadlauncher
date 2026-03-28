using System.Reflection;

namespace deUI;

public sealed class UIFactory : IUIFactory
{
    private readonly IUIServiceContainer container;
    private readonly UIHost host;

    public UIFactory(UIHost uiHost)
    {
        host = uiHost;
        container = uiHost.Services;
    }

    public TElement New<TElement>()
        where TElement : AUIElement
    {
        Type elementType = typeof(TElement);
        
        ConstructorInfo[] constructors = elementType.GetConstructors();

        if (constructors.Length == 0)
        {
            return Activator.CreateInstance(elementType) as TElement;
        }
        
        if (constructors.Length > 1)
        {
            Console.WriteLine($"Incorrect constructors count in {elementType.Name} ({constructors.Length}). " +
                              $"UI Elements must have only one constructor, which will be used in factory. " +
                              $"In constructor you should place services that are registered in UI DI Container");
        }

        ConstructorInfo constructor = constructors[0];

        ParameterInfo[] parameters = constructor.GetParameters();
        object[]        values     = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            values[i] = container.Get(parameters[i].ParameterType);
        }

        var element = (TElement)constructor.Invoke(values);
        element.OnHostSizeChanged(host.Renderer.GetSize());
        return element;
    }
}