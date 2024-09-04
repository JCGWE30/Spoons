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
            Debug.Log("Starting client");
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            Debug.Log("Starting host");
            NetworkManager.Singleton.StartHost();
        }
    }
}
