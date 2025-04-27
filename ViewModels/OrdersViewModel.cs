namespace Bookstore.ViewModels
{
    public partial class OrdersViewModel : ObservableObject
    {
        private readonly OrderService _orderService;
        private readonly AuthService _authService;

        [ObservableProperty]
        private ObservableCollection<Order> _orders = new();

        [ObservableProperty]
        private bool _isLoading;

        public OrdersViewModel(OrderService orderService, AuthService authService)
        {
            _orderService = orderService;
            _authService = authService;
            LoadOrdersCommand.Execute(null);
            CheckAdminAccess();


        }

        // ✅ Load Orders (Admin Only)
        [RelayCommand]
        public async Task LoadOrders()
        {
            try
            {
                IsLoading = true;
                var ordersList = await _orderService.GetAllOrders();
                Orders = new ObservableCollection<Order>(ordersList);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading orders: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ✅ Complete Order (Admin Only)
        [RelayCommand]
        public async Task CompleteOrder(Order order)
        {
            var confirm = await Shell.Current.DisplayAlert("Complete Order",
                "Are you sure you want to complete this order?", "Yes", "Cancel");

            if (!confirm) return;

            bool success = await _orderService.CompleteOrder(order.Id);
            if (success)
            {
                Orders.Remove(order);
                await Shell.Current.DisplayAlert("Success", "Order completed successfully!", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Failed", "Could not complete the order. Try again later.", "OK");
            }
        }


        private async void CheckAdminAccess()
        {
            var user = await _authService.GetStoredUserAsync();
            if (user == null || user.Role != "Admin")
            {
                await Shell.Current.DisplayAlert(
                    "Access Denied",
                    "This page is for admins only.",
                    "OK");
                await Shell.Current.GoToAsync(nameof(HomePage));
            }
        }
    }
}
