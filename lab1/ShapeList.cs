using System.Drawing;

class ShapeList
{
    private List<Shape> Shapes { get; set; }

    public ShapeList()
    {
        Shapes = new List<Shape>();
    }
    
    public void Add(Shape shape)
    {
        Shapes.Add(shape);
    }
    
    public void Remove(Shape shape)
    {
        Shapes.Remove(shape);
    }
    
    public void DrawAll(Graphics graphics)
    {
        foreach (Shape shape in Shapes)
        {
            shape.Draw(graphics);
        }
    }
}