using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Reflection;

public class RendercamaraSprite : MonoBehaviour
{
    public Camera camara;

    RawImage imagem;

    public bool ativado = false;

    public int comprimento;
    public int altura;

    RenderTexture renderTexture;
    Rect rect;
    Texture2D texture;

    private RectTransform rt;

    private void Awake()
    {
        imagem = GetComponent<RawImage>();
        rt = GetComponent<RectTransform>();

        renderTexture = new RenderTexture(comprimento, altura, 24);
        rect = new Rect(0, 0, comprimento, altura);
        texture = new Texture2D(comprimento, altura, TextureFormat.RGBA32, false);

        RenderTexture.active = renderTexture;
        imagem.texture = renderTexture;
        
    }

    private void FixedUpdate()
    {
        if (ativado)
        {
            GerarSprite();
        }
    }

    private void GerarSprite()
    {
        camara.targetTexture = renderTexture;
        camara.Render();
        camara.targetTexture = null;      
    }

    public void Aparecer()
    {
        ativado = true;
        imagem.texture = renderTexture;
        rt.DOAnchorPos(new Vector2(0f, -85f), 0.55f);
    }

    public void Desaparecer()
    {
        ativado = false;
        imagem.texture = null;
        rt.anchoredPosition = new Vector2(1700f, -85f);
    }
}
