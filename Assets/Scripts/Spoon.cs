using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class Spoon : NetworkBehaviour
{
    public bool visible;

    void Start()
    {
        SpoonManager.Register(this);
    }

    public void SetVisible(bool state)
    {
        visible = state;
        SetVisibleRpc(state);
    }

    [Rpc(SendTo.Everyone)]
    private void SetVisibleRpc(bool state)
    {
        gameObject.SetActive(state);
    }
}
