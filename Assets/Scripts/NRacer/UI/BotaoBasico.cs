using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotaoBasico : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate
        {
            foreach(IButton x in GetComponents<IButton>())
            {
                x.OnClick();
            }
        });
    }

    public void SairJogo()
    {
        FindObjectOfType<Controlador>().SairJogo();
    }
}
