using System;
using Newtonsoft.Json;

namespace bullhorn.Models
{
    //interface IOrder
    //{
    //    public string ActionType { get; set; }
    //    public string FromCookie { get; set; }
    //    public string? ResourceType { get; set; }
    //    public string[]? PushToCookies { get; set; }
    //}

    [JsonObject(MemberSerialization.OptOut)]

    public class Order
    {
        public string ActionType { get; set; }
        public string FromCookie { get; set; }
        public string? ResourceType { get; set; }
        public string[]? PushToCookies { get; set; }
        public Dictionary<dynamic, dynamic>? Meta { get; set; }
        public Order(
            string actionType,
            string fromCookie,
            string? resourceType,
            string[]? pushToCookies,
            Dictionary<dynamic, dynamic>? meta
            )
        {
            this.ActionType = actionType;
            this.FromCookie = fromCookie;
            this.ResourceType = resourceType;
            this.PushToCookies = pushToCookies;
            this.Meta = meta;
        }
    }
}

