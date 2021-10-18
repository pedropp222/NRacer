using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class guiMan : MonoBehaviour
{
    Rigidbody carro;

    public Text km;

    // Start is called before the first frame update
    void Start()
    {
        carro = GameObject.FindGameObjectWithTag("Vehicle").GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        km.text = (carro.velocity.magnitude * 3.6f).ToString("f0") + "km/h";
    }
}
