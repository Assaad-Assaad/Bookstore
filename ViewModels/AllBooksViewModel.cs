namespace Bookstore.ViewModels
{
    public partial class AllBooksViewModel : ObservableObject
    {
        private readonly BookService _bookService;
        private readonly AuthService _authService;
        private List<Book> _allBooks = new(); // Cache the list of books

        public ObservableCollection<Book> Books { get; } = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        public AllBooksViewModel(AuthService authService, BookService bookService)
        {
            _authService = authService;
            _bookService = bookService;

            // ✅ Load books asynchronously without blocking UI
            _ = Task.Run(async () => await LoadBooks());

            // ✅ Listen for book updates
            MessagingCenter.Subscribe<BookService>(this, "BooksUpdated", async (sender) => await LoadBooks());
        }

        // ✅ Fetch books from API or local storage
        [RelayCommand]
        public async Task LoadBooks()
        {
            if (IsLoading) return;
            IsLoading = true;

            try
            {
                _allBooks = await _bookService.GetAllBooks(); // Cache the books

                MainThread.BeginInvokeOnMainThread(FilterBooks); // Ensure filtering happens on UI thread
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading books: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", $"Unable to load books: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ✅ Search Filtering Logic
        private void FilterBooks()
        {
            if (_allBooks.Count == 0) return;

            var filteredBooks = string.IsNullOrWhiteSpace(SearchText)
                ? _allBooks
                : _allBooks.Where(b =>
                    b.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    b.Author.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                  .ToList();

            Books.Clear();
            foreach (var book in filteredBooks)
            {
                Books.Add(book);
            }
        }

        // ✅ Trigger search when query changes
        partial void OnSearchTextChanged(string value)
        {
            MainThread.BeginInvokeOnMainThread(FilterBooks);
        }

        // ✅ Navigate to BookDetailsPage when a book is clicked
        [RelayCommand]
        async Task GoToDetails(Book book)
        {
            if (book == null) return;

            await Shell.Current.GoToAsync(nameof(BookDetailsPage), true, new Dictionary<string, object>
            {
                {"Book", book}
            });
        }

        // ✅ Unsubscribe from MessagingCenter when ViewModel is disposed
        public void Dispose()
        {
            MessagingCenter.Unsubscribe<BookService>(this, "BooksUpdated");
        }
    }
}
