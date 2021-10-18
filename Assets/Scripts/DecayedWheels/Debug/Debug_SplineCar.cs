using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deprecado. classe para ver certas coisas sobre este carro, classe ja nao e usada
/// </summary>
public class Debug_SplineCar : MonoBehaviour
{
    public AISplineTest ai;

    public bool steerTarget, breakDistance;

    private void FixedUpdate()
    {
        if (steerTarget)
        {
            transform.position = ai.target + new Vector3(0f, 2f, 0f);
        }
        if (breakDistance)
        {
            transform.position = ai.brakeLocation + new Vector3(0f, 2f, 0f);
        }
    }
}
