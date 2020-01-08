using System;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Babou.API.Web.SwaggerHelpers
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
                        Title = "Babou API",
                        Version = description.ApiVersion.ToString(),
                        Contact = new OpenApiContact
                        {
                            Name = "Babou API",
                            Email = null,
                            Url = new Uri("/", UriKind.Relative)
                        },
                        Description = "IP Address services, PushBullet service, Url Shortening service, utilities and more to come.",
                        TermsOfService = new Uri("/TermsOfService", UriKind.Relative),
                        License = new OpenApiLicense()
                        {
                            Name = "Apache License", 
                            Url = new Uri("/License", UriKind.Relative)
                        }
                    });
            }
        }
    }
}
