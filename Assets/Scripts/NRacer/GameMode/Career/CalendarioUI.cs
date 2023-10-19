using Assets.Scripts.NRacer.GameMode.Career;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class CalendarioUI : MonoBehaviour, ICarreiraPainelUI
{
    public Text mesText;
    public Button anteriorMes;
    public Button proxMes;

    public GameObject diaPrefab;
    public Transform diasPainel;

    public CalendarioEventosUI eventosUI;

    private Calendario calendario;


    int mesAtual;

    public void IniciarCalendario()
    {
        calendario = ModoCarreiraUI.carreiraContext.calendario;
        mesAtual = calendario.GetMesAtual();

        anteriorMes.onClick.RemoveAllListeners();
        proxMes.onClick.RemoveAllListeners();

        anteriorMes.onClick.AddListener(() => MesAnterior());
        proxMes.onClick.AddListener(() => MesProximo());

        ApresentarMes(mesAtual);
    }

    private void MesAnterior()
    {
        mesAtual--;
        ApresentarMes(mesAtual);
    }

    private void MesProximo()
    {
        mesAtual++;
        ApresentarMes(mesAtual);       
    }

    private void ApresentarMes(int mes)
    {
        ResetPainel();

        mesText.text = calendario.GetMesNome(mesAtual);

        //Obter corridas / eventos para este mes
        List<CorridaData> corridas = ModoCarreiraUI.carreiraContext.ObterCorridasMes(mes);

        Debug.Log("Retornou " + corridas.Count + " resultados");

        for(int i = 0; i < calendario.GetDiasMes(mes);i++)
        {
            GameObject go = Instantiate(diaPrefab,diasPainel);
            CalendarioDiaUI dia = go.GetComponent<CalendarioDiaUI>();

            int d = i + 1;
            dia.SetDia(d);

            List<CorridaData> corridasDia = corridas.FindAll((c) => c.corridaRef.data.Dia == (i + 1));

            bool diaAtual = false;

            if (calendario.GetMesAtual() == mes)
            {
                if (calendario.GetDiaAtual() == d)
                {
                    dia.SetDiaAtual();
                    diaAtual = true;
                }
            }

            if (corridasDia.Count>0)
            {
                Debug.Log("Corrida dia " + d);
                dia.SetCorrida();
                dia.SetEventoClick(() => eventosUI.ApresentarEventos(corridasDia[0].corridaRef.data,corridasDia,diaAtual));
            }          
        }

        if (mesAtual+1 > 12)
        {
            proxMes.interactable = false;
        }
        else
        {
            proxMes.interactable = true;
        }

        if (mesAtual-1 < 1)
        {
            anteriorMes.interactable = false;
        }
        else
        {
            anteriorMes.interactable = true;
        }
    }

    private void ResetPainel()
    {
        for(int i = 0; i < diasPainel.childCount; i++)
        {
            Destroy(diasPainel.GetChild(i).gameObject);
        }
    }

    public void OnAtivar()
    {
        gameObject.SetActive(true);
        IniciarCalendario();
    }

    public void OnDesativar()
    {
        ResetPainel();
        gameObject.SetActive(false);
    }
}
