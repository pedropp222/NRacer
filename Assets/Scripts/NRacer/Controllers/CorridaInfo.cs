using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Classe que controla varias informaçoes sobre a corrida em que vamos participar como a lista de carros, o prémio,
/// numero de voltas e claro o resultado final da prova.
/// </summary>
//TODO: Ver o que se pode fazer aqui em relacao ao 'campeonatoId' e 'corridaId' porque as coisas vao ser procedurais
[System.Serializable]
public class CorridaInfo
{
    public List<Controlador.CarroData> startingGrid;
    public int premio;
    public int voltas;

    public int campeonatoID;
    public int corridaID;

    public CorridaResults resultado;

    public CorridaInfo(int premio, int voltas, int campID, int cID)
    {
        this.premio = premio;
        this.voltas = voltas;
        campeonatoID = campID;
        corridaID = cID;

        startingGrid = new List<Controlador.CarroData>();
        resultado = new CorridaResults();
    }
}

public class CorridaResults
{
    public int posicaoFinal;
}