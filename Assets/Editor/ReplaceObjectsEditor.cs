using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ReplaceObjectsEditor : EditorWindow
{
    private GameObject replacement = null;
    private bool keepTransforms = true;

    [MenuItem("Window/Replace Objects")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ReplaceObjectsEditor));
    }

    void OnGUI()
    {

        replacement = (GameObject)EditorGUILayout.ObjectField("Replacement Object", replacement, typeof(GameObject), true);
        keepTransforms = GUILayout.Toggle(keepTransforms, "Keep Object Transforms");

        if(GUILayout.Button("Replace Selected") && replacement != null)
        {
            foreach(Transform transform in Selection.transforms)
            {
                GameObject clone = (GameObject)PrefabUtility.InstantiatePrefab(replacement);
                clone.transform.SetParent(transform.parent);

                if(keepTransforms)
                {
                    clone.transform.localPosition = transform.localPosition;
                    clone.transform.localRotation = transform.localRotation;
                    clone.transform.localScale = transform.localScale;
                }

                Undo.RegisterCreatedObjectUndo(clone, "Created Replacement");
                Undo.DestroyObjectImmediate(transform.gameObject);
            }
        }
    }
}
