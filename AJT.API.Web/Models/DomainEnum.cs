using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Babou.API.Web.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Domains
    {
        /// <summary>
        /// https://s.babou.io/
        /// </summary>
        [EnumMember(Value = "https://s.babou.io/")]
        BabouIo,
        /// <summary>
        /// https://mrvl.co/
        /// </summary>
        [EnumMember(Value = "https://mrvl.co/")]
        //[Description("https://mrvl.co/")]
        MrvlCo
    }
}
