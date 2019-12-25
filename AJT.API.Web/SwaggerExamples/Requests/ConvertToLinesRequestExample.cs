using System.Text;
using Swashbuckle.AspNetCore.Filters;

namespace Babou.API.Web.SwaggerExamples.Requests
{
    public class ConvertToLinesRequestExample : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Item 1, Item 2, Item 2, Item 3, Item 4, Item 5, Item 6");
            sb.AppendLine("\tA tabed item");
            return sb.ToString();
        }
    }
}
