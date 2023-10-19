using Assets.Scripts.NRacer.Controllers;
using Assets.Scripts.NRacer.GameMode.Career;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using UnityEngine;


[System.Serializable]
public class CarreiraSaveData : SaveData
{
    public int dinheiro;
    public Calendario.CalendarioData calendario;
    public List<CarroPlayerData> carrosPlayer;
    public List<Campeonato> listaCampeonatos;

    public override void LoadGame()
    {
        CarreiraSaveData data = new CarreiraSaveData();

        if (File.Exists("savedata\\" + PlayerName + "\\career\\mainData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open("savedata\\" + PlayerName + "\\career\\mainData.dat", FileMode.Open);

            try
            {
                data = (CarreiraSaveData)bf.Deserialize(file);

                this.PlayerName = data.PlayerName;
                this.dinheiro = data.dinheiro;
                this.calendario = data.calendario;
                this.carrosPlayer = data.carrosPlayer;
                this.listaCampeonatos = data.listaCampeonatos;
                file.Close();

            }
            catch
            {
                Debug.LogError("Erro nao especificado ao tentar carregar o jogo.");
                file.Close();
            }

            Debug.Log("Carregou o jogador " + data.PlayerName + " no modo carreira.");
            file.Close();
        }
        else
        {
            Debug.Log("Nao existe modo carreira neste player, a iniciar pela primeira vez!");
            ModoCarreira mc = new ModoCarreira();
            mc.Inicializar();
            this.dinheiro = mc.dinheiro;
            this.calendario = mc.calendario.ParaCalendarioData();
            this.carrosPlayer = mc.carrosJogador;
            this.listaCampeonatos = mc.campeonatoLista;
            mc.nomePlayer = PlayerName;
            SaveGame();
        }
    }

    public override void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (!Directory.Exists("savedata\\"+PlayerName+"\\career"))
        {
            Directory.CreateDirectory("savedata\\" + PlayerName + "\\career");
        }
        FileStream file = File.Create("savedata\\" + PlayerName + "\\career\\mainData.dat");

        bf.Serialize(file, this);

        file.Close();

        Debug.Log("Guardou o jogador " + this.PlayerName + " no modo carreira.");
    }
}
