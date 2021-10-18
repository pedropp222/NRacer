using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueSlider : MonoBehaviour
{
    Text t;

    private void Start()
    {
        GetComponent<Slider>().onValueChanged.AddListener(delegate { RefreshText((int)GetComponent<Slider>().value); });
        t = transform.GetChild(3).GetComponent<Text>();

        GetComponent<Slider>().value = 5f;
    }

    void RefreshText(int v)
    {
        if (v <= 4)
        {
            t.text = "Fácil";
            if (v <= 2)
            {
                t.text = "Muito Fácil";
            }
        }
        else if (v >= 7)
        {
            t.text = "Difícil";
            if (v >= 9)
            {
                t.text = "Muito Difícil";
            }
        }
        else
        {
            t.text = "Médio";
        }
    }
}
