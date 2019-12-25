using System.Text;
using Swashbuckle.AspNetCore.Filters;

namespace Babou.API.Web.SwaggerExamples.Responses
{
    public class ConvertToLinesResponseExample : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Item 1");
            sb.AppendLine("Item 2");
            sb.AppendLine("Item 3");
            sb.AppendLine("Item 4");
            sb.AppendLine("Item 5");
            sb.AppendLine("Item 6");
            sb.AppendLine("A tabed item");
            return sb.ToString();
        }
    }
}
