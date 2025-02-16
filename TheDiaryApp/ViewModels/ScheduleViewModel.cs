using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Networking;

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
            var currentNetworkAccess = Connectivity.NetworkAccess;
            IsInternetAvailable = currentNetworkAccess == NetworkAccess.Internet;

            if (!IsInternetAvailable)
            {
                // Выводим сообщение об отсутствии интернета
                await Shell.Current.DisplayAlert("Внимание", "Нет интернета! Расписание может быть не актуальным.", "OK");
            }

            // Загрузите расписание для группы и подгруппы
            Schedule = await _reportRepo.ReportAsync("Кск-21-1", 1); // Укажите группу и подгруппу

            // Фильтруем расписание по типу недели
            FilterScheduleByWeekType();
        }

        private async void LoadScheduleAsync()
        {
            await LoadSchedule();
        }

        private async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            IsInternetAvailable = e.NetworkAccess == NetworkAccess.Internet;

            if (!IsInternetAvailable)
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
            if ((now.DayOfWeek == DayOfWeek.Saturday && now.Hour == 21) || now.DayOfWeek == DayOfWeek.Sunday)
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
            OnPropertyChanged(nameof(Schedule));
        }
    }
}