using System.Drawing;

class Program
{
    static void Main(string[] args)
    {
        ShapeList shapeList = new ShapeList();
        shapeList.Add(new Line(Color.Red, new Point(10, 10), 100));
        shapeList.Add(new Rectangle(Color.Green, new Point(50, 50), 200, 100));
        shapeList.Add(new Ellipse(Color.Blue, new Point(100, 100), 150, 80));
        shapeList.Add(new Triangle(Color.Yellow, new Point(150, 150), 120, 80, 100));
        shapeList.Add(new Polygon(Color.Pink, new Point(200, 200), new List<Point>()
        {
            new Point(200, 200),
            new Point(250, 250),
            new Point(300, 200),
            new Point(250, 150)
        }));
        shapeList.Add(new Circle(Color.Purple, new Point(300, 300), 50));

        using (Bitmap bitmap = new Bitmap(500, 500))
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                shapeList.DrawAll(graphics);
            }
            bitmap.Save("../../../shapes.png");
        }

        Console.WriteLine("Изображение сохранено в файле shapes.png");
    }
}
