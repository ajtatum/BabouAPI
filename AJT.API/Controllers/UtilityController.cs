using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AJT.API.Helpers.Filters;
using BabouExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AJT.API.Controllers
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
        public async Task<IActionResult> ConvertToCsv([Optional] bool surroundWithQuotes)
        {
            var requestBody = await Request.GetRawBodyStringAsync();

            if (requestBody.TryGetList(',', out var cleanString))
            {
                var returnValue = string.Empty;

                if(surroundWithQuotes == false)
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
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpGet]
        public IActionResult Encrypt()
        {
            var originalValue = Request.Headers["OriginalValue"].ToString();
            var encryptionKey = Request.Headers["EncryptionKey"].ToString();

            return new OkObjectResult(originalValue.Encrypt(encryptionKey));
        }

        /// <summary>
        /// Decrypts a string using the header values OriginalValue and DecryptionKey
        /// </summary>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpGet]
        public IActionResult Decrypt()
        {
            var originalValue = Request.Headers["OriginalValue"].ToString();
            var decryptionKey = Request.Headers["DecryptionKey"].ToString();

            return new OkObjectResult(originalValue.Decrypt(decryptionKey));
        }
    }
}