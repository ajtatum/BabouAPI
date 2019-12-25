using Swashbuckle.AspNetCore.Filters;

namespace Babou.API.Web.SwaggerExamples.Responses
{
    public class ConflictErrorResponseExample : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            return "There was a conflict with the request you sent.";
        }
    }
}
