using Swashbuckle.AspNetCore.Filters;

namespace AJT.API.Web.SwaggerExamples.Responses
{
    public class ConvertToCsvNullResponseExample : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            return "'Line 1','Line 2','Line 3','Line 4','Line 5','Line 6',1";
        }
    }
}
