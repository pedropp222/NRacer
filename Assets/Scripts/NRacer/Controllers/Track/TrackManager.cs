using Assets.Scripts.NRacer.Controllers.Track;
using NWH.VehiclePhysics;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputManagerEntry;

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

        private int currentLayout;

        private Controlador contr;
        public DesktopInputManager manager;
        public Meta meta;

        public static TrackManager instancia;

        public GameObject playerCarro { get; private set; }

        public int maximoVoltas { get; private set; }

        private void Awake()
        {
            instancia = this;
            contr = FindAnyObjectByType<Controlador>();
            carrosAtuais = new List<GameObject>();

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

                FindAnyObjectByType<GrelhaPartidaUI>().botaoIniciar.onClick.AddListener(delegate { LancarCarros(); });
            }
        }

        public void LancarCarros()
        {          
            foreach (VehicleController x in FindObjectsOfType<VehicleController>())
            {
                if (x.CompareTag("Vehicle"))
                {
                    if (playerCarro == null)
                    {
                        playerCarro = x.gameObject;
                    }
                    x.GetComponent<CarroInputDisable>().LancarCarro();
                    FindAnyObjectByType<VehicleChanger>().vehicles.Add(x);
                    manager.vehicleController = x;
                }
                else
                {
                    if (x.GetComponent<VehicleAI>()!=null)
                    {
                        x.GetComponent<VehicleAI>().LancarCarro();
                    }                  
                }
            }
        }

        public void CarregarLayout(int id)
        {
            if (id < 0 || id >= trackLayouts.Length)
            {
                Debug.LogError("ATENCAO. TENTOU CARREGAR UM LAYOUT DE ID "+id+", MAS SO EXISTEM "+trackLayouts.Length +" layouts!");
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

            foreach(GameObject go in carrosAtuais)
            {
                grelhaPartida.AdicionarVeiculo(go.GetComponent<CarroStats>());
            }
        }

        public void SpawnVeiculo(GameObject veiculo, bool jogador, Controlador.CarroData carroData)
        {
            if(carrosCarregados >= trackLayouts[currentLayout].spawnpointsParent.transform.childCount)
            {
                Debug.LogError("ERRO: Nao consegue instanciar um veiculo porque a pista nao tem spawns suficientes!");
                return;
            }

            GameObject o = Instantiate(veiculo);
            o.transform.position = trackLayouts[currentLayout].spawnpointsParent.transform.GetChild(carrosCarregados).position;
            o.transform.eulerAngles = trackLayouts[currentLayout].spawnpointsParent.transform.GetChild(carrosCarregados).localEulerAngles;
            o.GetComponent<CarroStats>().CarregarTrim(carroData.trimId);

            if (jogador)
            {
                playerCarro = o;
            }
            else
            {
                int min = Controlador.instancia.filtroAtual.baseDificuldade - 3;
                if (min < 0) min = 0;
                if (Controlador.instancia.filtroAtual.baseDificuldade != -1)
                {
                    o.GetComponent<VehicleAIDifficulty>().SetupDificuldade(UnityEngine.Random.Range(min, Controlador.instancia.filtroAtual.baseDificuldade));
                }
            }

            carrosCarregados++;
            carrosAtuais.Add(o);
        }
    }

    public enum RaceType
    {
        Free,
        Race
    }

}
