using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AJT.API.Web.Data;
using AJT.API.Web.Models.Database;
using AJT.API.Web.Models.Services;
using AJT.API.Web.Services.Interfaces;
using BabouExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AJT.API.Web.Areas.Identity.Pages.Account.Manage
{
    public class PushBulletModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICipherService _cipherService;

        public PushBulletModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICipherService cipherService)
        {
            _context = context;
            _userManager = userManager;
            _cipherService = cipherService;
        }
        
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name="Enable PushBullet Service")]
            public bool Enable { get; set; }

            [Display(Name = "Api Key")]
            public string ApiKey { get; set; }

            [Display(Name = "Encryption Key")]
            public string EncryptionKey { get; set; }
            public string HiddenEncryptedApiKey { get; set; }
            public string HiddenEncryptedEncryptionKey { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var pushBulletService = await _context.ApplicationUserServices.FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id && x.ApplicationServiceId == 1);

            var pushBulletSettings = pushBulletService != null
                ? JsonConvert.DeserializeObject<PushBulletSettings>(pushBulletService.ApplicationSettings)
                : new PushBulletSettings() {ApiKey = string.Empty, EncryptionKey = string.Empty};

            Input = new InputModel
            {
                Enable = pushBulletService != null,
                ApiKey = pushBulletSettings.ApiKey,
                EncryptionKey = pushBulletSettings.EncryptionKey,
                HiddenEncryptedApiKey = pushBulletSettings.ApiKey,
                HiddenEncryptedEncryptionKey = pushBulletSettings.EncryptionKey
            };
        }


        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Error: Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Error: Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (Input.Enable)
            {
                if (Input.Enable && Input.ApiKey.IsNullOrWhiteSpace())
                    ModelState.AddModelError("ApiKey", "Api Key is required.");

                if (Input.Enable && Input.EncryptionKey.IsNullOrWhiteSpace())
                    ModelState.AddModelError("EncryptionKey", "Encryption Key is required.");

                if (!ModelState.IsValid)
                {
                    StatusMessage = "Error: Please review the errors below.";
                    return Page();
                }

                var encryptedApiKey = Input.HiddenEncryptedApiKey;
                var encryptedEncryptionKey = Input.HiddenEncryptedEncryptionKey;

                if (Input.HiddenEncryptedApiKey != Input.ApiKey)
                {
                    encryptedApiKey = _cipherService.Encrypt(Input.ApiKey);
                }

                if (Input.HiddenEncryptedEncryptionKey != Input.EncryptionKey)
                {
                    encryptedEncryptionKey = _cipherService.Encrypt(Input.EncryptionKey);
                }

                var pushBulletSettings = new PushBulletSettings()
                {
                    ApiKey = encryptedApiKey,
                    EncryptionKey = encryptedEncryptionKey
                };

                var pushBulletSettingsString = JsonConvert.SerializeObject(pushBulletSettings);
                var pushBulletService = await _context.ApplicationUserServices.FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id && x.ApplicationServiceId == 1);
                if (pushBulletService == null)
                {
                    pushBulletService = new ApplicationUserService()
                    {
                        ApplicationUserId = user.Id,
                        ApplicationServiceId = 1,
                        ApplicationSettings = pushBulletSettingsString
                    };
                    _context.Add(pushBulletService);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    pushBulletService.ApplicationSettings = pushBulletSettingsString;
                    await _context.SaveChangesAsync();
                }
                
                StatusMessage = "Your PushBullet Settings have been encrypted and updated!";
            }
            else
            {
                var pushBulletService = await _context.ApplicationUserServices.FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id && x.ApplicationServiceId == 1);
                _context.Remove(pushBulletService);
                await _context.SaveChangesAsync();

                StatusMessage = "Your PushBullet integration has been removed.";
            }

            return RedirectToPage();
        }
    }
}
