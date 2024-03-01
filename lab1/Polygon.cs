using System.Drawing;

class Polygon : Shape
{
    private List<Point> Points { get; set; }
    
    public Polygon(Color color, Point location, List<Point> points)
    {
        Color = color;
        Location = location;
        Points = points;
    }

    public override void Draw(Graphics graphics)
    {
        using (Pen pen = new Pen(Color))
        {
            graphics.DrawPolygon(pen, Points.ToArray());
        }
    }
}