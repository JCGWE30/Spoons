using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;

namespace Menu2.Play
{
    public class RelayManager
    {
        public static int playerCount;
        public static bool isHost;
        public static RelayServerData serverData;
        public static JoinAllocation joinAllocation;
        
        public static async Task<string> CreateGame(int players)
        {
            try
            {
                isHost = true;
                playerCount = players;
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(players);

                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                serverData = new RelayServerData(allocation, "wss");

                return joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }

            return "";
        }

        public static async Task<bool> JoinGame(string code)
        {
            try
            {
                joinAllocation = await RelayService.Instance.JoinAllocationAsync(code);

                return true;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return false;
            }
        }
    }
}