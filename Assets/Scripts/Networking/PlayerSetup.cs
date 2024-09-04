using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    void Start()
    {
        if (IsServer)
        {
            CenterPoint.MoveToPoint(OwnerClientId, transform);
        }

        if (IsOwner)
        {
            Camera.main.transform.parent = transform;
            Camera.main.transform.localPosition = new Vector3(0, Constants.PLAYER_CAMERA_OFFSET, 0);
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            if (SpoonsPlayer.roundStarted)
                CenterPoint.RotateCamera(Camera.main);
            else
                Camera.main.transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
        }
    }
}
