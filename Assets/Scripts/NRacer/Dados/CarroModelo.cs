using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarroMarca", menuName = "JogoDados/CarroModelo", order = 2)]
public class CarroModelo : ScriptableObject
{
    public string nome;
    public int ano;

    [SerializeField] public CarroTrim[] trimsDisponiveis;
}
