using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Babou.API.Web.Helpers;
using Babou.API.Web.Helpers.Filters;
using Babou.API.Web.Helpers.Swagger;
using Babou.API.Web.SwaggerExamples.Requests;
using Babou.API.Web.SwaggerExamples.Responses;
using BabouExtensions;
using BabouExtensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Babou.API.Web.Areas.API
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class UtilityController : ControllerBase
    {
        private readonly ILogger<UtilityController> _logger;

        public UtilityController(ILogger<UtilityController> logger)
        {
            _logger = logger;
        }

        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost]
        [RawTextRequest]
        [Consumes(Constants.ContentTypes.TextPlain)]
        [Produces(Constants.ContentTypes.TextPlain)]
        [SwaggerOperation(
            Summary = "Converts strings or integers into CSV.",
            Description = "Looks at the raw request body. Request body can contain tab and line breaks.",
            OperationId = "ConvertToCsvNull")]
        [SwaggerRequestExample(typeof(string), typeof(ConvertToCsvNullRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, "Converted strings to distinct CSV values.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ConvertToCsvNullResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Either the request body is malformed or an exception was thrown.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestStringResponseExample))]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable, "AuthKey not found.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(AuthKeyNotFoundResponseExample))]
        public Task<IActionResult> ConvertToCsv()
        {
            return ConvertToCsv(null, true);
        }

        /// <param name="surroundWithQuotes">If true, the strings are surrounded by quotes. If false, they are not.</param>
        /// <param name="onlyDistinctValues">Determines if you want to return distinct values or not.</param>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost("{surroundWithQuotes:bool?}/{onlyDistinctValues:bool}")]
        [RawTextRequest]
        [Consumes(Constants.ContentTypes.TextPlain)]
        [Produces(Constants.ContentTypes.TextPlain)]
        [SwaggerOperation(
            Summary = "Converts strings or integers into CSV and returns distinct values.",
            Description = "Looks at the raw request body. Request body can contain tab and line breaks.",
            OperationId = "ConvertToCsvSurroundWithQuotes")]
        [SwaggerRequestExample(typeof(string), typeof(ConvertToCsvRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, "Converted strings to distinct CSV values.", typeof(List<string>))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ConvertToCsvResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Either the request body is malformed or an exception was thrown.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestStringResponseExample))]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable, "AuthKey not found.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(AuthKeyNotFoundResponseExample))]
        public async Task<IActionResult> ConvertToCsv([Required] bool? surroundWithQuotes, [Required] bool onlyDistinctValues)
        {
            var requestBody = await Request.GetRawBodyStringAsync();

            if (!requestBody.Contains(','))
            {
                requestBody = requestBody.Replace('\r', ',');
                requestBody = requestBody.Replace('\n', ',');
                requestBody = requestBody.Trim(',');
            }

            try
            {
                var returnValue = requestBody.ConvertToCsv(',', ',', surroundWithQuotes, true, " ", onlyDistinctValues);

                return new OkObjectResult(returnValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ConvertToCsv with surroundWithQuotes being {SurroundWithQuotes} and onlyDistinctValues bring {OnlyDistinctValues}", surroundWithQuotes, onlyDistinctValues);
                return new BadRequestObjectResult($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Takes a list of strings or integers separated by a comma and returns distinct values in new lines.
        /// </summary>
        /// <returns></returns>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost]
        [RawTextRequest]
        [Consumes(Constants.ContentTypes.TextPlain)]
        [Produces(Constants.ContentTypes.TextPlain)]
        [SwaggerOperation(
            Summary = "Converts CSV, tabs, and line breaks and returns distinct values on each line.",
            Description = "Looks at the raw request body.",
            OperationId = "ConvertToLines")]
        [SwaggerRequestExample(typeof(string), typeof(ConvertToLinesRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, "Converted strings to distinct new lines.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ConvertToLinesResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Either the request body is malformed or an exception was thrown.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestStringResponseExample))]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable, "AuthKey not found.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(AuthKeyNotFoundResponseExample))]
        public async Task<IActionResult> ConvertToLines()
        {
            try
            {
                var requestBody = await Request.GetRawBodyStringAsync();

                if (requestBody.TryGetList(',', out var cleanString))
                {
                    var stringReturn = string.Join(Environment.NewLine, cleanString);

                    return new OkObjectResult($"{stringReturn}");
                }

                return new BadRequestObjectResult($"Error: Bad request body: {requestBody}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ConvertToLines");
                return new BadRequestObjectResult($"Exception thrown: {ex.Message}.");
            }
        }

        /// <param name="DecryptedString">The string you wish to encrypt.</param>
        /// <param name="Key">Your encryption key.</param>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost]
        [Consumes(Constants.ContentTypes.TextPlain)]
        [Produces(Constants.ContentTypes.TextPlain)]
        [SwaggerOperation(
            Summary = "Encrypts a string using the header values decryptedString and key",
            OperationId = "EncryptString")]
        [SwaggerResponse(StatusCodes.Status200OK, "Encryption successful.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Either the paramters are empty or an exception was thrown.", typeof(BadRequestStringResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestStringResponseExample))]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable, "AuthKey not found.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(AuthKeyNotFoundResponseExample))]
        public IActionResult Encrypt([Required][FromHeader] string DecryptedString, [Required][FromHeader] string Key)
        {
            try
            {
                return new OkObjectResult(DecryptedString.EncryptUsingAes(Key));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EncryptString");
                return new BadRequestObjectResult($"Exception thrown: {ex.Message}.");
            }
        }

        /// <param name="EncryptedString">The string you wish to encrypt.</param>
        /// <param name="Key">Your key you use to decrypt the encrypted string..</param>
        [ServiceFilter(typeof(AuthKeyFilter))]
        [HttpPost]
        [Consumes(Constants.ContentTypes.TextPlain)]
        [Produces(Constants.ContentTypes.TextPlain)]
        [SwaggerOperation(
            Summary = "Decrypts a string using the header values encryptedString and key",
            OperationId = "DecryptString")]
        [SwaggerResponse(StatusCodes.Status200OK, "Decryption successful.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Either the paramters are empty or an exception was thrown.", typeof(BadRequestStringResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestStringResponseExample))]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable, "AuthKey not found.", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status406NotAcceptable, typeof(AuthKeyNotFoundResponseExample))]
        public IActionResult Decrypt([Required][FromHeader]string EncryptedString, [Required][FromHeader] string Key)
        {
            try
            {
                return new OkObjectResult(EncryptedString.DecryptUsingAes(Key));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DecryptString");
                return new BadRequestObjectResult($"Exception thrown: {ex.Message}.");
            }
        }
    }
}