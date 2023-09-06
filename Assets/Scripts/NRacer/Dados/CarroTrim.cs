using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CarroMarca", menuName = "JogoDados/CarroTrim", order = 3)]
public class CarroTrim : ScriptableObject
{
    public string nomeEspecial;

    public Raridade trimRaridade;

    //TODO: Nos trims retirar o 'override' e colocar mais informacoes de forma a reunir e poder carregar informacao total de um trim
    //e de um veiculo aqui

    [SerializeField] private bool overridePeso;
    [SerializeField] private int novoPeso;

    [SerializeField] private bool overridePotencia;
    [SerializeField] private float novaPotenciaKW;

    public int GetNovoPeso()
    {
        return overridePeso ? novoPeso : -1;
    }

    public float GetNovaPotencia()
    {
        return overridePotencia ? novaPotenciaKW: -1f;
    }

    //TODO: Possivelmente adicionar mais algumas raridades
    public enum Raridade
    {
        COMUM,
        INCOMUM,
        RARO,
        ESPECIAL,
        LENDARIO
    }
}
