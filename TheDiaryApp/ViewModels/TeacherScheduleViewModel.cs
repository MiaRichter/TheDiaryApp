using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;

namespace TheDiaryApp.ViewModels
{
    public class TeacherScheduleViewModel : INotifyPropertyChanged
    {
        private readonly TeacherRepo _teacherRepo;
        private List<TeacherShedule> _schedules;
        private bool _isRefreshing;
        private bool _isInternetAvailable = true;

        public TeacherScheduleViewModel(TeacherRepo teacherRepo)
        {
            _teacherRepo = teacherRepo;

            // Пытаемся сразу загрузить локальные данные
            var local = _teacherRepo.LoadLocalSchedule();
            if (local != null) Schedules = local;

            // Подписка на изменение состояния сети
            Connectivity.ConnectivityChanged += OnConnectivityChanged;

            // Команда для обновления
            RefreshCommand = new AsyncRelayCommand(RefreshScheduleAsync);

            // Загрузка расписания при запуске
            LoadScheduleAsync(); // Используем асинхронный метод без ожидания
        }

        public List<TeacherShedule> Schedules
        {
            get => _schedules;
            set
            {
                _schedules = value;
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

        public bool IsEmpty => Schedules?.Count == 0;

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
                SaveLocalCopy(Schedules);
                // Выводим сообщение об отсутствии интернета
                await Shell.Current.DisplayAlert("Внимание", "Нет интернета! Расписание может быть не актуальным.", "OK");
                return; // Прерываем загрузку расписания
            }

            // Загружаем расписание преподавателей
            Schedules = await _teacherRepo.ReportTAsync("Абдулвелеев И.Р."); // Укажите нужное имя преподавателя
        }

        private void SaveLocalCopy(List<TeacherShedule> schedules)
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "offline_schedule_teacher.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(schedules));
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
    }
}