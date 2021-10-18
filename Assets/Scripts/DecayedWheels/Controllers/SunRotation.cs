using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Colocar o sol numa rotaçao aleatoria ao começar o nivel
/// </summary>
public class SunRotation : MonoBehaviour
{
    public float minX, maxX;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(new Vector3(Random.Range(minX,maxX),transform.eulerAngles.y,transform.eulerAngles.z));
    }
    
}
