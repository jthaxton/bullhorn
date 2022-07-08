using System;
using Newtonsoft.Json;

namespace bullhorn
{
    public static class ExtendedSerializerExtensions
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        public static byte[] Serialize<T>(this T source)
        {
            //var buffer = new byte[1024 * 4];
            var asString = JsonConvert.SerializeObject(source, SerializerSettings);
            //byte[1024 * 4] bytes = Encoding.ASCII.GetBytes(someString);
            var res = System.Text.Encoding.ASCII.GetBytes(asString.PadRight(1024 * 4));
            //var succ = new byte[1024 * 4](System.Text.Encoding.Unicode.GetBytes(asString));
            return res;
        }

        public static T Deserialize<T>(this byte[] source)
        {
            var asString = System.Text.Encoding.Unicode.GetString(source);
            return JsonConvert.DeserializeObject<T>(asString);
        }
    }
}

