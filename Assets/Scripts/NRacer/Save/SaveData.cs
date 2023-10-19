using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string PlayerName;

    public virtual void LoadGame()
    {
        //O save data base apenas pode carregar no minimo o nome do jogador, tudo o resto é
        //de responsabilidade de cada classe filho
        if (PlayerName== null)
        {
            Debug.LogError("Impossivel carregar, pois o player name e nulo");
            return;
        }

        SaveData data = new SaveData();

        if (File.Exists("savedata\\" + PlayerName + "\\mainData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open("savedata\\" + PlayerName + "\\mainData.dat", FileMode.Open);

            try
            {
                data = (SaveData)bf.Deserialize(file);
                this.PlayerName = data.PlayerName;
                file.Close();
            }
            catch
            {
                Debug.LogError("Erro nao especificado ao tentar carregar o jogo.");
                file.Close();
            }

            Debug.Log("Carregou o jogador " + data.PlayerName + " no base save.");
            file.Close();
        }
        else
        {
            Debug.LogError("Nao existe jogador "+ PlayerName);
        }
    }
    public virtual void SaveGame()
    {
        if (PlayerName == null)
        {
            Debug.LogError("Impossivel gravar, pois o player name e nulo");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create("savedata\\" + PlayerName + "\\mainData.dat");

        bf.Serialize(file, this);

        file.Close();

        Debug.Log("Guardou o jogador " + this.PlayerName + " no modo base.");
    }
}