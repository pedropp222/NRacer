using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SimpleSlide : MonoBehaviour 
{
    public float finalX;
    public bool automatic;

    public float time=-1;

    private void Start() {
        if (automatic)
        {
            Animate(0.1f);
        }
    }


    public void Animate(float delay)
    {
        RectTransform rect = GetComponent<RectTransform>();

        rect.DOAnchorPosX(finalX,time<0?0.35f:time).SetDelay(delay);
    }
}