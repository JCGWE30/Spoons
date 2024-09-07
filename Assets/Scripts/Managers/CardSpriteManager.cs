using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpriteManager : MonoBehaviour
{
    [SerializeField] private CardHolder holder;

    public static CardSpriteManager instance;

    void Start()
    {
        instance = this;
    }

    public Sprite GetCard(string name)
    {
        foreach(Sprite sprite in holder.cards)
        {
            if (sprite.name == name)
                return sprite;
        }
        return null;
    }
}
