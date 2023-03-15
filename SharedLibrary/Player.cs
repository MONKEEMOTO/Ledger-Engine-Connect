using Newtonsoft.Json;

namespace SharedLibrary
{
    /// <summary>
    /// This shared library is so i can share Models with Unity directly.
    /// </summary>
    [Serializable]
    public class Player
    {
        public int playerGuid;          //identifier
        public string? playerName;      //players name
        public string? qrurl;           //url to the QR code for authentication
        public string? uuid;            //the player's XUMM UUID
    }
}