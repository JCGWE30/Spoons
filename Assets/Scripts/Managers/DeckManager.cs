using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DeckManager : NetworkBehaviour
{
    private static DeckManager instance;

    private void Start()
    {
        instance = this;
    }

    public static Player SetupDecks(List<Player> players)
    {
        players.ForEach((p) => p.deck.Wipe());
        Player dealer = players[Random.Range(0, players.Count)];
        dealer.SetDealer(players);
        dealer.deck.ShuffleDeck();
        foreach (var player in players)
        {
            player.isSafe = false;
            for(int i = 0; i < 4; i++)
            {
                player.hand[i] = dealer.deck.TakeCard();
            }
            player.SyncDecks();
        }
        return dealer;
    }
    public static void SwapCard(int cardSpot)
    {
        ulong localId = NetworkManager.Singleton.LocalClientId;
        instance.SwapCardRpc(localId, cardSpot);
    }
    public static void DiscardCard()
    {
        ulong localId = NetworkManager.Singleton.LocalClientId;
        instance.DiscardCardRpc(localId);
    }
    public static bool HasSafeCards(Player p)
    {
        Debug.Log("Scanning " + p.displayName + " for safe cards");
        int handValue = p.hand[0].value;
        foreach (var item in p.hand)
        {
            Debug.Log("Scanning " + item.GetName() + " for value " + handValue);
            if (item.value != handValue)
                return false;
        }
        Debug.Log("Cards are safe, taking spoon");
        return true;
    }
    [Rpc(SendTo.Server)]
    private void SwapCardRpc(ulong id,int cardSpot)
    {
        Player player = GameManager.GetPlayer(id);
        Player nextPlayer = GameManager.NextPlayer(player);
        Card card = player.deck.TakeCard();
        Card sendingCard = player.hand[cardSpot];

        player.hand[cardSpot] = card;
        player.SyncDecks();

        if (nextPlayer == null)
            return;
        nextPlayer.deck.AddCard(sendingCard);
        nextPlayer.SyncDecks();
    }

    [Rpc(SendTo.Server)]
    private void DiscardCardRpc(ulong id)
    {
        Player player = GameManager.GetPlayer(id);
        Player nextPlayer = GameManager.NextPlayer(player);

        Card card = player.deck.TakeCard();
        player.SyncDecks();

        if (nextPlayer == null)
            return;
        nextPlayer.deck.AddCard(card);
        nextPlayer.SyncDecks();
    }
}
