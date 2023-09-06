using Assets.Scripts.NRacer.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EncontrarCarroHUD : MonoBehaviour
{
    public RectTransform rect;
    public Text textoKMH;
    public Text pos;
    public Text volta;
    public Text gear;

    private void Start()
    {
        AutoHUD();
    }

    public void AutoHUD()
    {
        GameObject x = GameObject.FindGameObjectWithTag("Vehicle");

        if (x == null) return;

        Carro_HUD player = x.GetComponent<Carro_HUD>();

        GiveHudCar(player);
    }

    public void GiveHudCar(Carro_HUD carro)
    {
        carro.ReceberHUD(rect, textoKMH, pos, volta, gear, TrackManager.instancia.maximoVoltas);
    }

    private void Update()
    {
        //voltar ao menu imediatamente
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
}