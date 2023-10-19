using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.NRacer.GameMode.Career
{
    public class CalendarioEventoPainel : MonoBehaviour
    {
        public Button participarBotao;
        public Text campText;
        public Text eventoText;

        public void Inicializar(CorridaData dados, bool podeParticipar)
        {
            if (!podeParticipar)
            {
                participarBotao.interactable = false;
            }

            campText.text = "Campeonato \""+dados.campeonatoRef.nomeCampeonato+"\"";
            eventoText.text = "Evento " + (dados.eventoIndex+1) + " de " + dados.campeonatoRef.corridasLista.Length;
        }

    }
}
