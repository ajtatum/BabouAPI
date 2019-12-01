using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using AJT.API.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AJT.API.Web.Helpers.ExtensionMethods
{
    public static class UserManagerExtensions
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static HttpContext Current => _httpContextAccessor.HttpContext;

        public static ApplicationUser FindByApiAuthKey(this UserManager<ApplicationUser> um, string apiAuthKey)
        {
            return um?.Users?.FirstOrDefault(x=>x.ApiAuthKey == apiAuthKey);
        }

        public static async Task<string> GetApiAuthKey(this UserManager<ApplicationUser> um)
        {
            var user = await um?.GetUserAsync(Current.User);
            return user?.ApiAuthKey;
        }
    }
}
