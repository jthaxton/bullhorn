using System;
namespace bullhorn.Models
{
    public class SocketJarSingleton
    {
        public static Dictionary<string, System.Net.WebSockets.WebSocket> Store;
        public SocketJarSingleton()
        {
        }
        public static System.Net.WebSockets.WebSocket Get(string key)
        {
            return Store[key];
        }

        public static System.Net.WebSockets.WebSocket  Put <T>(string key, System.Net.WebSockets.WebSocket val)
        {
            Store[key] = val;
            return val;
        }

        public static bool ContainsKey(string key)
        {
            return Store.ContainsKey(key);
        }
    }
}

