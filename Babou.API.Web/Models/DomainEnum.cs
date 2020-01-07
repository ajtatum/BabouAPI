using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        [Description("https://s.babou.io/")]
        [Display(Name = "https://s.babou.io/")]
        BabouIo,
        /// <summary>
        /// https://mrvl.co/
        /// </summary>
        [EnumMember(Value = "https://mrvl.co/")]
        [Description("https://mrvl.co/")]
        [Display(Name = "https://mrvl.co/")]
        MrvlCo,
        [EnumMember(Value = "https://xmen.to/")]
        [Description("https://xmen.to/")]
        [Display(Name = "https://xmen.to/")]
        XMenTo
    }
}
