using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

namespace Assets.Scripts.NRacer.GameMode.Career
{
    public class CalendarioEventosUI : MonoBehaviour
    {
        public Text textoData;

        public GameObject objetoEventoPrefab;

        public PainelVerticalScrollUI painelEventos;

        public PistaInfoEventoUI pistaInfoEventoUI;

        public void ApresentarEventos(Calendario.CalendarioData data, List<CorridaData> corridas, bool diaAtual)
        {
            gameObject.SetActive(true);
            painelEventos.Reset();

            textoData.text = "Eventos para o dia: "+data.GetDataStringFancy();

            for(int i = 0; i < corridas.Count; i++)
            {
                GameObject go = painelEventos.InstanciarElemento(objetoEventoPrefab,10f);
                go.GetComponent<CalendarioEventoPainel>().Inicializar(corridas[i], diaAtual);

                //set contexto de corrida no controlador
                Controlador.instancia.corridaData = corridas[i];
                int k = i;

                go.GetComponent<CalendarioEventoPainel>().participarBotao.onClick.AddListener(() =>
                {
                    pistaInfoEventoUI.Inicializar(corridas[k]);
                });
            }
        }
    }
}
