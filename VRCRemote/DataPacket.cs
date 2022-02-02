using Newtonsoft.Json;

namespace VRCRemote
{
    public class DataPacket
    {
        [JsonProperty("userId")] private string _userId;
        [JsonProperty("username")] private string _username;

        public DataPacket(string username, string userId)
        {
            _username = username;
            _userId = userId;
        }

        public static DataPacket CreateHandshakePacket(string username, string userId)
        {
            return new DataPacket(username, userId);
        }
    }
}