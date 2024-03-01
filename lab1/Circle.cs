using System.Drawing;

class Circle : Shape
{
    private double Radius { get; set; }

    public Circle(Color color, Point location, double radius)
    {
        Color = color;
        Location = location;
        Radius = radius;
    }
    
    public override void Draw(Graphics graphics)
    {
        using (Pen pen = new Pen(Color))
        {
            graphics.DrawEllipse(pen, Location.X - (int)Radius, Location.Y - (int)Radius, (int)(2 * Radius), (int)(2 * Radius));
        }
    }
}