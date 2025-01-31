﻿using System.Windows.Media;
using OOTPiSP.GeometryFigures.Shared;
using OOTPiSP.Strategy;

namespace OOTPiSP.GeometryFigures.Rectangle;

public class MyRectangle : AbstractShape
{

    public override object TagShape => "3";

    public MyRectangle(MyPoint topLeft, MyPoint downRight, Brush backgroundColor, Brush penColor, int angle)
        : base(topLeft, downRight, backgroundColor, penColor, angle)
    {
        DrawStrategy = new RectangleDrawStrategy();
    }
    
    public virtual double GetHeight() => Math.Abs(TopLeft.Y - DownRight.Y); 
    public virtual double GetWidth() => Math.Abs(TopLeft.X - DownRight.X); 
    
    public override string ToString() =>
        $"{nameof(MyRectangle)}:({TopLeft.X}-{TopLeft.Y}; Width={GetWidth()}; Height={GetHeight()}";
}