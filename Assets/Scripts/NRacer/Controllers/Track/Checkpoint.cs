using Assets.Scripts.NRacer.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checkpoints sao triggers colocados no circuito que regista se um veiculo ja la passou nessa volta
/// se nao passou, regista e envia para esse veiculo a sua posiçao
/// </summary>
public class Checkpoint : MonoBehaviour
{
    public List<Volta> voltas;

    private void Start()
    {
        //TODO: Isto ta uma porcaria, e preciso alterar isto tudo, o sistema de voltas nao pode funcionar assim
        // No maximo tem que criar apenas 1 objeto volta quando e preciso

        voltas = new List<Volta>();        
        for (int i = 0; i < TrackManager.instancia.maximoVoltas; i++)
        {
            voltas.Add(new Volta());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CarroVolta c = other.transform.root.GetComponent<CarroVolta>();

        if (RegistarCarro(c, c.voltas))
        {
            Carro_HUD cHUD = other.transform.root.GetComponent<Carro_HUD>();

            if (cHUD != null)
            {
                cHUD.ReceberPosicao(RetornarPosicao(c, c.voltas) + 1);
            }
        }
    }

    /// <summary>
    /// Verificar se este carro ja passou ou nao
    /// </summary>
    /// <param name="c"></param>
    /// <param name="volta"></param>
    /// <returns></returns>
    public bool PassouCarro(CarroVolta c, int volta)
    {
        if (volta >= voltas.Count)
        {
            return false;
        }

        for (int i = 0; i < voltas[volta].carros.Count; i++)
        {
            if (voltas[volta].carros[i] == c)
            {
                return true;
            }
        }

        return false;
    }

    public bool RegistarCarro(CarroVolta carro, int volta)
    {
        if (volta >= voltas.Count || voltas[volta].carros.Contains(carro))
        {
            //Debug.Log("Carro ja foi registado");
            return false;
        }
        voltas[volta].carros.Add(carro);
        return true;
    }

    public int RetornarPosicao(CarroVolta carro, int volta)
    {
        for(int i = 0; i < voltas[volta].carros.Count;i++)
        {
            if (voltas[volta].carros[i] == carro)
            {
                return i;
            }
        }

        return -1;
    }
}

public class Volta
{
    public List<CarroVolta> carros;

    public Volta()
    {
        carros = new List<CarroVolta>();
    }
}