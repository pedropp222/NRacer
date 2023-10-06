using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Classe que controla as muitas coisas que vao ter no UI e faz comunicaçao direta com o controlador para fazer
/// coisas a nivel visual e interativo
/// </summary>
public class MenuUI : MonoBehaviour
{
    public Text tituloText;
    public GameObject contrato;

    public GameObject mainPanel;

    public string gameVersion;

    public GameObject background;

    public void RefreshUI(Controlador c)
    {
        if (c.NumeroJogadores()==0)
        {
            contrato.GetComponent<UIMenuPainel>().AtivarPainel();
        }
        else
        {
            mainPanel.SetActive(true);
        }

        tituloText.DOText("NRacer Ultimate\n"+gameVersion +(c.nomePlayerAtual.Length>0?" - "+c.nomePlayerAtual:""),1f).SetDelay(0.5f);
    }

    public void SetMenuBackground(bool ativado)
    {
        tituloText.gameObject.SetActive(ativado);
        background.gameObject.SetActive(ativado);
    }
}
