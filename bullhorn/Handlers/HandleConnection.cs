using System;
namespace bullhorn.Handlers
{
    public class HandleConnection
    {
        public HandleConnection()
        {
        }
        public async static void Run(
            Dictionary<string, System.Net.WebSockets.WebSocket?> dict,
            Dictionary<string, string> serializedBody,
            HttpContext context
            )
        {
            if (!ReceiveSocketValidator.IsValid(context.Request)) return;
            if (serializedBody["type"] == "subscribe")
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                if (!dict.ContainsKey("from_cookie")) {
                    dict[serializedBody["from_cookie"]] = webSocket;
                }
            }
            if (serializedBody["type"] == "push")
            {
                //if(dict.ContainsKey(serializedBody["to_cookie"]))
                //{
                //    var hashified = new Dictionary<string, string>();
                //    hashified.Add("to_cookie", serializedBody["to_cookie"]);
                //    hashified.Add("from_cookie", serializedBody["from_cookie"]);
                //    hashified.Add("meta", serializedBody["meta"]);

                //    var ser = ExtendedSerializerExtensions.Serialize(hashified);
                //    // TODO : FINISH THIS FUNCTION
                //    await dict["to_cookie"].SendAsync(
                //        new ArraySegment<byte>(ser, 0, 1024 * 4),
                //        receiveResult.MessageType,
                //        receiveResult.EndOfMessage,
                //        CancellationToken.None);
                //}
            }
        }
    }
}

