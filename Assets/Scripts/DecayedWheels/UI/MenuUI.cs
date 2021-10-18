﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Classe que controla as muitas coisas que vao ter no UI e faz comunicaçao direta com o controlador para fazer
/// coisas a nivel visual e interativo
/// </summary>
public class MenuUI : MonoBehaviour
{
    public Text dinheiroText;
    public Text progressText;
    public GameObject contrato;

    public void RefreshUI(Controlador c)
    {
        dinheiroText.text = c.dinheiro.ToString() + "$";
        progressText.text = "Completado " + c.percentagemJogoGanho + "%";
        if (c.NumeroJogadores()==0)
        {
            contrato.SetActive(true);
        }
    }
}