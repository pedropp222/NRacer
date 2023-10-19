using Assets.Scripts.NRacer.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Selecionar uma skin aleatoria para um carro
/// </summary>
public class VehicleRandomSkin : MonoBehaviour
{
    public MeshRenderer[] meshes;

    //aqui e para ter pinturas nos carros, e nao cores random
    public Material[] materiais;

    //cores random e aqui que se ativa
    public bool usarRandomCor;

    public Color corEscolhida;

    private void Start()
    {
        if (usarRandomCor)
        {
            corEscolhida = CarroCor.RANDOM().ParaCor();

            foreach(MeshRenderer l in meshes)
            {
                l.material.color = corEscolhida;
            }
        }
        else
        {
            int rand = Random.Range(0, materiais.Length);

            foreach (MeshRenderer l in meshes)
            {
                l.material = materiais[rand];
            }
        }
    }
}
