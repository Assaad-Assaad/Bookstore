namespace Bookstore;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (Preferences.Default.ContainsKey(Constants.OnboardingShown))
            await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
        else
            await Shell.Current.GoToAsync($"//{nameof(OnBoardingPage)}");
    }
}