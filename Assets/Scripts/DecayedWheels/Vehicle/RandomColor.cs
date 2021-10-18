using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cor random de um mesh
/// </summary>
public class RandomColor : MonoBehaviour
{
    public MeshRenderer[] meshes;

    private void Start()
    {
        Color x = new Color(Random.Range(0f, 255f) / 255f, Random.Range(0f, 255f) / 255f, Random.Range(0f, 255f) / 255f);

        foreach(MeshRenderer l in meshes)
        {
            l.materials[0].color = x;
        }
    }
}