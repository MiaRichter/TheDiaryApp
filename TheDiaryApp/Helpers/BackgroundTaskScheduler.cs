using System.Threading;
using Microsoft.Extensions.Logging;

namespace TheDiaryApp
{
    public class BackgroundTaskScheduler
    {
        private readonly ScheduleBackgroundTask _backgroundTask;
        private readonly ILogger<BackgroundTaskScheduler> _logger;

        public BackgroundTaskScheduler(ScheduleBackgroundTask backgroundTask, ILogger<BackgroundTaskScheduler> logger)
        {
            _backgroundTask = backgroundTask;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRunTime = GetNextRunTime(now);

                // Ожидаем до следующего времени запуска
                var delay = nextRunTime - now;
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, cancellationToken);
                }

                // Запускаем задачу
                await _backgroundTask.UpdateScheduleAsync();
            }
        }

        private DateTime GetNextRunTime(DateTime now)
        {
            // Устанавливаем время запуска (6:00 и 21:00)
            var runTimes = new[]
            {
                new DateTime(now.Year, now.Month, now.Day, 6, 0, 0),
                new DateTime(now.Year, now.Month, now.Day, 21, 0, 0)
            };

            // Находим следующее время запуска
            var nextRunTime = runTimes.FirstOrDefault(t => t > now);

            // Если следующее время запуска сегодня не найдено, планируем на завтра
            if (nextRunTime == default)
            {
                nextRunTime = runTimes[0].AddDays(1);
            }

            return nextRunTime;
        }
    }
}