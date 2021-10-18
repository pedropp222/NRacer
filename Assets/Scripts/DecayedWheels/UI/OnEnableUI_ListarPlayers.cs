using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnEnableUI_ListarPlayers : MonoBehaviour
{
    Controlador controlador;

    public GameObject botao;
    public Transform panel;

    private void OnEnable()
    {
        if (controlador == null)
        {
            controlador = FindObjectOfType<Controlador>();          
        }

        for(int i = 0; i < panel.childCount; i++)
        {
            DestroyImmediate(panel.GetChild(i).gameObject);
        }

        for (int i = 0; i < controlador.NumeroJogadores(); i++)
        {
            GameObject x = Instantiate(botao, panel);
            string player = controlador.GetPlayer(i).PlayerName;
            x.transform.GetChild(0).GetComponent<Text>().text = player;
            int j = i;
            x.GetComponent<Button>().onClick.AddListener(delegate
            {
                controlador.CarregarPlayer(player, Controlador.GameMode.arcada);
            });
        }
    }
}
