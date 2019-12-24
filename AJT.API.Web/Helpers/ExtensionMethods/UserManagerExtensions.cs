using System;
using System.Linq;
using System.Threading.Tasks;
using AJT.API.Web.Models.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace AJT.API.Web.Helpers.ExtensionMethods
{
    public static class UserManagerExtensions
    {
        private static IHttpContextAccessor _httpContextAccessor;
        private static ILogger _logger;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = Log.ForContext(typeof(UserManagerExtensions));
        }

        public static HttpContext Current => _httpContextAccessor.HttpContext;

        public static ApplicationUser FindByApiAuthKeyAsync(this UserManager<ApplicationUser> um, string apiAuthKey)
        {
            try
            {
                return um.Users.FirstOrDefault(x => x.ApiAuthKey == apiAuthKey);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UserManagerExtensions: Error finding user by Api AuthKey: {AuthKey}.", apiAuthKey);
                return null;
            }
        }

        public static async Task<string> GetApiAuthKeyAsync(this UserManager<ApplicationUser> um)
        {
            try
            {
                var user = await um.GetUserAsync(_httpContextAccessor.HttpContext.User);
                return user.ApiAuthKey;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UserManagerExtensions: Error retrieving user ApiAuthKey.");
                return string.Empty;
            }
        }

        public static async Task<ApplicationUser> GetCurrentUserAsync(this UserManager<ApplicationUser> um)
        {
            try
            {
                var user = await um.GetUserAsync(_httpContextAccessor.HttpContext.User);
                return user;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UserManagerExtensions: Error getting current user.");
                return null;
            }
        }
    }
}
