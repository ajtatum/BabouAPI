using System;
using Babou.API.Web.Areas.Identity;
using Babou.API.Web.Data;
using Babou.API.Web.Models.Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]
namespace Babou.API.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DefaultConnection")));

                services.AddIdentity<ApplicationUser, IdentityRole>(config =>
                    {
                        config.SignIn.RequireConfirmedEmail = true;

                        // Password settings.
                        config.Password.RequireDigit = true;
                        config.Password.RequireLowercase = true;
                        config.Password.RequireNonAlphanumeric = true;
                        config.Password.RequireUppercase = true;
                        config.Password.RequiredLength = 6;
                        config.Password.RequiredUniqueChars = 1;

                        // Lockout settings.
                        config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                        config.Lockout.MaxFailedAccessAttempts = 5;
                        config.Lockout.AllowedForNewUsers = true;

                        // User settings.
                        config.User.RequireUniqueEmail = true;
                    })
                    .AddDefaultUI()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

                services.AddAuthentication()
                    .AddCookie(options =>
                    {
                        options.Cookie.HttpOnly = true;
                        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                        options.Cookie.Name = "BabouApiApplication";
                        options.ExpireTimeSpan = TimeSpan.FromDays(7);

                        options.LoginPath = $"/Identity/Account/Login";
                        options.LogoutPath = $"/Identity/Account/Logout";
                        options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
                        options.SlidingExpiration = true;
                        options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                    })
                    .AddGoogle(options =>
                    {
                        options.ClientId = context.Configuration["Authentication:Google:ClientId"];
                        options.ClientSecret = context.Configuration["Authentication:Google:ClientSecret"];
                    })
                    .AddFacebook(options =>
                    {
                        options.AppId = context.Configuration["Authentication:Facebook:AppId"];
                        options.AppSecret = context.Configuration["Authentication:Facebook:AppSecret"];
                    })
                    .AddMicrosoftAccount(options =>
                    {
                        options.ClientId = context.Configuration["Authentication:Microsoft:ClientId"];
                        options.ClientSecret = context.Configuration["Authentication:Microsoft:ClientSecret"];
                    })
                    .AddTwitter(options =>
                    {
                        options.ConsumerKey = context.Configuration["Authentication:Twitter:ConsumerApiKey"];
                        options.ConsumerSecret = context.Configuration["Authentication:Twitter:ConsumerSecret"];
                    });
            });
        }
    }
}