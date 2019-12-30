using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Serilog;
using AJT.Defaults.Serilog;

namespace Babou.API.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options => options.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("appsettings.json", false, true);
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true, true);
                    configApp.AddEnvironmentVariables();
                    configApp.AddCommandLine(args);

                    if (hostContext.HostingEnvironment.IsDevelopment())
                    {
                        configApp.AddUserSecrets<Program>();
                    }
                    if (hostContext.HostingEnvironment.IsProduction() || hostContext.HostingEnvironment.IsStaging())
                    {
                        var azureServiceTokenProvider = new AzureServiceTokenProvider();
                        var keyVaultClient = new KeyVaultClient(
                            new KeyVaultClient.AuthenticationCallback(
                                azureServiceTokenProvider.KeyVaultTokenCallback));

                        configApp.AddAzureKeyVault(new AzureKeyVaultConfigurationOptions()
                        {
                            Client = keyVaultClient,
                            Manager = new DefaultKeyVaultSecretManager(),
                            Vault = $"https://{Environment.GetEnvironmentVariable("AzureKeyVaultName")}.vault.azure.net/",
                            ReloadInterval = TimeSpan.FromMinutes(30)
                        });
                    }
                })
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration.LoadDefaultConfig(hostingContext, "Babou API Web App", true, true, true, true);
                });
        }

    }
}
