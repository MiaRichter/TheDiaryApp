using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

public class SettingsViewModel : INotifyPropertyChanged
{
    private string _group;
    private int _activeSubGroup;

    public SettingsViewModel()
    {
        SaveCommand = new RelayCommand<string>(ChangeSubGroup);

        // Загрузка сохраненных настроек
        Group = Preferences.Get("Group", "КсК-21-1"); // Значение по умолчанию
        ActiveSubGroup = Preferences.Get("SubGroup", 1); // Значение по умолчанию
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

    public ICommand SaveCommand { get; }

    private void ChangeSubGroup(string subGroup)
    {
        if (int.TryParse(subGroup, out int subGroupInt))
        {
            ActiveSubGroup = subGroupInt;
            SaveSettings();
        }
    }

    private void SaveSettings()
    {
        Preferences.Set("Group", Group);
        Preferences.Set("SubGroup", ActiveSubGroup);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}