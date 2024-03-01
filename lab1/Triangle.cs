using System.Drawing;

class Triangle : Shape
{
    private double SideA { get; set; }
    private double SideB { get; set; }
    private double SideC { get; set; }
    
    public Triangle(Color color, Point location, double sideA, double sideB, double sideC)
    {
        Color = color;
        Location = location;
        SideA = sideA;
        SideB = sideB;
        SideC = sideC;
    }
    
    public override void Draw(Graphics graphics)
    {
        using (Pen pen = new Pen(Color))
        {
            int x1 = Location.X;
            int y1 = Location.Y;
            int x2 = Location.X + (int)SideA;
            int y2 = Location.Y;
            int x3 = Location.X + (int)(SideB * Math.Cos(Math.PI / 3));
            int y3 = Location.Y + (int)(SideB * Math.Sin(Math.PI / 3));
            
            graphics.DrawLine(pen, x1, y1, x2, y2);
            graphics.DrawLine(pen, x2, y2, x3, y3);
            graphics.DrawLine(pen, x3, y3, x1, y1);
        }
    }
}