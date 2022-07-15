using System;
using Microsoft.AspNetCore.Mvc;

namespace bullhorn
{
    public class ReceiveSocketValidator
    {
        public ReceiveSocketValidator()
        {
        }

        public static bool IsValid(HttpRequest req)
        {
            //if (req.Host.ToString().Length == 0) return false;
            ////var deserialized = ExtendedSerializerExtensions.DeserializeFromStream(req.Body).ToString();
            ////var hash = ExtendedSerializerExtensions.DeserializeJson(deserialized).Result;
            //var hash = ExtendedSerializerExtensions.DeserializeBody(req).Result;
            //if (!hash.ContainsKey("type")) return false;
            //if (!hash.ContainsKey("from_id")) return false;
            //if (!hash.ContainsKey("to_id")) return false;
            //if (!hash.ContainsKey("meta")) return false;
            return true;
        }
    }
}

