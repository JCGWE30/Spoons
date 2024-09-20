using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CircleAssembler))]
public class CircleSetter : Editor
{
    private float inputNumber;

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

        inputNumber = EditorGUILayout.FloatField("Radius", inputNumber);

        if (GUILayout.Button("Arrange Children in Circle"))
        {
            float spacing = 360 / t.childCount;
            float offX = t.position.x;
            float offZ = t.position.z;
            int objectIndex = 0;
            for (float i = 0; i <= 360; i += spacing)
            {
                float rad = Mathf.Deg2Rad * i;

                float x = Mathf.Cos(rad) * inputNumber + offX;
                float z = Mathf.Sin(rad) * inputNumber + offZ;
                if (objectIndex < t.childCount)
                    t.GetChild(objectIndex).transform.position = new Vector3(x, t.position.y, z);
                objectIndex++;
            }
        }
    }
}
