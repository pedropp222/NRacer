using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClick_ListaPlayers : MonoBehaviour
{
    Button b;

    private void Start()
    {
        b = GetComponent<Button>();

        if (FindObjectOfType<Controlador>().NumeroJogadores()>0)
        {
            b.interactable = true;
        }
        else
        {
            b.interactable = false;
        }
    }
}
