using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace AJT.API
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
                    if (hostContext.HostingEnvironment.IsProduction())
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
                    var storageAccountName = hostingContext.Configuration["Azure:CloudStorage:AccountName"];
                    var storageAccountKey = hostingContext.Configuration["Azure:CloudStorage:AccountKey"];
                    var storageTableName = hostingContext.Configuration["Azure:CloudStorage:StorageTable"];

                    var storageCredentials = new StorageCredentials(storageAccountName, storageAccountKey);
                    var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);

                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.FromLogContext()
                        .Enrich.WithExceptionDetails()
                        .Enrich.WithProperty("Application", "AJT.API")
                        .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment.EnvironmentName)
                        .WriteTo.Console(new CompactJsonFormatter(null))
                        .WriteTo.AzureTableStorage(cloudStorageAccount, storageTableName: storageTableName, writeInBatches: true, batchPostingLimit: 100, period: new TimeSpan(0, 0, 3));
                });
        }
    }

}
