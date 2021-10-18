using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A meta verifica todos os checkpoints e verifica se o veiculo percorreu todos os checkpoints
/// dessa volta. se sim, adiciona 1 volta
/// </summary>
public class Meta : MonoBehaviour
{
    Checkpoint[] checkpoints;
    Controlador controlador;

    public FimCorrida fim;

    List<GameObject> acabaram;

    private void Awake()
    {
        checkpoints = FindObjectsOfType<Checkpoint>();
        controlador = FindObjectOfType<Controlador>();

        acabaram = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        CarroVolta x = other.transform.root.GetComponent<CarroVolta>();

        foreach(Checkpoint ch in checkpoints)
        {
            if (!ch.PassouCarro(x,x.voltas))
            {
                Debug.Log("VOLTA INVALIDADA");
               return;
            }
        }

        x.SomarVolta();

        if (x.voltas == controlador.corridaAtual.voltas)
        {
            //ACABOU
            acabaram.Add(other.gameObject);


            if (x.GetComponent<VehicleAI>()==null)
            {
                //NOSSO CARRO
                //MOSTRAR PAINEL
                fim.MostrarPainel(acabaram.Count);

                //transformar nosso carro em AI

                VehicleAI a = x.gameObject.AddComponent<VehicleAI>();
                a.step = 0.1f;
                a.useNewCalc = true;
                a.brakeAgressive = 0.4f;
                a.brakeDeceleration = -4f;
                a.dificuldade = 1f;
                a.AlookUp = new AnimationCurve();
                a.AlookUp.AddKey(0f,0f);
                a.AlookUp.AddKey(1f,0.5f);
                a.BlookUp = new AnimationCurve();
                a.BlookUp.AddKey(0f, 0f);
                a.BlookUp.AddKey(1f, 1f);

                a.LancarCarro(true);


                FindObjectOfType<NWH.VehiclePhysics.DesktopInputManager>().gameObject.SetActive(false);
            }
        }
    }
}