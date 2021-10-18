using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(RoadCurvatureDebug))]
public class RoadCurvatureEditor : Editor
{
    SerializedProperty text;
    RoadCurvatureDebug rc;

    private void OnEnable()
    {
        rc = (RoadCurvatureDebug)target;
        text = serializedObject.FindProperty("textObj");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(text);
        serializedObject.ApplyModifiedProperties();

        if (text!=null)
        {
            if (GUILayout.Button("Create Texts"))
            {
                rc.InstantiateTexts();
            }
        }
    }
}
