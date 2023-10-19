using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodarObjeto : MonoBehaviour
{
    public float velocidade;

    Vector3 amount;
    private void Start()
    {
        amount = new Vector3(0, velocidade, 0);
    }

    private void FixedUpdate()
    {
        transform.Rotate(amount*Time.fixedDeltaTime);
    }
}
