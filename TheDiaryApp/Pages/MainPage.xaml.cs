using TheDiaryApp.Models;
using TheDiaryApp.PageModels;

namespace TheDiaryApp.Pages;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}