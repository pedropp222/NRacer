using NWH.VehiclePhysics;
using NWH.WheelController3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ver forças laterais do veiculo
/// </summary>
public class Debug_CarValues : MonoBehaviour
{
    public WheelController[] rodas;
    VehicleController vehicle;

    private void Start()
    {
        vehicle = GetComponent<VehicleController>();
    }

    private void FixedUpdate()
    {
        Debug.Log("Forca lateral: " + GetSideForce());
        //Debug.Log("Angular velocity: "+vehicle.vehicleRigidbody.angularVelocity.magnitude);
    }

    public float GetSideForce()
    {
        float total = 0f;

        foreach (WheelController x in rodas)
        {
            total += x.sideFriction.force;
        }

        total /= 4f;

        return total;
    }
}
