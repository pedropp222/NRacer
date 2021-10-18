using UnityEngine;

/// <summary>
/// Waypoint - objetos que o AI vai seguir - velocidade recomendada esta deprecado, nao e usado
/// </summary>
[ExecuteInEditMode()]
public class Waypoint : MonoBehaviour
{
    public float velocidadeRecomendada;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}