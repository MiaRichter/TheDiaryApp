namespace TheDiaryApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage(ScheduleViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}