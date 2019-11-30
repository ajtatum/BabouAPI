using System.Linq;
using AJT.API.Models;
using AJT.API.Services.Interfaces;
using BabouExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AJT.API.Helpers.Filters
{
    public class AuthKeyFilter : ActionFilterAttribute
    {
        private readonly ILogger<AuthKeyFilter> _logger;
        private readonly AppSettings _appSettings;
        private readonly IIpService _ipService;

        public AuthKeyFilter(ILogger<AuthKeyFilter> logger, IOptionsMonitor<AppSettings> appSettings, IIpService ipService)
        {
            _logger = logger;
            _appSettings = appSettings.CurrentValue;
            _ipService = ipService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress.ToString();
            var ipAddressDetails = _ipService.GetIpAddressDetails(remoteIp);

            var authKeyQueryString = context.HttpContext.Request.Query["AuthKey"];
            var authKeyHeader = context.HttpContext.Request.Headers["AuthKey"];

            var authKey = !authKeyQueryString.IsEmpty() ? authKeyQueryString.ToString() : authKeyHeader.ToString();
            var authKeys = new[] { _appSettings.AuthKeys.Default, _appSettings.AuthKeys.AppVeyor };

            if (!authKeys.Contains(authKey))
            {
                _logger.LogWarning("AuthKeyFilter: Attempt to access {Path} denied by {IPAddress} located at {City}, {State}, {Country}. Tried using AuthKey: {AuthKey}", 
                    context.HttpContext.Request.Path, remoteIp, ipAddressDetails.City, ipAddressDetails.RegionCode, ipAddressDetails.CountryCode, authKey);
                context.Result = new UnauthorizedResult();
                return;
            }

            _logger.LogInformation("AuthKeyFilter: {Path} is being accessed by {IpAddress} located at {City}, {State}, {Country}. Using AuthKey: {AuthKey}", 
                context.HttpContext.Request.Path, remoteIp, ipAddressDetails.City, ipAddressDetails.RegionCode, ipAddressDetails.CountryCode, authKey);

            base.OnActionExecuting(context);
        }
    }
}
