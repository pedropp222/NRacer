using Assets.Scripts.NRacer.Controllers;
using Assets.Scripts.NRacer.Vehicle;
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

    public FimCorrida fim;

    List<GameObject> acabaram;

    private void Awake()
    {
        checkpoints = FindObjectsOfType<Checkpoint>();

        acabaram = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("CarCol"))
        {
            return;
        }

        CarroVolta x = other.transform.root.GetComponent<CarroVolta>();
        bool player = other.transform.root.gameObject == TrackManager.instancia.playerCarro;

        foreach (Checkpoint ch in checkpoints)
        {
            if (!ch.PassouCarro(x,x.voltas))
            {
                Debug.Log("VOLTA INVALIDADA");
                if (player)
                {
                    TrackManager.instancia.playerCarro.GetComponent<CarroCronometro>().Reiniciar(false);
                }
               return;
            }
        }

        x.SomarVolta();
        if (player)
        {
            TrackManager.instancia.playerCarro.GetComponent<CarroCronometro>().Reiniciar(true);
        }


        if (x.voltas == TrackManager.instancia.maximoVoltas)
        {
            //ACABOU
            acabaram.Add(other.gameObject);

            if (x.GetComponent<VehicleAI>()==null)
            {
                //NOSSO CARRO
                //MOSTRAR PAINEL
                fim.MostrarPainel(acabaram.Count);
                TrackManager.instancia.playerCarro.GetComponent<CarroCronometro>().Parar();

                //transformar nosso carro em AI

                VehicleAI a = x.gameObject.AddComponent<VehicleAI>();
                a.step = 0.1f;
                a.useNewCalc = true;
                a.brakeAgressive = 0.4f;
                a.brakeDeceleration = -8f;
                a.dificuldade = 0.5f;
                a.AlookUp = new AnimationCurve();
                a.AlookUp.AddKey(0f,0f);
                a.AlookUp.AddKey(1f,0.5f);
                a.BlookUp = new AnimationCurve();
                a.BlookUp.AddKey(0f, 0f);
                a.BlookUp.AddKey(1f, 1f);
                a.useRubberbanding = false;

                a.LancarCarro(true);


                FindObjectOfType<NWH.VehiclePhysics.DesktopInputManager>().gameObject.SetActive(false);
            }
        }
    }
}