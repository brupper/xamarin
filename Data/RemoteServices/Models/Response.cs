using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Brupper.Data.RemoteServices.Models
{
    public class Response
    {
        [JsonProperty(PropertyName = "validation")]
        public object Validation { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public IEnumerable<string> Errors { get; set; }
            = new string[0];

        [JsonProperty(PropertyName = "exception")]
        public ServerException Exception { get; set; }
            = new ServerException();

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "serverTimeSec")]
        public long ServerTimeSec { get; set; }
            = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        [JsonIgnore]
        public DateTimeOffset ServerTime
            => DateTimeOffset.FromUnixTimeSeconds(ServerTimeSec);
    }

    public class Response<TData> : Response
    {
        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public TData Data { get; set; }
    }
}
