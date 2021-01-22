using Newtonsoft.Json;

namespace Brupper.Data.RemoteServices.Models
{
    public class ServerException
    {
        [JsonProperty(PropertyName = "message")]
        public string ServerMessage { get; set; }

        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        public override string ToString() => ServerMessage + "\n\n" + Code;
    }
}
