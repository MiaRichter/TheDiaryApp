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
    }
}