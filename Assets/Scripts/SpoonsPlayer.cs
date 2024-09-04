using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SpoonsPlayer : NetworkBehaviour
{

    [SerializeField] private TMP_Text nameTag;
    [SerializeField] private TMP_Text dealerTag;
    [SerializeField] private TMP_Text livesCount;

    [SerializeField] private Deck deck;
    [SerializeField] private Deck hand;
    [SerializeField] private GameObject marker;

    public static SpoonsPlayer localInstance;
    public static bool roundStarted { get; private set; }
    public static int playerCount { get { return players.Count; } }

    private static List<SpoonsPlayer> players = new List<SpoonsPlayer>();

    public delegate void RoundStartHandler(int playerCount);

    public static RoundStartHandler onRoundStart;

    public delegate void RoundEndHandler();

    public static RoundEndHandler onRoundEnd;

    private bool hasSpoon;

    private int letters;


    void Start()
    {
        Debug.Log("SpoonsPlayer");
        if (IsOwner)
            localInstance = this;
        if (!IsServer)
            return;
        players.Add(this);
        if (players.Count >= Constants.DEBUG_EXPECTED_PLAYER_SIZE)
        {
            StartRound();
        }
    }

    private void Update()
    {
        SetTextPositions();
    }

    private void SetTextPositions()
    {
        if (IsOwner)
        {
            foreach (var item in FindObjectsOfType<SpoonsPlayer>())
            {
                Vector3 directionToTarget = gameObject.transform.position - item.nameTag.transform.position;

                directionToTarget = Quaternion.Euler(0, 180f, 0) * directionToTarget;

                directionToTarget.y = 0;

                item.nameTag.transform.forward = directionToTarget;
                item.livesCount.transform.forward = directionToTarget;
                item.dealerTag.transform.forward = directionToTarget;
            }
        }
    }

    public static void StartRound()
    {
        if (playerCount == 1)
        {
            localInstance.TopTextRpc(string.Format(Constants.ROUND_WINNER_TEXT, players[0].OwnerClientId));
            NetworkManager.Singleton.DisconnectClient(0);
            return;
        }
        SpoonsPlayer dealer = players[Random.Range(0, players.Count)];
        dealer.deck.ShuffleDeck();
        Debug.Log(dealer.OwnerClientId + "Is the dealer");
        localInstance.TopTextRpc(string.Format(Constants.ROUND_START_TEXT,dealer.OwnerClientId));
        dealer.SetDealerRpc(dealer.OwnerClientId);

        foreach (SpoonsPlayer player in players)
        {
            for (int i = 0; i < 4; i++)
            {
                Card card = dealer.deck.TakeCard();
                player.AddCardToHand(card);
            }
        }

        foreach (Card c in dealer.deck.GetCards())
        {
            dealer.AddCardToDeckRpc(c.serializableCard, dealer.RpcTarget.Single(dealer.OwnerClientId, RpcTargetUse.Temp));
        }
        UIManager.instance.OnTopTextEnd += () =>
        {
            dealer.StartRoundRpc(players.Count);
            UIManager.instance.OnTopTextEnd = null;
        };
    }

    public static void EndRound()
    {
        foreach (SpoonsPlayer player in players)
        {
            player.deck.Wipe();
        }
        roundStarted = false;
        localInstance.TopTextRpc(Constants.ROUND_END_TEXT);
        localInstance.EndRoundRpc();
        UIManager.instance.OnTopTextEnd += StartNewRound;
    }

    public static void KillNoLive()
    {
        foreach(SpoonsPlayer player in players)
        {
            if (player.letters == Constants.SPOON_TRIGGER_WORD_LENGTH)
            {
                PlayerKiller.instance.KillPlayer(player);
                return;
            }
        }
    }

    public void RemovePlayer(SpoonsPlayer player)
    {
        players.Remove(player);
    }

    public static void StartNewRound()
    {
        UIManager.instance.OnTopTextEnd = null;
        StartRound();
    }

    public Deck GetDeck()
    {
        return deck;
    }

    public Deck GetHand()
    {
        return hand;
    }

    public SpoonsPlayer GetById(ulong id)
    {
        foreach (var item in FindObjectsOfType<SpoonsPlayer>())
        {
            if (item.OwnerClientId == id)
                return item;
        }
        return null;
    }
    public SpoonsPlayer GetNext()
    {
        int id = players.IndexOf(this);
        int newId = id+=1;
        if (id >= players.Count)
        {
            return players[0];
        }
        return players[newId];
    }
    private void AddCardToHand(Card card)
    {
        hand.AddCard(card);
        AddCardToHandRpc(card.serializableCard, RpcTarget.Single(OwnerClientId, RpcTargetUse.Temp));
    }

    public void SwapCard(int cardToSwitch, Card card)
    {
        hand.SetCard(cardToSwitch, card);
        if(!IsHost)
            SwapCardRpc(cardToSwitch);
    }

    public void SendCard(Card card)
    {
        if (IsHost)
        {
            SpoonsPlayer player = GetNext();
            player.deck.AddCard(card);
            player.RecieveCardRpc(card.serializableCard, RpcTarget.Single(player.OwnerClientId, RpcTargetUse.Temp));
        }
        else
        {
            SendCardRpc();
        }
    }

    public void GiveLetter(SpoonsPlayer player, bool preMature)
    {
        if (preMature)
            localInstance.TopTextRpc(Constants.ROUND_SPOON_EARLY_TEXT);
        else
            localInstance.TopTextRpc(Constants.ROUND_SPOON_LATE_TEXT);

        player.letters += 1;

        string livesLeftText = string.Format(Constants.ROUND_LIVES_LEFT, player.letters, Constants.SPOONS_TRIGGER_WORD.Substring(0, player.letters));

        localInstance.TopTextRpc(livesLeftText);
        if (player.letters == Constants.SPOON_TRIGGER_WORD_LENGTH)
        {
            UIManager.instance.OnTopTextEnd = KillNoLive;
            localInstance.TopTextRpc(Constants.ROUND_NO_LIVES);
        }
    }
    [Rpc(SendTo.Everyone)]
    public void DeactivateRpc()
    {
        if (IsOwner)
        {
            Camera.main.transform.parent = null;
            CenterPoint.MoveCameraToDeathPos(Camera.main);
        }
        foreach(Transform transform in transform)
        {
            transform.gameObject.SetActive(false);
        }
    }
    [Rpc(SendTo.Everyone)]
    private void TopTextRpc(FixedString64Bytes text)
    {
        UIManager.instance.topTexts.Add(text.ToString());
    }
    [Rpc(SendTo.SpecifiedInParams)]
    private void RecieveCardRpc(SerializableCard card, RpcParams rpcParams)
    {
        Debug.Log("Recieved new message");
        deck.AddCard(new Card(card));
    }
    [Rpc(SendTo.Server)]
    private void SendCardRpc()
    {
        SpoonsPlayer player = GetNext();
        Card card = deck.TakeCard();
        player.deck.AddCard(card);
        RecieveCardRpc(card.serializableCard, RpcTarget.Single(player.OwnerClientId, RpcTargetUse.Temp));
    }
    [Rpc(SendTo.Server)]
    private void SwapCardRpc(int cardIndex)
    {
        hand.SetCard(cardIndex, deck.TakeCard());
        Debug.Log(OwnerClientId + " now has a hand of");
        foreach (var item in hand.GetCards())
        {
            Debug.Log(item.GetName());
        }
    }
    [Rpc(SendTo.Everyone)]
    private void StartRoundRpc(int count)
    {
        foreach (SpoonsPlayer player in FindObjectsOfType<SpoonsPlayer>())
        {
            if (player.letters == 0)
                player.livesCount.text = "None";
            else
                player.livesCount.text = Constants.SPOONS_TRIGGER_WORD.Substring(0, player.letters);
        }
        roundStarted = true;
        onRoundStart?.Invoke(count);
    }
    [Rpc(SendTo.Everyone)]
    private void EndRoundRpc()
    {
        foreach(SpoonsPlayer player in FindObjectsOfType<SpoonsPlayer>())
        {
            player.dealerTag.text = "";
        }
        localInstance.deck.Wipe();
        Spoon.ReactivateAllSpoons();
        roundStarted = false;
        onRoundEnd?.Invoke();
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void AddCardToHandRpc(SerializableCard card,RpcParams rpcParams)
    {
        if(!IsHost)
            hand.AddCard(new Card(card));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void AddCardToDeckRpc(SerializableCard card, RpcParams rpcParams)
    {
        if(!IsHost)
            deck.AddCard(new Card(card));
    }

    [Rpc(SendTo.Everyone)]
    private void SetDealerRpc(ulong dealer)
    {
        SpoonsPlayer playerObject = GetById(dealer);
        Debug.Log(playerObject);
        playerObject.dealerTag.text = Constants.SPOONS_DEALER_NAME;
    }

    [Rpc(SendTo.Server)]
    public void TakeSpoonRpc()
    {
        if (!roundStarted)
            return;
        bool match = true;
        Card lastcard = null;
        foreach(Card card in hand.GetCards())
        {
            if (lastcard == null)
            {
                lastcard = card;
                continue;
            }
            if (lastcard.value != card.value)
            {
                match = false;
                break;
            }
            lastcard = card;
        }
        if (match)
        {
            hasSpoon = true;
            if (!Spoon.TakeSpoon())
                return;
            EndRound();
            foreach (SpoonsPlayer player in players)
            {
                if (!player.hasSpoon)
                {
                    GiveLetter(player,false);
                    break;
                }
            }
        }
        else
        {
            EndRound();
            GiveLetter(this,true);
        }
        Debug.Log(OwnerClientId + " Attempted to pull a spoon, Sucessful? " + match);
    }

    [Rpc(SendTo.Everyone)]
    public void DeductSpoonRpc()
    {
        if (IsHost)
            return;
        Spoon.TakeSpoon();
    }
}
