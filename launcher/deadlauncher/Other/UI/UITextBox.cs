using SFML.Graphics;
using SFML.System;

namespace leditor.UI;

public class UITextBox : AUIElement
{
    public string Text
    {
        get => textOriginal.DisplayedString;
        set
        {
            textOriginal.DisplayedString = value;
            MinimalSize = Utils.TextSize(textOriginal);
        }
    }

    private Text textOriginal;
    private List<Text> totalLines = new();
    private int linesCount = -1;
    private string displayString;

    public UITextBox(UIHost host, string text = "") : 
        base(host, default)
    {
        this.textOriginal = host.Fabric.MakeText("X");
        displayString = text;
    }

    private int currentWidth;

    public override void UpdateLayout()
    {
        textOriginal.Position = Rect.Position;

        int textWidth = (int)Rect.Width;
        
        if (textWidth != currentWidth)
        {
            currentWidth = textWidth;
            BuildLines();
        }

        if (linesCount > 0 && totalLines[0].Position != Rect.Position)
        {
            UpdatePositions();
        }
    }

    private void BuildLines()
    {
        int textWidth = (int)Rect.Width;
        List<string> lines = new();
        string[] words = displayString.Split(" ");

        textOriginal.DisplayedString = words[0];
        for (var i = 1; i < words.Length; i++)
        {
            string word = words[i];
            string prevStr = textOriginal.DisplayedString;

            textOriginal.DisplayedString += " " + word;
            
            if (textOriginal.GetGlobalBounds().Size.X >= textWidth)
            {
                lines.Add(prevStr);
                textOriginal.DisplayedString = word;
            }
        }
        if(textOriginal.DisplayedString != "")
        {
            lines.Add(textOriginal.DisplayedString);
        }

        textOriginal.DisplayedString = "";

        Text prev = null;
        Vector2f minimalSize = new Vector2f(textWidth, 0);
        
        for (var i = 0; i < lines.Count; i++)
        {
            string line = lines[i];
            
            if (i >= totalLines.Count) totalLines.Add(new Text(textOriginal));

            totalLines[i].DisplayedString = line;
            
            prev = totalLines[i];
            minimalSize.Y += prev.GetGlobalBounds().Height;
        }

        linesCount = lines.Count;
        MinimalSize = minimalSize;
        Rect = new FloatRect(Rect.Position, MinimalSize);
        
        UpdatePositions();
    }

    private void UpdatePositions()
    {
        Text prev = null;
        for (var i = 0; i < linesCount; i++)
        {
            if (prev == null)
            {
                totalLines[i].Position = Rect.Position;
            }
            else
            {
                int height = (int)(prev.GetGlobalBounds().Height);
                if (height == 0) height = (int)Host.Style.FontSize* i;
                
                totalLines[i].Position = prev.Position + new Vector2f(0, height);
            }
            
            prev = totalLines[i];
        }
    }

    public override void Draw(RenderTarget target)
    {
        for (var i = 0; i < linesCount; i++)
        {
            var line = totalLines[i];
            target.Draw(line);
        }
    }
}