using TheDiaryApp.Models;
using TheDiaryApp.Repositories;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TheDiaryApp.ViewModels
{
    public class ScheduleViewModel : INotifyPropertyChanged
    {
        private readonly ReportRepo _reportRepo;
        private StructuredSchedule _schedule;

        public ScheduleViewModel(ReportRepo reportRepo)
        {
            _reportRepo = reportRepo;
            LoadSchedule();
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

        public bool IsEmpty => Schedule?.WeekData?.Count == 0;

        private async void LoadSchedule()
        {
            // Загрузите расписание для группы и подгруппы
            Schedule = await _reportRepo.ReportAsync("Кск-21-1", 1); // Укажите группу и подгруппу
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}