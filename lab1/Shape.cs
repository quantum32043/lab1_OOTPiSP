using System.Drawing;

public abstract class Shape
{
    protected Color Color { get; set; } 
    protected Point Location { get; set; } 

    public abstract void Draw(Graphics graphics);
}