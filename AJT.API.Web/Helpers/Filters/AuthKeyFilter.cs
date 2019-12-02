using System.Linq;
using AJT.API.Web.Data;
using AJT.API.Web.Services.Interfaces;
using BabouExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace AJT.API.Web.Helpers.Filters
{
    public class AuthKeyFilter : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthKeyFilter> _logger;
        private readonly IIpService _ipService;

        public AuthKeyFilter(ApplicationDbContext context, ILogger<AuthKeyFilter> logger, IIpService ipService)
        {
            _context = context;
            _logger = logger;
            _ipService = ipService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress.ToString();
            var ipAddressDetails = _ipService.GetIpAddressDetails(remoteIp);

            var authKeyQueryString = context.HttpContext.Request.Query["AuthKey"];
            var authKeyHeader = context.HttpContext.Request.Headers["AuthKey"];

            var authKey = !authKeyQueryString.IsEmpty() ? authKeyQueryString.ToString() : authKeyHeader.ToString();
            var authKeys = _context.Users.Select(x => x.ApiAuthKey).ToList();

            if (!authKeys.Contains(authKey))
            {
                _logger.LogWarning("AuthKeyFilter: Attempt to access {Path} denied by {IPAddress} located at {City}, {State}, {Country}. Tried using AuthKey: {AuthKey}", 
                    context.HttpContext.Request.Path, remoteIp, ipAddressDetails.City, ipAddressDetails.RegionCode, ipAddressDetails.CountryCode, authKey);
                context.Result = new UnauthorizedResult();
                return;
            }

            var authKeyUser = _context.Users.FirstOrDefault(x => x.ApiAuthKey == authKey)?.UserName;

            _logger.LogInformation("AuthKeyFilter: {Path} is being accessed by {IpAddress} located at {City}, {State}, {Country}. Using AuthKey: {AuthKey} by {AuthKeyUser}", 
                context.HttpContext.Request.Path, remoteIp, ipAddressDetails.City, ipAddressDetails.RegionCode, ipAddressDetails.CountryCode, authKey, authKeyUser);

            base.OnActionExecuting(context);
        }
    }
}
