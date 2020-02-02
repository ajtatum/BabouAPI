using System;
using System.Text;
using System.Threading.Tasks;
using Babou.API.Web.Data;
using Babou.API.Web.Helpers;
using Babou.API.Web.Models.Database;
using Babou.API.Web.Services.Interfaces;
using BabouExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Babou.API.Web.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;

        public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger, 
                           IEmailService emailService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
            _context = context;
        }

        /// <summary>
        /// Tries to create a user and adds them to the Member role.
        /// </summary>
        /// <param name="email">Email address of the user</param>
        /// <param name="password">Password for the user. If null or empty, a password is automatically generated.</param>
        /// <param name="userName">Username of the user.</param>
        /// <param name="fullName">Full Name of the user.</param>
        /// <returns>Will either return an <see cref="ApplicationUser"/> or null.</returns>
        public async Task<ApplicationUser> QuickCreateUser(string email, string password, string userName, string fullName)
        {
            var user = new ApplicationUser
            {
                Email = email,
                UserName = !userName.IsNullOrWhiteSpace() ? userName : email,
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
                await SetEmailConfirmedAsync(user);

                /*
                 * TODO: Message Queuing Service
                 * Each message takes up to 800ms, thus being the major delay when a new user signs up through this method.
                 * Slack wants replies to be under 5s, so with these methods being called it's going to be hard to accomplish.
                 * Need to investigate message queuing service to handle this after fulfilling request.
                 * For now, the Slack user still gets the short URL, but they receive a confusing error from Slack.
                */
                await _emailService.SendQuickWelcomeMessage(user);
                await _emailService.SendNewUserMessage(user);

                _logger.LogInformation("TryCreateUser: User {UserName} created a new account with password.", userName);

                return user;
            }
            else
            {
                _logger.LogInformation("TryCreateUser: Unable to create user {UserName}. Errors: {@Errors}", userName, result.Errors);
                return null;
            }
        }

        public async Task SetEmailConfirmedAsync(ApplicationUser user)
        {
            try
            {
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserService: Error setting EmailConfirmed and LockoutEnabled.");
            }
        }

        /// <summary>
        /// Generates a random password
        /// </summary>
        /// <returns>A random string</returns>
        private static string GenerateRandomPassword()
        {
            var rand = new Random();
            const string bigAlphas = "ABCDEFGHIJKLMNOPQRSTUVWXY";
            const string smallAlphas = "abcdefghjlkmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string symbols = "!@#$%^&*()_+";
            var pool = $"{bigAlphas}{smallAlphas}{numbers}{symbols}";

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
