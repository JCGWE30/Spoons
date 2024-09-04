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
            CenterPoint.RotateCamera(Camera.main);
        }
    }
}
