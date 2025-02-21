using Microsoft.Extensions.Logging;
using TheDiaryApp.Pages;

namespace TheDiaryApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Загрузка сохраненной темы
            var savedTheme = Preferences.Get("SelectedTheme", "Auto");
            ApplyTheme(savedTheme);
            MainPage = new AppShell();
        }

        private void ApplyTheme(string theme)
        {
            switch (theme)
            {
                case "Light":
                    Application.Current.UserAppTheme = AppTheme.Light;
                    break;
                case "Dark":
                    Application.Current.UserAppTheme = AppTheme.Dark;
                    break;
                case "Auto":
                    Application.Current.UserAppTheme = AppTheme.Unspecified;
                    break;
            }
        }
    }
}