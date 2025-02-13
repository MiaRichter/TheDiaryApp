using TheDiaryApp.Helpers;
using TheDiaryApp.Repositories;
using TheDiaryApp.ViewModels;

namespace TheDiaryApp.Pages
{
    public partial class SchedulePage : ContentPage
    {
        public SchedulePage()
        {
            InitializeComponent();

            // Установите ViewModel
            var reportRepo = new ReportRepo(new ExcelParser(), new ReplacementParser());
            BindingContext = new ScheduleViewModel(reportRepo);
        }
    }
}