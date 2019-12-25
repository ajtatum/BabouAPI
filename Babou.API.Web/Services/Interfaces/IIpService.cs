using IpStack.Models;

namespace Babou.API.Web.Services.Interfaces
{
    public interface IIpService
    {
        string GetRemoteIpV4();
        string GetRemoteIpV6();
        string GetRemoteIp();
        IpAddressDetails GetIpAddressDetails(string ipAddress);
    }
}
