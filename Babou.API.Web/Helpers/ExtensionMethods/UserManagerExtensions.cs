using System;
using System.Linq;
using System.Threading.Tasks;
using Babou.API.Web.Data;
using Babou.API.Web.Models.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Babou.API.Web.Helpers.ExtensionMethods
{
    public static class UserManagerExtensions
    {
        private static IHttpContextAccessor _httpContextAccessor;
        private static ILogger _logger;
        private static ApplicationDbContext _context;

        public static void Configure(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = Log.ForContext(typeof(UserManagerExtensions));
            _context = new ApplicationDbContext(configuration);
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

        public static async Task<string> GetFullNameAsync(this UserManager<ApplicationUser> um)
        {
            try
            {
                var user = await um.GetUserAsync(_httpContextAccessor.HttpContext.User);
                return user.FullName ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UserManagerExtensions: Error retrieving user FullName.");
                return string.Empty;
            }
        }

        public static async Task<bool> SetFullNameAsync(this UserManager<ApplicationUser> um, string fullName)
        {
            try
            {
                var user = await um.GetUserAsync(_httpContextAccessor.HttpContext.User);

                user.FullName = fullName;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UserManagerExtensions: Error saving Full Name.");
                return false;
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
