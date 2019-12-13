using AJT.API.Web.Models;
using AJT.API.Web.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace AJT.API.Web.Services
{
    public class CipherService : ICipherService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly AppSettings _appSettings;

        public CipherService(IDataProtectionProvider dataProtectionProvider, IOptionsMonitor<AppSettings> appSettings)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _appSettings = appSettings.CurrentValue;
        }

        public string Encrypt(string input)
        {
            var protector = _dataProtectionProvider.CreateProtector(_appSettings.Azure.KeyVault.DataProtectionSecret);
            return protector.Protect(input);
        }

        public string Decrypt(string cipherText)
        {
            var protector = _dataProtectionProvider.CreateProtector(_appSettings.Azure.KeyVault.DataProtectionSecret);
            return protector.Unprotect(cipherText);
        }
    }
}
