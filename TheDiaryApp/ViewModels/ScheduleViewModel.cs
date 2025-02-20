using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Networking;
using Newtonsoft.Json;

namespace TheDiaryApp.ViewModels
{
    public class ScheduleViewModel : INotifyPropertyChanged
    {
        private readonly ReportRepo _reportRepo;
        private StructuredSchedule _schedule;
        private bool _isRefreshing;
        private bool _isInternetAvailable = true;

        public ScheduleViewModel(ReportRepo reportRepo)
        {
            _reportRepo = reportRepo;
            // Пытаемся сразу загрузить локальные данные
            var local = _reportRepo.LoadLocalSchedule();
            if (local != null) Schedule = local;
            // Подписка на изменение состояния сети
            Connectivity.ConnectivityChanged += OnConnectivityChanged;

            // Команда для обновления
            RefreshCommand = new AsyncRelayCommand(RefreshScheduleAsync);

            // Загрузка расписания при запуске
            LoadScheduleAsync(); // Используем асинхронный метод без ожидания
        }

        public StructuredSchedule Schedule
        {
            get => _schedule;
            set
            {
                _schedule = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEmpty));
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public bool IsInternetAvailable
        {
            get => _isInternetAvailable;
            set
            {
                _isInternetAvailable = value;
                OnPropertyChanged();
            }
        }

        public bool IsEmpty => Schedule?.WeekData?.Count == 0;

        public ICommand RefreshCommand { get; }

        private async Task RefreshScheduleAsync()
        {
            IsRefreshing = true;

            try
            {
                await LoadSchedule(); // Теперь это асинхронный вызов
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task LoadSchedule()
        {
            // Проверяем доступность интернета
            bool isInternetAvailable = await CheckInternetAccessAsync();
            IsInternetAvailable = isInternetAvailable;

            if (!isInternetAvailable)
            {
                SaveLocalCopy(Schedule);
                // Выводим сообщение об отсутствии интернета
                await Shell.Current.DisplayAlert("Внимание", "Нет интернета! Расписание может быть не актуальным.", "OK");
                return; // Прерываем загрузку расписания
            }

            // Загружаем сохраненные настройки
            string group = Preferences.Get("Group", "КсК-21-1"); // Значение по умолчанию
            int subGroup = Preferences.Get("SubGroup", 1); // Значение по умолчанию
                                                           // Пытаемся загрузить локальную копию
            var localSchedule = _reportRepo.LoadLocalSchedule();
            if (localSchedule != null)
            {
                Schedule = localSchedule;
            }
            // Загружаем расписание для сохраненной группы и подгруппы
            Schedule = await _reportRepo.ReportAsync(group, subGroup);

            // Фильтруем расписание по типу недели
            FilterScheduleByWeekType();
        }

        private void SaveLocalCopy(StructuredSchedule schedule)
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "offline_schedule.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(schedule));
        }

        private async Task<bool> CheckInternetAccessAsync()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Добавляем задержку перед проверкой
                    await Task.Delay(1000); // 1 секунда

                    // Пытаемся выполнить запрос к надежному ресурсу
                    var response = await httpClient.GetAsync("https://www.google.com");
                    return response.IsSuccessStatusCode; // Если ответ успешный, интернет доступен
                }
            }
            catch
            {
                // Если произошла ошибка, интернет недоступен
                return false;
            }
        }

        private async void LoadScheduleAsync()
        {
            await LoadSchedule();
        }

        private async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            // Проверяем доступность интернета
            bool isInternetAvailable = await CheckInternetAccessAsync();
            IsInternetAvailable = isInternetAvailable;

            if (!isInternetAvailable)
            {
                // Выводим сообщение об отсутствии интернета
                await Shell.Current.DisplayAlert("Внимание", "Нет интернета! Расписание может быть не актуальным.", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool IsEvenWeek()
        {
            // Получаем текущую дату
            DateTime now = DateTime.Now;

            // Получаем номер недели в году
            var calendar = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            int weekNumber = calendar.GetWeekOfYear(now, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            // Возвращаем true, если неделя четная, иначе false
            bool isParity = weekNumber % 2 == 0;
            if ((now.DayOfWeek == DayOfWeek.Saturday && now.Hour >= 21) || now.DayOfWeek == DayOfWeek.Sunday)
            {
                isParity = !isParity;
            }
            return isParity;
        }

        private void FilterScheduleByWeekType()
        {
            if (Schedule == null || Schedule.WeekData == null)
                return;

            // Определяем тип текущей недели
            string currentWeekType = IsEvenWeek() ? "Четная неделя" : "Нечетная неделя";

            // Фильтруем данные, оставляя только текущую неделю
            var filteredWeekData = Schedule.WeekData
                .Where(week => week.Key == currentWeekType)
                .ToDictionary(week => week.Key, week => week.Value);

            // Обновляем Schedule
            Schedule.WeekData = filteredWeekData;
            // Устанавливаем флаг IsToday для сегодняшнего дня
            SetTodayFlag();
            OnPropertyChanged(nameof(Schedule));
        }

        private void SetTodayFlag()
        {
            if (Schedule == null || Schedule.WeekData == null)
                return;

            var now = DateTime.Now;

            // Сдвигаем текущий день недели в зависимости от условий
            if (now.DayOfWeek == DayOfWeek.Saturday && now.Hour >= 21) now = now.AddDays(2); // Если суббота и время 21:00 или позже, сдвигаем на понедельник
            if (now.DayOfWeek == DayOfWeek.Sunday) now = now.AddDays(1); // Если воскресенье, сдвигаем на понедельник
            if (now.Hour >= 21) now = now.AddDays(1); // Если 21:00 то смени отображение текущего дня на следующий

            // Получаем текущий день недели после сдвига
            string today = now.DayOfWeek switch
            {
                DayOfWeek.Monday => "Понедельник",
                DayOfWeek.Tuesday => "Вторник",
                DayOfWeek.Wednesday => "Среда",
                DayOfWeek.Thursday => "Четверг",
                DayOfWeek.Friday => "Пятница",
                DayOfWeek.Saturday => "Суббота",
                _ => null
            };

            if (today == null)
                return;

            // Устанавливаем флаг IsToday для сегодняшнего дня
            foreach (var week in Schedule.WeekData.Values)
            {
                if (week.ContainsKey(today))
                {
                    foreach (var schedule in week[today])
                    {
                        schedule.IsToday = true;
                    }
                }
            }
        }
    }
}