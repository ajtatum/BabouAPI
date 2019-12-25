using System;

namespace Babou.API.Web.Helpers.Swagger
{
    public class RawTextRequestAttribute : Attribute
    {
        public RawTextRequestAttribute()
        {
            MediaType = "text/plain";
        }
        public string MediaType { get; set; }
    }
}
