using Newtonsoft.Json;
using System.Collections.Generic;

namespace Brupper.Identity.B2C.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class Entity : NotifyPropertyChanged
    {
        protected internal Entity() { }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id", Required = Newtonsoft.Json.Required.Default)]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "@odata.type", Required = Newtonsoft.Json.Required.Default)]
        public string ODataType { get; set; }

        [JsonExtensionData(ReadData = true, WriteData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }

    }
}
