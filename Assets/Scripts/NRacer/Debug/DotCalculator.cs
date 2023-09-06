using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotCalculator : MonoBehaviour
{
    Transform[] waypoints;

    private void Awake()
    {
        waypoints = GetComponentsInChildren<Transform>();
    }


    float calc(Vector3 A, Vector3 B, Vector3 C)
    {
        Vector3 ab = B - A;
        Vector3 bc = C - B;

        return Vector3.Dot(ab.normalized, bc.normalized);
    }

    [ExecuteInEditMode]
    public float CalculateDot(int id)
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            waypoints = GetComponentsInChildren<Transform>();
        }

        if (id == 0)
        {
            return calc(waypoints[waypoints.Length - 1].position, waypoints[id].position, waypoints[id + 1].position);
        }
        else if (id == waypoints.Length - 1)
        {
            return calc(waypoints[id - 1].position, waypoints[id].position, waypoints[0].position);
        }
        else
        {
            try
            {
                return calc(waypoints[id - 1].position, waypoints[id].position, waypoints[id + 1].position);
            }
            catch
            {
                return 0.99f;
            }
        }
    }
}
