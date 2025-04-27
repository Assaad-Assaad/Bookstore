namespace Bookstore.ViewModels
{
    public partial class CartViewModel : ObservableObject
    {
        private readonly CartService _cartService;
        private readonly OrderService _orderService;
        private readonly AuthService _authService;

        [ObservableProperty]
        private ObservableCollection<CartItem> _cartItems = new();

        [ObservableProperty]
        private decimal _totalPrice;

        [ObservableProperty]
        private bool _isCartEmpty;

        public CartViewModel(CartService cartService, OrderService orderService, AuthService authService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _authService = authService;

            LoadCart();



        }

        // ✅ Load Cart Items
        [RelayCommand]
        public async Task LoadCart()
        {
            try
            {
                // 🔥 Check if user is logged in before loading cart
                var user = await _authService.GetStoredUserAsync();
                if (user == null)
                {
                    await Shell.Current.DisplayAlert(
                        "Access Denied",
                        "The cart is only available for logged-in users. Would you like to log in?",
                        "Yes",
                        "No");

                    await Shell.Current.GoToAsync("//HomePage"); // ✅ Redirect non-logged-in users
                    return;
                }

                var items = await _cartService.GetCartFromLocalStorage();
                CartItems = new ObservableCollection<CartItem>(items);
                UpdateTotalPrice();
                IsCartEmpty = CartItems.Count == 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading cart: {ex.Message}");
            }
        }


        // ✅ Remove Item from Cart
        [RelayCommand]
        public async Task RemoveFromCart(CartItem item)
        {
            try
            {
                CartItems.Remove(item);
                await _cartService.RemoveFromCart(item); // ✅ Fixed: Proper removal
                UpdateTotalPrice();
                IsCartEmpty = CartItems.Count == 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error removing item: {ex.Message}");
            }
        }

        // ✅ Clear Cart
        [RelayCommand]
        public async Task ClearCart()
        {
            CartItems.Clear();
            await _cartService.ClearCart(); // ✅ Fixed: Clear local storage
            UpdateTotalPrice();
            IsCartEmpty = true;
        }

        // ✅ Place Order (Handles Offline Mode)
        [RelayCommand]
        public async Task PlaceOrder()
        {
            // ✅ Ensure user is logged in before placing an order
            var userIdStr = await SecureStorage.GetAsync("user_id");
            if (string.IsNullOrEmpty(userIdStr))
            {
                await Shell.Current.DisplayAlert("Login Required", "You need to log in to place an order.", "OK");
                await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
                return;
            }

            if (CartItems.Count == 0)
            {
                await Shell.Current.DisplayAlert("Cart Empty", "Add items before placing an order.", "OK");
                return;
            }

            var order = new Order
            {
                CartItems = CartItems.ToList(),
                Status = Connectivity.NetworkAccess == NetworkAccess.Internet ? "Completed" : "PendingSync"
            };

            var success = await _orderService.PlaceOrder(order);
            if (success)
            {
                await _cartService.ClearCart(); // ✅ Fixed: Clear local cart after ordering
                CartItems.Clear();
                IsCartEmpty = true;
                await Shell.Current.DisplayAlert("Success", "Order placed successfully!", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Failed", "Order failed. Try again later.", "OK");
            }
        }

        // ✅ Update Total Price
        private void UpdateTotalPrice()
        {
            TotalPrice = CartItems.Sum(item => item.Book.Price * item.Quantity);
        }



       
    }
}
