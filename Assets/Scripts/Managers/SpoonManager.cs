using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class SpoonManager : MonoBehaviour
{
    private static SpoonManager instance;
    private static List<Spoon> spoons = new List<Spoon>();

    private bool canTake;

    void Start()
    {
        GameManager.onGameEnd += () => spoons.Clear();
        instance = this;
        GameManager.onRoundStart += SetupSpoons;
        GameManager.onRoundEnd += ResetSpoons;
    }

    public static void Register(Spoon spoon)
    {
        spoons.Add(spoon);
    }

    public static bool CanTake(bool shouldTake)
    {
        return shouldTake || instance.canTake;
    }

    public static bool TakeSpoon()
    {
        instance.canTake = true;
        if (GetSpoons(true).Count() == 1)
            return true;
        GetSpoons(true).First().SetVisible(false);
        return false;
    }


    private void SetupSpoons(List<Player> players)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;
        Debug.Log("Setting up spoons");
        int count = players.Count-1;
        for(int index = 0; index < GetSpoons(false).Count(); index++)
        {
            GetSpoons(false)[index].SetVisible(count > index);
        }
    }

    private void ResetSpoons(Player loser)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;
        canTake = false;
        foreach(Spoon spoon in GetSpoons(false))
        {
            spoon.SetVisible(true);
        }
    }

    private static List<Spoon> GetSpoons(bool activeOnly)
    {
        if (activeOnly)
            return spoons.Where(s => s.visible).ToList();

        return spoons;
    }
}
