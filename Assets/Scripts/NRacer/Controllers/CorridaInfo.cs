using Assets.Scripts.NRacer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// Classe que controla varias informaçoes sobre a corrida em que vamos participar como a lista de carros, o prémio,
/// numero de voltas e claro o resultado final da prova.
/// </summary>
//TODO: Ver o que se pode fazer aqui em relacao ao 'campeonatoId' e 'corridaId' porque as coisas vao ser procedurais
[System.Serializable]
public class CorridaInfo
{
    public List<CarroData> startingGrid;
    public CorridaPremio premio;
    public int voltas;

    public int campeonatoID;
    public int corridaID;

    public CorridaResults resultado;

    public CorridaInfo(CorridaPremio premio, int voltas, int campID, int cID)
    {
        this.premio = premio;
        this.voltas = voltas;
        campeonatoID = campID;
        corridaID = cID;

        startingGrid = new List<CarroData>();
        resultado = new CorridaResults();
    }
}

public class CorridaResults
{
    public int posicaoFinal;
}

public class CorridaPremio
{
    public TipoPremio tipo;
    public int valor;
    public CarroData carroPremio;

    public static CorridaPremio PremioDinheiro(int valor)
    {
        return new CorridaPremio(TipoPremio.DINHEIRO, valor);
    }

    public static CorridaPremio PremioCarro(CarroData carro)
    {
        return new CorridaPremio(TipoPremio.VEICULO, carro);
    }

    private CorridaPremio(TipoPremio tipo, int valor)
    {
        this.tipo = tipo;
        this.valor = valor;
    }

    private CorridaPremio(TipoPremio tipo, CarroData valor)
    {
        this.tipo = tipo;
        this.carroPremio = valor;
    }

    public static CorridaPremio PremioDefault { get { return new CorridaPremio(TipoPremio.DINHEIRO, 0); } }
}

public enum TipoPremio
{
    DINHEIRO,
    VEICULO
}
