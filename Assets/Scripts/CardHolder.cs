using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObjects/CardHolder")]
public class CardHolder : ScriptableObject
{
    public List<Sprite> cards;
}
