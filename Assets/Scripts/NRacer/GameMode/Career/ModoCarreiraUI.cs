using Assets.Scripts.NRacer.Controllers;
using Assets.Scripts.NRacer.GameMode.Career.Reward;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.NRacer.GameMode.Career
{
    public class ModoCarreiraUI : MonoBehaviour
    {
        public static ModoCarreiraUI instancia;

        public static ModoCarreira carreiraContext;
        [SerializeField] public ModoCarreira carreiraDebug;

        public RecompensaManager recompensaManager;

        public Text dataText;
        public Text dinheiroText;

        public MensagemPainelUI mensagensUI;
        public CarroLootboxControlador carroLootboxControlador; 

        private List<ICarreiraPainelUI> paineisUI;

        public Button[] botoesTop;

        private void Awake()
        {
            Debug.LogWarning("Modo Carreira UI Awake");

            if (instancia == null)
            {
                instancia = this;

                if (Controlador.instancia.modoAtual is ModoCarreira carreira)
                {
                    carreiraContext = carreira;
                    carreiraDebug = carreira;
                    carreiraContext.carreiraUI = this;
                    Debug.Log("Carregou carreira context");
                }
                else
                {
                    Debug.Log("Nao carregou carreira context porque modoAtual e " + Controlador.instancia.modoAtual.GetType().Name);
                }

                paineisUI = new List<ICarreiraPainelUI>();
                paineisUI.AddRange(transform.GetComponentsInChildren<ICarreiraPainelUI>(true));

                RefreshBarraTop();

                //PRIMEIRA VEZ QUE INICIA O MODO CARREIRA, RECOMPENSAR COM UM VEICULO
                if (carreiraContext.carrosJogador.Count == 0)
                {
                    //FindAnyObjectByType<CarroLootboxControlador>(FindObjectsInactive.Include).IniciarLootbox(new CarroFiltro() { usarDesempenho = true, minDesempenho = 5, maxDesempenho = 12 });

                    SetBotoesTop(false);


                    carreiraContext.GerarCampeonato("Liga dos Ultimos", 7, 5, 200, 3, 3, 1);
                    carreiraContext.GerarCampeonato("Taca dos Iniciantes", 5, 20, 250, 6, 3, 1);
                    carreiraContext.GerarCampeonato("Concentracao dos Amadores", 12, 40, 500, 15, 2, 1);

                    recompensaManager.DarRecompensa(RecompensaData.LOOTBOX(new() { usarDesempenho = true, minDesempenho = 5, maxDesempenho = 12 }));

                    Controlador.instancia.GravarJogo();

                }
            }
        }

        public void ReceberResultadoCorrida(CorridaInfo info)
        {
            if (info.premio.tipo == TipoPremio.DINHEIRO)
            {
                carreiraContext.dinheiro += info.premio.valor;
                mensagensUI.AparecerMensagem("Parabens!", "Venceste a corrida e ganhaste uns incriveis " + info.premio.valor + "$!!!!");
                Controlador.instancia.GravarJogo();
            }
            else
            {
                mensagensUI.AparecerMensagem("Parabens!", "Venceste a corrida e ganhaste um veiculo novo!!!", () =>
                {
                    carroLootboxControlador.IniciarLootbox(info.premio.carroPremio);
                });
            }
        }

        public void SetBotoesTop(bool estado)
        {
            foreach(Button b in botoesTop)
            {
                b.interactable = estado;
            }
        }

        public void RefreshBarraTop()
        {
            if (carreiraContext == null) return;

            dataText.text = carreiraContext.calendario.GetDataString();
            dinheiroText.text = carreiraContext.dinheiro + "$";
        }

        public void DesativarTodosPaineis()
        {
            foreach(var item in paineisUI)
            {
                item.OnDesativar();
            }
        }

        public void AvancarDia()
        {
            if(carreiraContext.AvancarDia())
            {
                Debug.Log("Comecar um novo dia!!");
                RefreshBarraTop();
                Controlador.instancia.GravarJogo();
            }
            else
            {
                Debug.LogWarning("Existe um evento pendente para este dia, nao podes avancar!!! (pra ja)");
                mensagensUI.AparecerMensagem("Aviso", "Tens eventos pendentes para o dia de hoje. Participa em todos os eventos primeiro.");
            }
        }
    }
}