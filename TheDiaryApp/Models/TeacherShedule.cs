using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TheDiaryApp.Models
{
    public class TeacherShedule : INotifyPropertyChanged
    {
        private bool _isToday;
        public bool IsToday
        {
            get => _isToday;
            set
            {
                if (_isToday != value)
                {
                    _isToday = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string? Teacher { get; set; } // преподаватель
        public string? LessenNumber { get; set; } // номер пары
        public string? Subject { get; set; } // название пары
        public string? Room { get; set; } // аудитория
        public string? GroupName { get; set; } // название группы
        public string? Time { get; set; } // Время проведения пары
    }
}
