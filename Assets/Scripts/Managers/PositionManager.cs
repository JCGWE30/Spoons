using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

public struct StareLocation : INetworkSerializable
{
    public float x;
    public float y;
    public float z;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref x);
        serializer.SerializeValue(ref y);
        serializer.SerializeValue(ref z);
    }
}

public class PositionManager : NetworkBehaviour
{
    private static PositionManager instance;

    [SerializeField] private GameObject centerPoint;

    public static Vector3 starePosition;
    
    private Vector3 lerpStart;
    private Vector3 lerpEnd;
    private float lerpDuration = 0.5f;
    private float elapsedLerp;
    private bool isLerping;

    private void Start()
    {
        centerPoint.SetActive(false);
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

    private Vector3 GetStarePosition(StareLocation loc)
    {
        Vector3 vec = new Vector3();
        vec.x = loc.x;
        vec.y = loc.y;
        vec.z = loc.z;
        return vec;
    }

    private StareLocation ConvertToStare(Vector3 vec)
    {
        return new StareLocation()
        {
            x = vec.x,
            y = vec.y,
            z = vec.z
        };
    }

    private void Update()
    {
        if (isLerping)
        {
            elapsedLerp += Time.deltaTime;

            float t = elapsedLerp / lerpDuration;

            Vector3 targetPosition = Vector3.Lerp(lerpStart, lerpEnd, t);

            starePosition = targetPosition;
            if (t >= 1f)
                isLerping = false;
        }
    }

    private void SetStare(Vector3 location,ulong player)
    {
        StartLerpRpc(ConvertToStare(location), player);
    }

    public static void LookAtSpoons()
    {
        instance.SetStare(instance.centerPoint.transform.position,999);
    }
    public static void LookAtMiddle()
    {
        Vector3 vec = instance.centerPoint.transform.position;
        vec.y += 2f;
        instance.SetStare(vec,999);
    }
    public static void LookAtPlayer(Player player)
    {
        instance.SetStare(player.gameObject.transform.position, player.OwnerClientId);
    }

    [Rpc(SendTo.Everyone)]
    private void StartLerpRpc(StareLocation location, ulong player)
    {
        if (Player.localPlayer.OwnerClientId == player)
            return;
        lerpStart = starePosition;
        lerpEnd = GetStarePosition(location);
        elapsedLerp = 0f;
        isLerping = true;
    }
}
