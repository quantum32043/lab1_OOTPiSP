using System.Windows.Media;
using OOTPiSP.GeometryFigures.Shared;
using OOTPiSP.Strategy;

namespace OOTPiSP.GeometryFigures.Triangle;

public class MyTriangle : AbstractShape
{
    public MyPoint VertexOX { get; set; } 
    public MyPoint VertexOY { get; set; }
    
    public override object TagShape => "5";

    public MyTriangle(MyPoint topLeft, MyPoint downRight, Brush bgColor, Brush penColor, int angle)
        : base(topLeft, downRight, bgColor, penColor, angle)
    {
        CalculateVertexByX(TopLeft, DownRight);
        CalculateVertexByY(TopLeft, DownRight);
        DrawStrategy = new TriangleDrawStrategy();
    }
    
    public void CalculateVertexByX(MyPoint vertex, MyPoint endPoint)
    {
        VertexOX = new(vertex.X + Math.Abs(endPoint.X - vertex.X), vertex.Y);
    }

    public void CalculateVertexByY(MyPoint vertex, MyPoint endPoint)
    {
        double sideX = Math.Abs(vertex.X - endPoint.X);
        double sideY = Math.Abs(vertex.Y - endPoint.Y);
        
        double center = sideX / 2;
        double height = sideY;
        VertexOY = new MyPoint(vertex.X + center, vertex.Y - height);
    }

    public override string ToString() =>
        $"{nameof(MyTriangle)}: Vertex1=({TopLeft.X}-{TopLeft.Y}), Vertex2=({VertexOX.X}-{VertexOX.Y}), Vertex3=({VertexOY.X}-{VertexOY.Y})";

}