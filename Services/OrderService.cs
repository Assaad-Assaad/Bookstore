namespace Bookstore.Services
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;
        private readonly BookstoreDatabase _dbContext;

        public OrderService(HttpClient httpClient, BookstoreDatabase dbContext)
        {
            
            _dbContext = dbContext;
            _httpClient = httpClient;
            Connectivity.ConnectivityChanged += async (sender, e) =>
            {
                if (e.NetworkAccess == NetworkAccess.Internet)
                    await SyncOrders();
            };
        }

        // ✅ Place Order (Stores locally if offline)
        public async Task<bool> PlaceOrder(Order order)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                order.Status = "PendingSync"; // Offline order
                await _dbContext.SaveItemAsync(order);
                return true;
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseAddress}/api/Orders/ConfirmOrder", order);
                if (!response.IsSuccessStatusCode)
                {
                    order.Status = "FailedSync";
                    await _dbContext.SaveItemAsync(order);
                    return false;
                }

                order.Status = "Completed"; // Successfully placed online
                await _dbContext.SaveItemAsync(order);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Order Placement Failed: {ex.Message}");
                order.Status = "FailedSync";
                await _dbContext.SaveItemAsync(order);
                return false;
            }
        }

        // ✅ Sync Orders when online
        public async Task SyncOrders()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet) return;

            var pendingOrders = await _dbContext.GetAllItemsAsync<Order>();
            foreach (var order in pendingOrders.Where(o => o.Status == "PendingSync" || o.Status == "FailedSync"))
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseAddress}/api/Orders/ConfirmOrder", order);
                    if (response.IsSuccessStatusCode)
                    {
                        order.Status = "Completed";
                        await _dbContext.SaveItemAsync(order);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Sync Error: {ex.Message}");
                }
            }
        }

        // ✅ Get All Orders for Admin
        public async Task<List<Order>> GetAllOrders()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet) return await _dbContext.GetAllItemsAsync<Order>();

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Order>>($"{ApiConfig.BaseAddress}/api/Orders/AllOrders");
                return response ?? await _dbContext.GetAllItemsAsync<Order>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error fetching orders: {ex.Message}");
                return await _dbContext.GetAllItemsAsync<Order>();
            }
        }

        public async Task<bool> CompleteOrder(int orderId)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Debug.WriteLine($"⚠️ Order {orderId} completion delayed (offline mode).");
                return false;
            }

            try
            {
                var response = await _httpClient.PostAsync($"{ApiConfig.BaseAddress}/api/Orders/CompleteOrder/{orderId}", null);
                if (response.IsSuccessStatusCode)
                {
                    await _dbContext.DeleteItemByIdAsync<Order>(orderId);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error completing order {orderId}: {ex.Message}");
            }

            return false;
        }
    }
}
