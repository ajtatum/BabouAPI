using System;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AJT.API.Web.SwaggerHelpers
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo()
                    {
                        Title = $"AJT API {description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                        Contact = new OpenApiContact
                        {
                            Name = "AJ Tatum",
                            Email = "aj@ajt.io",
                            Url = new Uri("https://ajt.io")
                        },
                    });
            }
        }
    }
}
