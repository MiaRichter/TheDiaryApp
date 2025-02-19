using Microsoft.Extensions.Logging;
using TheDiaryApp.Pages;

namespace TheDiaryApp
{
    public partial class App : Application
    {
        private CancellationTokenSource _cancellationTokenSource;

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            // Запускаем фоновую задачу
            _cancellationTokenSource = new CancellationTokenSource();
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
        }
    }
}