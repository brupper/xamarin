using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Brupper.Push.Models
{
    public class PushRequest
    {
        public string Environment { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public List<JObject> Data { get; set; }
    }
}
