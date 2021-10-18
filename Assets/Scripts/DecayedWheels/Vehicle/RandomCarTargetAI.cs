using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe que atira a camara chase basica para qualquer carro AI e que inicializa tambem o HUD
/// para ver as stats desse carro (velocidade, rpm, voltas, posiçao, etc.)
/// </summary>
public class RandomCarTargetAI : MonoBehaviour
{
    public UnityStandardAssets.Cameras.AutoCam cam;
    List<GameObject> carros = new List<GameObject>();

    int atual = -1;

    EncontrarCarroHUD carroHUD;

    bool player = false;

    private void Start()
    {
        VehicleAI[] carrosNew = FindObjectsOfType<VehicleAI>();

        foreach (VehicleAI x in carrosNew)
        {
            carros.Add(x.gameObject);
        }

        carroHUD = FindObjectOfType<EncontrarCarroHUD>();

        if (cam.Target == null && GameObject.FindGameObjectWithTag("Vehicle")==null)
        {
            cam.DarTarget(RandomCar());
        }
        else
        {
            cam.DarTarget(GameObject.FindGameObjectWithTag("Vehicle"));
            player = true;
        }
    }

    public GameObject RandomCar()
    {
        if (atual!=-1)
        {
            carros[atual].GetComponent<Carro_HUD>().StopHUD();
        }

        int num = Random.Range(0, carros.Count);

        if (carros.Count > 1)
        {
            while (num == atual)
            {
                num = Random.Range(0, carros.Count);
            }
        }
        
        atual = num;

        carroHUD.GiveHudCar(carros[num].GetComponent<Carro_HUD>());

        return carros[num].gameObject;
    }

    private void Update()
    {
        if (player) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            cam.DarTarget(RandomCar());
        }
    }
}