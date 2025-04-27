

namespace Bookstore.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly CommonService _commonService;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(Initials))]
        private string _fullName = "Not Logged In";

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private bool _isLoggedIn;

        [ObservableProperty]
        private bool _isBusy; // For showing loading indicators

        public string Initials
        {
            get
            {
                var parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length == 1)
                    return FullName.Length == 1 ? FullName : FullName[..2];
                return parts[0][0] + " " + parts[1][0];
            }
        }

        public ProfileViewModel(AuthService authService, CommonService commonService)
        {
            _authService = authService;
            _commonService = commonService;
            _commonService.LoginStatusChanged += OnLoginStatusChanged;
            LoadUserData();
        }

        private void OnLoginStatusChanged(object? sender, EventArgs e) => LoadUserData();

        [RelayCommand]
        private async Task LoginLogoutAsync()
        {
            IsBusy = true;

            try
            {
                if (!IsLoggedIn)
                {
                    await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
                }
                else
                {
                    await _authService.LogoutAsync();
                    await Shell.Current.DisplayAlert("Logged Out", "You have been logged out successfully.", "OK");
                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error during login/logout: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void LoadUserData()
        {
            try
            {
                var user = await _authService.GetStoredUserAsync();
                if (user != null)
                {
                    FullName = user.FullName;
                    Email = user.Email;
                    IsLoggedIn = true;
                }
                else
                {
                    FullName = "Not Logged In";
                    Email = string.Empty;
                    IsLoggedIn = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading user data: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to load user data. Please try again.", "OK");
            }
        }
    }
}