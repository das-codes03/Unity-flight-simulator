using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DragCurve 
{
    public double maxDrag = 1;
    public double minDrag = 0.01;
    public double minDragAoa = 0;
    private double r(double aoa)
    {
        return Math.PI / 180 * aoa;
    }
    public static void Average(DragCurve c1, DragCurve c2, DragCurve output, double ratio)
    {
        output.maxDrag = (c1.maxDrag * (1 - ratio) + c2.maxDrag * ratio);
        output.minDrag = (c1.minDrag * (1 - ratio) + c2.minDrag * ratio);
        output.minDragAoa = (c1.minDragAoa * (1 - ratio) + c2.minDragAoa * ratio);

    }
    public double Evaluate(double aoa)

    {
        double Cd = maxDrag - Math.Cos(r(aoa + minDragAoa)) * (maxDrag - minDrag) + minDrag;
        return Cd;
    }
}
