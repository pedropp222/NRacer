using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPainelUI : MonoBehaviour
{
    public Text percText;

    bool loading = false;

    public void CarregarNivel(int id)
    {
        if (loading) return;
        GetComponent<UIPainelEventos>().AtivarPainel();
        StartCoroutine(CarregarAsync(id));
        
    }

    private IEnumerator CarregarAsync(int id)
    {
        loading = true;      
        yield return new WaitForSeconds(0.2f);
        AsyncOperation load = SceneManager.LoadSceneAsync(id);
        while(!load.isDone)
        {
            percText.text = Mathf.Floor(load.progress) + "%";
            Debug.Log("PROGRESSO: "+load.progress);
            yield return new WaitForEndOfFrame();
        }
        load.allowSceneActivation = true;
        loading = false;
    }
}
