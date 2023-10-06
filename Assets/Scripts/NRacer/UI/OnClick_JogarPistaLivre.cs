using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClick_JogarPistaLivre : MonoBehaviour, IButton
{
    public LivreGameModeUI livreGame;
    public LoadingPainelUI loadingUI;

    public void OnClick()
    {
        livreGame.IniciarCorrida(loadingUI);      
    }
}
