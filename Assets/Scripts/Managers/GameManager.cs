using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public delegate void GameStartHandler();
    public delegate void GameEndHandler(Player winer);

    public delegate void RoundStartHandler(List<Player> players);
    public delegate void RoundEndHandler(Player loser);

    public static RoundStartHandler onRoundStart;
    public static RoundEndHandler onRoundEnd;

    public static GameStartHandler onGameStart;
    public static GameEndHandler onGameEnd;



    private static GameManager instance;
    private List<Player> playerList { get { return playersInRound.Values.ToList(); } }

    private Dictionary<ulong, Player> playersInRound = new Dictionary<ulong, Player>();
    public static bool roundStarted = false;


    private void Start()
    {
        instance = this;
    }

    public static void PlayerJoin(Player player)
    {
        instance.HandleJoin(player);
    }
    public static Player NextPlayer(Player player)
    {
        int index = instance.playerList.IndexOf(player);
        index++;
        if (index >= instance.playerList.Count)
            index = 0;
        Player p = instance.playerList[index];
        if (p.dealer)
            return null;
        return p;
    }
    public static Player GetPlayer(ulong id)
    {
        return instance.playersInRound[id];
    }
    public static void TakeSpoon(ulong id)
    {
        instance.HandleSpoonRpc(id);
    }

    public static void EliminatePlayer(ulong id)
    {
        GetPlayer(id).Kill();
        instance.playersInRound.Remove(id);
    }
    public void HandleJoin(Player player)
    {
        Debug.Log("Player Join");
        playersInRound[player.OwnerClientId] = player;

        if (playersInRound.Count == Constants.DEBUG_EXPECTED_PLAYER_SIZE)
            StartRound();
    }

    public static void StartRound()
    {
        if (instance.playerList.Count == 1)
        {
            UIManager.SendTopText(new[] { "Congrats on winning!" }, Constants.PLAYER_TOPTEXT_TIME, null);
            return;
        }
        PositionManager.ArrangePlayers(instance.playerList);
        Player dealer = DeckManager.SetupDecks(instance.playerList);
        string startText = string.Format(Constants.ROUND_START_TEXT, dealer.displayName);
        TopTextEndHandler endEvent = () =>
        {
            roundStarted = true;
            instance.StartRoundRpc(instance.playersInRound.Keys.ToArray());
        };
        UIManager.SendTopText(new[] { startText }, Constants.PLAYER_TOPTEXT_TIME, endEvent);
        
    }

    public void EndRound(Player loser,bool preMature)
    {
        EndRoundRpc(loser.OwnerClientId);
        List<string> texts = new List<string>();
        texts.Add(Constants.ROUND_END_TEXT);
        loser.letters++;

        if (preMature)
            texts.Add(string.Format(Constants.ROUND_SPOON_EARLY_TEXT,loser.displayName));
        else
            texts.Add(string.Format(Constants.ROUND_SPOON_LATE_TEXT, loser.displayName));

        texts.Add(string.Format(Constants.ROUND_LIVES_LEFT, loser.letters, Constants.SPOONS_TRIGGER_WORD.Substring(0, loser.letters)));

        if (loser.letters >= Constants.SPOON_TRIGGER_WORD_LENGTH)
        {
            texts.Add(Constants.ROUND_NO_LIVES);
            playersInRound.Remove(loser.OwnerClientId);
        }

        TopTextEndHandler endEvent = () =>
        {
            if (loser.letters >= Constants.SPOON_TRIGGER_WORD_LENGTH)
                EliminationManager.instance.KillPlayer(loser);
            else
                StartRound();
        };

        UIManager.SendTopText(texts.ToArray(), Constants.PLAYER_TOPTEXT_TIME, endEvent);
    }
    [Rpc(SendTo.Server)]
    private void HandleSpoonRpc(ulong id)
    {
        Player player = GetPlayer(id);
        if (player.isSafe)
            return;
        bool success = SpoonManager.CanTake(DeckManager.HasSafeCards(player));
        if (success)
        {
            player.isSafe = true;
            player.SetSafeRpc();
            if (!SpoonManager.TakeSpoon())
                return;
            foreach (Player loopP in playersInRound.Values)
            {
                if (!loopP.isSafe)
                {
                    EndRound(loopP, false);
                    return;
                }
            }
        }
        else
        {
            EndRound(player, true);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void StartRoundRpc(ulong[] pids)
    {
        roundStarted = true;
        Debug.Log("Starting round");
        List<Player> roundPlayers = new List<Player>();
        foreach(Player p in FindObjectsOfType<Player>())
        {
            if (pids.Contains(p.OwnerClientId))
                roundPlayers.Add(p);
        }
        onRoundStart?.Invoke(roundPlayers);
    }
    [Rpc(SendTo.Everyone)]
    private void EndRoundRpc(ulong id)
    {
        roundStarted = false;
        foreach (Player p in FindObjectsOfType<Player>())
        {
            if (p.OwnerClientId == id)
            {
                onRoundEnd?.Invoke(p);
                return;
            }
        }
    }
}
