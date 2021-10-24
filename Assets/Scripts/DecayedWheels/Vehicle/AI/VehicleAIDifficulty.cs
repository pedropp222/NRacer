using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NWH.VehiclePhysics;

/// <summary>
/// Inicializar os valores de vários scripts que irao determinar a dificuldade geral do veiculo AI
/// </summary>
public class VehicleAIDifficulty : MonoBehaviour
{
    VehicleController vehicle;
    List<Wheel> rodas;
    VehicleAI ai;

    public int initialValue = -1;

    Rigidbody rb;

    private void Awake()
    {
        rodas = new List<Wheel>();

        vehicle = GetComponent<VehicleController>();
        ai = GetComponent<VehicleAI>();

        rb = GetComponent<Rigidbody>();

        if (initialValue != -1)
        {
            SetupDificuldade(initialValue);
        }
    }

    public void SetupDificuldade(int x)
    {
        if (x <= 0) x = 1;

        initialValue = x;

        CarroStats st = GetComponent<CarroStats>();

        if (x <= 4)
        {
            st.dificuldadeAI = CarroStats.Dificuldade.Facil;
            if (x <= 2)
            {
                st.dificuldadeAI = CarroStats.Dificuldade.Muito_Facil;
            }
        }
        else if (x >= 7)
        {
            st.dificuldadeAI = CarroStats.Dificuldade.Dificil;
            if (x >= 9)
            {
                st.dificuldadeAI = CarroStats.Dificuldade.Muito_Dificil;
            }
        }
        else
        {
            st.dificuldadeAI = CarroStats.Dificuldade.Medio;
        }

        //Debug.Log(gameObject.name + "-" + st.dificuldadeAI);

        float initialBrake = -5.5f;
        float initialAgressive = 0.40f;

        float initialForce = 0.98f;

        initialBrake -= 0.28f * x;
        initialAgressive += 0.05f * x;

        if (x <= 5)
        {
            initialForce += 0.01f * x;
        }
        else
        {
            initialForce += 0.018f * x;
        }

        if (x<4)
        {
            vehicle.drivingAssists.abs.intensity += 0.1f;
            vehicle.drivingAssists.tcs.intensity += 0.1f;
            vehicle.drivingAssists.stability.intensity += 0.1f;
            ai.dificuldade = 0.75f;
            rb.mass *= 1.15f;
        }
        else
        {
            vehicle.drivingAssists.abs.intensity += 0.2f;
            vehicle.drivingAssists.tcs.intensity += 0.2f;
            vehicle.drivingAssists.stability.intensity += 0.2f;
            ai.dificuldade = 1f;
            if (x>6)
            {
                vehicle.drivingAssists.abs.intensity += 0.3f;
                vehicle.drivingAssists.tcs.intensity += 0.3f;
                vehicle.drivingAssists.stability.intensity += 0.3f;
                ai.dificuldade = 2f;
                rb.mass *= 0.95f;
                if (x > 8)
                {
                    vehicle.drivingAssists.abs.intensity += 0.3f;
                    vehicle.drivingAssists.tcs.intensity += 0.1f;
                    vehicle.drivingAssists.stability.intensity += 0.2f;
                    rb.mass *= 0.96f;
                    if (x == 10)
                    {
                        vehicle.drivingAssists.abs.intensity += 0.1f;
                        vehicle.drivingAssists.tcs.intensity += 0.1f;
                        vehicle.drivingAssists.stability.intensity += 0.1f;
                        rb.mass *= 0.96f;
                    }
                }
            }
        }

        ai.brakeAgressive = Mathf.Clamp(initialAgressive,0f,0.84f);
        ai.brakeDeceleration = initialBrake;

        foreach(Wheel wh in vehicle.Wheels)
        {
            wh.WheelController.sideFriction.forceCoefficient = initialForce;
            wh.wheelController.sideFriction.slipCoefficient = Mathf.Clamp( wh.wheelController.sideFriction.slipCoefficient,1.0f,initialForce-0.10f); 
        }

        //Debug.Log("Inicializou ai com dificuldade " + x);
    }
}
