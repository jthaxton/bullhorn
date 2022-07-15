using System;
namespace bullhorn
{
    public class WebsocketSubscriber
    {
        public WebsocketSubscriber(HttpContext httpcontext)
        {
            using var websocket = httpcontext.WebSockets.AcceptWebSocketAsync();
        }

        public void Subscribe(string id)
        {

        }
    }
}

