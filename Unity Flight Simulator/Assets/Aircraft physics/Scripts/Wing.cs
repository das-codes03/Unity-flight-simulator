using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Airfoil;

public class Wing : MonoBehaviour
{
    Airfoil[] airfoils;
    public double wingspan;
    public double LAngle;
    public double RAngle;
    public double coordL;
    public double coordR;
    public bool invert;
    public LiftCurve liftCurveL;
    public LiftCurve liftCurveR;
    public DragCurve dragCurve;
    public int subdivisions;
    ControlSurface[] surfaces;
    public Force[] forces;

    void Start()
    {
        airfoils = new Airfoil[subdivisions];
        forces = new Force[subdivisions];
        surfaces = this.GetComponentsInChildren<ControlSurface>();
        for (int i = 0; i < airfoils.Length; i++)
        {
            airfoils[i] = new Airfoil();
        }
    }
    public void UpdateAirfoil(Vector3 airflow, Rigidbody rBody, double density)
    {
        for (int i = 0; i < airfoils.Length; i++)
        {
            double ratio = (i + 0.5f) / subdivisions;
            airfoils[i].span = wingspan / subdivisions;
            airfoils[i].spanVector = transform.right;
            airfoils[i].coordLength = ratio * coordR + (1 - ratio) * coordL;
            airfoils[i].worldPosition = transform.position + transform.right * (float)ratio * (float)wingspan * (invert ? -1 : 1);
            LiftCurve.Average(liftCurveL, liftCurveR, airfoils[i].liftCurve, ratio);//TODO: ratio
            double angle = (RAngle * ratio + LAngle * (1 - ratio));
            airfoils[i].coordVector = Quaternion.AngleAxis((float)angle, transform.right) * transform.forward;
        }
        for (int i = 0; i < surfaces.Length; ++i)
        {
            double l = (surfaces[i].position - surfaces[i].span / 2.0) / wingspan * subdivisions;
            double r = (surfaces[i].position + surfaces[i].span / 2.0) / wingspan * subdivisions;
            double prevX = l;
            for (int x = (int)Math.Ceiling(l); x < r+ 1; ++x)
            {
                double rat = math.clamp(x, l, r)-prevX;
                surfaces[i].Affect(airfoils[(invert?-x:x-1)].liftCurve, rat);
                prevX = x;
            }
        }
        //airfoil.coordVectorR = Quaternion.AngleAxis((float)RAngle, Vector3.right) * transform.forward;
        for (int i = 0; i < Mathf.Min(forces.Length, airfoils.Length); ++i)
        {   
            forces[i] = airfoils[i].GetForce(airflow, rBody, density);
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 p1 = transform.position;
        Vector3 p2 = transform.position + transform.right * (float)wingspan * (invert ? -1 : 1);
        Vector3 p3 = p1 + Quaternion.AngleAxis((float)LAngle, transform.right) * transform.forward * (float)coordL;
        Vector3 p4 = p2 + Quaternion.AngleAxis((float)RAngle, transform.right) * transform.forward * (float)coordR;
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p4);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p1, p3);
        for (int i = 0; i < surfaces.Length; ++i)
        {
            Vector3 c1 = p1 + surfaces[i].transform.right * (float)(surfaces[i].span / 2 + surfaces[i].position);
            Vector3 c2 = p1 + surfaces[i].transform.right * (float)(-surfaces[i].span / 2 + surfaces[i].position);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(c1, c2);
        }

    }
    public Force[] GetForce()
    {
        //update airfoil
        /*   UpdateAirfoil();*/

        return forces;
    }
    public Wing()
    {

    }
}
