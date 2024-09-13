

using System.Collections.Generic;

public class Constants
{
#if UNITY_EDITOR
    public static readonly bool DEBUG_MODE = true;
#else
    public static readonly bool DEBUG_MODE = false;
#endif

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
    public static readonly float SPOONS_TOGGLE_VIEW_COOLDOWN_MODIFIER = 1f;
    public static readonly float SPOONS_VIEW_TIMER = 1f;

    //Top Texts
    public static readonly string ROUND_GAME_START = "Welcome to spoons, lets get started!";
    public static readonly string ROUND_START_TEXT = "A new round is starting! {0} is the dealer. Good luck!";
    public static readonly string ROUND_END_TEXT = "The round has ended!";
    public static readonly string ROUND_SPOON_EARLY_TEXT = "{0} has jumped the gun and grabbed a spoon too early!";
    public static readonly string ROUND_SPOON_LATE_TEXT = "{0} was too slow to react and failed to grab a spoon!";
    public static readonly string ROUND_LIVES_LEFT = "They have {0} letters spelling {1}";
    public static readonly string ROUND_NO_LIVES = "They will be taken out back shortly";
    public static readonly string ROUND_WINNER_TEXT = "Congrats {0}, you win!";
    public static readonly string ROUND_DISCONNECT_PLAYER = "Someone left. Starting a new round";
    public static readonly string ROUND_MODIFIER_INSTA_KILL = "The one life modifier is enabled. So be careful as you only get 1 letter before getting eliminated!";

    //Lobby Defaults
    public static readonly int LOBBY_MAX_PLAYERS = 6;
    public static readonly int LOBBY_MIN_PLAYERS = 2;
    public static readonly float LOBBY_HEARTBEAT_COOLDOWN = 30f;
    public static readonly float LOBBY_UPDATE_COOLDOWN = 1f;

    //Lobby Texts
    public static readonly string TEXTS_NONAME = "No name set";
    public static readonly string TEXTS_LONGNAME = "Name too long (1-16 characters)";
    public static readonly string TEXTS_ILLEGAL = "Name contains illegal characters";

    public static readonly string TEXTS_NOLOBBY = "No lobby found";
    public static readonly string TEXTS_NOCODE = "No code entered/Code is invalid";
    public static readonly string TEXTS_UNKNOWN = "Unknown Error";
    public static readonly string TEXTS_CONFLICT = "Lobby conflict";
    public static readonly string TEXTS_FULL = "Lobby full";
    public static readonly string TEXTS_NOLOBBIES = "No lobbies found";

    //Networking Keys
    public static readonly string KEY_PLAYER_NAME = "PlayerName";
    public static readonly string KEY_PLAYER_SKIN = "PlayerSkin";
    public static readonly string KEY_LOBBY_RELAYCODE = "RelayCode";
    public static readonly string KEY_LOBBY_MODIFIER_INSTAKILL = "StateInstaKill";

    //Leaderboards
    public static readonly string LEADERBOARD_ID = "4_of_a_kinds";

    //Auth
    public static readonly string AUTH_DEFAULTNAME = "SpoonsPlayer";

    //PlayerPrefs
    public static readonly string PREFS_MUSICVOLUME = "musicVolume";
    public static readonly string PREFS_SFXVOLUME = "sfxVolume";
    public static readonly string PREFS_ACTIVESKIN = "activeSkin";

    //Skins
    public static readonly string SKINS_EQUIP = "Equip Skin";
    public static readonly string SKINS_UNEQUIP = "Unequip Skin";
    public static readonly string SKINS_LOCKED = "Locked";
    public static readonly string SKINS_RESPONSE_EQUIP = "Skin equipped";
    public static readonly string SKINS_RESPONSE_UNEQUIP = "Skin unequipped";
}
