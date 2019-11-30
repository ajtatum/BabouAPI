using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AJT.API.Services
{
    public class IpService
    {
        private readonly IHttpContextAccessor _accessor;

        public IpService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
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

    }
}
