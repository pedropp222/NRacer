using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(WaypointToSpline))]
public class WaypointSplineEditor : Editor
{
    SerializedProperty splineObj;
    SerializedProperty prefab;
    WaypointToSpline rc;

    private void OnEnable()
    {
        rc = (WaypointToSpline)target;
        splineObj = serializedObject.FindProperty("spline");
        prefab = serializedObject.FindProperty("p");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(splineObj);
        EditorGUILayout.PropertyField(prefab);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Create Texts"))
        {
            rc.InstantiateSpline();
        }
    }
}
