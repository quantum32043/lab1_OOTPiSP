using System.Drawing;

class Line : Shape
{
    private double Length { get; set; }

    public Line(Color color, Point location, double length)
    {
        Color = color;
        Location = location;
        Length = length;
    }
    
    public override void Draw(Graphics graphics)
    {
        using (Pen pen = new Pen(Color))
        {
            int x2 = Location.X + (int)(Length * Math.Cos(Math.PI / 4));
            int y2 = Location.Y + (int)(Length * Math.Sin(Math.PI / 4));
            
            graphics.DrawLine(pen, Location, new Point(x2, y2));
        }
    }
}