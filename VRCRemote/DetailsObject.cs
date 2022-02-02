using Newtonsoft.Json;

namespace VRCRemote
{
    public class DetailsObject
    {
        [JsonProperty("worldId")] public string WorldId;
        [JsonProperty("worldName")] public string WorldName;
    }
}