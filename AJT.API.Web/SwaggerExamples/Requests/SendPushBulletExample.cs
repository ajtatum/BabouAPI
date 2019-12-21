using System.Collections.Generic;
using AJT.API.Web.Models;
using Swashbuckle.AspNetCore.Filters;

namespace AJT.API.Web.SwaggerExamples.Requests
{
    public class SendPushBulletExample : IExamplesProvider<PushBullet>
    {
        public PushBullet GetExamples()
        {
            return new PushBullet
            {
                DeviceNickNames = new List<string>() {"Chrome", "Laptop"},
                Title = "Test Notification",
                Body = "This is a PushBullet notification.",
                Url = "https://babou.io"
            };
        }
    }
}
