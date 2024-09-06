using ParrelSync;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpoonsNetworker : MonoBehaviour
{
    private NetworkManager manager = NetworkManager.Singleton;
    private void Start()
    {
        manager = NetworkManager.Singleton;
        if (ClonesManager.IsClone())
        {
            Debug.Log("Starting client");
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            Debug.Log("Starting host");
            NetworkManager.Singleton.StartHost();
            SetupServerEvents();
        }
    }


    private void SetupServerEvents()
    {
        manager.OnClientConnectedCallback += HandleConnection;
        HandleConnection(manager.LocalClientId);
    }

    private void HandleConnection(ulong id)
    {
        if (manager.ConnectedClients.Count > Constants.DEBUG_EXPECTED_PLAYER_SIZE)
        {
            NetworkManager.Singleton.DisconnectClient(id, Constants.NETWORK_DISCONNECT_MESSAGE);
        }
        else
        {
            Player p = manager.ConnectedClients[id].PlayerObject.GetComponent<Player>();
            GameManager.PlayerJoin(p);
        }
    }

    public static BaseRpcTarget TargetUid(ulong id,NetworkBehaviour behv)
    {
        return behv.RpcTarget.Single(id, RpcTargetUse.Temp);
    }
}
