using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Babou.API.Web.Helpers;
using Babou.API.Web.Helpers.ExtensionMethods;
using Babou.API.Web.Models.Database;
using Babou.API.Web.Services.Interfaces;
using BabouExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Babou.API.Web.Services
{
    public class UserService : IUserService
    {
        private static readonly Random Random = new Random();
        private const string BigAlphas = "ABCDEFGHIJKLMNOPQRSTUVWXY";
        private const string SmallAlphas = "abcdefghjlkmnopqrstuvwxyz";
        private const string Numbers = "0123456789";
        private const string NonAlphas = "!@#$%^&*()_+";
        private static readonly string Pool = $"{BigAlphas}{SmallAlphas}{Numbers}{NonAlphas}";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;

        public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger, 
                           IEmailService emailService)
        {
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
        }

        /// <summary>
        /// Tries to create a user and adds them to the Member role.
        /// </summary>
        /// <param name="email">Email address of the user</param>
        /// <param name="password">Password for the user. If null or empty, a password is automatically generated.</param>
        /// <param name="username">Username of the user.</param>
        /// <param name="fullName">Full Name of the user.</param>
        /// <returns>Will either return an <see cref="ApplicationUser"/> or null.</returns>
        public async Task<ApplicationUser> QuickCreateUser(string email, string password, string username, string fullName)
        {
            var user = new ApplicationUser
            {
                Email = email,
                UserName = !username.IsNullOrWhiteSpace() ? username : email,
                FullName = !fullName.IsNullOrWhiteSpace() ? fullName : "Unknown",
                ApiAuthKey = Guid.NewGuid().ToString("N")
            };

            if (password.IsNullOrWhiteSpace())
            {
                password = GenerateRandomPassword();
            }

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Constants.Roles.Member);
                await _userManager.SetEmailConfirmedAsync(user.Id);

                _logger.LogInformation("TryCreateUser: User {UserName} created a new account with password.", username);

                return user;
            }
            else
            {
                _logger.LogInformation("Unable to create user {UserName}. Errors: {@Errors}", username, result.Errors);
                return null;
            }
        }

        private static readonly object ThreadLock = new object();

        /// <summary>
        /// Generates a random password
        /// </summary>
        /// <returns>A random string</returns>
        private string GenerateRandomPassword()
        {
            string pool;
            Random rand;

            lock (ThreadLock)
            {
                pool = Pool;
                rand = Random;
            }

            var poolBuilder = new StringBuilder(pool);

            var poolBuilderString = poolBuilder.ToString();

            var output = new char[24];
            for (var i = 0; i < 24; i++)
            {
                var charIndex = rand.Next(0, poolBuilderString.Length);
                output[i] = poolBuilderString[charIndex];
            }

            return new string(output);
        }

    }
}
