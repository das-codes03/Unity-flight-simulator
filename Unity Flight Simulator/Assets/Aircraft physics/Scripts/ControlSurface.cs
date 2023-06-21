using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSurface : MonoBehaviour
{
    public double Amount;
    public double length;
    public double span;
    public double MaxAoaShift = 10;
    public double MaxCoefShift = 1;
    public double position;
    public double MaxCritAoaShift = 1;
    public void Affect(LiftCurve c, double ratio)
    {
        c.Modify(MaxAoaShift * Amount * ratio, MaxCoefShift * Amount * ratio, MaxCritAoaShift * Amount * ratio, c);
    }
}
