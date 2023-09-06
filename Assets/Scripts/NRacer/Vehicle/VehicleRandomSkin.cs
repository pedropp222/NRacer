using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Selecionar uma skin aleatoria para um carro
/// </summary>
public class VehicleRandomSkin : MonoBehaviour
{
    public MeshRenderer[] meshes;

    public Material[] materiais;

    private void Start()
    {
        int rand = Random.Range(0, materiais.Length);

        foreach (MeshRenderer l in meshes)
        {
            l.material = materiais[rand];
        }
    }
}
