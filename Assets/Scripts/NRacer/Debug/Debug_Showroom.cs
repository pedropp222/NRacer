using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_Showroom : MonoBehaviour
{
    private Controlador ctr;

    private ShowroomControlador show;

    private void Start()
    {
        ctr = GetComponent<Controlador>();
        show = FindAnyObjectByType<ShowroomControlador>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            show.ApresentarShowroom(ctr.carros, ShowroomControlador.TipoShowroom.escolher);
        }
    }
}
