using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LivreGameModeUI : MonoBehaviour
{
    public GameObject botaoPrefab;

    
    public GameObject listaPistasObj;


    public Text carroSelText;

    public Text pistaSelText;

    bool pistaSetup = false;

    int pistaSelected = 2;

    private void OnEnable()
    {
        Debug.Log("Atualizar ui");
        carroSelText.text = "Carro: " + Controlador.instancia.GetCarroNome(Controlador.instancia.carroSelecionado);

        if (!pistaSetup)
        {

            GameObject btn = Instantiate(botaoPrefab, listaPistasObj.transform);
            btn.transform.GetChild(0).GetComponent<Text>().text = "Driving Park";
            btn.GetComponent<Button>().onClick.AddListener(() => SetPistaSelecionada(2, "Driving Park"));

            btn = Instantiate(botaoPrefab, listaPistasObj.transform);
            btn.transform.GetChild(0).GetComponent<Text>().text = "Rolling Hills";
            btn.GetComponent<Button>().onClick.AddListener(() => SetPistaSelecionada(3, "Rolling Hills"));

            SetPistaSelecionada(2,"Driving Park");
            pistaSetup = true;
        }
    }


    private void SetPistaSelecionada(int id, string name)
    {
        pistaSelText.text = "Pista: " + name;
        pistaSelected = id;
    }

    public void IniciarCorrida(LoadingPainelUI loading)
    {
        Controlador.instancia.GerarCorridaLivre(pistaSelected);
        loading.CarregarNivel(pistaSelected);
    }
}
