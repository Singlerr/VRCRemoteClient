using System;
using Newtonsoft.Json;

namespace VRCRemote
{
    public class PacketSerializer
    {
        public static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static bool TryDeserialize<T>(string data, out T result)
        {
            result = default;
            try
            {
                result = Deserialize<T>(data);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}