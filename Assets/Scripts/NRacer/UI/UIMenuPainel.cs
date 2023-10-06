using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIMenuPainel : MonoBehaviour
{
    protected bool ativado;

    /// <summary>
    /// O botao que vai estar 'selecionado' por defeito quando o painel aparece
    /// </summary>
    public Button botaoUIPrincipal;

    public abstract void AtivarPainel();

    public abstract void DesativarPainel();
}
