using NWH.VehiclePhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarroHand : MonoBehaviour
{
    public float originalAngular;
    VehicleController vehicle;

    Rigidbody rb;

    //TODO: Verificar se vale a pena existir esta classe e esta funcionalidade aqui, quando se poderia passar para o desktop input manager possivelmente

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
                vehicle.input.Handbrake = 1f;
            }
            rb.angularDrag = 0f;
        }
        else
        {
            vehicle.input.Handbrake = 0f;
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
