using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;

public class SettingsViewModel : INotifyPropertyChanged
{
    private string _group;
    private int _activeSubGroup;
    private string _selectedTheme;

    public SettingsViewModel()
    {
        SaveCommand = new RelayCommand<string>(ChangeSubGroup);
        ChangeThemeCommand = new RelayCommand<string>(ChangeTheme);

        // Загрузка сохраненных настроек
        Group = Preferences.Get("Group", "КсК-21-1"); // Значение по умолчанию
        ActiveSubGroup = Preferences.Get("SubGroup", 1); // Значение по умолчанию
        SelectedTheme = Preferences.Get("SelectedTheme", "Auto"); // Значение по умолчанию

        // Применение темы при запуске
        ApplyTheme(SelectedTheme);
    }

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

    public int ActiveSubGroup
    {
        get => _activeSubGroup;
        set
        {
            if (_activeSubGroup != value)
            {
                _activeSubGroup = value;
                OnPropertyChanged();
            }
        }
    }

    public string SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (_selectedTheme != value)
            {
                _selectedTheme = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand SaveCommand { get; }
    public ICommand ChangeThemeCommand { get; }

    private void ChangeSubGroup(string subGroup)
    {
        if (int.TryParse(subGroup, out int subGroupInt))
        {
            ActiveSubGroup = subGroupInt;
            SaveSettings();
        }
    }

    private void ChangeTheme(string theme)
    {
        SelectedTheme = theme;
        ApplyTheme(theme);
        SaveSettings();
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

    private void SaveSettings()
    {
        Preferences.Set("Group", Group);
        Preferences.Set("SubGroup", ActiveSubGroup);
        Preferences.Set("SelectedTheme", SelectedTheme);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}