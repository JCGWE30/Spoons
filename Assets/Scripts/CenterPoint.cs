using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class CenterPoint : MonoBehaviour
{
    private static CenterPoint instance;

    private List<Vector3> points = new List<Vector3>();
    private void Start()
    {
        instance = this;
        Destroy(gameObject.GetComponent<MeshRenderer>());
        GeneratePoints(5);
    }

    public static void GeneratePoints(int count)
    {
        float spacing = 360 / count;
        int radius = Constants.CENTER_CIRCLE_RADIUS;
        float offX = instance.transform.position.x;
        float offY = instance.transform.position.y;
        for (float i = 0; i <= 360; i+=spacing)
        {
            float rad = Mathf.Deg2Rad * i;

            float x = Mathf.Cos(rad) * radius + offX;
            float y = Mathf.Sin(rad) * radius + offY;
            instance.points.Add(new Vector3(x, Constants.CENTER_GROUND_POSITION, y));
        }
    }

    public static void MoveToPoint(ulong index, Transform transform)
    {
        transform.position = instance.points[(int)index];
    }

    public static void RotateCamera(Camera camera)
    {
        Transform transform = camera.transform;

        Vector3 target = instance.transform.position;

        Vector3 direction = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);

        Quaternion targetRot = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);
    }
}
