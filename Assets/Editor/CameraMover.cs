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
            if (GUILayout.Button("Move Camera"))
            {
                Camera.main.transform.position = transform.position;
                Camera.main.transform.eulerAngles = transform.eulerAngles;
            }
            if (GUILayout.Button("Move To Camera"))
            {
                transform.position = Camera.main.transform.position;
                transform.eulerAngles = Camera.main.transform.eulerAngles;
            }
        }
    }
}
