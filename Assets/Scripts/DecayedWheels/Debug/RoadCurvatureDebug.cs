using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoadCurvatureDebug : MonoBehaviour
{
    public GameObject textObj;

    dotCalculator dt;

    [ExecuteInEditMode]
    public void InstantiateTexts()
    {
        dt = GetComponent<dotCalculator>();

        for(int i = 0; i < transform.childCount; i++)
        {
            float m = GetMedias(i);
            GameObject go = Instantiate(textObj, transform.GetChild(i).transform.position + new Vector3(0f, 5f, 0f), Quaternion.identity);
            go.transform.GetChild(0).GetComponent<Text>().text = m.ToString();
        }
    }

    [ExecuteInEditMode]
    float GetMedias(int i)
    {
        int length = transform.GetComponentsInChildren<Waypoint>().Length;

        float media = -1f;

        if (i + 2 >= length)
        {
            media = (dt.CalculateDot(i) + dt.CalculateDot(0) + dt.CalculateDot(1)) / 3f;
        }
        else if (i + 1 >= length)
        {
            media = (dt.CalculateDot(i) + dt.CalculateDot(0) + dt.CalculateDot(1)) / 3f;
        }
        else
        {
            media = (dt.CalculateDot(i) + dt.CalculateDot(i + 1) + dt.CalculateDot(i + 2)) / 3f;
        }

        return media;
    }
}
