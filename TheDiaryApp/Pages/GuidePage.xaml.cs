namespace TheDiaryApp.Pages
{
    public partial class GuidePage : ContentPage
    {
        public GuidePage()
        {
            InitializeComponent();
        }

        private async void OnAdaptationGuideClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Адаптация", "Здесь будет подробный гайд по адаптации.", "OK");
        }

        private async void OnStudyGuideClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Учебный процесс", "Здесь будет подробный гайд по учебному процессу.", "OK");
        }

        private async void OnDormitoryGuideClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Общежитие", "Здесь будет подробный гайд по жизни в общежитии.", "OK");
        }

        private async void OnActivitiesGuideClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Внеучебная деятельность", "Здесь будет подробный гайд по внеучебной деятельности.", "OK");
        }
    }
}