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
        /// https://babou.io/go/
        /// </summary>
        [EnumMember(Value = "https://babou.io/go/")]
        [Description("https://babou.io/go/")]
        [Display(Name = "https://babou.io/go/")]
        BabouIoGo,
        /// <summary>
        /// https://mrvl.co/
        /// </summary>
        [EnumMember(Value = "https://mrvl.co/")]
        [Description("https://mrvl.co/")]
        [Display(Name = "https://mrvl.co/")]
        MrvlCo
    }
}
