using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsAnimator : MonoBehaviour
{
    public GameObject[] buttons;

    void OnEnable() 
    {
        float dl = 0.20f;
        foreach(GameObject go in buttons)
        {
            go.GetComponent<SimpleSlide>().Animate(dl);
            dl+=0.20f;
        }
    }
}
