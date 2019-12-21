using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AJT.API.Web.Helpers;
using AJT.API.Web.Helpers.Filters;
using AJT.API.Web.Helpers.Swagger;
using AJT.API.Web.Models;
using AJT.API.Web.SwaggerExamples.Requests;
using BabouExtensions;
using BabouExtensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace AJT.API.Web.Areas.API
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class UtilityController : ControllerBase
    {
        /// <summary>
        /// Converts a string of integers into a CSV. Returns distinct values.
        /// </summary>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost("{surroundWithQuotes:bool=true}")]
        [Consumes(Constants.ContentTypes.TextPlain)]
        [Produces(Constants.ContentTypes.TextPlain)]
        [SwaggerRequestExample(typeof(string), typeof(ConvertToCsvExample))]
        [RawTextRequest]
        public async Task<IActionResult> ConvertToCsv(bool surroundWithQuotes = true)
        {
            var requestBody = await Request.GetRawBodyStringAsync();

            if (requestBody.TryGetList(',', out var cleanString))
            {
                var returnValue = string.Empty;

                if (surroundWithQuotes == false)
                    returnValue = string.Join(',', cleanString);
                else
                {
                    cleanString.ForEach(x => { returnValue += $"'{x}',"; });

                    returnValue = returnValue.TrimEnd(',');
                }

                return new OkObjectResult($"{returnValue}");
            }

            return new BadRequestObjectResult($"{requestBody}");
        }

        /// <summary>
        /// Takes a list of strings or integers separated by a comma and returns distinct values in new lines.
        /// </summary>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost]
        [Consumes(Constants.ContentTypes.TextPlain)]
        [Produces(Constants.ContentTypes.TextPlain)]
        [RawTextRequest]
        public async Task<IActionResult> ConvertToLines()
        {
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
        /// <param name="originalString">The string you wish to encrypt.</param>
        /// <param name="encryptionKey">Your encryption key.</param>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost]
        [Consumes(Constants.ContentTypes.TextPlain)]
        [Produces(Constants.ContentTypes.TextPlain)]
        public IActionResult Encrypt([Required][FromHeader] string originalString, [Required][FromHeader] string encryptionKey)
        {
            return new OkObjectResult(originalString.EncryptUsingAes(encryptionKey));
        }

        /// <summary>
        /// Decrypts a string using the header values OriginalValue and DecryptionKey
        /// </summary>
        /// <param name="encryptedString">The string you wish to encrypt.</param>
        /// <param name="decryptionKey">Your key you use to decrypt the encrypted string..</param>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost]
        [Consumes(Constants.ContentTypes.TextPlain)]
        [Produces(Constants.ContentTypes.TextPlain)]
        public IActionResult Decrypt([Required][FromHeader]string encryptedString, [Required][FromHeader] string decryptionKey)
        {
            return new OkObjectResult(encryptedString.DecryptUsingAes(decryptionKey));
        }
    }
}