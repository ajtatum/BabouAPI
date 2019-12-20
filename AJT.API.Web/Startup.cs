using System;
using System.Threading.Tasks;
using AJT.API.Web.Helpers.ExtensionMethods;
using AJT.API.Web.Helpers.Filters;
using AJT.API.Web.Models;
using AJT.API.Web.Models.Database;
using AJT.API.Web.Services;
using AJT.API.Web.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;


namespace AJT.API.Web
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
                .SetApplicationName("AJT API Web App")
                .PersistKeysToAzureBlobStorage(container, $"{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}/dataprotectionkeys.xml")
                .ProtectKeysWithAzureKeyVault(Configuration["Azure:KeyVault:EncryptionKey"], Configuration["Azure:KeyVault:ClientId"], Configuration["Azure:KeyVault:ClientSecret"]);

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.Always;
            });

            services.AddMvc()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddRazorPages()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .AddRazorRuntimeCompilation();

            services.Configure<AppSettings>(Configuration);

            services.AddScoped<ClientIpFilter>();
            services.AddScoped<AuthKeyFilter>();
            services.AddScoped<IIpService, IpService>();
            services.AddScoped<ISendNewUserNotificationService, SendNewUserNotificationService>();
            services.AddScoped<ICipherService, CipherService>();
            services.AddScoped<IPushBulletAppService, PushBulletAppService>();
            services.AddScoped<IUrlShortenerService, UrlShortenerService>();
            services.AddTransient<IEmailSender, EmailService>();

            services.AddSingleton<ISlackService, SlackService>();

            services.AddHostedService<SlackBackgroundService>();

            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "AJT API", Version = "v1"});
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
            });

            services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:InstrumentationKey"]);

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            UserManagerExtensions.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AJT API V1");
            });

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();

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
