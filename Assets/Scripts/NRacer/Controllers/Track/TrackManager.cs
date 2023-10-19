using Assets.Scripts.NRacer.Controllers.Track;
using NWH.VehiclePhysics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.NRacer.Controllers
{
    /// <summary>
    /// Classe que organiza informacoes sobre a pista atual.
    /// </summary>
    public class TrackManager : MonoBehaviour
    {
        [SerializeField] private TrackLayout[] trackLayouts;

        private int carrosCarregados = 0;
        private List<GameObject> carrosAtuais;

        private List<IObjetoPausa> objetoPausas;

        private int currentLayout;

        private Controlador contr;
        public DesktopInputManager manager;
        public Meta meta;

        public static TrackManager instancia;

        public GameObject playerCarro { get; private set; }

        public int maximoVoltas { get; private set; }

        private GestorInputsJogo inputs;

        private void Awake()
        {
            instancia = this;
            contr = Controlador.instancia;
            carrosAtuais = new List<GameObject>();
            objetoPausas = new List<IObjetoPausa>();
            inputs = FindAnyObjectByType<GestorInputsJogo>();

            if (inputs != null)
            {
                Debug.Log("Vai usar o novo sistema de inputs");

                inputs.AtivarConduzir();
                inputs.PausaEvent += Inputs_PausaEvent;
            }

            if (manager == null)
            {
                manager = FindAnyObjectByType<DesktopInputManager>();
            }

            if (meta == null)
            {
                meta = FindAnyObjectByType<Meta>();
            }

            if (contr == null)
            {
                Debug.Log("Iniciar pista em modo debug");

                CarregarLayout(0);

                GameObject gp = GameObject.Find("grelhaPartida");

                if (gp != null) gp.SetActive(false);

                maximoVoltas = 99;

                LancarCarros();
            }
            else
            {
                Debug.Log("Iniciar pista com o controlador");
                maximoVoltas = contr.corridaAtual.voltas;

                //O que o botao da grelha de partida tem que fazer quando clicado
                Button btn = FindAnyObjectByType<GrelhaPartidaUI>().botaoIniciar;
                if (btn != null)
                {
                    btn.onClick.AddListener(delegate { IniciarCountdown(); });
                    btn.Select();
                }

                for (int i = 0; i < contr.corridaAtual.startingGrid.Count; i++)
                {
                    Debug.Log("Colocar um carro: " + contr.corridaAtual.startingGrid[i]);

                    //teu carro
                    if (contr.corridaAtual.startingGrid[i].id == -1)
                    {
                        SpawnVeiculoPlayer(contr.carros[contr.carroSelecionado.id], contr.carroSelecionado);
                        playerCarro = carrosAtuais[carrosAtuais.Count - 1];
                    }
                    else
                    {
                        SpawnVeiculo(contr.aiCarros[contr.corridaAtual.startingGrid[i].id], contr.corridaAtual.startingGrid[i]);
                    }

                }

                RefreshGrelhaPartida();
            }
        }

        private void PlayerCarroSetup()
        {
            VehicleController vc = playerCarro.GetComponent<VehicleController>();

            FindAnyObjectByType<VehicleChanger>().vehicles.Add(vc);
            manager.vehicleController = vc;
        }

        /// <summary>
        /// Iniciar o controlador de countdown e so no fim e que comeca a corrida
        /// </summary>
        public void IniciarCountdown()
        {
            PlayerCarroSetup();

            for (int i = 0; i < carrosAtuais.Count; i++)
            {
                carrosAtuais[i].GetComponent<VehicleController>().Active = true;
            }

            FindAnyObjectByType<CountdownUI>().IniciarCountdown(this);
        }

        /// <summary>
        /// Desprender os carros e comecar a corrida
        /// </summary>
        public void LancarCarros()
        {
            foreach (VehicleController x in FindObjectsOfType<VehicleController>())
            {
                x.AtivarRodas();

                if (x.CompareTag("Vehicle"))
                {
                    if (playerCarro == null)
                    {
                        playerCarro = x.gameObject;
                        PlayerCarroSetup();
                    }
                    x.GetComponent<CarroInputDisable>().LancarCarro();
                }
                else
                {
                    if (x.GetComponent<VehicleAI>() != null)
                    {
                        x.GetComponent<VehicleAI>().LancarCarro();
                    }
                }

                objetoPausas.AddRange(x.GetComponents<IObjetoPausa>());
                x.Active = true;
            }
        }

        public void CarregarLayout(int id)
        {
            if (id < 0 || id >= trackLayouts.Length)
            {
                Debug.LogError("ATENCAO. TENTOU CARREGAR UM LAYOUT DE ID " + id + ", MAS SO EXISTEM " + trackLayouts.Length + " layouts!");
                return;
            }

            Debug.Log("Carregou layout " + id + ": " + trackLayouts[id].layoutName);
            currentLayout = id;
        }

        /// <summary>
        /// Obter o objeto parent dos waypoints do layout atual. Usado pelo vehicle AI
        /// </summary>
        public GameObject GetAiWaypoints()
        {
            return trackLayouts[currentLayout].waypointsAi;
        }

        /// <summary>
        /// Adicionar os veiculos que foram instanciados numa corrida para aparecerem na grelha de partida
        /// </summary>
        public void RefreshGrelhaPartida()
        {
            GrelhaPartidaUI grelhaPartida = FindAnyObjectByType<GrelhaPartidaUI>();

            if (grelhaPartida == null)
            {
                Debug.LogError("Nao existe objeto de grelha de partida");
                return;
            }

            foreach (GameObject go in carrosAtuais)
            {
                grelhaPartida.AdicionarVeiculo(go.GetComponent<CarroStats>());
            }
        }

        private bool VerificarEspacos()
        {
            if (carrosCarregados >= trackLayouts[currentLayout].spawnpointsParent.transform.childCount)
            {
                Debug.LogError("ERRO: Nao consegue instanciar um veiculo porque a pista nao tem spawns suficientes!");
                return false;
            }

            return true;
        }

        public void SpawnVeiculoPlayer(GameObject veiculo, CarroPlayerData carroData)
        {
            if (!VerificarEspacos()) return;

            GameObject o = Instantiate(veiculo);
            o.transform.position = trackLayouts[currentLayout].spawnpointsParent.transform.GetChild(carrosCarregados).position;
            o.transform.eulerAngles = trackLayouts[currentLayout].spawnpointsParent.transform.GetChild(carrosCarregados).localEulerAngles;
            o.GetComponent<CarroStats>().CarregarTrim(carroData.trimId);

            //desligar os veiculos
            o.GetComponent<VehicleController>().Active = false;

            carrosCarregados++;
            carrosAtuais.Add(o);
        }

        public void SpawnVeiculo(GameObject veiculo, CarroData carroData)
        {
            if (!VerificarEspacos()) return;

            GameObject o = Instantiate(veiculo);
            o.transform.position = trackLayouts[currentLayout].spawnpointsParent.transform.GetChild(carrosCarregados).position;
            o.transform.eulerAngles = trackLayouts[currentLayout].spawnpointsParent.transform.GetChild(carrosCarregados).localEulerAngles;
            o.GetComponent<CarroStats>().CarregarTrim(carroData.trimId);

            //desligar os veiculos
            o.GetComponent<VehicleController>().Active = false;

            int min = Controlador.instancia.filtroAtual.baseDificuldade - 3;
            if (min < 0) min = 0;
            if (Controlador.instancia.filtroAtual.baseDificuldade != -1)
            {
                o.GetComponent<VehicleAIDifficulty>().SetupDificuldade(UnityEngine.Random.Range(min, Controlador.instancia.filtroAtual.baseDificuldade));
            }

            carrosCarregados++;
            carrosAtuais.Add(o);
        }

        bool isPausa = false;

        private void ProcessarPausa()
        {
            if (objetoPausas.Count > 0)
            {
                isPausa = !isPausa;
                foreach (IObjetoPausa p in objetoPausas)
                {
                    if (isPausa)
                    {
                        p.OnPausa();
                    }
                    else
                    {
                        p.OnResume();
                    }
                }
            }
        }

        private void Inputs_PausaEvent(UnityEngine.InputSystem.InputActionPhase obj)
        {
            if (obj == UnityEngine.InputSystem.InputActionPhase.Performed)
            {
                ProcessarPausa();
            }
        }
    }

    public enum TipoCorrida
    {
        Free,
        Race
    }

}
