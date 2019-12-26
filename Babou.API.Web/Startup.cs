using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json.Serialization;
using Babou.API.Web.Helpers.ExtensionMethods;
using Babou.API.Web.Helpers.Filters;
using Babou.API.Web.Helpers.Swagger;
using Babou.API.Web.Models;
using Babou.API.Web.Services;
using Babou.API.Web.Services.Interfaces;
using Babou.API.Web.SwaggerHelpers;
using Babou.AspNetCore.SecurityExtensions.ContentSecurityPolicy;
using Babou.AspNetCore.SecurityExtensions.CustomHeaders;
using Babou.AspNetCore.SecurityExtensions.ExpectCT;
using Babou.AspNetCore.SecurityExtensions.FeaturePolicy;
using Babou.AspNetCore.SecurityExtensions.FrameOptions;
using Babou.AspNetCore.SecurityExtensions.ReferrerPolicy;
using Babou.AspNetCore.SecurityExtensions.XContentTypeOptions;
using Babou.AspNetCore.SecurityExtensions.XRobotsTag;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Babou.API.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; set; }

        public static IConfigurationRoot ConfigurationRoot => (IConfigurationRoot)Configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            var azureStorageAccountConnection = Configuration["Azure:Storage:ConnectionString"];
            var cloudStorageAccount = CloudStorageAccount.Parse(azureStorageAccountConnection);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var container = cloudBlobClient.GetContainerReference(Configuration["Azure:Storage:DataProtectionContainer"]);

            services.AddDataProtection()
                .SetApplicationName("Babou API Web App")
                .PersistKeysToAzureBlobStorage(container, $"{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}/dataprotectionkeys.xml")
                .ProtectKeysWithAzureKeyVault(Configuration["Azure:KeyVault:EncryptionKey"], Configuration["Azure:KeyVault:ClientId"], Configuration["Azure:KeyVault:ClientSecret"]);

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.Always;
            });

            services.AddMvc()
                .AddNewtonsoftJson(x =>
                {
                    x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    x.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddRazorPages()
                .AddNewtonsoftJson(x =>
                {
                    x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    x.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                .AddRazorRuntimeCompilation();

            services.Configure<AppSettings>(Configuration);

            services.AddScoped<ClientIpFilter>();
            services.AddScoped<AuthKeyFilter>();
            services.AddScoped<IIpService, IpService>();
            services.AddScoped<ISendNewUserNotificationService, SendNewUserNotificationService>();
            services.AddScoped<ICipherService, CipherService>();
            services.AddScoped<IPushBulletAppService, PushBulletAppService>();
            services.AddScoped<IUrlShortenerService, UrlShortenerService>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddSingleton<ApplicationInsightsJsHelper>();

            services.AddApiVersioning(
                options =>
                {
                    options.ReportApiVersions = true;
                    options.DefaultApiVersion = ApiVersion.Parse("1.0");
                    options.AssumeDefaultVersionWhenUnspecified = true;
                });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<RawTextRequestOperationFilter>();
                c.AddSecurityDefinition("AuthKey", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "AuthKey",
                    Description = "The AuthKey you're provided when you register.",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "AuthKey" }
                        },
                        new string[] { }
                    }
                });

                c.ExampleFilters();
                c.EnableAnnotations();
                c.OperationFilter<AddResponseHeadersFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
            });

            services.AddSwaggerExamplesFromAssemblyOf<Startup>();

            services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:InstrumentationKey"]);

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, IApiVersionDescriptionProvider provider)
        {
            UserManagerExtensions.Configure(
                app.ApplicationServices.GetRequiredService<IHttpContextAccessor>(), Configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // forwarded Header middleware
            var fordwardedHeaderOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            fordwardedHeaderOptions.KnownNetworks.Clear();
            fordwardedHeaderOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(fordwardedHeaderOptions);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());

                    c.DefaultModelRendering(ModelRendering.Example);
                    c.DefaultModelsExpandDepth(-1);
                }
            });

            var nonce = Guid.NewGuid().ToString("N");
            app.Use(async (ctx, next) =>
            {
                ctx.Items["csp-nonce"] = nonce;
                await next();
            });

            app.UseContentSecurityPolicy(new CspDirectiveList
            {
                DefaultSrc = CspDirective.Self.AddHttpsScheme(),
                StyleSrc = StyleCspDirective.Self.AddUnsafeInline().AddHttpsScheme(),
                ScriptSrc = ScriptCspDirective.Self.AddNonce(nonce).AddHttpsScheme()
                    .AddSource("https://az416426.vo.msecnd.net")
                    .AddSource("https://www.googletagmanager.com")
                    .AddSource("https://www.google-analytics.com")
                    .AddSource("https://connect.facebook.net")
                    .AddSource("https://www.facebook.com/"),
                ImgSrc = CspDirective.Self.AddDataScheme().AddHttpsScheme(),
                FontSrc = CspDirective.Self.AddHttpsScheme(),
                ConnectSrc = CspDirective.Self.AddHttpsScheme()
                    .AddSource("https://dc.services.visualstudio.com/")
            });

            app.AddCustomHeaders("Report-To", "{\"group\":\"default\",\"max_age\":31536000,\"endpoints\":[{\"url\":\"https://ajtio.report-uri.com/a/d/g\"}],\"include_subdomains\":true}");

            app.UseFeaturePolicy(
                new FeatureDirectiveList()
                    .AddNone(PolicyFeature.Microphone)
                    .AddNone(PolicyFeature.Camera)
                    .AddSelf(PolicyFeature.FullScreen)
            );

            app.UseFrameOptions(FrameOptionsPolicy.Deny);

            app.UseXContentTypeOptions(XContentTypeOptions.NoSniff);

            app.UseReferrerPolicy(ReferrerPolicy.Origin);

            app.UseXRobotsTag(false, false);

            app.UseExpectCT(true, new TimeSpan(7, 0, 0, 0), new Uri("https://ajtio.report-uri.com/r/d/ct/enforce"));

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseResponseCompression();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
