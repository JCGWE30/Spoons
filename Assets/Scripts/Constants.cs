

using System.Collections.Generic;

public class Constants
{
    //Debug
    public static readonly int DEBUG_EXPECTED_PLAYER_SIZE = 2;

    //Center Point
    public static readonly float CENTER_GROUND_POSITION = 2f;
    public static readonly int CENTER_CIRCLE_RADIUS = 5;

    //Player Info
    public static readonly float PLAYER_CAMERA_OFFSET = 0.8f;
    public static readonly float PLAYER_TOPTEXT_TIME = 2f;

    //Card Info
    public static readonly List<string> CARD_SUITES = new List<string> { "Hearts", "Spades", "Diamonds", "Clubs" };
    public static readonly List<string> CARD_VALUES = new List<string> { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };

    //Spoons info
    public static readonly string SPOONS_DEALER_NAME = "DEALER";
    public static readonly string SPOONS_TRIGGER_WORD = "SPOON";
    public static readonly int SPOON_TRIGGER_WORD_LENGTH = 5;
    public static readonly float SPOONS_TOGGLE_VIEW_COOLDOWN = 5f;
    public static readonly float SPOONS_VIEW_TIMER = 3f;

    //Top Texts
    public static readonly string ROUND_START_TEXT = "A new round is starting! {0} is the dealer. Good luck!";
    public static readonly string ROUND_END_TEXT = "The round has ended!";
    public static readonly string ROUND_SPOON_EARLY_TEXT = "{0} has jumped the gun and grabbed a spoon to early!";
    public static readonly string ROUND_SPOON_LATE_TEXT = "{0} was too slow to react and failed to grab a spoon!";
    public static readonly string ROUND_LIVES_LEFT = "They have {0} letters spelling {1}";
    public static readonly string ROUND_NO_LIVES = "They will be taken out back shortly";
}
