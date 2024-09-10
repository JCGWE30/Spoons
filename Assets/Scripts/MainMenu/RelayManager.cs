using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public static RelayServerData relayData;
    public static string relayCode;
    public static bool isHost = false;
    public static string localName;
    public static int lobbySize;

    private static RelayManager instance;
    private void Start()
    {
        instance = this;
    }

    public static async Task<string> CreateGame(int players, string name)
    {
        try
        {
            lobbySize = players;
            localName = name;
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(players);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            relayData = new RelayServerData(allocation, "wss");
            isHost = true;
            return joinCode;
        }catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
        return "";
    }

    public static async Task JoinRelay(string joinCode,string name)
    {
        try
        {
            relayCode = joinCode;
            localName = name;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
