using Swashbuckle.AspNetCore.Filters;

namespace Babou.API.Web.SwaggerExamples.Responses
{
    public class BadRequestStringResponseExample : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            return "Either the request body is malformed or an exception was thrown.";
        }
    }
}
