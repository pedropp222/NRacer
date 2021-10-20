using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Classe debug para iniciar logo uma corrida de um campeonato
/// </summary>
public class Debug_SetupRace : MonoBehaviour, IButton
{
    public int campeonatoID;
    public int corridaID;

    public Slider dificuldade;

    public void Click()
    {
        Campeonato camp = GetCampeonatoByID(campeonatoID);
        if (camp == null) { Debug.LogError("Erro: nao encontrou campeonato com id " + campeonatoID); return; }
        //camp.corridasLista[corridaID].baseDificuldade = (int)dificuldade.value;
        //camp.corridasLista[corridaID].maxOponentes = 5;
        FindObjectOfType<Controlador>().GerarCorrida(camp.corridasLista[corridaID],new CorridaInfo(camp.corridasLista[corridaID].premioDinheiro,camp.corridasLista[corridaID].voltas,camp.id,corridaID));
    }

    public void OnClick()
    {
        Click();
    }

    /// <summary>
    /// Deprecado, ja nao e preciso, usar controlador.campeonatos
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static Campeonato GetCampeonatoByID(int i)
    {
        foreach(Campeonato camp in FindObjectsOfType<Campeonato>())
        {
            if (camp.id == i)
            {
                return camp;
            }
        }

        return null;
    }
}