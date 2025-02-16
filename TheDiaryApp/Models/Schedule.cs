namespace TheDiaryApp.Models
{
    public class Schedule
    {
        public string? WeekType { get; set; } // Тип недели (Нечетная/Четная)
        public string? DayOfWeek { get; set; } // День недели
        public int LessonNumber { get; set; } // Номер занятия
        public string? Subject { get; set; } // Название предмета
        public string? Teacher { get; set; } // Преподаватель
        public string? Room { get; set; } // Аудитория
        public string? GroupName { get; set; } // Название группы
        public int SubGroup { get; set; } // Номер подгруппы
        public string? Time { get; set; } // Время проведения пары
    }
    public class StructuredSchedule
    {
        public Dictionary<string, Dictionary<string, List<Schedule>>> WeekData { get; set; }
            = new Dictionary<string, Dictionary<string, List<Schedule>>>();
    }
}
