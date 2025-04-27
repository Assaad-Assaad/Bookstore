using CommunityToolkit.Maui.Core;

namespace Bookstore.ViewModels
{
    public partial class AuthViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private bool _isRegistrationMode;  // True = Register, False = Login

        [ObservableProperty]
        private LoginModel _loginModel = new();

        [ObservableProperty]
        private RegisterModel _registerModel = new();

        [ObservableProperty]
        private bool _isBusy;

        public AuthViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private void ToggleMode()
        {
            IsRegistrationMode = !IsRegistrationMode; // Toggle login/register mode
        }

        [RelayCommand]
        private async Task Submit()
        {
            if (IsRegistrationMode)
            {
                if (string.IsNullOrWhiteSpace(RegisterModel.FullName) ||
                    string.IsNullOrWhiteSpace(RegisterModel.Email) ||
                    string.IsNullOrWhiteSpace(RegisterModel.Password))
                {
                    await Toast.Make("All fields are required").Show();
                    return;
                }

                IsBusy = true;
                bool success = await _authService.RegisterAsync(RegisterModel);
                IsBusy = false;

                if (success)
                {
                    await Toast.Make("Registration successful. Please log in.").Show();
                    IsRegistrationMode = false; // Switch to login mode
                }
                else
                {
                    await Toast.Make("Registration failed. Check your internet connection or try again later.", ToastDuration.Long).Show();
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(LoginModel.Email) ||
                    string.IsNullOrWhiteSpace(LoginModel.Password))
                {
                    await Toast.Make("Email and Password are required").Show();
                    return;
                }

                IsBusy = true;
                bool success = await _authService.LoginAsync(LoginModel);
                IsBusy = false;

                if (success)
                {
                    await Shell.Current.GoToAsync(nameof(HomePage));
                }
                else
                {
                    await Toast.Make("Invalid email or password. Please try again.", ToastDuration.Long).Show();
                }
            }
        }

        [RelayCommand]
        public async Task SkipForNow()
        {
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }
    }
}
