using System.Text.Json;

namespace Bookstore.Services
{
    public class BookService
    {
        private readonly HttpClient _httpClient;
        private readonly BookstoreDatabase _dbContext;
        private bool IsOnline() => Connectivity.NetworkAccess == NetworkAccess.Internet;

        public BookService(HttpClient httpClient, BookstoreDatabase dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task<List<Book>> GetAllBooks()
        {
            try
            {
                // Fetch from local DB first
                var localBooks = await _dbContext.GetAllItemsAsync<Book>();
                Debug.WriteLine($"📦 Local books found: {localBooks.Count}");

                return localBooks;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ BookService Error: {ex.Message}");
                return new List<Book>();
            }
        }

        private async Task SyncBooksFromApi()
        {
            try
            {
                if (!IsOnline())
                {
                    Debug.WriteLine("❌ Offline - Skipping sync.");
                    return;
                }

                Debug.WriteLine("🌐 Online - Syncing books...");

                var response = await _httpClient.GetAsync($"{ApiConfig.BaseAddress}/api/Books");
                if (response.IsSuccessStatusCode)
                {
                    var bookResponse = await response.Content.ReadFromJsonAsync<Root>();
                    if (bookResponse?.Books != null && bookResponse.Books.Count > 0)
                    {
                        foreach (var apiBook in bookResponse.Books)
                        {
                            // Check if the book already exists locally
                            var localBook = await _dbContext.GetItemAsync<Book>(apiBook.Id);

                            if (localBook == null)
                            {
                                // Insert new book
                                await _dbContext.SaveItemAsync(apiBook);
                            }
                            else
                            {
                                // Update existing book
                                localBook.Title = apiBook.Title;
                                localBook.Description = apiBook.Description;
                                localBook.Author = apiBook.Author;
                                localBook.PhotoUrl = apiBook.PhotoUrl;
                                localBook.Price = apiBook.Price;

                                await _dbContext.SaveItemAsync(localBook);
                            }
                        }

                        // Notify UI to refresh
                        MessagingCenter.Send(this, "BooksUpdated");
                    }
                }
                else
                {
                    Debug.WriteLine($"❌ Failed to fetch books. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Sync failed: {ex.Message}");
            }
        }
    }
}

