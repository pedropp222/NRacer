using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClick_MudarLevel : MonoBehaviour, IButton
{
    public int id;
    public void OnClick()
    {
        SceneManager.LoadScene(id);
    }
}
