using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AJT.API.Helpers.Filters
{
    public class ClientIpCheckFilter : ActionFilterAttribute
    {
        private readonly ILogger<ClientIpCheckFilter> _logger;
        private readonly string _safeList;

        public ClientIpCheckFilter(ILogger<ClientIpCheckFilter> logger, IConfiguration configuration)
        {
            _logger = logger;
            _safeList = configuration["AdminSafeList"];
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            _logger.LogInformation("Remote IpAddress: {RemoteIp} accessing {Action}", remoteIp, context.ActionDescriptor.DisplayName);

            var ip = _safeList.Split(';');

            var badIp = true;
            foreach (var address in ip)
            {
                if (remoteIp.IsIPv4MappedToIPv6)
                {
                    remoteIp = remoteIp.MapToIPv4();
                }
                var testIp = IPAddress.Parse(address);
                if (testIp.Equals(remoteIp))
                {
                    badIp = false;
                    break;
                }
            }

            if (badIp)
            {
                _logger.LogInformation("Forbidden Request from Remote IP address: {RemoteIp} accessing {Action}", remoteIp, context.ActionDescriptor.DisplayName);
                context.Result = new StatusCodeResult(401);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}