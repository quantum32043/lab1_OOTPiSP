using System.Windows.Media;
using System.Runtime.Serialization.Formatters.Binary;

namespace OOTPiSP.GeometryFigures.Shared;
[Serializable]
public class AbstractShapeBin
{
    public object TagShape { get; set; }

    public int Angle { get; set; }
    
    public MyPoint TopLeft { get; set; }
    
    public MyPoint DownRight { get; set; }
    
    public double StrokeThickness { get; set; } = 1;
    
    public string BackgroundColorString { get; set; }
    public string PenColorString { get; set; }

    // Методы для получения SolidColorBrush из строкового представления цвета
    public Brush GetBackgroundColor()
    {
        return (SolidColorBrush)new BrushConverter().ConvertFromString(BackgroundColorString);
    }

    public Brush GetPenColor()
    {
        return (SolidColorBrush)new BrushConverter().ConvertFromString(PenColorString);
    }
}