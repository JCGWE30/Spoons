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
        GeneratePoints(Constants.DEBUG_EXPECTED_PLAYER_SIZE);
    }

    public static void GeneratePoints(int count)
    {
        float spacing = 360 / count;
        float radius = Constants.CENTER_CIRCLE_RADIUS;
        float offX = instance.gameObject.transform.position.x;
        float offY = instance.gameObject.transform.position.y;
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

    public static void MoveCameraToDeathPos(Camera cam)
    {
        Vector3 pos = instance.gameObject.transform.position;
        pos.y += 10f;
        cam.transform.position = pos;
        cam.transform.eulerAngles = new Vector3(90, 0, 0);
    }

    public static void RotateCamera(Camera camera)
    {
        camera.transform.LookAt(instance.transform.position);
    }
}
