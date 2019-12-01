using Newtonsoft.Json;

namespace AJT.API.Models
{
    public class AppVeyorCustom : PushBullet
    {
        [JsonProperty("projectName", Required = Required.Always)]
        public string ProjectName { get; set; }
    }
}
