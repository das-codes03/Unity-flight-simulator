using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftDynamics : MonoBehaviour
{
    public Transform COM;
    Rigidbody rBody;
    public Wing[] wings;
    public Vector3 airflow;
    void Start()
    {
        rBody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //update the center of mass
        if (COM != null)
            rBody.centerOfMass = transform.InverseTransformPoint(COM.position);
        for(int i = 0; i < wings.Length; i++) {
            
            var f = wings[i].GetForce();
            foreach(var f2 in f)
            {
                rBody.AddForceAtPosition(f2.force, f2.point);
            }
            wings[i].UpdateAirfoil(airflow, rBody, 1);
        }

    }
}
