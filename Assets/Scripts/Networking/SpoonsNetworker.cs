using ParrelSync;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpoonsNetworker : MonoBehaviour
{
    private void Start()
    {
        if (ClonesManager.IsClone())
        {
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            NetworkManager.Singleton.StartHost();
        }
    }
}
