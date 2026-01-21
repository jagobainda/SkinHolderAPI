using SkinHolderAPI.Application.Loggers;

namespace SkinHolderAPI.Application.Background;

public class LogCleanupService(IServiceScopeFactory scopeFactory, ILogger<LogCleanupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var spainTime = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"));
            
            DateTime nextMonday;
            var daysUntilMonday = ((int)DayOfWeek.Monday - (int)spainTime.DayOfWeek + 7) % 7;

            if (daysUntilMonday == 0) nextMonday = spainTime.TimeOfDay < new TimeSpan(4, 30, 0) ? spainTime.Date : spainTime.Date.AddDays(7);
            else nextMonday = spainTime.Date.AddDays(daysUntilMonday);
            
            var targetTime = nextMonday.Add(new TimeSpan(4, 30, 0));
            var targetTimeUtc = TimeZoneInfo.ConvertTimeToUtc(targetTime, TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"));
            
            var delay = targetTimeUtc - now;
            
            if (delay.TotalSeconds > 0)
            {
                logger.LogInformation("Next log cleanup scheduled for {TargetTime} (Spain time). Waiting {Hours}h {Minutes}m", 
                    targetTime, delay.Hours + (delay.Days * 24), delay.Minutes);
                
                await Task.Delay(delay, stoppingToken);
            }

            try
            {
                using var scope = scopeFactory.CreateScope();
                var logLogic = scope.ServiceProvider.GetRequiredService<ILogLogic>();

                var cutoffDate = DateTime.UtcNow.AddMonths(-4);
                await logLogic.DeleteOldLogsAsync(cutoffDate);

                logger.LogInformation("Weekly log cleanup completed. Deleted logs older than {Date}", cutoffDate);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during weekly log cleanup");
            }
        }
    }
}