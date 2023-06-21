using System;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public class LiftCurve
{

    public double mp = 1;
    public double mn = -1;
    public double ap = 15;
    public double an = -15;
    public double steepP = 10;
    public double steepN = 10;
    const double PI = Math.PI;
    public double fMax = 1.2;
    public double offsetAOA;
    public float buffetting = 0.5f;
    double r(double deg)
    {
        return PI / 180 * deg;
    }

    public static void Average(LiftCurve c1, LiftCurve c2, LiftCurve output, double ratio)
    {
        if (output == null)
        {
            output = new LiftCurve();
        }
        output.mp = (c1.mp * (1 - ratio) + c2.mp * ratio);
        output.mn = (c1.mn * (1 - ratio) + c2.mn * ratio);
        output.ap = (c1.ap * (1 - ratio) + c2.ap * ratio);
        output.an = (c1.an * (1 - ratio) + c2.an * ratio);
        output.steepP = (c1.steepP * (1 - ratio) + c2.steepP * ratio);
        output.steepN = (c1.steepN * (1 - ratio) + c2.steepN * ratio);
        output.fMax = (c1.fMax * (1 - ratio) + c2.fMax * ratio);
        output.offsetAOA = (c1.offsetAOA * (1 - ratio) + c2.offsetAOA * ratio);

    }
    public void Modify(double aoaShift, double mpShift, double aCritShift, LiftCurve output)
    {
        output.ap += aCritShift;
        output.offsetAOA+= aoaShift;
        output.mp+= mpShift;
    }
    public double Evaluate(double aoa)
    {
        aoa += offsetAOA;
        double M = (mp - mn) / 2;
        double A = PI / r(ap - an);
        double f_k = (an < aoa && aoa < ap) ? ((mn + mp) / 2 + M * Math.Sin(A * r(aoa - (ap + an) / 2))) : 0;
        double s_flat = fMax * Math.Sin(2 * r(aoa)) *(1+ UnityEngine.Random.Range(-buffetting,buffetting));
        double X = Math.Pow(Math.Sin(PI * Math.Clamp(((aoa - ap) / 90 * steepP + 1) / 2, 0, 1)), 2);
        double Y = Math.Pow(Math.Sin(PI * Math.Clamp(((aoa - an) / 90 * steepN + 1) / 2, 0, 1)), 2);

        double stall_p = aoa > ap ? X * mp + s_flat * (1 - X) : 0;
        double stall_n = aoa < an ? Y * mn + s_flat * (1 - Y) : 0;
        double Cl = f_k + stall_p + stall_n;

        return Cl;
    }
}
