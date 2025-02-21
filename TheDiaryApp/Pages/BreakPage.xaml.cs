using System;
using System.Timers;
using Microsoft.Maui.Controls;

namespace TheDiaryApp.Pages
{
    public partial class BreakPage : ContentPage
    {
        // ���������� ��� � �������
        private readonly TimeSpan[] lessonTimes = new TimeSpan[]
        {
            new TimeSpan(8, 30, 0),  // ������ ������ ����
            new TimeSpan(10, 0, 0),  // ����� ������ ����
            new TimeSpan(10, 10, 0), // ������ ������ ����
            new TimeSpan(11, 40, 0), // ����� ������ ����
            new TimeSpan(12, 20, 0), // ������ ������� ����
            new TimeSpan(13, 50, 0), // ����� ������� ����
            new TimeSpan(14, 20, 0), // ������ ��������� ����
            new TimeSpan(15, 50, 0), // ����� ��������� ����
            new TimeSpan(16, 0, 0),  // ������ ����� ����
            new TimeSpan(17, 30, 0), // ����� ����� ����
            new TimeSpan(17, 40, 0), // ������ ������ ����
            new TimeSpan(19, 10, 0)  // ����� ������ ����
        };

        private System.Timers.Timer _timer;

        public BreakPage()
        {
            InitializeComponent();
            StartTimer();
        }

        private void StartTimer()
        {
            _timer = new System.Timers.Timer(1000); // ������ � ���������� 1 �������
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // ��������� ��������� � �������� ������
            Device.BeginInvokeOnMainThread(() =>
            {
                UpdateCurrentTime();
                CheckBreakStatus();
            });
        }

        private void UpdateCurrentTime()
        {
            CurrentTimeLabel.Text = $"������� �����: {DateTime.Now:HH:mm:ss}";
        }

        private void CheckBreakStatus()
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            bool isSaturday = DateTime.Now.DayOfWeek == DayOfWeek.Saturday; // ���������, ������� �� �������

            for (int i = 0; i < lessonTimes.Length - 1; i += 2)
            {
                TimeSpan lessonStart = lessonTimes[i];
                TimeSpan lessonEnd = lessonTimes[i + 1];

                if (currentTime >= lessonStart && currentTime < lessonEnd)
                {
                    // ���� ������� ����� ������ ����
                    TimeSpan breakStart = lessonEnd;
                    TimeSpan breakEnd;

                    if (isSaturday)
                    {
                        // � ������� �������� ������ 10 �����
                        breakEnd = lessonEnd.Add(TimeSpan.FromMinutes(10));
                    }
                    else
                    {
                        // � ��������� ��� ���������� ����������
                        if (i + 2 < lessonTimes.Length)
                        {
                            breakEnd = lessonTimes[i + 2];
                        }
                        else
                        {
                            // ���� ��� ��������� ����, �������� ���
                            breakEnd = lessonEnd;
                        }
                    }

                    // ������� ���������� � ������� ����
                    LessonTimeLabel.Text = $"����: {lessonStart:hh\\:mm} - {lessonEnd:hh\\:mm}";

                    // ������� ���������� � ��������� ��������
                    BreakTimeLabel.Text = $"��������� ��������: {breakStart:hh\\:mm} - {breakEnd:hh\\:mm}";

                    // ������� ������������ ��������
                    TimeSpan breakDuration = breakEnd - breakStart;
                    BreakDurationLabel.Text = $"������������ ��������: {breakDuration.TotalMinutes} �����";

                    BreakStatusLabel.Text = "������ ��������: �� ��������";
                    return;
                }

                if (i + 2 < lessonTimes.Length && currentTime >= lessonEnd && currentTime < (isSaturday ? lessonEnd.Add(TimeSpan.FromMinutes(10)) : lessonTimes[i + 2]))
                {
                    // ���� ������� ����� ������ ��������
                    TimeSpan breakStart = lessonEnd;
                    TimeSpan breakEnd = isSaturday ? lessonEnd.Add(TimeSpan.FromMinutes(10)) : lessonTimes[i + 2];

                    // ������� ���������� � ������� ��������
                    BreakTimeLabel.Text = $"������� ��������: {breakStart:hh\\:mm} - {breakEnd:hh\\:mm}";

                    // ������� ������������ ��������
                    TimeSpan breakDuration = breakEnd - breakStart;
                    BreakDurationLabel.Text = $"������������ ��������: {breakDuration.TotalMinutes} �����";

                    BreakStatusLabel.Text = "������ ��������: ��������";
                    return;
                }
            }

            // ���� ������� ����� ��� ����������
            LessonTimeLabel.Text = "������ ��� ����.";
            BreakTimeLabel.Text = "������ ��� ��������.";
            BreakDurationLabel.Text = "������������ ��������: 0 �����";
            BreakStatusLabel.Text = "������ ��������: ��� ��������";
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}