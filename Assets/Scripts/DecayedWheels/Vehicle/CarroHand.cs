using NWH.VehiclePhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarroHand : MonoBehaviour
{
    float originalAngular;
    VehicleController vehicle;


    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        originalAngular = rb.angularDrag;
        vehicle = GetComponent<VehicleController>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (vehicle.SpeedKPH < 10f)
            {
                vehicle.transmission.Gear = 0;
            }
            rb.angularDrag = 0f;
        }
        else
        {
            if (vehicle.SpeedKPH < 25f)
            {
                rb.angularDrag = 0f;
            }
            else
            {
                rb.angularDrag = originalAngular;
            }
        }
    }
}
