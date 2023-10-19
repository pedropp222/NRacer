using Assets.Scripts.NRacer.GameMode.Career;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MensagemPainelUI : MonoBehaviour
{
    public Text tituloText;
    public Text mensagemText;

    public Button botaoFechar;

    public void AparecerMensagem(string titulo, string mensagem)
    {
        ModoCarreiraUI.instancia.SetBotoesTop(false);

        tituloText.text = titulo;
        mensagemText.text = mensagem;

        gameObject.SetActive(true);

        botaoFechar.onClick.RemoveAllListeners();
        botaoFechar.onClick.AddListener(() => {
            ModoCarreiraUI.instancia.SetBotoesTop(true);
            gameObject.SetActive(false);
            });
    }

    public void AparecerMensagem(string titulo, string mensagem, UnityAction botaoEventoExtra)
    {
        AparecerMensagem(titulo, mensagem);
        botaoFechar.onClick.AddListener(botaoEventoExtra);
    }
}
