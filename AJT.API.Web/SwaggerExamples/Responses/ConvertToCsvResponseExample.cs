using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;

namespace Babou.API.Web.SwaggerExamples.Responses
{
    public class ConvertToCsvResponseExample : IExamplesProvider<List<string>>
    {
        public List<string> GetExamples()
        {
            return new List<string> {"'Line 1','Line 2','Line 3','Line 4','Line 5','Line 6','1'", "Line 1,Line 2,Line 3,Line 4,Line 5,Line 6,1"};
        }
    }
}
