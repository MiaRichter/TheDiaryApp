namespace TheDiaryApp.Pages;

public partial class TeacherIsSchedule : ContentPage
{
    public TeacherIsSchedule(TeacherScheduleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Привязываем ViewModel
    }
}