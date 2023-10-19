using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasaUI : MonoBehaviour, ICarreiraPainelUI
{
    public void OnAtivar()
    {
        gameObject.SetActive(true);
    }

    public void OnDesativar()
    {
        gameObject.SetActive(false);
    }
}
