using System.Linq;
using System.Net.Sockets;
using Babou.API.Web.Data;
using BabouExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Babou.API.Web.Helpers.Filters
{
    public class AuthKeyFilter : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthKeyFilter> _logger;

        public AuthKeyFilter(ApplicationDbContext context, ILogger<AuthKeyFilter> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = string.Empty;
            try
            {
                remoteIp = context.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            catch (SocketException socketException)
            {
                _logger.LogError("AuthKeyFilter: Error retrieving IP Address. Error: {@Exception}", socketException);
            }

            var authKeyQueryString = context.HttpContext.Request.Query["AuthKey"];
            var authKeyHeader = context.HttpContext.Request.Headers["AuthKey"];

            var authKey = (!authKeyHeader.IsEmpty() ? authKeyHeader.ToString() : authKeyQueryString.ToString()) ?? string.Empty;

            var authKeyValid = _context.Users.Any(x => x.ApiAuthKey == authKey);

            if (!authKeyValid)
            {
                _logger.LogWarning("AuthKeyFilter: Attempt to access {Path} denied by {IPAddress}. Tried using AuthKey: {AuthKey}",
                    context.HttpContext.Request.Path, remoteIp, authKey);
                context.Result = new UnauthorizedResult();
                return;
            }

            _logger.LogInformation("AuthKeyFilter: {Path} is being accessed by {IpAddress}. Using AuthKey: {AuthKey}",
                context.HttpContext.Request.Path, remoteIp, authKey.Substring(0, 10));

            base.OnActionExecuting(context);
        }
    }
}
