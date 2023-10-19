using Assets.Scripts.NRacer.GameMode.Career;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClick_IniciarCarreira : MonoBehaviour, IButton
{
    public void OnClick()
    {
        Controlador.instancia.CarregarJogoGameMode(new ModoCarreira());

        SceneManager.LoadScene(1);
    }
}
