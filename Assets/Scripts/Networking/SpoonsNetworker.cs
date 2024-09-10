using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpoonsNetworker : MonoBehaviour
{
    private NetworkManager manager = NetworkManager.Singleton;
    private async void Start()
    {
        manager = NetworkManager.Singleton;

        if (RelayManager.isHost)
        {
            manager.GetComponent<UnityTransport>().SetRelayServerData(RelayManager.relayData);
            Debug.Log("Starting host");
            NetworkManager.Singleton.StartHost();
            SetupServerEvents();
        }
        else
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(RelayManager.relayCode);
            manager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "wss"));

            Debug.Log("Starting client");
            NetworkManager.Singleton.StartClient();
            manager.OnClientDisconnectCallback += HandleServerStop;
        }
    }


    private void SetupServerEvents()
    {
        manager.OnClientConnectedCallback += HandleConnection;
        manager.OnClientDisconnectCallback += HandleDisconnection;
    }

    private void HandleConnection(ulong id)
    {
        if (manager.ConnectedClients.Count > RelayManager.lobbySize)
        {
            NetworkManager.Singleton.DisconnectClient(id, Constants.NETWORK_DISCONNECT_MESSAGE);
        }
        else
        {
            Player p = manager.ConnectedClients[id].PlayerObject.GetComponent<Player>();
            GameManager.PlayerJoin(p);
        }
    }

    private void HandleDisconnection(ulong id)
    {
        Debug.Log("Player disconnected " + id);
        GameManager.DisconnectPlayer(id);
    }

    private void HandleServerStop(ulong id)
    {
        Debug.Log("Client cut");
        SceneManager.LoadScene("MainMenu");
    }

    public static BaseRpcTarget TargetUid(ulong id,NetworkBehaviour behv)
    {
        return behv.RpcTarget.Single(id, RpcTargetUse.Temp);
    }
}
