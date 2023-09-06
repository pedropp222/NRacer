using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NWH.VehiclePhysics;

/// <summary>
/// Ver o input de um carro, seja ai ou player
/// </summary>
public class Debug_PlayerInput : MonoBehaviour
{
    public Text texto;

    VehicleController vehicle;


    private void Start()
    {
        vehicle = GetComponent<VehicleController>();
    }

    private void FixedUpdate()
    {
        texto.text = "Vertical: "+vehicle.input.Vertical;
        texto.text += "\nHorizontal: " + vehicle.input.Horizontal;
    }
}
