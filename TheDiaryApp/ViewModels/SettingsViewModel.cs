using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace TheDiaryApp.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private string _group;
        private int _subGroup;
        private readonly ReportRepo _reportRepo;

        public SettingsViewModel(ReportRepo reportRepo)
        {
            _reportRepo = reportRepo;
            SaveCommand = new RelayCommand(SaveSettings);
            // Загрузка сохраненных настроек
            Group = Preferences.Get("Group", "КсК-21-1"); // Значение по умолчанию
            SubGroup = Preferences.Get("SubGroup", 1); // Значение по умолчанию
        }

        // Свойство для группы
        public string Group
        {
            get => _group;
            set
            {
                if (_group != value)
                {
                    _group = value;
                    OnPropertyChanged();
                }
            }
        }

        // Свойство для подгруппы
        public int SubGroup
        {
            get => _subGroup;
            set
            {
                if (_subGroup != value)
                {
                    _subGroup = value;
                    OnPropertyChanged();
                }
            }
        }

        // Команда для сохранения настроек
        public ICommand SaveCommand { get; }

        // Метод для сохранения настроек
        private void SaveSettings()
        {
            // Сохраняем настройки (например, в Preferences)
            Preferences.Set("Group", Group);
            Preferences.Set("SubGroup", SubGroup);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}