using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiAnimacaoEnable : MonoBehaviour, IUIAnimacao
{
    public bool inverterOnDesativar;

    public IEnumerator AnimacaoAtivar()
    {
        gameObject.SetActive(true);
        yield return null;
    }

    public IEnumerator AnimacaoDesativar()
    {
        gameObject.SetActive(inverterOnDesativar ? true : false);
        yield return null;
    }
}
