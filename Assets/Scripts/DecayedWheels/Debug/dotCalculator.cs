using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dotCalculator : MonoBehaviour
{
    Waypoint[] waypoints;

    private void Awake()
    {
        waypoints = GetComponentsInChildren<Waypoint>();
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
            waypoints = GetComponentsInChildren<Waypoint>();
        }

        if (id == 0)
        {
            return calc(waypoints[waypoints.Length - 1].transform.position, waypoints[id].transform.position, waypoints[id + 1].transform.position);
        }
        else if (id == waypoints.Length - 1)
        {
            return calc(waypoints[id - 1].transform.position, waypoints[id].transform.position, waypoints[0].transform.position);
        }
        else
        {
            try
            {
                return calc(waypoints[id - 1].transform.position, waypoints[id].transform.position, waypoints[id + 1].transform.position);
            }
            catch
            {
                return 0.99f;
            }
        }
    }
}
