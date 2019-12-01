using IpStack.Models;

namespace AJT.API.Web.Services.Interfaces
{
    public interface IIpService
    {
        string GetRemoteIpV4();
        string GetRemoteIpV6();
        string GetRemoteIp();
        IpAddressDetails GetIpAddressDetails(string ipAddress);
    }
}
