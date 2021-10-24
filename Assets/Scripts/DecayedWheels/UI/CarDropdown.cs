using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDropdown : MonoBehaviour
{
    public void ValueChanged(int i)
    {
        FindObjectOfType<Controlador>().SetCarroSelected(i);
    }
}
