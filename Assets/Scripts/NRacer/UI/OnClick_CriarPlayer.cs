using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Classe para criar um novo jogador, para ser colocado num botao.
/// este botao precisa de um inputfield de onde vai buscar o nome, deopis disso o controlador
/// trata de verificar se ja existe ou se o nome é invalido ou se pode realmente criar.
/// </summary>
public class OnClick_CriarPlayer : MonoBehaviour, IButton
{
    public InputField texto;


    Button b;
    void Start()
    {
        b = GetComponent<Button>();

        b.interactable = false;

        texto.onValueChanged.AddListener(delegate
        {
            Interact();
        });
    }

    public void OnClick()
    {
        if (texto.text.Length > 0)
        {
            Controlador.instancia.CriarPlayer(texto.text);
        }
    }

    void Interact()
    {
        if (texto.text.Length > 0)
        {
            b.interactable = true;
        }
        else
        {
            b.interactable = false;
        }
    }
}
