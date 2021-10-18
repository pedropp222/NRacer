using UnityEngine;
using System.Collections;

/// <summary>
/// Classe que guarda as regras da corrida, basicamente os filtros e alguma informaçao basica 
/// por ex a pista, nº de voltas, filtros básicos de potencia max, peso, traçao, etc. dificuldade AI
/// e se ja se ganhou
/// </summary>
[System.Serializable]
public class CorridaRules
{
    public bool ganhou;

    public int voltas;
    public int nivel;

    public int premioDinheiro;
    public int premioCarro;

    public int maxHP;
    public int maxPeso;

    public int maxOponentes = 3;

    public int baseDificuldade = 5;

    public bool filtrarTracao;
    public CarroStats.Tracao tracao;
}