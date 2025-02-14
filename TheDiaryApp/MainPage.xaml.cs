namespace TheDiaryApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var reportRepo = new ReportRepo(new ExcelParser(), new ReplacementParser());
            BindingContext = new ScheduleViewModel(reportRepo);
        }
    }

}
