using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClick_IniciarArcade : MonoBehaviour, IButton
{
    public GameObject[] debugListaCarros;

    Controlador ctr;

    void Start()
    {
        ctr = FindObjectOfType<Controlador>();
    }

    public void OnClick()
    {
        

        ShowroomControlador showroom = FindObjectOfType<ShowroomControlador>();

        showroom.ApresentarShowroom(debugListaCarros, ShowroomControlador.TipoShowroom.escolher);
    }
}
