#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;


public class Airfoil
{

    public Vector3 coordVector;     //Coord line vector
    public Vector3 spanVector;      //wing span vector
    public Vector3 worldPosition;   //leading edge position
    double surfaceArea;
    public double span;
    public double coordLength=1;
    public struct Force
    {
        public Vector3 point;
        public Vector3 force;
        public Force(Vector3 p, Vector3 f)
        {
            point = p;
            force = f;
        }
    }
    public struct forceData
    {
        public Force[] forces;
        public forceData(int divisions)
        {
            forces = new Force[divisions];
        }
    };

    public LiftCurve liftCurve;
    public DragCurve dragCurve;


    double Clamp90(double angle)
    {
        angle = angle % 360.0;
        if (angle > 90) angle -= 180;
        if (angle < -90) angle += 180;
        return angle;
    }

    /// <summary>
    /// Returns net Force
    /// </summary>
    /// <param name="airflow"></param>
    /// <param name="density"></param>
    /// <returns></returns>
    public Force GetForce(Vector3 wind, Rigidbody rBody, double density)
    {

        Vector3 pt = worldPosition;
        surfaceArea = span * coordLength/2;
        Vector3 airflow = -rBody.GetPointVelocity(pt) + wind;

        Debug.DrawLine(pt, pt + spanVector, Color.red);
        Debug.DrawLine(pt, pt + coordVector, Color.green);
        Vector3 coordAirflow = Vector3.ProjectOnPlane(airflow, spanVector);
        Debug.DrawLine(pt, pt + airflow / 100, Color.blue);
        double aoa = 0;
        if (airflow.magnitude != 0)
            aoa = Vector3.SignedAngle(coordVector, coordAirflow, spanVector);
        aoa = Clamp90(aoa);
        /*  double cl = averageDouble(LiftCurveL.Evaluate((float)aoa), LiftCurveR.Evaluate((float)aoa));
          double cd = averageDouble(DragCurveL.Evaluate((float)aoa), DragCurveR.Evaluate((float)aoa));*/
        double cl = liftCurve.Evaluate(aoa);
        double cd = dragCurve.Evaluate(aoa);
        double vSqr = Math.Pow(coordAirflow.magnitude, 2);
        //formula 0.5 * rho * v^2 * Cl * A
        double f = -0.5 * density * vSqr * cl * surfaceArea;
        double d = 0.5 * density * vSqr * cd * surfaceArea;
        Vector3 liftAxis = Vector3.Cross(coordAirflow, spanVector).normalized;
        Vector3 fVector = liftAxis * (float)f;
        Vector3 dVector = coordAirflow.normalized * (float)d;


        Force fdata = new Force(pt, fVector + dVector);
        Debug.DrawLine(pt, pt + fVector / 1000, Color.green);
        Debug.DrawLine(pt, pt + dVector / 1000, Color.red);



        return fdata;
    }
    public Airfoil()
    {
        liftCurve= new LiftCurve();
        dragCurve= new DragCurve();
    }

}

