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

    Controlador controlador;

    private void Start()
    {
        controlador = FindObjectOfType<Controlador>();

        AutoHUD();
    }

    public void AutoHUD()
    {
        GameObject x = GameObject.FindGameObjectWithTag("Vehicle");

        if (x == null) return;

        Carro_HUD player = x.GetComponent<Carro_HUD>();

        if (controlador != null)
        {
            player.ReceberHUD(rect, textoKMH, pos, volta, gear, controlador.corridaAtual.voltas);
        }
        else
        {
            player.ReceberHUD(rect, textoKMH, pos, volta, gear, 999);
        }
    }

    public void GiveHudCar(Carro_HUD carro)
    {
        if (controlador != null)
        {
            carro.ReceberHUD(rect, textoKMH, pos, volta, gear, controlador.corridaAtual.voltas);
        }
        else
        {
            carro.ReceberHUD(rect, textoKMH, pos, volta, gear, 999);
        }
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