using NWH.VehiclePhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarroHand : MonoBehaviour
{
    public float originalAngular;
    VehicleController vehicle;

    private Transmission.TransmissionType originalTransmission;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        originalAngular = rb.angularDrag;
        vehicle = GetComponent<VehicleController>();

        originalTransmission = vehicle.transmission.transmissionType;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (vehicle.SpeedKPH < 10f)
            {
                //vehicle.transmission.transmissionType = Transmission.TransmissionType.Manual;
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
                //vehicle.transmission.transmissionType = Transmission.TransmissionType.AutomaticSequential;
            }
        }
    }
}
