using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

namespace Assets.Scripts.NRacer.Vehicle
{
    public class CarroCronometro : MonoBehaviour
    {
        private List<float> temposVoltas;

        private float tempoAtual;

        public GameObject uiCronometroObjeto;

        private TextMeshProUGUI cronometroText;
        private TextMeshProUGUI ultimoText;
        private TextMeshProUGUI recordeText;

        bool running = false;

        private void Start()
        {
            temposVoltas = new List<float>();

            if (uiCronometroObjeto == null)
            {
                uiCronometroObjeto = GameObject.Find("uiCronometro");
                cronometroText = uiCronometroObjeto.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                ultimoText = uiCronometroObjeto.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                recordeText = uiCronometroObjeto.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            }

            RefreshUI();
        }

        public void Reiniciar(bool adicionarVolta)
        {
            if (!running)
            {
                Iniciar();
            }
            else
            {
                if (adicionarVolta)
                {
                    temposVoltas.Add(tempoAtual);
                    AtualizarRecordes();
                }
                tempoAtual = 0f;
            }
        }

        public void Iniciar()
        {
            running = true;
        }

        public void Parar()
        {
            running = false;
        }

        private void FixedUpdate()
        {
            if (running)
            {
                tempoAtual += Time.fixedDeltaTime;
                RefreshUI();
            }
        }

        private void AtualizarRecordes()
        {
            ultimoText.text = "Ultima Volta: " + FormatarCronometro(temposVoltas.Last());
            recordeText.text = "Melhor Volta: " + FormatarCronometro(temposVoltas.Min());
        }

        private string FormatarCronometro(float valor)
        {
            int millis = (int)(valor * 1000);
            int minutos = millis / (60 * 1000);
            int segundos = (millis / 1000) % 60;
            int millisegundos = millis % 1000;

            return $"{minutos}:{segundos:D2}.{millisegundos:D3}";
        }

        private void RefreshUI()
        {
            cronometroText.text = FormatarCronometro(tempoAtual);
        }
    }
}