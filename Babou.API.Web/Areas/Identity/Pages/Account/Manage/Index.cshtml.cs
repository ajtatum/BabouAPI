using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Babou.API.Web.Helpers.ExtensionMethods;
using Babou.API.Web.Models.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BabouExtensions;
using Microsoft.Extensions.Logging;

namespace Babou.API.Web.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [ViewData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please provide a username.")]
            public string Username { get; set; }

            [Display(Name="Your Name")]
            [Required(ErrorMessage = "Please provide your name.")]
            public string FullName { get; set; }

            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }
            
            [Required]
            [Display (Name = "Api Key")]
            public string ApiKey { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var fullName = await _userManager.GetFullNameAsync();
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var apiAuthKey = await _userManager.GetApiAuthKeyAsync();

            Input = new InputModel
            {
                Username = userName,
                FullName = fullName,
                PhoneNumber = phoneNumber,
                ApiKey = apiAuthKey
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var statusMessage = Request.Query["statusMessage"].ToString();

            if (!statusMessage.IsNullOrWhiteSpace())
            {
                StatusMessage = statusMessage;
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                if (!ModelState.IsValid)
                {
                    await LoadAsync(user);
                    return Page();
                }

                var username = await _userManager.GetUserNameAsync(user);
                if (Input.Username != username)
                {
                    var setUserNameResult = await _userManager.SetUserNameAsync(user, Input.Username);
                    if (!setUserNameResult.Succeeded)
                    {
                        return RedirectToPage(new { statusMessage = "Error: That username has already been taken." });
                    }
                }

                var fullName = await _userManager.GetFullNameAsync();
                if (Input.FullName != fullName)
                {
                    var setFullName = await _userManager.SetFullNameAsync(Input.FullName);

                    if (!setFullName)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        throw new InvalidOperationException($"Unexpected error occurred setting Full Name for user with ID '{userId}'.");
                    }
                }


                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (Input.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                    }
                }

                await _signInManager.RefreshSignInAsync(user);
                return RedirectToPage(new { statusMessage = "Your profile has been updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Saving User Profile. {Message}", ex.Message);
                return RedirectToPage(new { statusMessage = $"Error: {ex.Message}" });
            }
        }
    }
}
