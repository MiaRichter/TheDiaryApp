using System;
using System.Timers;
using Microsoft.Maui.Controls;

namespace TheDiaryApp.Pages
{
    public partial class BreakPage : ContentPage
    {
        // Расписание пар и перемен
        private readonly TimeSpan[] lessonTimes = new TimeSpan[]
        {
            new TimeSpan(8, 30, 0),  // Начало первой пары
            new TimeSpan(10, 0, 0),  // Конец первой пары
            new TimeSpan(10, 10, 0), // Начало второй пары
            new TimeSpan(11, 40, 0), // Конец второй пары
            new TimeSpan(12, 20, 0), // Начало третьей пары
            new TimeSpan(13, 50, 0), // Конец третьей пары
            new TimeSpan(14, 20, 0), // Начало четвертой пары
            new TimeSpan(15, 50, 0), // Конец четвертой пары
            new TimeSpan(16, 0, 0),  // Начало пятой пары
            new TimeSpan(17, 30, 0), // Конец пятой пары
            new TimeSpan(17, 40, 0), // Начало шестой пары
            new TimeSpan(19, 10, 0)  // Конец шестой пары
        };

        private System.Timers.Timer _timer;

        public BreakPage()
        {
            InitializeComponent();
            StartTimer();
        }

        private void StartTimer()
        {
            _timer = new System.Timers.Timer(1000); // Таймер с интервалом 1 секунда
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Обновляем интерфейс в основном потоке
            Device.BeginInvokeOnMainThread(() =>
            {
                UpdateCurrentTime();
                CheckBreakStatus();
            });
        }

        private void UpdateCurrentTime()
        {
            CurrentTimeLabel.Text = $"Текущее время: {DateTime.Now:HH:mm:ss}";
        }

        private void CheckBreakStatus()
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            bool isSaturday = DateTime.Now.DayOfWeek == DayOfWeek.Saturday; // Проверяем, суббота ли сегодня

            for (int i = 0; i < lessonTimes.Length - 1; i += 2)
            {
                TimeSpan lessonStart = lessonTimes[i];
                TimeSpan lessonEnd = lessonTimes[i + 1];

                if (currentTime >= lessonStart && currentTime < lessonEnd)
                {
                    // Если текущее время внутри пары
                    TimeSpan breakStart = lessonEnd;
                    TimeSpan breakEnd;

                    if (isSaturday)
                    {
                        // В субботу перемена длится 10 минут
                        breakEnd = lessonEnd.Add(TimeSpan.FromMinutes(10));
                    }
                    else
                    {
                        // В остальные дни используем расписание
                        if (i + 2 < lessonTimes.Length)
                        {
                            breakEnd = lessonTimes[i + 2];
                        }
                        else
                        {
                            // Если это последняя пара, перемены нет
                            breakEnd = lessonEnd;
                        }
                    }

                    // Выводим информацию о текущей паре
                    LessonTimeLabel.Text = $"Пара: {lessonStart:hh\\:mm} - {lessonEnd:hh\\:mm}";

                    // Выводим информацию о следующей перемене
                    BreakTimeLabel.Text = $"Следующая перемена: {breakStart:hh\\:mm} - {breakEnd:hh\\:mm}";

                    // Выводим длительность перемены
                    TimeSpan breakDuration = breakEnd - breakStart;
                    BreakDurationLabel.Text = $"Длительность перемены: {breakDuration.TotalMinutes} минут";

                    BreakStatusLabel.Text = "Статус перемены: Не началась";
                    return;
                }

                if (i + 2 < lessonTimes.Length && currentTime >= lessonEnd && currentTime < (isSaturday ? lessonEnd.Add(TimeSpan.FromMinutes(10)) : lessonTimes[i + 2]))
                {
                    // Если текущее время внутри перемены
                    TimeSpan breakStart = lessonEnd;
                    TimeSpan breakEnd = isSaturday ? lessonEnd.Add(TimeSpan.FromMinutes(10)) : lessonTimes[i + 2];

                    // Выводим информацию о текущей перемене
                    BreakTimeLabel.Text = $"Текущая перемена: {breakStart:hh\\:mm} - {breakEnd:hh\\:mm}";

                    // Выводим длительность перемены
                    TimeSpan breakDuration = breakEnd - breakStart;
                    BreakDurationLabel.Text = $"Длительность перемены: {breakDuration.TotalMinutes} минут";

                    BreakStatusLabel.Text = "Статус перемены: Началась";
                    return;
                }
            }

            // Если текущее время вне расписания
            LessonTimeLabel.Text = "Сейчас нет пары.";
            BreakTimeLabel.Text = "Сейчас нет перемены.";
            BreakDurationLabel.Text = "Длительность перемены: 0 минут";
            BreakStatusLabel.Text = "Статус перемены: Нет перемены";
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}