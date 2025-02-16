using Microsoft.Extensions.Logging;

namespace TheDiaryApp
{
    public partial class App : Application
    {
        private readonly BackgroundTaskScheduler _taskScheduler;
        private CancellationTokenSource _cancellationTokenSource;

        public App(BackgroundTaskScheduler taskScheduler)
        {
            InitializeComponent();
            _taskScheduler = taskScheduler;
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            // Запускаем фоновую задачу
            _cancellationTokenSource = new CancellationTokenSource();
            await _taskScheduler.StartAsync(_cancellationTokenSource.Token);
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            // Останавливаем фоновую задачу при сворачивании приложения
            _cancellationTokenSource?.Cancel();
        }

        protected override async void OnResume()
        {
            base.OnResume();

            // Перезапускаем фоновую задачу при возобновлении работы приложения
            _cancellationTokenSource = new CancellationTokenSource();
            await _taskScheduler.StartAsync(_cancellationTokenSource.Token);
        }
    }
}