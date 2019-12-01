using AJT.API.Models;
using AJT.API.Web.Services.Interfaces;
using IpStack;
using IpStack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AJT.API.Web.Services
{
    public class IpService : IIpService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly AppSettings _appSettings;

        public IpService(IHttpContextAccessor accessor, IOptionsMonitor<AppSettings> appSettings)
        {
            _accessor = accessor;
            _appSettings = appSettings.CurrentValue;
        }

        public string GetRemoteIpV4()
        {
            return _accessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
        public string GetRemoteIpV6()
        {
            return _accessor.HttpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
        }

        public string GetRemoteIp()
        {
            return _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public IpAddressDetails GetIpAddressDetails(string ipAddress)
        {
            var client = new IpStackClient(_appSettings.IpStackApiKey, true);

            var ipAddressDetails = client.GetIpAddressDetails(ipAddress);

            return ipAddressDetails;
        }

    }
}
