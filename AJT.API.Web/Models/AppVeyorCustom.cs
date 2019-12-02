using Newtonsoft.Json;

namespace AJT.API.Web.Models
{
    public class AppVeyorCustom : PushBullet
    {
        [JsonProperty("projectName", Required = Required.Always)]
        public string ProjectName { get; set; }
    }
}
