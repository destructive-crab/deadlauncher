using SFML.System;

namespace deUI;

public sealed class UIHost
{
    public TElement New<TElement>()
        where TElement : AUIElement
    {
        return Factory.New<TElement>();
    }
    
    public readonly IInputsHandler      InputsHandler;
    public readonly IUIFactory          Factory;
    public readonly IUIServiceContainer Services;
    public readonly IUITree             Tree;
    public readonly IUIRenderer         Renderer;
    
    public readonly UIStyle Style;

    public readonly UIFabric Fabric;

    public readonly Queue<Action> UpdateActionsQueue = [];

    public UIHost(UIStyle style, Vector2f size)
    {
        Style = style;

        Services      = new UIServiceContainer ();
        
        Fabric        = new UIFabric           (this);
        InputsHandler = new InputsHandler      ();
        Factory       = new UIFactory          (Services);
        Tree          = new UITree             (this);
        Renderer      = new UIRenderer         (Tree);

        Services.Register(this);
        Services.Register(InputsHandler);
        Services.Register(Factory);
        Services.Register(Tree);
        Services.Register(Renderer);
        
        Renderer.SetSize(size);
    }

    public void Update()
    {
        InputsHandler.Begin();
        {
            Tree.GetRoot()?.ProcessClicks();
        }
        InputsHandler.End();

        while (UpdateActionsQueue.TryDequeue(out var action))
        {
            action.Invoke();
        }
    }
}