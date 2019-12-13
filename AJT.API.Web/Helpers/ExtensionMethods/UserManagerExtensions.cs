﻿using System.Linq;
using System.Threading.Tasks;
using AJT.API.Web.Models.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

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

        public static ApplicationUser FindByApiAuthKeyAsync(this UserManager<ApplicationUser> um, string apiAuthKey)
        {
            return um?.Users?.FirstOrDefault(x=>x.ApiAuthKey == apiAuthKey);
        }

        public static async Task<string> GetApiAuthKeyAsync(this UserManager<ApplicationUser> um)
        {
            var user = await um?.GetUserAsync(Current.User);
            return user?.ApiAuthKey;
        }
    }
}
