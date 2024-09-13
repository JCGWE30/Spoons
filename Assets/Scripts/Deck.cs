using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct SerializableCard : INetworkSerializable
{
    public int suite;
    public int value;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref suite);
        serializer.SerializeValue(ref value);
    }
}

public class Card
{
    public SerializableCard serializableCard { get { return new SerializableCard { suite = suite, value = value }; } }
    public int suite;
    public int value;

    public Card(int suite, int value)
    {
        this.suite = suite;
        this.value = value;
    }

    public Card(SerializableCard card)
    {
        suite = card.suite;
        value = card.value;
    }

    public string GetName()
    {
        string name = Constants.CARD_VALUES[value] + " of " + Constants.CARD_SUITES[suite];
        return name;
    }

    public Sprite GetSprite()
    {
        string search = Constants.CARD_VALUES[value].ToLower() + "_of_" + Constants.CARD_SUITES[suite].ToLower();
        return CardSpriteManager.instance.GetCard(search);
    }
}

public class Deck : MonoBehaviour
{
    private List<Card> cards = new List<Card>();
    public int cardCount { get { return cards.Count; } }
    void Start()
    {

    }

    public void DebugDeck()
    {
        cards.Clear();
        foreach (var suite in Constants.CARD_SUITES)
        {
            foreach (var value in Constants.CARD_VALUES)
            {
                cards.Add(new Card(0, 0));
            }
        }
    }

    public void FullShuffle() {
        cards.Clear();
        cards.Add(new Card(0, 0));
        cards.Add(new Card(0, 1));
        cards.Add(new Card(0, 2));
        cards.Add(new Card(0, 3));
        cards.Add(new Card(0, 4));
        cards.Add(new Card(0, 5));
        cards.Add(new Card(0, 6));
        cards.Add(new Card(0, 7));
        cards.Add(new Card(0, 8));
        cards.Add(new Card(0, 9));
        return;
        foreach (var suite in Constants.CARD_SUITES)
        {
            foreach (var value in Constants.CARD_VALUES)
            {
                int suiteIndex = Constants.CARD_SUITES.IndexOf(suite);
                int valueIndex = Constants.CARD_VALUES.IndexOf(value);

                cards.Add(new Card(suiteIndex, valueIndex));
            }
        }
        cards = cards.OrderBy(x => Random.Range(0, int.MaxValue)).ToList();
    }

    public void Shuffle()
    {
        cards = cards.OrderBy(x => Random.Range(0, int.MaxValue)).ToList();
    }

    public Card TakeCard()
    {
        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public Card NextCard()
    {
        Card card = cards[0];
        return card;
    }

    public Card GetCard(int index)
    {
        return cards[index];
    }
    public List<Card> GetCards()
    {
        return cards;
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
    }
    public void SetCard(int index,Card card)
    {
        cards[index] = card;
    }
    public void Wipe()
    {
        cards.Clear();
    }
}
