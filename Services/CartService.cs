namespace Bookstore.Services
{
    public class CartService
    {
        private readonly HttpClient _httpClient;
        private readonly BookstoreDatabase _dbContext;

        public CartService(HttpClient httpClient, BookstoreDatabase dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        // ✅ Add to Cart (Offline Supported)
        public async Task<bool> AddToCart(CartItem item)
        {
            var userIdStr = await SecureStorage.GetAsync("user_id");
            if (string.IsNullOrEmpty(userIdStr))
            {
                await Shell.Current.DisplayAlert("Login Required", "You need to log in to add items to the cart.", "OK");
                await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
                return false;
            }

            if (!int.TryParse(userIdStr, out int userId)) return false;

            item.UserId = userId; // ✅ Assign UserId

            var existingItem = await _dbContext.GetItemAsync<CartItem>(item.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity; // ✅ Merge duplicate items
                await _dbContext.SaveItemAsync(existingItem);
            }
            else
            {
                await _dbContext.SaveItemAsync(item);
            }

            return true;
        }

        // ✅ Sync Cart When Online
        public async Task SyncCart()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet) return;

            var cartItems = await _dbContext.GetAllItemsAsync<CartItem>();
            if (cartItems.Count == 0) return;

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseAddress}/api/Orders/AddToCart", cartItems);
                if (response.IsSuccessStatusCode)
                {
                    await _dbContext.DeleteAllItemsAsync<CartItem>();
                }
                else
                {
                    Debug.WriteLine($"⚠️ Cart sync failed, will retry later.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error syncing cart: {ex.Message}");
            }
        }

        public async Task<List<CartItem>> GetCartFromLocalStorage()
        {
            return await _dbContext.GetAllItemsAsync<CartItem>();
        }

        public async Task<bool> RemoveFromCart(CartItem item)
        {
            await _dbContext.DeleteItemAsync(item);
            return true;
        }

        public async Task ClearCart()
        {
            await _dbContext.DeleteAllItemsAsync<CartItem>();
        }
    }
}
