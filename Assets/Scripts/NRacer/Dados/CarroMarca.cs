using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CarroMarca", menuName = "JogoDados/CarroMarca", order = 1)]
public class CarroMarca : ScriptableObject
{
    public string nome;
    public MarcaRegiao regiao;
    public Image imagemLogo;
}

public enum MarcaRegiao
{
    EUROPA,
    AMERICA,
    ASIA,
    OUTRO
}
