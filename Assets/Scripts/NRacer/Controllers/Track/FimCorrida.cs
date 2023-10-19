using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts.NRacer.GameMode.Career;

/// <summary>
/// Classe que controla o painel UI de fim de corrida, em que mostra posiçao, premio, e botao para sair
/// </summary>
public class FimCorrida : MonoBehaviour
{
    public GameObject painel;

    public void MostrarPainel(int posicao)
    {
        painel.SetActive(true);
        painel.transform.GetChild(1).GetComponent<Text>().text = posicao+"º";
        //painel.transform.GetChild(2).GetComponent<Text>().text = "Ganhaste " + controlador.corridaAtual.premio/posicao+"$";
        painel.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate
        {
            Controlador.instancia.corridaAtual.resultado.posicaoFinal = posicao;
            //TODO: E preciso montar um cenario completamente novo de ganhar um veiculo, que muito decerteza depende do gamemode e de como o carro tem que ser 
            //apresentado ao player, estilo gran turismo ou entao lootbox :)
            /*if (posicao == 1 && controlador.filtroAtual.premioCarro != -1 && controlador.saveDataPlayerAtual.corridas.campeonatos[controlador.corridaAtual.campeonatoID].ganhos[controlador.corridaAtual.corridaID]==false)
            {
                //carregar win carro!!!
                SceneManager.LoadScene(1);
            }
            else
            {
                //voltar ao menu logo
                SceneManager.LoadScene(0);
            }*/

            //Voltar ao menu do modo carreira em vez do menu basico
            if (Controlador.instancia.modoAtual is ModoCarreira)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        });
        painel.transform.GetChild(3).GetComponent<Button>().Select();
    }
}