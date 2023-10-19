using UnityEngine;
using System.Collections;

/// <summary>
/// Esta classe permite criar um campeonato que ira estar num objeto do unity
/// permite criar um campeonato com inumeras corridas [corridasLista], cada corrida com as suas
/// regras distintas
/// </summary>
[System.Serializable]
public class Campeonato
{
    public int id = -1;
    public string nomeCampeonato;

    [SerializeField]
    public CorridaRules[] corridasLista;

    public void SetGanho(int i)
    {
        corridasLista[i].ganhou = true;
    }

    public void SetParticipado(int i)
    {
        corridasLista[i].participou = true;
    }

    public int GetCorridasGanhas()
    {
        int final = 0;

        for(int i = 0; i < corridasLista.Length;i++)
        {
            if (corridasLista[i].ganhou) final++;
        }

        return final;
    }

    public bool TudoGanho()
    {
        return GetCorridasGanhas() == corridasLista.Length;
    }
}