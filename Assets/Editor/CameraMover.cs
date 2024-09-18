using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Transform))]
public class CameraMover : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Transform transform = (Transform)target;

        if (transform.gameObject.name == "CameraMarker")
        {
            if(GUILayout.Button("Move Camera"))
            {
                Camera.main.transform.position = transform.position;
            }
        }
    }
}
