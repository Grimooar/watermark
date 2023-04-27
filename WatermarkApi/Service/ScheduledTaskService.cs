using Microsoft.Extensions.Logging;

namespace WatermarkApi.Service
{
    public class ScheduledTaskService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        public ScheduledTaskService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var imageService = scope.ServiceProvider.GetRequiredService<IImageService>();
                    Console.WriteLine($"Background service triggered at {DateTime.Now}");

                    await imageService.DeleteExpiredImages();

                    await Task.Delay(new TimeSpan(6, 0, 0)); //6, 0, 0
                }
                
            }
        }
    }
}
