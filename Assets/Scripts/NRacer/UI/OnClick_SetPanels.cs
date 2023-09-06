using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClick_SetPanels : MonoBehaviour, IButton
{
    public GameObject ativar, desativar;

    public void OnClick()
    {
        if (ativar != null)
        {
            ativar.SetActive(true);
        }

        if (desativar != null)
        {
            desativar.SetActive(false);
        }
    }
}
