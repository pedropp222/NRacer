using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Classe para mudar a posiçao relativa da camara em relaçao a um carro
/// </summary>
public class CameraOffset : MonoBehaviour
{
    public UnityStandardAssets.Cameras.AutoCam cam;
    public Vector3 amount;

    private void Start()
    {
        cam = FindObjectOfType<UnityStandardAssets.Cameras.AutoCam>();
        if (cam==null)return;
        if (!cam.offsetDone)
        {
            //prioritar o nosso carro apenas
            if (gameObject.CompareTag("Vehicle"))
            {
                cam.transform.GetChild(0).transform.Translate(amount, Space.Self);
                cam.offsetDone = true;
            }

            
        }
    }
}