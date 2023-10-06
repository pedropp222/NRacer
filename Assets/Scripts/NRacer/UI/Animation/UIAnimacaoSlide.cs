using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class UIAnimacaoSlide : MonoBehaviour, IUIAnimacao
{
    public Vector2 posInicial;
    public Vector2 posFinal;

    //public bool automatic;

    RectTransform rect;

    public float time=-1;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Animate(float delay)
    {
        rect.DOAnchorPos(posFinal,time<0?0.35f:time).SetDelay(delay);
    }

    public IEnumerator AnimacaoAtivar()
    {
        bool complete = false;
        var tween = rect.DOAnchorPos(posFinal, time < 0 ? 0.35f : time);

        tween.OnComplete(() => complete = true);
        yield return new WaitWhile(() => !complete);
    }

    public IEnumerator AnimacaoDesativar()
    {
        bool complete = false;
        var tween = rect.DOAnchorPos(posInicial, time < 0 ? 0.35f : time);

        tween.OnComplete(() => complete = true);
        yield return new WaitWhile(() => !complete);
    }
}