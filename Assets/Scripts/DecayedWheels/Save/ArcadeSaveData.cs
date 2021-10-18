using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class ArcadeSaveData : SaveData, ISaveGame
{
    public int ano;
    public int semana;
    public int pontos;
    public List<int> teusCarros;

    public override void LoadGame(string name)
    {
        ArcadeSaveData data = new ArcadeSaveData();

        //limpeza do string, ficar apenas com o nome
        name = name.Replace("savedata\\", null);

        if (File.Exists("savedata\\" + name + "\\arcade\\mainData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open("savedata\\" + name + "\\arcade\\mainData.dat",FileMode.Open);
        
            try
            {
                data = (ArcadeSaveData)bf.Deserialize(file);

                this.PlayerName = data.PlayerName;
                this.ano = data.ano;
                this.semana = data.semana;
                this.pontos = data.pontos;
                this.teusCarros = data.teusCarros;
                file.Close();

            }
            catch
            {
                Debug.LogError("Erro nao especificado ao tentar carregar o jogo.");
                file.Close();
            }

            Debug.Log("Carregou o jogador " + data.PlayerName + " no modo arcade.");
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
        FileStream file = File.Create("savedata\\"+name+"\\arcade\\mainData.dat");

        ArcadeSaveData data = new ArcadeSaveData();

        data.PlayerName = name;

        data.ano = ano;
        data.semana = semana;
        data.pontos = pontos;
        data.teusCarros = teusCarros;

        bf.Serialize(file, data);

        file.Close();

        Debug.Log("Guardou o jogador "+data.PlayerName+" no modo arcade.");
    }
}