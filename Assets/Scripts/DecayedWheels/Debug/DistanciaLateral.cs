using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DEPRECADO, classe para ver a distancia lateral em relaçao a outro objeto
/// </summary>
public class DistanciaLateral : MonoBehaviour
{
    public Transform target;

    private void FixedUpdate()
    {
        //Debug.Log(Vector3.Cross(transform.TransformDirection(new Vector3(-Mathf.Cos(90f * Mathf.Rad2Deg), 0f, Mathf.Sin(90f * Mathf.Rad2Deg))), target.position).magnitude);

        if (Vector3.Cross(transform.TransformDirection(new Vector3(-Mathf.Cos(90f * Mathf.Rad2Deg), 0f, Mathf.Sin(90f * Mathf.Rad2Deg))),target.position).magnitude>165f)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            Debug.Log("Lateral Distance: " + distance);
        }
    }
}
