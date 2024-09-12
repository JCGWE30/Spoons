using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObjects/SkinItem")]
public class SkinObject : ScriptableObject
{
    public Material texture;
    public string skinName;
    public string unlockReqs;
}

public interface ISkinCaller
{
    public Task<bool> canEquip(string playerId);
    public GameObject onDeath(GameObject deathObject);
}