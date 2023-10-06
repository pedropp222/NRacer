using UnityEngine;
using System.Collections;
using Assets.Scripts.NRacer.Controllers;

/// <summary>
/// Classe simples que guarda a volta em que este veiculo se encontra
/// Usado por Checkpoint, Meta, CarroHUD
/// </summary>
public class CarroVolta : MonoBehaviour
{
    public int voltas = 0;

    private TrackManager tm;

    private void Start()
    {
        tm = FindObjectOfType<TrackManager>();
    }

    public void SomarVolta()
    {
        voltas++;
    }

    public bool UltimaVolta()
    {
        return voltas == tm.maximoVoltas-1;
    }
}