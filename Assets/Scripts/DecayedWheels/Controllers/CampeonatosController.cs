using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// Classe que serve para guardar todos os campeonatos e todas as corridas.
/// Guarda apenas o id do campeonato, id da corrida e bool ganhou ou nao
/// </summary>
[System.Serializable]
public class CampeonatosController
{
    [SerializeField]
    public List<CampSave> campeonatos = new List<CampSave>();
}

/// <summary>
/// Representaçao mais simples de um campeonato e as suas corridas
/// </summary>
[System.Serializable]
public class CampSave
{
    public int id;
    public List<bool> ganhos = new List<bool>();

    public int GetGanhos()
    {
        int f = 0;
        for(int i = 0; i < ganhos.Count; i++)
        {
            if (ganhos[i])
            {
                f++;
            }
        }

        return f;
    }
}