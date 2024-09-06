using ParrelSync;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class Player : NetworkBehaviour
{
    public Deck deck;
    public List<Card> hand = new List<Card>();
    public NetworkVariable<FixedString128Bytes> _name = new NetworkVariable<FixedString128Bytes>("");
    public string displayName { get { return _name.Value.ToString(); } }

    public int letters;
    public bool dealer;
    public bool isSafe;
    private bool isDead = false;

    public static Player localPlayer;

    public delegate void GenericHandler();

    public static GenericHandler onSyncCards;
    public static GenericHandler onSyncDealer;
    public static GenericHandler onIsSafe;

    private void Start()
    {
        if (IsOwner)
        {
            Camera.main.transform.parent = transform;
            Camera.main.transform.position = new Vector3(0, Constants.PLAYER_CAMERA_OFFSET, 0);
            localPlayer = this;
        }
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
            _name.Value = Constants.DEBUG_NAMES[Random.Range(0, Constants.DEBUG_NAMES.Count)];
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

    public void SetDealer()
    {
        foreach(Player player in FindObjectsOfType<Player>())
        {
            player.dealer = IsOwner;
            SyncDealerRpc(player.dealer);
        }
    }

    public void Kill()
    {
        isDead = true;
        transform.Find("Marker").transform.forward = new Vector3(90, 0, 0);
    }

    [Rpc(SendTo.Owner)]
    private void SyncDeckRpc(SerializableCard[] deckCards, SerializableCard[] handCards)
    {
        deck.Wipe();
        hand.Clear();
        foreach (var card in deckCards)
        {
            deck.AddCard(new Card(card));
        }
        foreach (var card in handCards)
        {
            hand.Add(new Card(card));
        }
        onSyncCards?.Invoke();
    }
    [Rpc(SendTo.Owner)]
    private void SyncDealerRpc(bool isDealer)
    {
        dealer = isDealer;
        if(IsOwner)
            onSyncDealer?.Invoke();
    }
    [Rpc(SendTo.Owner)]
    public void SetSafeRpc()
    {
        onIsSafe?.Invoke();
    }
}
