using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CarroMarca", menuName = "JogoDados/CarroTrim", order = 3)]
public class CarroTrim : ScriptableObject
{
    public string nomeEspecial;

    public Raridade trimRaridade;

    public int preco;
    public float zeroAos100;

    public int peso;
    public float potenciaKW;

    public float upgradeIncrementarPotencia;
    public float upgradeDecrementarPeso;
    public float upgradeIncrementarVeloMaxima;

    public Desempenho GetDesempenhoTexto(float pontos)
    {
        if(pontos <= 13f)
        {
            return Desempenho.SUCATA;
        }
        else if (pontos <= 25f)
        {
            return Desempenho.MAU;
        }
        else if (pontos <= 45f)
        {
            return Desempenho.MEDIANO;
        }
        else if(pontos <= 70f)
        {
            return Desempenho.BOM;
        }
        else if (pontos <= 150f)
        {
            return Desempenho.MUITO_BOM;
        }
        else if (pontos <= 300f)
        {
            return Desempenho.INCRIVEL;
        }
        return Desempenho.DIVINAL;
    }

    public enum Desempenho
    {
        SUCATA,//F
        MAU,//E
        MEDIANO,//D
        BOM,//C
        MUITO_BOM,//B
        INCRIVEL,//A
        DIVINAL//S
    }

    public enum Raridade
    {
        MUITO_COMUM,//F
        COMUM,//E
        MEDIANO,//D
        RARO,//C
        MUITO_RARO,//B
        ESPECIAL,//A
        UNICO//S
    }
}
