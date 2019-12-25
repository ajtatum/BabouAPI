using System.Linq;
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
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress.ToString();

            var authKeyQueryString = context.HttpContext.Request.Query["AuthKey"];
            var authKeyHeader = context.HttpContext.Request.Headers["AuthKey"];

            var authKey = !authKeyQueryString.IsEmpty() ? authKeyQueryString.ToString() : authKeyHeader.ToString();
            var authKeys = _context.Users.Select(x => x.ApiAuthKey).ToList();

            if (!authKeys.Contains(authKey))
            {
                _logger.LogWarning("AuthKeyFilter: Attempt to access {Path} denied by {IPAddress}. Tried using AuthKey: {AuthKey}", 
                    context.HttpContext.Request.Path, remoteIp, authKey);
                context.Result = new UnauthorizedResult();
                return;
            }

            var authKeyUser = _context.Users.FirstOrDefault(x => x.ApiAuthKey == authKey)?.UserName;

            _logger.LogInformation("AuthKeyFilter: {Path} is being accessed by {IpAddress}. Using AuthKey: {AuthKey} by {AuthKeyUser}", 
                context.HttpContext.Request.Path, remoteIp, authKey, authKeyUser);

            base.OnActionExecuting(context);
        }
    }
}
