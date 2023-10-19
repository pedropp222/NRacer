using Assets.Scripts.NRacer.Dados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.NRacer.GameMode.Career
{
    public class PistaInfoEventoUI : MonoBehaviour, ICarreiraPainelUI
    {
        public Image pistaSprite;
        public Text pistaNome;
        public Text layoutNome;
        public Text voltasText;

        public Text corridaContextoText;

        public Button selectCarroButton;

        public ShowroomControladorCarreira garagem;

        public void Inicializar(CorridaData dados)
        {
            Reset();

            PistaData pista = Controlador.instancia.GetPistaByNivelId(dados.corridaRef.nivel.nivelId);

            pistaSprite.sprite = pista.layouts[dados.corridaRef.nivel.layoutId].pistaImagem;

            pistaNome.text = pista.pistaNome;

            layoutNome.text = pista.layouts[dados.corridaRef.nivel.layoutId].layoutNome;

            voltasText.text = dados.corridaRef.voltas + " Voltas";

            //corridaContextoText.text = "Min Desempenho: " + dados.corridaRef.minDesempenho + "\nMax Desempenho: " + dados.corridaRef.maxDesempenho;

            selectCarroButton.onClick.AddListener(() =>
            {
                ModoCarreiraUI.instancia.DesativarTodosPaineis();
                garagem.OnAtivar();
                garagem.ApresentarShowroom(ModoCarreiraUI.carreiraContext.carrosJogador.ToArray(), ShowroomControladorCarreira.TipoShowroom.escolher,
                    dados.corridaRef.filtroVeiculos);
                
                OnDesativar();
            });

            gameObject.SetActive(true);
        }

        public void OnAtivar()
        {
            
        }

        public void OnDesativar()
        {
            Reset();
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            selectCarroButton.onClick.RemoveAllListeners();
            pistaSprite.sprite = null;
            pistaNome.text = "";
            layoutNome.text = "";
            voltasText.text = "";
        }
    }
}
