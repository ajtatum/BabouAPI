using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IpStack.Models;

namespace AJT.API.Services.Interfaces
{
    public interface IIpService
    {
        string GetRemoteIpV4();
        string GetRemoteIpV6();
        string GetRemoteIp();
        IpAddressDetails GetIpAddressDetails(string ipAddress);
    }
}
