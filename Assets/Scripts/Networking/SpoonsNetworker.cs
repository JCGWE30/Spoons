using System.Collections;
using System.Collections.Generic;
using Menu2.Play;
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
    private void Start()
    {
        manager = NetworkManager.Singleton;
        
        if (RelayManager.isHost)
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(RelayManager.serverData);
            NetworkManager.Singleton.StartHost();
            SetupServerEvents();
        }
        else
        {
            var serverData = new RelayServerData(RelayManager.joinAllocation, "wss");
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
            
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
        if (manager.ConnectedClients.Count > RelayManager.playerCount)
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
