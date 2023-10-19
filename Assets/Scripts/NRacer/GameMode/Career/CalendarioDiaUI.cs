using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

namespace Assets.Scripts.NRacer.GameMode.Career
{
    public class CalendarioDiaUI : MonoBehaviour
    {
        public Color corCorrida;
        public Color corDiaAtual;
        public Color corDiaCorrida;

        private int diaCalendario;

        bool atual = false;

        public void SetDiaAtual()
        {
            GetComponent<Image>().color = corDiaAtual;
            atual = true;
        }

        public void SetCorrida()
        {
            if (atual)
            {
                GetComponent<Image>().color = corDiaCorrida;
            }
            else
            {
                GetComponent<Image>().color = corCorrida;
            }
            transform.GetChild(1).GetComponent<Text>().text = "Corrida";
        }

        public void SetDia(int dia)
        {
            diaCalendario = dia;
            transform.GetChild(0).GetChild(0).GetComponent<Text>().text = diaCalendario.ToString();
        }

        public void SetEventoClick(UnityAction e)
        {
            GetComponent<Button>().onClick.AddListener(e);
        }
    }
}
