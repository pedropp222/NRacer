using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class SaveData : ISaveGame
{
    public string PlayerName;

    public virtual void LoadGame(string n)
    {
        //O save data base apenas pode carregar no minimo o nome do jogador, tudo o resto é
        //de responsabilidade de cada classe filho


        //limpeza do string, ficar apenas com o nome
        n = n.Replace("savedata\\", null);

        SaveData data = new SaveData();

        if (File.Exists("savedata\\" + n + "\\arcade\\mainData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open("savedata\\" + n + "\\arcade\\mainData.dat", FileMode.Open);

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
            Debug.LogError("Nao existe jogador "+n);
        }
    }
    public virtual void SaveGame(string n)
    {
        Debug.LogError("Nao se pode chamar este metodo de gravar pois nao tem info de nenhum gamemode");
    }
}