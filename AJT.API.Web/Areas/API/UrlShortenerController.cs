using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AJT.API.Web.Data;
using AJT.API.Web.Helpers.Filters;
using AJT.API.Web.Models.Database;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AJT.API.Web.Areas.API
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class UrlShortenerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UrlShortenerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost]
        public async Task<IActionResult> Post([Required][FromQuery] string longUrl, [Optional][FromQuery] string token)
        {
            var userAuthKey = Request.Headers["AuthKey"].ToString();
            var applicationUser = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.ApiAuthKey == userAuthKey);

            if (token.IsNullOrWhiteSpace())
                token = null;

            if (token != null)
            {
                var tokenTaken = await _context.ShortenedUrls.FirstOrDefaultAsync(x => x.Id == token);
                if (tokenTaken != null)
                    return new BadRequestObjectResult($"The token {token} has already been reserved.");
            }

            var shortenUrl = new ShortenedUrl()
            {
                Id = token ?? GenerateToken(),
                LongUrl = longUrl,
                CreatedBy = applicationUser.Id,
                CreatedOn = DateTime.Now,
            };
            shortenUrl.ShortUrl = $"https://api.ajt.io/go/{shortenUrl.Id}";
            _context.Add(shortenUrl);
            await _context.SaveChangesAsync();

            return new OkObjectResult(shortenUrl.ShortUrl);
        }

        private string GenerateToken()
        {
            var urlSafe = string.Empty;
            Enumerable.Range(48, 75)
                .Where(i => i < 58 || i > 64 && i < 91 || i > 96)
                .OrderBy(o => new Random().Next())
                .ToList()
                .ForEach(i => urlSafe += Convert.ToChar(i));

            return urlSafe.Substring(new Random().Next(0, urlSafe.Length), new Random().Next(2, 8));;
        }
    }
}
