using Newtonsoft.Json;
using System.Collections.Generic;

namespace Brupper.Push.Models
{
    public partial class ApnsPayload
    {
        [JsonProperty("data")]
        public Dictionary<string, string> Data { get; set; }
            = new Dictionary<string, string>();

        [JsonProperty("aps")]
        public Aps Aps { get; set; }
    }

    public partial class Aps
    {
        [JsonProperty("category")]
        public string Category { get; set; }
            = "NEW_MESSAGE_CATEGORY";

        [JsonProperty("sound")]
        public string Sound { get; set; }
            = "bingbong.aiff";

        [JsonProperty("alert")]
        public Alert Alert { get; set; }
            = new Alert();
    }

    public partial class Alert
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
