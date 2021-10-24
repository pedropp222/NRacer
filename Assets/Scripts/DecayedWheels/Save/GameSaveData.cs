using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class GameSaveData : SaveData, ISaveGame
{
    public CampeonatosController corridas;
    public int dinheiro = 0;   

    public List<int> carros = new List<int>();

    public override void LoadGame(string name)
    {
        GameSaveData data = new GameSaveData();

        //limpeza do string, ficar apenas com o nome
        name = name.Replace("savedata\\", null);

        if (File.Exists("savedata\\" + name + "\\game\\mainData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open("savedata\\" + name + "\\game\\mainData.dat", FileMode.Open);

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
            Debug.Log("Nao existe jogador " + name);
        }
    }

    public override void SaveGame(string name)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create("savedata\\" + name + "\\game\\mainData.dat");

        GameSaveData data = new GameSaveData
        {
            PlayerName = name,

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