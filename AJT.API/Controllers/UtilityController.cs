using System.Text;
using System.Threading.Tasks;
using AJT.API.Helpers;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
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

        public UtilityController(ILogger<UtilityController> logger)
        {
            _logger = logger;
        }

        [ServiceFilter(typeof(AuthKeyAuthorize))]
        [HttpGet]
        public async Task<IActionResult> ConvertIntToList()
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

        [ServiceFilter(typeof(AuthKeyAuthorize))]
        [HttpGet]
        public async Task<IActionResult> ConvertStringToList()
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

        [ServiceFilter(typeof(AuthKeyAuthorize))]
        [HttpGet]
        public IActionResult Encrypt()
        {
            _logger.LogInformation("Function Encrypt processed a request.");

            var originalValue = Request.Headers["OriginalValue"].ToString();
            var encryptionKey = Request.Headers["EncryptionKey"].ToString();

            return new OkObjectResult(originalValue.Encrypt(encryptionKey));
        }

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