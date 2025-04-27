using Microsoft.Extensions.DependencyInjection;

namespace Bookstore
{
    public partial class AppShell : Shell
    {
        private readonly AuthService _authService;

        public AppShell(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;

            // ✅ Listen for authentication state changes
            _authService.OnAuthStateChanged += () => MainThread.InvokeOnMainThreadAsync(UpdateOrderTabVisibility);

            // ✅ Set order tab visibility initially
            MainThread.InvokeOnMainThreadAsync(UpdateOrderTabVisibility);



            Routing.RegisterRoute(nameof(AllBooksPage), typeof(AllBooksPage));
            Routing.RegisterRoute(nameof(AuthPage), typeof(AuthPage));
            Routing.RegisterRoute(nameof(BookDetailsPage), typeof(BookDetailsPage));
            Routing.RegisterRoute(nameof(CartPage), typeof(CartPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(OnBoardingPage), typeof(OnBoardingPage));
            Routing.RegisterRoute(nameof(OrdersPage), typeof(OrdersPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));



        }

        private async Task UpdateOrderTabVisibility()
        {
            var isAdmin = await SecureStorage.GetAsync("user_role") == "Admin";

            var ordersTab = this.Items.FirstOrDefault(x => x.Route == "OrdersPage");
            if (ordersTab != null)
            {
                ordersTab.IsVisible = isAdmin;
            }
        }

        protected override async void OnNavigating(ShellNavigatingEventArgs args)
        {
            var target = args.Target.Location.OriginalString;

            if (target.Contains("CartPage"))
            {
                var user = await _authService.GetStoredUserAsync();

                if (user == null)
                {
                    bool goToLogin = await Shell.Current.DisplayAlert(
                        "Access Denied",
                        "The cart is only available for logged-in users. Would you like to log in?",
                        "Yes",
                        "No");

                    if (goToLogin)
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await Shell.Current.GoToAsync("//AuthPage");
                        });
                    }
                    else
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await Shell.Current.GoToAsync("//HomePage");
                        });
                    }

                    args.Cancel(); // Prevent navigation to CartPage
                    return;
                }
            }
        }
    }
}

