using SFML.Graphics;
using SFML.System;

namespace deadlauncher;

public sealed class RunningLine<TElement>
    where TElement : Transformable, Drawable
{
    public TElement BaseElementShared => clone(baseElement);
        
    private TElement baseElement;
        
    private int lineLength;
    private int elementWidth;

    private Func<TElement, TElement> clone;
        
    private readonly List<TElement> line = new();

    public void SetElementWidth(int width)
    {
        elementWidth = width;
    }
        
    public void Clear()
    {
        line.Clear();
        baseElement = null;
        lineLength = 0;
        elementWidth = 0;
        clone = null;
    }
        
    public void Build(TElement drawable, int lineWidth, int x, int y, int elementWidth, Func<TElement, TElement> clone)
    {
        Clear();
            
        baseElement = drawable;
        this.lineLength = lineWidth;
        this.elementWidth = elementWidth;
        this.clone = clone;

        int elementsCount = (lineLength / elementWidth) + 2;
        for (int i = 0; i < elementsCount; i++)
        {
            line.Add(clone(baseElement));
        }
            
        UpdatePositions(new Vector2f(x, y));
    }

    public void Foreach(Action<TElement> each)
    {
        foreach (TElement element in line)
        {
            each.Invoke(element);
        }
    }

    public void UpdatePositions(Vector2f leadPosition)
    {
        line[0].Position = leadPosition;
        TElement prev = null;
            
        foreach (TElement element in line.ToArray())
        {
            if(prev == null)
            {
                prev = element;
                continue;
            }

            element.Position = new Vector2f(prev.Position.X - elementWidth, prev.Position.Y);
            prev = element;
        }
    }
        
    public void MovePositions(float delta)
    {
        line[0].Position = new Vector2f(line[0].Position.X + delta, line[0].Position.Y);

        if (line[0].Position.X > lineLength)
        {
            TElement tmp = line[0];
            line.RemoveAt(0);
            line.Add(tmp);
        }
            
        line[0].Position = new Vector2f(line[0].Position.X + delta, line[0].Position.Y);
            
        TElement prev = null;
            
        foreach (TElement element in line.ToArray())
        {
            if(prev == null)
            {
                prev = element;
                continue;
            }

            element.Position = new Vector2f(prev.Position.X - elementWidth, prev.Position.Y);
            prev = element;
        }
    }

    public void Draw(RenderTarget target)
    {
        foreach (TElement drawable in line)
        {
            target.Draw(drawable);
        }
    }
}