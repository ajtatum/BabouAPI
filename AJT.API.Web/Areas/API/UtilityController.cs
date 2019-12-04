using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AJT.API.Web.Helpers.Filters;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AJT.API.Web.Areas.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class UtilityController : ControllerBase
    {
        /// <summary>
        /// Converts a string of integers into a CSV. Returns distinct values.
        /// </summary>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpGet("{surroundWithQuotes:bool=false}")]
        [Consumes("text/plain")]
        [Produces("text/plain")]
        public async Task<IActionResult> ConvertToCsv([Optional] bool surroundWithQuotes)
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
        /// Takes a list of strings or integers and returns distinct values in new lines.
        /// </summary>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpGet]
        [Consumes("text/plain")]
        [Produces("text/plain")]
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
        [HttpGet]
        public IActionResult Encrypt([Required][FromHeader] string originalString, [Required][FromHeader] string encryptionKey)
        {
            return new OkObjectResult(originalString.Encrypt(encryptionKey));
        }

        /// <summary>
        /// Decrypts a string using the header values OriginalValue and DecryptionKey
        /// </summary>
        /// <param name="encryptedString">The string you wish to encrypt.</param>
        /// <param name="decryptionKey">Your key you use to decrypt the encrypted string..</param>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpGet]
        public IActionResult Decrypt([Required][FromHeader]string encryptedString, [Required][FromHeader] string decryptionKey)
        {
            return new OkObjectResult(encryptedString.Decrypt(decryptionKey));
        }
    }
}