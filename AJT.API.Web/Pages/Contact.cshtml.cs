using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Babou.API.Web.Helpers.Attributes;
using Babou.API.Web.Helpers.ExtensionMethods;
using Babou.API.Web.Models.Database;
using Babou.API.Web.Services.Interfaces;
using BabouExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Babou.API.Web.Pages
{
    public class ContactModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public ContactModel(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [ViewData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please provide your email address.")]
            [EmailAddress]
            [Display(Name = "Your Email")]
            public string FromEmail { get; set; }

            [Required(ErrorMessage = "Please provide your name.")]
            [Display(Name = "Your Name")]
            [NoHtml(ErrorMessage = "HTML is not allowed in the name field.")]
            public string FromName { get; set; }
            
            [Required]
            [NoHtml(ErrorMessage = "HTML is not allowed in the subject field.")]
            public string Subject { get; set; }
            
            [Required(ErrorMessage = "Please tell us why you're contacting us.")]
            [NoHtml(ErrorMessage = "HTML is not allowed in the message field.")]
            public string Message { get; set; }
        }

        public async Task<IActionResult> OnGet()
        {
            var statusMessage = Request.Query["statusMessage"].ToString();

            if (!statusMessage.IsNullOrWhiteSpace())
            {
                StatusMessage = statusMessage;
            }

            var user = await _userManager.GetCurrentUserAsync();
            if (user != null)
            {
                Input = new InputModel
                {
                    FromEmail = user.Email,
                    FromName = "",
                    Subject = "",
                    Message = ""
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = "Error: Please review the errors below.";
                return Page();
            }


            await _emailService.SendContactMessage(Input.FromEmail, Input.FromName, Input.Subject, Input.Message.StripHtml());

            return RedirectToPage(new {statusMessage = "Your message has been sent!"});
        }
    }
}