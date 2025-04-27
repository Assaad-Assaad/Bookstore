using System.Security.Cryptography;
using System.Text;

namespace Bookstore.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly BookstoreDatabase _dbContext;
        private readonly CommonService _commonService;

        public event Action OnAuthStateChanged;

        public AuthService(HttpClient httpClient, BookstoreDatabase dbContext, CommonService commonService)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _commonService = commonService;
        }

        // Hash Password for Offline Storage
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        // Generate Temporary Password for Offline Users
        private string GenerateTemporaryPassword()
        {
            return Guid.NewGuid().ToString().Substring(0, 8) + "Ab!"; // Example: "a1b2c3d4Ab!"
        }

        // Register User (Offline First, Then Online)
        public async Task<bool> RegisterAsync(RegisterModel model)
        {
            try
            {
                var newUser = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password), // Hash Password
                    Role = "User",
                    Token = null, // No token until synced with API
                    NeedsSync = true // Mark for syncing
                };

                await _dbContext.SaveItemAsync(newUser);
                await StoreUserInSecureStorage(newUser);

                _commonService.ToggleLoginStatus();
                OnAuthStateChanged?.Invoke();

                // Try to sync immediately if online
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    await SyncOfflineUsers();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Registration failed: {ex.Message}");
                await Toast.Make("Registration failed.").Show();
                return false;
            }
        }

        // Login (Offline First, Then Online)
        public async Task<bool> LoginAsync(LoginModel model)
        {
            try
            {
                // Check offline login first
                var hashedPassword = HashPassword(model.Password);
                var user = await _dbContext.GetUserByEmailAsync(model.Email);

                if (user != null && user.PasswordHash == hashedPassword)
                {
                    await StoreUserInSecureStorage(user);
                    _commonService.ToggleLoginStatus();
                    OnAuthStateChanged?.Invoke();
                    return true;
                }

                // If offline login fails, try online login
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseAddress}/api/Auth/Login", model);
                    if (!response.IsSuccessStatusCode) return false;

                    var onlineUser = await response.Content.ReadFromJsonAsync<User>();
                    if (onlineUser == null || string.IsNullOrEmpty(onlineUser.Token)) return false;

                    await _dbContext.SaveItemAsync(onlineUser); // Save user locally
                    await StoreUserInSecureStorage(onlineUser);

                    _commonService.ToggleLoginStatus();
                    OnAuthStateChanged?.Invoke();
                    return true;
                }

                await Toast.Make("User not found. Try logging in online first.").Show();
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Login failed: {ex.Message}");
                await Toast.Make("Login failed.").Show();
                return false;
            }
        }

        // Logout (Clear Secure Storage and Update Local DB)
        public async Task LogoutAsync()
        {
            try
            {
                // Clear secure storage
                SecureStorage.Remove("auth_token");
                SecureStorage.Remove("user_id");
                SecureStorage.Remove("user_name");
                SecureStorage.Remove("user_role");

                // Update local user record (remove token)
                var user = await GetStoredUserAsync();
                if (user != null)
                {
                    user.Token = null;
                    await _dbContext.SaveItemAsync(user);
                }

                OnAuthStateChanged?.Invoke();
                _commonService.ToggleLoginStatus();

                // Redirect to HomePage
                await Shell.Current.GoToAsync("//HomePage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Logout failed: {ex.Message}");
            }
        }

        // Sync Offline Users with API
        public async Task SyncOfflineUsers()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet) return;

            var offlineUsers = await _dbContext.GetAllItemsAsync<User>();
            foreach (var user in offlineUsers.Where(u => u.NeedsSync))
            {
                try
                {
                    // Ensure user has a password (generate if missing)
                    var password = string.IsNullOrEmpty(user.PasswordHash) ? GenerateTemporaryPassword() : user.PasswordHash;

                    var syncModel = new RegisterModel
                    {
                        Email = user.Email,
                        FullName = user.FullName,
                        Password = password
                    };

                    var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseAddress}/api/Auth/SyncUser", syncModel);
                    if (response.IsSuccessStatusCode)
                    {
                        var syncedUser = await response.Content.ReadFromJsonAsync<User>();
                        if (syncedUser != null)
                        {
                            syncedUser.NeedsSync = false; // Mark as synced
                            await _dbContext.SaveItemAsync(syncedUser);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error syncing user {user.Id}: {ex.Message}");
                }
            }
        }

        // Store User Data in Secure Storage
        private async Task StoreUserInSecureStorage(User user)
        {
            await SecureStorage.SetAsync("user_id", user.Id.ToString());
            await SecureStorage.SetAsync("user_name", user.FullName);
            await SecureStorage.SetAsync("user_role", user.Role ?? "User");
            await SecureStorage.SetAsync("auth_token", user.Token);
        }

        // Get Stored User from Local DB
        public async Task<User?> GetStoredUserAsync()
        {
            var userIdStr = await SecureStorage.GetAsync("user_id");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId)) return null;

            return await _dbContext.GetItemAsync<User>(userId);
        }

        // Check if User is Logged In
        public async Task<bool> IsUserLoggedInAsync()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            return !string.IsNullOrEmpty(token);
        }

        // Check if User is Admin
        public async Task<bool> IsUserAdminAsync()
        {
            var user = await GetStoredUserAsync();
            return user?.Role == "Admin";
        }
    }
}