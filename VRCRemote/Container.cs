using Newtonsoft.Json;

namespace VRCRemote
{
    public class Container
    {
        [JsonProperty("creator")] public string Creator;

        [JsonProperty("queueName")] public string QueueName;
    }
}