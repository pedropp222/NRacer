using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CheckWeird : MonoBehaviour
{   
    void Start() {
        GameObject x = GameObject.Find("CloudsEditorState");

        if (x!=null)
        {
            Debug.Log("x existe?");
            DestroyImmediate(x);
        }
        else
        {
            Debug.Log("nope");
        }
    }
}
