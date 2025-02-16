using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;

namespace TheDiaryApp
{
    public class ScheduleBackgroundTask
    {
        private readonly ReportRepo _reportRepo;
        private readonly ILogger<ScheduleBackgroundTask> _logger;

        public ScheduleBackgroundTask(ReportRepo reportRepo, ILogger<ScheduleBackgroundTask> logger)
        {
            _reportRepo = reportRepo;
            _logger = logger;
        }

        public async Task UpdateScheduleAsync()
        {
            try
            {
                // Загрузите расписание
                var schedule = await _reportRepo.ReportAsync("Кск-21-1", 1);

                // Логируем успешное обновление
                _logger.LogInformation("Расписание успешно обновлено в фоновом режиме.");

                // Показываем уведомление (опционально)
                var toast = Toast.Make("Расписание обновлено!", ToastDuration.Short, 14);
                await toast.Show();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении расписания в фоновом режиме.");
            }
        }
    }
}