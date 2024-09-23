using System.Collections;
using System.Collections.Generic;
using LobbyPlayer = Unity.Services.Lobbies.Models.Player;
using UnityEngine;
using TMPro;

public class PlayerIDHolder : MonoBehaviour
{
    private void Start()
    {
        UpdateVisuals();
    }

    private LobbyPlayer player;
    public string playerId { get { return player?.Id ?? ""; } }

    public static bool TryGetPlayer(string id, out PlayerIDHolder outPlayer)
    {
        foreach(PlayerIDHolder player in FindObjectsOfType<PlayerIDHolder>())
        {
            if (player?.playerId == id)
            {
                outPlayer = player;
                return true;
            }
        }
        outPlayer = null;
        return false;
    }

    public void Set(LobbyPlayer player)
    {
        this.player = player;
        UpdateVisuals();
    }

    public void Wipe()
    {
        player = null;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        gameObject.SetActive(player != null);
    }
}
