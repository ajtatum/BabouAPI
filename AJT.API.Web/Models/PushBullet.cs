using System.Collections.Generic;
using Newtonsoft.Json;

namespace AJT.API.Web.Models
{
    public class PushBullet
    {
        [JsonProperty("devices", NullValueHandling = NullValueHandling.Include)]
        public List<string> DeviceNickNames { get; set; }
        [JsonProperty("channel", NullValueHandling = NullValueHandling.Include)]
        public string Channel { get; set; }
        [JsonProperty("title", Required = Required.Always)]
        public string Title { get; set; }
        [JsonProperty("body", Required = Required.Always)]
        public string Body { get; set; }
        [JsonProperty("url", NullValueHandling = NullValueHandling.Include)]
        public string Url { get; set; }
    }
}
