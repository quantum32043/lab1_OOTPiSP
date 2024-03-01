using System.Drawing;

class Rectangle : Shape
{
    private double Width { get; set; }
    private double Height { get; set; }

    public Rectangle(Color color, Point location, double width, double height)
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
            graphics.DrawRectangle(pen, Location.X, Location.Y, (int)Width, (int)Height);
        }
    }
}