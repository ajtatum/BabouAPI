using System;
using AJT.API.Models;
using AJT.API.Web.Helpers.Filters;
using AJT.API.Web.Services;
using AJT.API.Web.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddRazorPages()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddRazorRuntimeCompilation();

            services.Configure<AppSettings>(Configuration);

            services.AddScoped<ClientIpFilter>();
            services.AddScoped<AuthKeyFilter>();
            services.AddScoped<IIpService, IpService>();

            services.AddSingleton<ISlackService, SlackService>();

            services.AddHostedService<SlackBackgroundService>();

            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:InstrumentationKey"]);

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

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
