using System.Text;
using Swashbuckle.AspNetCore.Filters;

namespace AJT.API.Web.SwaggerExamples.Requests
{
    public class ConvertToCsvExample : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Line 1");
            sb.AppendLine("Line 1");
            sb.AppendLine("      Line 2    ");
            sb.AppendLine("Line 3");
            sb.AppendLine("Line 3");
            sb.AppendLine("Line 4");
            sb.AppendLine("Line 5");
            sb.AppendLine("Line 6");
            sb.AppendLine("1");

            return sb.ToString();
        }
    }
}
