using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Babou.API.Web.Models.Database;
using Babou.API.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Babou.API.Web.Pages.UrlShortener
{
    public class ManageModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ManageModel> _logger;
        private readonly IUrlShortenerService _urlShortenerService;

        public ManageModel(UserManager<ApplicationUser> userManager, ILogger<ManageModel> logger,
            IUrlShortenerService urlShortenerService)
        {
            _userManager = userManager;
            _logger = logger;
            _urlShortenerService = urlShortenerService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public List<ShortenedUrl> ShortenedUrls { get; set; }

        [BindProperty]
        public ShortenedUrl ShortenedUrl { get; set; }

        [BindProperty]
        public int ClickTimeFrame { get; set; }

        public async Task<IActionResult> OnGet(int? clickTimeFrame)
        {
            if (clickTimeFrame.HasValue && clickTimeFrame < -90)
                clickTimeFrame = -90;

            ClickTimeFrame = clickTimeFrame ?? -30;

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Error: Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            ShortenedUrls = await _urlShortenerService.GetShortenedUrlsByUserId(user.Id);

            return Page();
        }

        public async Task<IActionResult> OnPostUpdate(int id)
        {
            try
            {
                var shortenedUrl = await _urlShortenerService.UpdateById(id, ShortenedUrl.LongUrl);
                StatusMessage = $"{shortenedUrl.ShortUrl} has been updated!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnPostUpdate: Unable to find ShortenedUrl {Id}", id);
                StatusMessage = $"Error: Something went wrong updating {id}.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            try
            {
                var shortenedUrl = await _urlShortenerService.DeleteById(id);
                StatusMessage = $"{shortenedUrl.ShortUrl} has been deleted!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnPostDelete: Unable to find ShortenedUrl {Id}", id);
                StatusMessage = $"Error: Something went wrong retrieving Shortened Url Token {id}.";
                return Page();
            }
        }
    }
}