using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Babou.API.Web.Helpers.Swagger
{
    public class RawTextRequestOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!(context.MethodInfo.GetCustomAttributes(true)
                .SingleOrDefault((attribute) => attribute is RawTextRequestAttribute) is RawTextRequestAttribute rawTextRequestAttribute)) 
                return;

            operation.RequestBody = new OpenApiRequestBody();
            operation.RequestBody.Content.Add(rawTextRequestAttribute.MediaType, new OpenApiMediaType()
            {
                Schema = new OpenApiSchema()
                {
                    Type = "string"
                }
            });
        }
    }
}
