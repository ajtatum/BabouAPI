using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Babou.API.Web.Data;
using Babou.API.Web.Helpers;
using Babou.API.Web.Helpers.Filters;
using Babou.API.Web.Models;
using Babou.API.Web.Services.Interfaces;
using Babou.API.Web.SwaggerExamples.Responses;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Babou.API.Web.Areas.API
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UrlShortenerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUrlShortenerService _urlShortenerService;
        private readonly ILogger<UrlShortenerController> _logger;
        private readonly AppSettings _appSettings;
        private readonly IUserService _userService;

        public UrlShortenerController(ApplicationDbContext context, IUrlShortenerService urlShortenerService,
            ILogger<UrlShortenerController> logger, IOptionsMonitor<AppSettings> appSettings, IUserService userService)
        {
            _context = context;
            _urlShortenerService = urlShortenerService;
            _logger = logger;
            _appSettings = appSettings.CurrentValue;
            _userService = userService;
        }

        /// <param name="longUrl">The URL you wish to shorten</param>
        /// <param name="domain">The domain to use. BabouIo links to https://s.babou.io/, MrvlCo links to https://mrvl.co/, and XMenTo links to https://xmen.to. Default is BabouIo</param>
        /// <param name="token">What gets appended to your short url. Use your own or let the app generate a random one.</param>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost]
        [Produces(Constants.ContentTypes.TextPlain)]
        [SwaggerOperation(
            Summary = "Converts a long URL to a short URL.",
            Description = "You can try and specify your own token (what gets appended to the domain) or let the application generate a token.",
            OperationId = "CreateShortUrl")]
        [SwaggerResponse(StatusCodes.Status200OK, "Sucessfully created a short URL.", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid domain, token taken, or there was an error generating a random token. Otherwise see exception message.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestStringResponseExample))]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable, "AuthKey not found.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(AuthKeyNotFoundResponseExample))]
        public async Task<IActionResult> Post([Required][FromHeader] string longUrl, [Optional][FromHeader] Domains? domain, [Optional][FromHeader] string token)
        {
            var userAuthKey = Request.Headers["AuthKey"].ToString();

            try
            {
                var applicationUser = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.ApiAuthKey == userAuthKey);

                if (applicationUser == null)
                {
                    _logger.LogWarning("UrlShortenerController: Error finding user with the AutKey {AuthKey}", userAuthKey);
                    return new BadRequestObjectResult($"Error finding user with the AuthKey {userAuthKey}.");
                }

                if (domain == null)
                {
                    domain = Domains.BabouIo;
                }

                var domainString = domain.Value.GetAttributeOfType<DescriptionAttribute>().Description;

                var allowedDomains = Enum<Domains>.GetListByDescription();

                if (!allowedDomains.Contains(domainString))
                {
                    return new BadRequestObjectResult($"Domain not available. Please choose from {string.Join(", ", allowedDomains)}.");
                }

                var tokenTaken = true;

                if (!token.IsNullOrWhiteSpace())
                {
                    try
                    {
                        tokenTaken = await _urlShortenerService.CheckIfTokenIsAvailable(token, domainString);
                    }
                    catch (DuplicateNameException dne)
                    {
                        _logger.LogError(dne, "UrlShortenerController: Token {Token} is already being used by domain {Domain}. Requested by {UserId}.", token, domain, applicationUser.Id);
                        return new BadRequestObjectResult(dne.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "UrlShortenerController: Error while trying to create custom token {Token} for domain {Domain}. Requested by {UserId}.", token, domain, applicationUser.Id);
                        return new BadRequestObjectResult(ex.Message);
                    }
                }

                var shortenedUrl = tokenTaken
                    ? await _urlShortenerService.CreateByUserId(applicationUser.Id, longUrl, domainString)
                    : await _urlShortenerService.CreateByUserId(applicationUser.Id, longUrl, domainString, token);

                _logger.LogInformation("UrlShortenerController: New Shortened Url Created for {LongUrl} as {ShortUrl}. Requested by {UserId}.", shortenedUrl.LongUrl, shortenedUrl.ShortUrl, applicationUser.Id);

                return new OkObjectResult(shortenedUrl.ShortUrl);
            }
            catch (Exception ex)
            {
                var domainString = domain.HasValue ? domain.Value.GetAttributeOfType<EnumMemberAttribute>().Value : "DomainError";
                _logger.LogError(ex, "UrlShortenerController: Error creating short url for {LongUrl}, {Domain} and {Token}. Requested by {AuthKey}", longUrl, domainString, token, userAuthKey);

                return new BadRequestObjectResult(ex.Message);
            }
        }

        /// <summary>
        /// Creates or uses an existing account with the provided email address and creates a short URL.
        /// <para>Once you've used this method, you can log into babou.io and do "Forgot Password" to login
        /// and retrieve your AuthKey and have more control over your short url creation.
        /// </para>
        /// </summary>
        /// <param name="longUrl"></param>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost("marvel")] 
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> CreateMarvelShortUrl([Required] [FromHeader][Url] string longUrl, [Required] [FromHeader][EmailAddress] string emailAddress)
        {
            var userAuthKey = Request.Headers["AuthKey"].ToString();

            try
            {
                var authKeyUser = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.ApiAuthKey == userAuthKey);

                var applicationUser = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == emailAddress) 
                                      ?? await _userService.QuickCreateUser(emailAddress, null, emailAddress, null);

                if(applicationUser == null)
                    return new BadRequestObjectResult($"Error finding or creating user with email address {emailAddress}");

                var domainString = Domains.MrvlCo.GetAttributeOfType<EnumMemberAttribute>().Value;

                var shortenedUrl = await _urlShortenerService.CreateByUserId(applicationUser.Id, longUrl, domainString);

                _logger.LogInformation("UrlShortenerController: CreateMarvelShortUrl - New Shortened Url Created for {LongUrl} as {ShortUrl}. Requested by {UserId} via {AuthKeyUserId}.", shortenedUrl.LongUrl, shortenedUrl.ShortUrl, applicationUser.Id, authKeyUser.Id);

                return new OkObjectResult(shortenedUrl.ShortUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UrlShortenerController: CreateMarvelShortUrl - Error creating shortened url from email address.");
                return new BadRequestObjectResult($"There was an error processing your request. Error: {ex.Message}");
            }
        }
    }
}
