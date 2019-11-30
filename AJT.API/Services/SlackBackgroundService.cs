using System;
using System.Threading;
using System.Threading.Tasks;
using AJT.API.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AJT.API.Services
{
    public class SlackBackgroundService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SlackBackgroundService(IServiceProvider services)
        {
            _serviceProvider = services;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var slackService = scope.ServiceProvider.GetRequiredService<ISlackService>();

            slackService.SendBotMessage();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
