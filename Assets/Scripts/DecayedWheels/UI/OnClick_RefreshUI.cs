using UnityEngine;

public class OnClick_RefreshUI : MonoBehaviour, IButton 
{    public void OnClick()
    {
        Controlador x = FindObjectOfType<Controlador>();
        x.MenuUI();
    }
}