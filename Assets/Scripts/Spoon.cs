using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Spoon : MonoBehaviour
{
    private static List<Spoon> activeSpoons = new List<Spoon>();
    private bool fullinit = false;

    private void Start()
    {
        if (!fullinit)
        {
            fullinit = true;
            SpoonsPlayer.onRoundStart += SetupSpoons;
            if (SpoonsPlayer.roundStarted)
                SetupSpoons(SpoonsPlayer.playerCount);
        }
    }

    private static void SetupSpoons(int count)
    {
        Debug.Log("Setting up spoons with " + count + " players");
        activeSpoons.Clear();
        int index = 1;
        foreach (Spoon spoon in FindObjectsOfType<Spoon>()) 
        {
            if (index < count)
            {
                activeSpoons.Add(spoon);
                spoon.gameObject.SetActive(true);
            }
            else
            {
                spoon.gameObject.SetActive(false);
            }
            index++;
        }
    }

    public static bool TakeSpoon()
    {
        activeSpoons[0].gameObject.SetActive(false);
        activeSpoons.RemoveAt(0);
        return activeSpoons.Count == 0;
    }
    public static void ReactivateAllSpoons()
    {
        foreach (var item in FindObjectsOfType<Spoon>(true))
        {
            item.gameObject.SetActive(true);
        }
    }
}
