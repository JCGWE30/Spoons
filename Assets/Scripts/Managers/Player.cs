using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class Player : NetworkBehaviour
{
    [SerializeField] private TMP_Text dealerText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject marker;
    [SerializeField] private GameObject deadMarker;

    public Deck deck;
    public Card[] hand = new Card[4];
    public NetworkVariable<FixedString128Bytes> _name = new NetworkVariable<FixedString128Bytes>("");
    public NetworkVariable<FixedString128Bytes> _skin = new NetworkVariable<FixedString128Bytes>("");
    public string displayName { get { return _name.Value.ToString(); } }
    public string activeSkin { get { return _skin.Value.ToString(); } }

    private NetworkVariable<int> _letters = new NetworkVariable<int>(0);
    public int letters { get { return _letters.Value; } set { _letters.Value = value; } }
    public bool dealer;
    public bool isSafe;
    public bool isDead = false;

    public static Player localPlayer;

    public delegate void GenericHandler();

    public static GenericHandler onSyncCards;
    public static GenericHandler onSyncDealer;
    public static GenericHandler onIsSafe;

    private void Start()
    {
        deadMarker.SetActive(false);
        GameManager.onGameEnd += () =>
        {
            onSyncCards = null;
            onSyncDealer = null;
            onIsSafe = null;
        };
        if (IsOwner)
        {
            Camera.main.transform.parent = transform;
            Camera.main.transform.position = new Vector3(0, Constants.PLAYER_CAMERA_OFFSET, 0);
            localPlayer = this;
        }

        _skin.OnValueChanged += (x, y) =>
        {
            UpdateSkin();
        };
        if (activeSkin != "")
            UpdateSkin();
    }

    private void Update()
    {
        if (IsOwner)
        {
            Camera.main.transform.LookAt(PositionManager.starePosition);
        }
        else
        {
            dealerText.transform.LookAt(localPlayer.transform);

            nameText.transform.LookAt(localPlayer.transform);

            livesText.transform.LookAt(localPlayer.transform);
            if (isDead)
            {
                marker.SetActive(false);
                deadMarker.SetActive(true);
                dealerText.text = "";
                nameText.text = "";
                livesText.text = "";
                return;
            }
            dealerText.text = dealer ? Constants.SPOONS_DEALER_NAME : "";
            nameText.text = displayName;
            livesText.text = Constants.SPOONS_TRIGGER_WORD.Substring(0, letters);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            string name = AuthenticationService.Instance.PlayerName ?? "SpoonsPlayer " + OwnerClientId;
            
            Debug.Log("My name is " + name);
            SetNameRpc(name);
            SetSkinRpc(SkinsHandler.ActiveSkin);
        }
    }
    public void SyncDecks()
    {
        List<SerializableCard> deckCards = new List<SerializableCard>();
        List<SerializableCard> handCards = new List<SerializableCard>();
        foreach (Card c in deck.GetCards())
        {
            deckCards.Add(c.serializableCard);
        }
        foreach (Card c in hand)
        {
            handCards.Add(c.serializableCard);
        }
        SyncDeckRpc(deckCards.ToArray(), handCards.ToArray());
    }

    public void SetDealer(List<Player> players)
    {
        foreach(Player p in players)
        {
            p.SyncDealerRpc(OwnerClientId);
        }
    }

    public void Kill()
    {
        isDead = true;
        transform.Find("Marker").transform.forward = new Vector3(90, 0, 0);
        KillRpc();
    }

    private void UpdateSkin()
    {
        marker.GetComponent<MeshRenderer>().material = SkinsHandler.GetMaterial(activeSkin);
    }

    [Rpc(SendTo.Everyone)]
    private void KillRpc()
    {
        if (IsOwner)
            UIManager.MarkDead();
        isDead = true;
    }
    [Rpc(SendTo.Owner)]
    private void SyncDeckRpc(SerializableCard[] deckCards, SerializableCard[] handCards)
    {
        deck.Wipe();
        foreach (var card in deckCards)
        {
            deck.AddCard(new Card(card));
        }
        for(int i=0;i<4;i++)
        {
            hand[i] = new Card(handCards[i]);
        }
        onSyncCards?.Invoke();
    }
    [Rpc(SendTo.Everyone)]
    private void SyncDealerRpc(ulong dealerId)
    {
        dealer = dealerId == OwnerClientId;
        if(IsOwner)
            onSyncDealer?.Invoke();
    }
    [Rpc(SendTo.Owner)]
    public void SetSafeRpc()
    {
        if(DeckManager.HasSafeCards(this))
            LeaderboardManager.UpdateScore();
        onIsSafe?.Invoke();
    }
    [Rpc(SendTo.Server)]
    public void SetNameRpc(string name)
    {
        if (displayName == "")
            _name.Value = name;
    }
    [Rpc(SendTo.Server)]
    public void SetSkinRpc(string skin)
    {
        if (skin == "none")
            return;
        _skin.Value = skin;
    }
}
