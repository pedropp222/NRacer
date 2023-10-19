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

    public void Click()
    {
        //FindObjectOfType<Controlador>().GerarCorrida(camp.corridasLista[corridaID],new CorridaInfo(CorridaPremio.PremioDefault,camp.corridasLista[corridaID].voltas,camp.id,corridaID));
    }

    public void OnClick()
    {
        Click();
    }
}