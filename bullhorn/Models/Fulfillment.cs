using System;
using Newtonsoft.Json;

namespace bullhorn.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Fulfillment
    {
        public string ResourceType { get; set; }
        public object? Meta { get; set; }
        public Fulfillment(
            string resourceType,
            object? meta
            )
        {
             this.ResourceType = resourceType;
             this.Meta = meta;
        }
    }
}

