using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Classe debug para fazer dump da informaçao de uma corrida de um campeonato e toda a informaçao possivel como
/// os filtros da corrida, carros que podem participar tendo em conta esses filtros e simulaçao de grid final com
/// raridade tida em conta
/// </summary>
public class DebugUI_DumpCampeonato : MonoBehaviour
{
    public Text texto;

    Campeonato campeonato;

    Controlador cnt;

    private void Start()
    {
        string finalString = "";

        cnt = FindObjectOfType<Controlador>();

        campeonato = GetComponent<Campeonato>();

        finalString += "Campeonato " + campeonato.nomeCampeonato+"\n";

        List<Controlador.CarroData> carros = new List<Controlador.CarroData>();

        for (int i = 0; i < campeonato.corridasLista.Length; i++)
        {
            finalString += "Pista: "+SceneManager.GetSceneByBuildIndex(campeonato.corridasLista[i].nivel).path+"\n" +
                "Voltas: "+ campeonato.corridasLista[i].voltas+"\n"+
                "Max potencia: "+ campeonato.corridasLista[i].maxHP;

            carros.AddRange(cnt.GetCarrosPossiveis(campeonato.corridasLista[i]));

            finalString += "Carros possiveis:\n";

            for(int k = 0; k < carros.Count; k++)
            {
                finalString += cnt.carros[carros[k].id].GetComponent<CarroStats>().NomeResumido(false)+"(NA)\n";
            }

            if (carros.Count==0)
            {
                finalString += "NENHUM";
                return;
            }           
        }

        finalString += "SIMULAR GRID 6 CARROS\n";

        int numCarros = 6;

        List<Controlador.CarroData> simularGrid = new List<Controlador.CarroData>();

        simularGrid.AddRange(cnt.FiltrarPorRaridade2(carros.ToArray(), numCarros));


        foreach(Controlador.CarroData l in simularGrid)
        {
            finalString += cnt.carros[l.id].GetComponent<CarroStats>().NomeResumido(false) + "(NA)\n";
        }

        texto.text = finalString;
    }

    private void FixedUpdate()
    {
        //refresh informaçao, vai obrigar a criar um novo grid diferente!
        if (Input.GetKeyDown(KeyCode.R))
        {
            Start();
        }
    }
}
