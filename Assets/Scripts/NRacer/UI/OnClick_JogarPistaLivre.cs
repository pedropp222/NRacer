using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClick_JogarPistaLivre : MonoBehaviour, IButton
{
    public void OnClick()
    {
        Controlador.instancia.GerarCorridaLivre();
    }
}
