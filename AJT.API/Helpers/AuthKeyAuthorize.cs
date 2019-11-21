using System.Linq;
using AJT.API.Models;
using AJT.API.Services;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AJT.API.Helpers
{
    /// <summary>
    /// Checks the QueryString or Headers for AuthKey and if it matches the AnonymousAuthKey
    /// </summary>
    public class AuthKeyAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly ILogger<AuthKeyAuthorize> _logger;
        private readonly AppSettings _appSettings;
        public AuthKeyAuthorize(ILogger<AuthKeyAuthorize> logger, IOptionsMonitor<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.CurrentValue;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authKeyQueryString = context.HttpContext.Request.Query["AuthKey"];
            var authKeyHeader = context.HttpContext.Request.Headers["AuthKey"];

            var authKey = !authKeyQueryString.IsEmpty() ? authKeyQueryString.ToString() : authKeyHeader.ToString();

            //var config = Startup.Configuration;

            var authKeys = new[] { _appSettings.AuthKeys.Default, _appSettings.AuthKeys.AppVeyor };

            if (!authKeys.Contains(authKey))
            {
                _logger.LogWarning(" Attempt to access {Path} denied by {IPAddress}.", context.HttpContext.Request.Path, context.HttpContext.Connection.RemoteIpAddress);
                context.Result = new UnauthorizedResult();
            }

        }
    }
}
