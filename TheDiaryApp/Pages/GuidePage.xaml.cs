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
            await DisplayAlert("���������", "����� ����� ��������� ���� �� ���������.", "OK");
        }

        private async void OnStudyGuideClicked(object sender, EventArgs e)
        {
            await DisplayAlert("������� �������", "����� ����� ��������� ���� �� �������� ��������.", "OK");
        }

        private async void OnDormitoryGuideClicked(object sender, EventArgs e)
        {
            await DisplayAlert("���������", "����� ����� ��������� ���� �� ����� � ���������.", "OK");
        }

        private async void OnActivitiesGuideClicked(object sender, EventArgs e)
        {
            await DisplayAlert("���������� ������������", "����� ����� ��������� ���� �� ���������� ������������.", "OK");
        }
    }
}