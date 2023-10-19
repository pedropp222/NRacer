using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainelVerticalScrollUI : MonoBehaviour
{
    public RectTransform rect;

    public float posAtual = 0f;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Reset()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
        posAtual = 0f;
    }

    public GameObject InstanciarElemento(GameObject obj, float margem)
    {
        GameObject go = Instantiate(obj,transform);

        RectTransform goRect = go.GetComponent<RectTransform>();

        goRect.anchoredPosition = new Vector2(0f, posAtual - margem);

        posAtual -= margem + goRect.sizeDelta.y;

        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -posAtual);

        return go;
    }
}
