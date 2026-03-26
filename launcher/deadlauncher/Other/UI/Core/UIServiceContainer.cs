namespace deUI;

public sealed class UIServiceContainer : IUIServiceContainer
{
    private readonly Dictionary<Type, object> registry = new();

    public TService? Get<TService>()
        where TService : class
    {
        if(registry.TryGetValue(typeof(TService), out var value))
        {
            return value as TService;
        }

        return null;
    }

    public object? Get(Type serviceType)
    {
        if (registry.TryGetValue(serviceType, out var value))
        {
            return value;
        }
        else
        {
            throw new Exception($"Service of type {serviceType.Name} is not presented in registry");
        }
    }
    
    public object[] GetAll()
    {
        return registry.Values.ToArray();
    }

    public void Register<TService>(TService service)
        where TService : class
    {
        Type serviceType = typeof(TService);
        
        if (service == null)
        {
            Console.WriteLine($"Null service provided as {serviceType.Name} in registration");
            return;
        }

        if (registry.ContainsKey(serviceType))
        {
            Console.WriteLine($"Service {serviceType.Name} already in registry");
            return;
        }
        
        registry.Add(serviceType, service);
    }

    public void RemoveFromRegistry<TService>()
        where TService : class
    {
        Type serviceType = typeof(TService);
        
        if (registry.ContainsKey(serviceType))
        {
            Console.WriteLine($"Service {serviceType.Name} not presented in registry");
            return;
        }

        registry.Remove(serviceType);
    }
}