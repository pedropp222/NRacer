using NWH.VehiclePhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Desativar completamente o input de qualquer veiculo e prende-lo. Util para varios cenarios
/// </summary>
public class CarroInputDisable : MonoBehaviour
{
    VehicleController vehicle;

    private void Awake()
    {
        vehicle = GetComponent<VehicleController>();
        vehicle.transmission.transmissionType = Transmission.TransmissionType.Manual;
        //vehicle.input.blocked = true;
        vehicle.transmission.Gear = 0;

        if (SceneManager.GetActiveScene().buildIndex <= 1)
        {
            transform.Find("CarCamera").gameObject.SetActive(false);
            vehicle.input.blocked = true;
        }
    }

    private void LateUpdate()
    {
        vehicle.input.Handbrake = 1f;
        vehicle.transmission.Gear = 0;
    }

    public void LancarCarro()
    {
        Debug.Log("LANCAR CARRO: SCRIPT "+vehicle);
        if (Controlador.instancia != null)
        {
            vehicle.transmission.transmissionType = Controlador.instancia.mudancasManuais ?
                Transmission.TransmissionType.Manual :
                Transmission.TransmissionType.AutomaticSequential;
        }
        else
        {
            vehicle.transmission.transmissionType = Transmission.TransmissionType.AutomaticSequential;
        }
        vehicle.input.blocked = false;
        vehicle.transmission.Gear = 1;
        vehicle.input.Handbrake = 0f;
        Destroy(this);
    }
}
