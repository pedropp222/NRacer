using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.NRacer.Controllers;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using JetBrains.Annotations;

namespace Assets.Scripts.NRacer.GameMode.Career
{
    [System.Serializable]
    public class ModoCarreira : Controllers.GameMode
    {
        private static int contagemCampeonatos;

        public string nomePlayer;

        public Calendario calendario;
        public int dinheiro;

        public List<Campeonato> campeonatoLista;
        public List<Campeonato> campeonatosCompletados;

        public List<CarroPlayerData> carrosJogador;

        public ModoCarreiraUI carreiraUI;

        private CarreiraSaveData saveData;

        public override void Inicializar()
        {
            campeonatoLista = new List<Campeonato>();
            campeonatosCompletados = new List<Campeonato>();
            carrosJogador = new List<CarroPlayerData>();
            calendario = new Calendario();
            dinheiro = 1000;
            nomePlayer = "Ninguem";
            contagemCampeonatos = 0;
        }

        public void InicializarFromSave(CarreiraSaveData dados)
        {
            calendario = new Calendario(dados.calendario);
            dinheiro = dados.dinheiro;
            carrosJogador = dados.carrosPlayer;

            campeonatoLista = new List<Campeonato>();
            campeonatosCompletados = new List<Campeonato>();

            campeonatoLista = dados.listaCampeonatos;

            contagemCampeonatos = dados.listaCampeonatos.Count;

            nomePlayer = dados.PlayerName;

            saveData = dados;
        }

        public override void ReceberResultadoCorrida(CorridaInfo info)
        {
            campeonatoLista[info.campeonatoID].SetParticipado(info.corridaID);
            if (info.resultado.posicaoFinal == 1)
            {
                campeonatoLista[info.campeonatoID].SetGanho(info.corridaID);
                if (campeonatoLista[info.campeonatoID].TudoGanho())
                {
                    //Dar premio de campeonato

                    campeonatosCompletados.Add(campeonatoLista[info.campeonatoID]);
                    campeonatoLista.Remove(campeonatoLista[info.campeonatoID]);
                }

                carreiraUI.ReceberResultadoCorrida(info);
            }
        }

        public List<CorridaData> ObterCorridasMes(int mes)
        {
            Debug.Log("Obter corridas para o ano " + calendario.GetAnoAtual() + ", mes " + mes);

            List<CorridaData> infos = new List<CorridaData>();

            foreach(Campeonato cmp in campeonatoLista)
            {
                for(int i = 0; i < cmp.corridasLista.Length; i++)
                {
                    if (cmp.corridasLista[i].data.Ano == calendario.GetAnoAtual() && cmp.corridasLista[i].data.Mes == mes)
                    {
                        infos.Add(new CorridaData(cmp, cmp.corridasLista[i],i));
                    }
                }
            }

            return infos;
        }

        public List<CorridaData> GetCorridasData(Calendario.CalendarioData data)
        {
            List<CorridaData> eventos = new List<CorridaData>();

            foreach(Campeonato cmp in campeonatoLista)
            {
                for(int i = 0; i < cmp.corridasLista.Length;i++)
                {
                    if (cmp.corridasLista[i].data == data)
                    {
                        eventos.Add(new CorridaData(cmp, cmp.corridasLista[i],i));
                    }
                }
            }

            return eventos;
        }

        public bool ExisteCorridaData(Calendario.CalendarioData data)
        {
            return campeonatoLista.Find((c) => c.corridasLista.Any((cc) => cc.data == data)) != null;
        }

        public void GerarCampeonato(string campeonatoNome, int numPistas, int diaInicial, int premioDinheiro, int desempenhoInicial, int incrementoLinear, int incrementoPower)
        {
            Campeonato camp = new Campeonato();

            Debug.Log("Iniciar geracao de campeonato");

            camp.nomeCampeonato = campeonatoNome;
            camp.id = contagemCampeonatos;
            contagemCampeonatos++;

            List<CorridaRules> corridas = new List<CorridaRules>();

            int currentDia = diaInicial;

            for(int i = 0; i < numPistas; i++)
            {
                var cr = new CorridaRules();
                
                cr.premioDinheiro = premioDinheiro;
                cr.nivel = Controlador.instancia.PistaAleatoria(); cr.voltas = 2; cr.maxOponentes = 5;
                cr.data = Calendario.CalendarioData.CriarFromDia(currentDia);
                currentDia += Random.Range(4, 9);
                //Debug.Log("Criou corrida para o dia: " + cr.data.GetDataString());

                int des = desempenhoInicial + (int)Mathf.Pow(incrementoLinear * i, 1.0f + incrementoPower / 100f);

                cr.filtroVeiculos = new CarroFiltro()
                {
                    usarDesempenho = true,
                    minDesempenho = des,
                    maxDesempenho = des + 10
                };

                corridas.Add(cr);
            }

            camp.corridasLista = corridas.ToArray();

            campeonatoLista.Add(camp);
            Debug.Log("Gerou um campeonato com " + numPistas + " corridas, a iniciar no dia "+diaInicial);

            saveData.listaCampeonatos = campeonatoLista;
        }

        public bool AvancarDia()
        {
            List<CorridaData> corridasDias = GetCorridasData(calendario.ParaCalendarioData());

            if (corridasDias.Count > 0)
            {
                foreach(CorridaData d in corridasDias)
                {
                    if (!d.corridaRef.participou)
                    {
                        return false;
                    }
                }

                calendario.IncrementarDia();
                saveData.calendario = calendario.ParaCalendarioData();
                return true;
            }
            else
            {
                calendario.IncrementarDia();
                saveData.calendario = calendario.ParaCalendarioData();
                return true;
            }
        }

        public override void ObterCarro(CarroPlayerData carro)
        {
            carrosJogador.Add(carro);
            saveData.carrosPlayer = carrosJogador;
            Controlador.instancia.GravarJogo();
        }
    }
}

public struct CorridaData
{
    public Campeonato campeonatoRef;
    public CorridaRules corridaRef;
    public int eventoIndex;

    public CorridaData(Campeonato cmp, CorridaRules corrida, int index)
    {
        this.campeonatoRef = cmp;
        this.corridaRef = corrida;
        this.eventoIndex = index;
    }

    public static CorridaData VAZIO
    {
        get
        {
            return new CorridaData() { campeonatoRef = null, corridaRef = null, eventoIndex = -1 };
        }     
    }

    public static bool operator == (CorridaData a, CorridaData b) 
    {
        return a.campeonatoRef == b.campeonatoRef && a.corridaRef == b.corridaRef && a.eventoIndex == b.eventoIndex;
    }

    public static bool operator != (CorridaData a, CorridaData b)
    {
        return !(a == b);
    }
}
