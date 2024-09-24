using System.Collections;
using System.Collections.Generic;
using LobbyPlayer = Unity.Services.Lobbies.Models.Player;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class PlayerIDHolder : MonoBehaviour
{
    private LobbyPlayer player;
    private Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();
        GetComponent<Outline>().enabled = false; ;
        UpdateVisuals();
    }

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

    public static PlayerIDHolder GetPlayer(string id)
    {
        if(TryGetPlayer(id, out PlayerIDHolder outPlayer))
            return outPlayer;
        return null;
    }

    public void Set(LobbyPlayer player)
    {
        this.player = player;
        UpdateVisuals();
    }

    public void SetHighlightState(bool state)
    {
        outline.enabled = state;
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
