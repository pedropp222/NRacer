using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;


[CustomEditor(typeof(PrefabBrush))]
[ExecuteInEditMode]
public class PrefabBrush_Editor : Editor
{
    PrefabBrush brush;

    RaycastHit hit;

    SerializedProperty prefabs;

    bool instantiate = true;

    private void OnEnable()
    {
        brush = (PrefabBrush)target;
        prefabs = serializedObject.FindProperty("listaObjetos");
    }

    public override void OnInspectorGUI()
    {
        brush.diametro = EditorGUILayout.IntSlider("Diâmetro: ",brush.diametro, 0, 100);
        brush.densidade = EditorGUILayout.IntSlider("Densidade: ", brush.densidade, 1, 50);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(prefabs, true);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
        brush.parent = (GameObject)EditorGUILayout.ObjectField("Parent: ",brush.parent, typeof(GameObject), true);
    }

    private void OnSceneGUI()
    {
        Vector2 mousePos = Event.current.mousePosition;
        if (Event.current.shift)
        {
            if (instantiate)
            {
                instantiate = !instantiate;
                for (int i = 0; i < brush.densidade; i++)
                {
                    if (Physics.Raycast(DarRay(mousePos+new Vector2(Random.Range(-brush.diametro,brush.diametro), Random.Range(-brush.diametro, brush.diametro))), out hit))
                    {
                        if (hit.collider.tag == "Terrain")
                        {
                            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(brush.listaObjetos[Random.Range(0, brush.listaObjetos.Length)]);
                            go.transform.position = hit.point;
                            go.transform.eulerAngles = new Vector3(go.transform.eulerAngles.x, Random.Range(0f, 359f), go.transform.eulerAngles.z);
                            float scale = Random.Range(0.7f, 1.2f);
                            go.transform.localScale = new Vector3(scale, scale, scale);
                            if (brush.parent != null)
                            {
                                go.transform.parent = brush.parent.transform;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            instantiate = true;
        }       
    }

    Ray DarRay(Vector2 pos)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(pos);
        return ray;
    }
}
