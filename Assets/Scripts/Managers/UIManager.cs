using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public delegate void TopTextEndHandler();


public struct TopText
{
    public List<string> texts;
    public float showDuration;
    public TopTextEndHandler endEvent;
}
public class UIManager : NetworkBehaviour
{
    private static UIManager instance;

    private List<TopText> activeTopTexts = new List<TopText>();
    private float cycleTime = -50f;
    public static bool viewingCard { get { return UIHandler.instance.drawingCard; } }

    private void Start()
    {
        instance = this;
        Player.onSyncCards += Sync;
    }

    public static void SendTopText(string[] text, float active, TopTextEndHandler endEvent)
    {
        TopText topText = new TopText { texts = text.ToList(), showDuration = active, endEvent = endEvent };

        if (instance.activeTopTexts.Count == 0)
        {
            instance.cycleTime = Time.time;
            instance.SendTopTextRpc(topText.texts[0]);
        }

        instance.activeTopTexts.Add(topText);
    }

    public static void SendTopText(string text, float active, TopTextEndHandler endEvent)
    {
        TopText topText = new TopText { texts = new []{text}.ToList(), showDuration = active, endEvent = endEvent };

        if (instance.activeTopTexts.Count == 0)
        {
            instance.cycleTime = Time.time;
            instance.SendTopTextRpc(topText.texts[0]);
        }

        instance.activeTopTexts.Add(topText);
    }

    public static void ViewSpoons()
    {
        UIHandler.instance.ViewSpoons();
    }

    public static void TakeCard()
    {
        if (Player.localPlayer.deck.cardCount == 0)
            return;
        if (UIHandler.instance.drawingCard)
            return;
        UIHandler.instance.drawingCard = true;
        Card card = Player.localPlayer.deck.NextCard();
        UIHandler.instance.drawSprite = card.GetSprite();
    }

    public static void SwapCard(int card)
    {
        if (!UIHandler.instance.drawingCard)
            return;
        UIHandler.instance.drawingCard = false;
        DeckManager.SwapCard(card);
    }

    public static void DiscardCard()
    {
        if (!UIHandler.instance.drawingCard)
            return;
        UIHandler.instance.drawingCard = false;
        DeckManager.DiscardCard();
    }

    public static void TakeSpoon()
    {
        GameManager.TakeSpoon(NetworkManager.Singleton.LocalClientId);
    }

    private void Sync()
    {
        UIHandler.instance.cardSprites[0] = Player.localPlayer.hand[0].GetSprite();
        UIHandler.instance.cardSprites[1] = Player.localPlayer.hand[1].GetSprite();
        UIHandler.instance.cardSprites[2] = Player.localPlayer.hand[2].GetSprite();
        UIHandler.instance.cardSprites[3] = Player.localPlayer.hand[3].GetSprite();
        UIHandler.instance.renderDraw = Player.localPlayer.deck.cardCount>0;
        UIHandler.instance.letters = Constants.SPOONS_TRIGGER_WORD.Substring(0, Player.localPlayer.letters);
    }

    private void Update()
    {
        if (activeTopTexts.Count == 0)
            return;
        TopText activeText = activeTopTexts.First();
        if (Time.time > cycleTime + activeText.showDuration )
        {
            cycleTime = Time.time;

            activeText.texts.RemoveAt(0);
            if (activeText.texts.Count == 0)
            {
                activeTopTexts.Remove(activeText);
                UpdateTopText();
                activeText.endEvent?.Invoke();
            }
            UpdateTopText();
        }
    }

    private void UpdateTopText()
    {
        if (activeTopTexts.Count > 0)
        {
            TopText topTextValue = activeTopTexts.First();
            string text = topTextValue.texts.First() ?? "";
            SendTopTextRpc(text);
        }
        else
        {
            SendTopTextRpc("");
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SendTopTextRpc(FixedString128Bytes text)
    {
        UIHandler.instance.topTextValue = text.ToString();
    }
}
