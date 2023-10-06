using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPainelEventos : UIMenuPainel
{
    public bool desativarInverterSequencia;

    public Component[] animacoesSequencia;

    bool aCorrer = false;

    public bool ativo = false;

    public override void AtivarPainel()
    {
        Debug.Log("Iniciar sequencia aparecer");
        gameObject.SetActive(true);
        StartCoroutine(CorrerSequencia());
    }

    private IEnumerator CorrerSequencia()
    {
        if (!aCorrer && !ativo)
        {
            aCorrer = true;
            for (int i = 0; i < animacoesSequencia.Length; i++)
            {
                yield return ((IUIAnimacao)animacoesSequencia[i]).AnimacaoAtivar();
            }

            if (botaoUIPrincipal != null)
            {
                botaoUIPrincipal.Select();
            }

            aCorrer = false;
            ativo = true;
        }
    }

    private IEnumerator CorrerSequenciaDesativar()
    {
        if (!aCorrer && ativo)
        {
            aCorrer = true;

            if (desativarInverterSequencia)
            {
                for (int i = animacoesSequencia.Length - 1; i >= 0; i--)
                {
                    yield return ((IUIAnimacao)animacoesSequencia[i]).AnimacaoDesativar();
                }
            }
            else
            {
                for (int i = 0; i < animacoesSequencia.Length; i++)
                {
                    yield return ((IUIAnimacao)animacoesSequencia[i]).AnimacaoDesativar();
                }
            }

            aCorrer = false;
            ativo = false;
            gameObject.SetActive(false);
        }
    }

    public override void DesativarPainel()
    {
        Debug.Log("Iniciar sequencia desaparecer");
        StartCoroutine(CorrerSequenciaDesativar());
    }
}
