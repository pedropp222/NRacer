using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEventoBotoes : MonoBehaviour, IUIAnimacao
{
    public Button[] botoes;

    public IEnumerator AnimacaoAtivar()
    {
        for(int i = 0; i < botoes.Length; i++)
        {
            botoes[i].interactable = true;
        }
        yield return null;
    }

    public IEnumerator AnimacaoDesativar()
    {
        for (int i = 0; i < botoes.Length; i++)
        {
            botoes[i].interactable = false;
        }
        yield return null;
    }
}
