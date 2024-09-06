using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionManager : MonoBehaviour
{
    private static PositionManager instance;

    private void Start()
    {
        instance = this;
    }

    public static void ArrangePlayers(List<Player> objects)
    {
        float spacing = 360 / objects.Count;
        float radius = Constants.CENTER_CIRCLE_RADIUS;
        float offX = instance.gameObject.transform.position.x;
        float offZ = instance.gameObject.transform.position.z;
        int objectIndex = 0;
        for (float i = 0; i <= 360; i += spacing)
        {
            float rad = Mathf.Deg2Rad * i;

            float x = Mathf.Cos(rad) * radius + offX;
            float z = Mathf.Sin(rad) * radius + offZ;
            if(objectIndex<objects.Count)
                objects[objectIndex].transform.position = new Vector3(x, Constants.CENTER_GROUND_POSITION, z);
            objectIndex++;
        }
    }
}
