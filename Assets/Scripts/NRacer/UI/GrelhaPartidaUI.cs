using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrelhaPartidaUI : MonoBehaviour
{
    public GameObject carBoxPrefab;
    public GameObject painelCarros;

    public Button botaoIniciar;

    private int atual = 1;


    public void AdicionarVeiculo(CarroStats carro)
    {
        GameObject go = Instantiate(carBoxPrefab,painelCarros.transform);

        go.transform.GetChild(0).GetComponent<Text>().text = atual.ToString();
        go.transform.GetChild(1).GetComponent<Text>().text = carro.NomeCompleto();

        atual++;
    }
}
