
namespace Bookstore.Views
{
    public partial class OnBoardingPage : ContentPage
    {
        public OnBoardingPage()
        {
            InitializeComponent();

            Preferences.Default.Set(Constants.OnboardingShown, string.Empty);

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
        }
    }

}

