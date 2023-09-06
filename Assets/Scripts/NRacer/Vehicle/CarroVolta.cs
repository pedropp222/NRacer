using UnityEngine;
using System.Collections;

/// <summary>
/// Classe simples que guarda a volta em que este veiculo se encontra
/// Usado por Checkpoint, Meta, CarroHUD
/// </summary>
public class CarroVolta : MonoBehaviour
{
    public int voltas = 0;

    public void SomarVolta()
    {
        voltas++;
    }
}