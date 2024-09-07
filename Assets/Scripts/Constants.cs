

using System.Collections.Generic;

public class Constants
{
    //Networking
    public static readonly string NETWORK_DISCONNECT_MESSAGE = "Sorry this lobby is full";

    //Center Point
    public static readonly float CENTER_GROUND_POSITION = 2f;
    public static readonly float CENTER_CIRCLE_RADIUS = 5f;

    //Player Info
    public static readonly float PLAYER_CAMERA_OFFSET = 0.8f;
    public static readonly float PLAYER_TOPTEXT_TIME = 3.5f;

    //Card Info
    public static readonly List<string> CARD_SUITES = new List<string> { "Hearts", "Spades", "Diamonds", "Clubs" };
    public static readonly List<string> CARD_VALUES = new List<string> { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };

    //Spoons info
    public static readonly string SPOONS_DEALER_NAME = "DEALER";
    public static readonly string SPOONS_TRIGGER_WORD = "SPOON";
    public static readonly int SPOON_TRIGGER_WORD_LENGTH = 5;
    public static readonly float SPOONS_TOGGLE_VIEW_COOLDOWN = 2f;
    public static readonly float SPOONS_VIEW_TIMER = 1f;

    //Top Texts
    public static readonly string ROUND_START_TEXT = "A new round is starting! {0} is the dealer. Good luck!";
    public static readonly string ROUND_END_TEXT = "The round has ended!";
    public static readonly string ROUND_SPOON_EARLY_TEXT = "{0} has jumped the gun and grabbed a spoon to early!";
    public static readonly string ROUND_SPOON_LATE_TEXT = "{0} was too slow to react and failed to grab a spoon!";
    public static readonly string ROUND_LIVES_LEFT = "They have {0} letters spelling {1}";
    public static readonly string ROUND_NO_LIVES = "They will be taken out back shortly";
    public static readonly string ROUND_WINNER_TEXT = "Congrats {0}, you win!";

    //Lobby Defaults
    public static readonly int LOBBY_MAX_PLAYERS = 4;
    public static readonly int LOBBY_MIN_PLAYERS = 2;
    public static readonly float LOBBY_HEARTBEAT_COOLDOWN = 30f;
    public static readonly float LOBBY_UPDATE_COOLDOWN = 1f;

    //Lobby Texts
    public static readonly string TEXTS_NONAME = "Failed to join lobby: No name set";
    public static readonly string TEXTS_NOLOBBY = "Failed to join lobby: No lobby found";
    public static readonly string TEXTS_NOCODE = "Failed to join lobby: No code entered";

    //Networking Keys
    public static readonly string KEY_PLAYER_NAME = "PlayerName";
    public static readonly string KEY_RELAY_CODE = "RelayCode";
}
