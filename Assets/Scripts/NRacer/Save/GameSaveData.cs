using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.NRacer.Controllers;

[System.Serializable]
[Obsolete("Isto nao e mais pra usar, refere-se a um game mode arcaico qualquer")]
public class GameSaveData : SaveData
{
    public CampeonatosController corridas;
    public int dinheiro = 0;   

    public List<CarroData> carros = new List<CarroData>();

    public override void LoadGame()
    {
        GameSaveData data = new GameSaveData();


        if (File.Exists("savedata\\" + PlayerName + "\\game\\mainData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open("savedata\\" + PlayerName + "\\game\\mainData.dat", FileMode.Open);

            try
            {
                data = (GameSaveData)bf.Deserialize(file);

                this.PlayerName = data.PlayerName;
                this.dinheiro = data.dinheiro;
                this.corridas = data.corridas;
                this.carros = data.carros;
                file.Close();

            }
            catch
            {
                Debug.LogError("Erro nao especificado ao tentar carregar o jogo.");
                file.Close();
            }

            Debug.Log("Carregou o jogador " + data.PlayerName + " no modo game.");
            file.Close();
        }
        else
        {
            Debug.Log("Nao existe jogador " + PlayerName);
        }
    }

    public override void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create("savedata\\" + PlayerName + "\\game\\mainData.dat");

        GameSaveData data = new GameSaveData
        {
            PlayerName = this.PlayerName,

            dinheiro = dinheiro,
            corridas = corridas,
            carros = carros
        };

        bf.Serialize(file, data);

        file.Close();

        Debug.Log("Guardou o jogador " + data.PlayerName + " no modo game.");
    }

    public void GravarCorridaWin(CampeonatosController ctr)
    {
        corridas = ctr;
    }
}