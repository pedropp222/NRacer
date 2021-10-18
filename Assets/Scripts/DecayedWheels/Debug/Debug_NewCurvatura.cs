using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DEPRECADO - verificava curvatura de uma seçao
/// </summary>
public class Debug_NewCurvatura : MonoBehaviour
{
    AIControllerV3 AI;


    private void Start()
    {
        AI = GetComponent<AIControllerV3>();
    }

    private void FixedUpdate()
    {
        Debug.Log("Curvatura nos prox. 100 metros: " + CalculaCurvatura(AI.atual,100f));
    }

    float CalculaCurvatura(int atual, float distancia)
    {
        float distanciaTotal = 0;
        float finalDot = 0;

        while (distanciaTotal < distancia)
        {
            int proximo = atual + 1;

            float distanciaProximo = Vector3.Distance(AI.waypoints[atual].transform.position, AI.waypoints[proximo].transform.position);
            float curvaturaLocal = AI.waypointDot.CalculateDot(atual);

            distanciaTotal += distanciaProximo;

            if (distanciaTotal > distancia)
            {
                distanciaProximo -= (distanciaTotal - distancia);
            }
      
             finalDot += curvaturaLocal * (distanciaProximo / distancia);

            atual = proximo;
        }
        return finalDot;
    }
}
