using Assets.Scripts.NRacer.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownUI : MonoBehaviour
{
    public Text textoCountdown;

    public void IniciarCountdown(TrackManager tm)
    {
        StartCoroutine(Countdown(tm));
    }

    private IEnumerator Countdown(TrackManager tm)
    {
        for(int i = 5; i >= 1; i--)
        {
            textoCountdown.text = i.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        textoCountdown.text = "";
        tm.LancarCarros();
    }
}
