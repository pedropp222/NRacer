using UnityEngine;
using System.Collections;
using Assets.Scripts.NRacer.GameMode.Career;
using Assets.Scripts.NRacer.Dados;
using Assets.Scripts.NRacer.Controllers;

/// <summary>
/// Classe que guarda as regras da corrida, basicamente os filtros e alguma informaçao basica 
/// por ex a pista, nº de voltas, filtros básicos de potencia max, peso, traçao, etc. dificuldade AI
/// e se ja se ganhou
/// </summary>
[System.Serializable]
public class CorridaRules
{
    public bool ganhou;
    public bool participou;

    public int voltas;
    public PistaInfo nivel;
    public Calendario.CalendarioData data;

    public int premioDinheiro;
    public int premioCarro;
    public int premioCarroTrim;

    public CarroFiltro filtroVeiculos;

    public int maxOponentes = 3;

    public int baseDificuldade = 5;

    //selecionar sempre o carro AI com este id, se for -1 ignorar
    public int forceAICar = -1;

    public static CorridaRules CorridaLivre(PistaInfo pista)
    {
        CorridaRules r = new CorridaRules
        {
            filtroVeiculos = CarroFiltro.VAZIO(),
            data = Calendario.CalendarioData.CriarFromDia(1),
            maxOponentes = 5,
            voltas = 2,
            nivel = pista
        };

        return r;
    }
}