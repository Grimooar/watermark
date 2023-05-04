using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watermark.Bot.Abstract;

namespace Watermark.Bot.Services
{
    public class ScheduledTaskService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ScheduledTaskService> _logger;

        public ScheduledTaskService(IServiceProvider serviceProvider, ILogger<ScheduledTaskService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var userDataService = scope.ServiceProvider.GetRequiredService<IUserDataService>();
                    _logger.LogInformation($"Background service triggered at {DateTime.Now}");

                    await userDataService.DeleteExpiredUserData();

                    await Task.Delay(new TimeSpan(24, 0, 0)); //6, 0, 0
                }
            }
        }
    }
}
