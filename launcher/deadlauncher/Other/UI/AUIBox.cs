namespace deUI;

public abstract class AUIBox : AUIElement
{
    protected AUIBox(UIHost host) : base(host)
    {
    }

    public abstract IEnumerable<AUIElement> GetChildren();

    public abstract void RemoveChild(AUIElement child);

    public void RemoveAllChildren()
    {
        IEnumerable<AUIElement> all = GetChildren();
        all = new List<AUIElement>(all);
        
        foreach (AUIElement element in all)
        {
            RemoveChild(element);
        }
    }

    protected abstract void UpdateMinimalSize();

    public override void ProcessClicks()
    {
        IEnumerable<AUIElement> children = GetChildren();
        
        foreach (var child in children)
        {
            Host.InputsHandler.PushClickProcessor(child.ProcessClicks);
        }
    }

    public void OnChildUpdate()
    {
        var size = MinimalSize;
        UpdateMinimalSize();
        
        if (size == MinimalSize)
        {
            Host.UpdateActionsQueue.Enqueue(UpdateLayout);
        }
    }
}