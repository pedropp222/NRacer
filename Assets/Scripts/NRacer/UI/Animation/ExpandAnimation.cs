using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ExpandAnimation : MonoBehaviour
{
    RectTransform rect;

    public float finalWidth;

    public bool delay;

    void OnEnable()
    {
        rect = GetComponent<RectTransform>();

        rect.sizeDelta = new Vector2(0f,rect.sizeDelta.y);

        if (delay)
        {
            rect.DOSizeDelta(new Vector2(finalWidth,rect.sizeDelta.y),0.60f)
            .SetDelay(0.5f);
        }
        else
        {
             rect.DOSizeDelta(new Vector2(finalWidth,rect.sizeDelta.y),0.60f);
        }
    }
}
