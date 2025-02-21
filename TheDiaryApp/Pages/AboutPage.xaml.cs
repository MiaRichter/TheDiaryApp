using System;
using Microsoft.Maui.Controls;

namespace TheDiaryApp.Pages
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private async void OnPayPalButtonClicked(object sender, EventArgs e)
        {
            // ������� ������ �� PayPal
            await Launcher.OpenAsync("https://www.paypal.com/your-donate-link");
        }

        private async void OnPatreonButtonClicked(object sender, EventArgs e)
        {
            // ������� ������ �� Patreon
            await Launcher.OpenAsync("https://www.patreon.com/your-donate-link");
        }
    }
}