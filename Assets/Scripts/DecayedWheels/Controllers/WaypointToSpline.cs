using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

/// <summary>
/// DEPRECADO - transformava waypoints em spline usando uma biblioteca
/// </summary>
public class WaypointToSpline : MonoBehaviour
{
    public BezierSpline spline;

    public GameObject p;

    [ExecuteInEditMode]
    public void InstantiateSpline()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            if (i%3==0)
            {
                GameObject x = Instantiate(p, spline.transform);
                x.transform.position = transform.GetChild(i).transform.position;
            }
        }
    }
}
