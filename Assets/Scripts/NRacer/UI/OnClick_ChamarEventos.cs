using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClick_ChamarEventos : MonoBehaviour, IButton
{
    public UIMenuPainel painelAtivar;
    public UIMenuPainel painelDesativar;

    public void OnClick()
    {
        if (painelAtivar != null)
        {
            painelAtivar.AtivarPainel();
        }
        if (painelDesativar != null)
        {
            painelDesativar.DesativarPainel();
        }
    }
}
