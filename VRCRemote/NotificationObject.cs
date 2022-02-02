using System;
using Newtonsoft.Json;
using VRCRemote;

namespace VRCTower
{
    public class NotificationObject
    {
        [JsonProperty("created_at")] public DateTime CreatedAt;
        [JsonProperty("details")] public DetailsObject Details;
        [JsonProperty("id")] public string Id;
        [JsonProperty("message")] public string Message;
        [JsonProperty("seen")] public bool Seen;
        [JsonProperty("senderUserId")] public string SenderUserId;
        [JsonProperty("senderUsername")] public string SenderUsername;
    }
}