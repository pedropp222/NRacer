using NWH.VehiclePhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Desativar completamente o input de qualquer veiculo e prende-lo. Util para varios cenarios
/// </summary>
public class CarroInputDisable : MonoBehaviour
{
    VehicleController vehicle;

    private void Awake()
    {
        vehicle = GetComponent<VehicleController>();
        vehicle.transmission.transmissionType = NWH.VehiclePhysics.Transmission.TransmissionType.Manual;
        vehicle.input.blocked = true;
        vehicle.transmission.Gear = 0;       
    }

    private void Update()
    {
        vehicle.input.Handbrake = 1f;
    }

    public void LancarCarro()
    {
        vehicle.transmission.transmissionType = NWH.VehiclePhysics.Transmission.TransmissionType.AutomaticSequential;
        vehicle.input.blocked = false;
        vehicle.transmission.Gear = 1;
        vehicle.input.Handbrake = 0f;
        Destroy(this);
    }
}
