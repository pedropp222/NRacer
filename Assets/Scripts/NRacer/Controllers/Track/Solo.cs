using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NWH.VehiclePhysics;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// TODO - apagar isto so depois de verificar que mais nenhum scene faz uso disto
/// Classe que controla o jogo se faltar elementos, como controlador, spawns, voltas, veiculo jogador, etc.
/// apos isso ele toma varias decisoes para evitar problemas por falta de algum desses
/// elementos
/// </summary>
[Obsolete("Nao e para usar mais isto, e para retirar dos scenes e substituir por 'TrackManager'")]
public class Solo : MonoBehaviour
{
    DesktopInputManager manager;

    Controlador cnt;

    private void Start()
    {
        manager = GetComponent<DesktopInputManager>();

        cnt = FindObjectOfType<Controlador>();

        if (cnt == null)
        {

            /*if (FindObjectOfType<Meta>() != null)
            {
                FindObjectOfType<Meta>().gameObject.SetActive(false);
            }

            foreach (Checkpoint x in FindObjectsOfType<Checkpoint>())
            {
                x.gameObject.SetActive(false);
            }*/


            GameObject gp = GameObject.Find("grelhaPartida");

            if (gp != null) gp.SetActive(false);

            foreach (VehicleController x in FindObjectsOfType<VehicleController>())
            {
                if (x.CompareTag("Vehicle"))
                {
                    x.GetComponent<CarroInputDisable>().LancarCarro();
                    GetComponent<VehicleChanger>().vehicles.Add(x);
                    manager.vehicleController = x;
                }
                else
                {
                    if (cnt == null)
                    {
                        x.GetComponent<VehicleAI>()?.LancarCarro();
                    }
                }
            }
        }      
    }
}