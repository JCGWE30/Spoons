using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraPointer))]
public class CameraMover : Editor
{
    private void OnEnable()
    {
        Debug.Log("Ok here we go");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Transform transform = (target as CameraPointer).transform;

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
