using UnityEngine;
using System.Collections;

/// <summary>
/// Classe para criar os nodes para o AI seguir automaticamente ao conduzirmos na pista
/// </summary>
public class NodeCreator : MonoBehaviour
{
    [Header("Tecla para ativar - V")]
    public GameObject node;
    public NWH.VehiclePhysics.VehicleController cr;

    bool canChange = false;
    bool acolocar = false;

    public bool useDistance = false;
    GameObject lastObj = null;

    void Start()
    {
        cr = GetComponent<NWH.VehiclePhysics.VehicleController>();
    }

    IEnumerator colocar()
    {
        yield return new WaitForSeconds(0.6f);
        Instantiate(node, transform.position, Quaternion.identity);
        if (acolocar)
        {
            StartCoroutine(colocar());
        }
    }

    void Update()
    {
        if (!useDistance)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                acolocar = !acolocar;
                canChange = true;
            }
            if (canChange)
            {
                canChange = false;
                StartCoroutine(colocar());
            }
        }
        else
        {
            if (acolocar)
            {
                if (lastObj == null)
                {
                    lastObj = Instantiate(node, transform.position, Quaternion.identity);
                }
                else
                {
                    if (Vector3.Distance(transform.position, lastObj.transform.position) >= 18f)
                    {
                        lastObj = Instantiate(node, transform.position, Quaternion.identity);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                acolocar = !acolocar;
            }
        }
    }
}