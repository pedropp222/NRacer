using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NWH.VehiclePhysics;

public class CarWinAnimation : MonoBehaviour
{

    public Light[] luzes;
    public Text texto;
    public UIAnimacaoSlide slideBTN;

    VehicleController vehicleController;

    AudioSource asource;

    private void Start() 
    {

        Controlador c = FindObjectOfType<Controlador>();

        if (c!=null)
        {
            Instantiate(c.aiCarros[c.filtroAtual.premioCarro],new Vector3(0f,5f,0f),Quaternion.identity);
        }

        StartCoroutine(Anim());

        asource = GetComponent<AudioSource>();
        vehicleController = FindObjectOfType<VehicleController>();
    }

    IEnumerator Anim()
    {
        luzes[0].shadowStrength = 0f;
        luzes[1].shadowStrength = 0f;
        luzes[2].shadowStrength = 0f;

        yield return new WaitForSeconds(1.2f);
        luzes[0].enabled = true;
        luzes[0].GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1.0f);
        luzes[1].enabled = true;
        luzes[1].GetComponent<AudioSource>().Play();
        texto.DOColor(new Color(1.0f,1.0f,1.0f,1.0f),1.35f);
        yield return new WaitForSeconds(0.6f);
        luzes[2].enabled = true;
        luzes[2].GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1.45f);
        FindObjectOfType<Rigidbody>().isKinematic = false;

        yield return new WaitForSeconds(0.5f);

        luzes[0].DOShadowStrength(0.8f,1f);
        luzes[1].DOShadowStrength(0.8f,1f);
        luzes[2].DOShadowStrength(0.8f,1f);

        yield return new WaitForSeconds(2.0f);

        asource.clip = vehicleController.sound.engineIdleComponent.Clip;
        asource.loop = true;
        asource.pitch = vehicleController.sound.engineIdleComponent.pitch;
        asource.volume = 0f;
        asource.Play();

        asource.DOFade(0.3f,1f);
        slideBTN.Animate(0f);
    }
}
