using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivreGameModeUI : MonoBehaviour
{
    public GameObject botaoPrefab;

    
    public GameObject listaPistasObj;


    public Text carroSelText;

    public Text pistaSelText;


    private void OnEnable()
    {
        Debug.Log("Atualizar ui");
        carroSelText.text = "Carro: " + Controlador.instancia.GetCarroNome(Controlador.instancia.carroSelecionado);
    }

}
