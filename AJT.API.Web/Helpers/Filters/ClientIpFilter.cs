using System.Linq;
using AJT.API.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AJT.API.Web.Helpers.Filters
{
    public class ClientIpFilter : ActionFilterAttribute
    {
        private readonly ILogger<ClientIpFilter> _logger;
        private readonly IIpService _ipService;
        private readonly string _safeList;

        public ClientIpFilter(ILogger<ClientIpFilter> logger, IIpService ipService, IConfiguration configuration)
        {
            _logger = logger;
            _ipService = ipService;
            _safeList = configuration["AdminSafeList"];
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress.ToString();
            var ipAddressDetails = _ipService.GetIpAddressDetails(remoteIp);

            var ipAddressSafeListArray = _safeList.Split(';');

            var badIp = !ipAddressSafeListArray.Contains(remoteIp);

            if (badIp)
            {
                _logger.LogWarning("ClientIpFilter: Attempt to access {Path} denied by {IPAddress} located at {City}, {State}, {Country}.", context.HttpContext.Request.Path, remoteIp, ipAddressDetails.City, ipAddressDetails.RegionCode, ipAddressDetails.CountryCode);
                context.Result = new UnauthorizedResult();
                return;
            }

            _logger.LogInformation("ClientIpFilter: {Path} is being accessed by {IpAddress} located at {City}, {State}, {Country}.", context.HttpContext.Request.Path, remoteIp, ipAddressDetails.City, ipAddressDetails.RegionCode, ipAddressDetails.CountryCode);

            base.OnActionExecuting(context);
        }
    }
}