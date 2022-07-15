using System;
using System.Text.Json;
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
            var asString = JsonConvert.SerializeObject(source, SerializerSettings);
            var res = System.Text.Encoding.ASCII.GetBytes(asString.PadRight(1024 * 4));
            return res;
        }

        public static T Deserialize<T>(this byte[] source)
        {
            var asString = System.Text.Encoding.Unicode.GetString(source);
            return JsonConvert.DeserializeObject<T>(asString);
        }

        public static T DeserializeFromStream<T>(Stream stream)
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(jsonTextReader);
                //return serializer.Deserialize(jsonTextReader);
            }
        }

        public static async Task<Dictionary<string, string>> DeserializeJson(Stream jsonText)
        {
            MemoryStream ms = new MemoryStream();
            jsonText.CopyTo(ms);
            
            var stream = new MemoryStream(ms.ToArray());

            var jsonDocument = JsonDocument.Parse(stream);
            var dictionary = jsonDocument.RootElement
                .EnumerateObject()
                .ToDictionary(property => property.Name, property => property.Value.ToString());
            return dictionary;
        }

        public static async Task<T> DeserializeBody <T>(HttpRequest req)
        {
            return await System.Text.Json.JsonSerializer.DeserializeAsync <T>(req.Body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
            );
        }
    }
    //    public static async Task<Dictionary<string, string>> DeserializeBody(HttpRequest req)
    //    {
    //        var deserialized = DeserializeFromStream(req.Body).ToString();
    //        return DeserializeJson(deserialized).Result;
    //    }
    //}
}

