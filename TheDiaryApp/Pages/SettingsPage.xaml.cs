using Syncfusion.Maui.Toolkit.Carousel;

namespace TheDiaryApp.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel viewModel)
    {
		InitializeComponent();
        BindingContext = viewModel;
    }
}