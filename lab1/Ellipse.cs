using System.Drawing;

class Ellipse : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public Ellipse(Color color, Point location, double width, double height)
    {
        Color = color;
        Location = location;
        Width = width;
        Height = height;
    }
    
    public override void Draw(Graphics graphics)
    {
        using (Pen pen = new Pen(Color))
        {
            graphics.DrawEllipse(pen, Location.X, Location.Y, (int)Width, (int)Height);
        }
    }
}