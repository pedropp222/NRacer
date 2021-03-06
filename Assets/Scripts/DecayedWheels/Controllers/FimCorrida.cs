using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Classe que controla o painel UI de fim de corrida, em que mostra posiçao, premio, e botao para sair
/// </summary>
public class FimCorrida : MonoBehaviour
{
    public GameObject painel;
    Controlador controlador;

    private void Start()
    {
        controlador = FindObjectOfType<Controlador>();
    }

    public void MostrarPainel(int posicao)
    {
        painel.SetActive(true);
        painel.transform.GetChild(1).GetComponent<Text>().text = posicao+"º";
        painel.transform.GetChild(2).GetComponent<Text>().text = "Ganhaste" + controlador.corridaAtual.premio/posicao;
        painel.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate
        {
            controlador.corridaAtual.resultado.posicaoFinal = posicao;
            if (posicao == 1 && controlador.filtroAtual.premioCarro != -1 && controlador.saveDataPlayerAtual.corridas.campeonatos[controlador.corridaAtual.campeonatoID].ganhos[controlador.corridaAtual.corridaID]==false)
            {
                //carregar win carro!!!
                SceneManager.LoadScene(1);
            }
            else
            {
                //voltar ao menu logo
                SceneManager.LoadScene(0);
            }
        });
    }
}