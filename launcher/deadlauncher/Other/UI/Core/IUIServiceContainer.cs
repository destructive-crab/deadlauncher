namespace deUI;

public interface IUIServiceContainer
{
    TService? Get<TService>               ()                where TService : class;
    object?   Get                         (Type serviceType);

    object[] GetAll                       ();
    
    
    void     Register<TService>           (TService service) where TService : class;
    void     RemoveFromRegistry<TService> ()                 where TService : class;
}