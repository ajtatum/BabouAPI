using System;
using System.Text;
using System.Threading.Tasks;
using AJT.API.Helpers;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AJT.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class UtilityController : ControllerBase
    {
        private readonly ILogger<UtilityController> _logger;
        private readonly IHttpContextAccessor _accessor;

        public UtilityController(ILogger<UtilityController> logger, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _accessor = accessor;
        }

        /// <summary>
        /// Converts a string of integers into a CSV. Returns distinct values.
        /// </summary>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyAuthorize))]
        [HttpGet]
        public async Task<IActionResult> ConvertIntToCsv()
        {
            _logger.LogInformation("ConvertIntToList processed a request.");

            var requestBody = await Request.GetRawBodyStringAsync();

            if (requestBody.TryGetList(',', out var cleanString))
            {
                var stringReturn = string.Join(',', cleanString);

                return new OkObjectResult($"{stringReturn}");
            }

            return new BadRequestObjectResult($"{requestBody}");
        }

        /// <summary>
        /// Converts a string into a CSV where each item is surrounded by a quote. Returns distinct values.
        /// </summary>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyAuthorize))]
        [HttpGet]
        public async Task<IActionResult> ConvertStringToCsv()
        {
            _logger.LogInformation("ConvertStringToList processed a request.");

            var requestBody = await Request.GetRawBodyStringAsync();

            if (requestBody.TryGetList(',', out var cleanString))
            {
                var stringReturn = string.Empty;

                cleanString.ForEach(x => { stringReturn += $"'{x}',"; });

                stringReturn = stringReturn.TrimEnd(',');

                return new OkObjectResult($"{stringReturn}");
            }

            return new BadRequestObjectResult($"{requestBody}");
        }

        /// <summary>
        /// Takes a list of strings or integers and returns distinct values in new lines.
        /// </summary>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyAuthorize))]
        [HttpGet]
        public async Task<IActionResult> ConvertToLines()
        {
            _logger.LogInformation("ConvertToLines processed a request.");

            var requestBody = await Request.GetRawBodyStringAsync();

            if (requestBody.TryGetList(',', out var cleanString))
            {
                var stringReturn = string.Join(Environment.NewLine, cleanString);

                return new OkObjectResult($"{stringReturn}");
            }

            return new BadRequestObjectResult($"{requestBody}");
        }

        /// <summary>
        /// Encrypts a string using the header values OriginalValue and EncryptionKey
        /// </summary>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyAuthorize))]
        [HttpGet]
        public IActionResult Encrypt()
        {
            _logger.LogInformation("Function Encrypt processed a request.");

            var originalValue = Request.Headers["OriginalValue"].ToString();
            var encryptionKey = Request.Headers["EncryptionKey"].ToString();

            return new OkObjectResult(originalValue.Encrypt(encryptionKey));
        }

        /// <summary>
        /// Decrypts a string using the header values OriginalValue and DecryptionKey
        /// </summary>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyAuthorize))]
        [HttpGet]
        public IActionResult Decrypt()
        {
            _logger.LogInformation("Function Decrypt processed a request.");

            var originalValue = Request.Headers["OriginalValue"].ToString();
            var decryptionKey = Request.Headers["DecryptionKey"].ToString();

            return new OkObjectResult(originalValue.Decrypt(decryptionKey));
        }
    }
}