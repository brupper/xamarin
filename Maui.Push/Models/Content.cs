using Newtonsoft.Json;
using System.Collections.Generic;

namespace Brupper.Push.Models
{
    [JsonObject]
    public class Content
    {
        public Content()
        {
            Name = "default";   //By default cannot be empty, must have at least 3 characters
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("custom_data")]
        public Dictionary<string, string> CustomData { get; set; }
    }
}
