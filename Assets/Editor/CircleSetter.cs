using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CircleAssembler))]
public class CircleSetter : Editor
{
    private float radius;
    private bool textState;

    private void OnEnable()
    {
        Debug.Log("Oni");
    }
    public override void OnInspectorGUI()
    {
        Debug.Log("Movig fast");
        base.OnInspectorGUI();

        Transform t = (target as CircleAssembler).transform;

        Debug.Log(t.gameObject.name + " " + t.childCount);

        if (t.childCount == 0)
            return;

        radius = EditorGUILayout.FloatField("Radius", radius);
        textState = EditorGUILayout.Toggle("Auto Face Text", textState);

        if (GUILayout.Button("Arrange Children in Circle"))
        {
            float spacing = 360 / t.childCount;
            float offX = t.position.x;
            float offZ = t.position.z;
            int objectIndex = 0;
            for (float i = 0; i <= 360; i += spacing)
            {
                float rad = Mathf.Deg2Rad * i;

                float x = Mathf.Cos(rad) * radius + offX;
                float z = Mathf.Sin(rad) * radius + offZ;
                if (objectIndex < t.childCount)
                    t.GetChild(objectIndex).transform.position = new Vector3(x, t.position.y, z);

                if (textState)
                {
                    TMP_Text name = t.GetComponentInChildren<TMP_Text>();
                    if (name == null)
                        return;
                    name.transform.LookAt(t);
                }

                objectIndex++;
            }
        }
    }
}
