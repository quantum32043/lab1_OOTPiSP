﻿using System.Windows.Media;
using OOTPiSP.GeometryFigures.Shared;

namespace OOTPiSP.GeometryFigures.Rectangle;

public class MySquare : MyRectangle
{
    public override object TagShape => "2";

    public MySquare(MyPoint topLeft, MyPoint downRight, Brush backgroundColor, Brush penColor, int angle)
        : base(topLeft, downRight, backgroundColor, penColor, angle) 
    { }

    public override string ToString() =>
        $"{nameof(MySquare)}:({TopLeft.X}-{TopLeft.Y}; Side={GetWidth()};";
    
    public override double GetWidth() => Math.Abs(TopLeft.X - DownRight.X);
    public override double GetHeight() => Math.Abs(TopLeft.X - DownRight.X);

}