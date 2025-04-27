namespace Bookstore.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly BookService _bookService;
        private readonly CommonService _commonService;

        [ObservableProperty]
        private string welcomeMessage;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _hasBooks;

        [ObservableProperty]
        private string _fullName = "Stranger";

        public ObservableCollection<Book> FavoriteBooks { get; } = new();
        public ObservableCollection<Book> BestSellers { get; } = new();

        public HomeViewModel(AuthService authService, BookService bookService, CommonService commonService)
        {
            _authService = authService;
            _bookService = bookService;
            _commonService = commonService;
            _commonService.LoginStatusChanged += OnLoginStatusChanged;
            //LoadWelcomeMessage();
            LoadUserData();
            LoadBooks();

            // Listen for book updates
            MessagingCenter.Subscribe<BookService>(this, "BooksUpdated", async (sender) => await LoadBooks());
            
        }

        private async void OnLoginStatusChanged(object? sender, EventArgs e) => LoadUserData();

        private async void LoadUserData()

        {

            var user = await _authService.GetStoredUserAsync();
            if (user != null)
            {
                FullName = user.FullName;
                
            }
            else
            {
                FullName = "Stranger";
                
            }
        }

        //private async void LoadWelcomeMessage()
        //{
        //    var user = await _authService.GetStoredUserAsync();
        //    WelcomeMessage = user != null ? $"Welcome, {user.FullName}!" : "Welcome, Stranger!";
        //}

        [RelayCommand]
        public async Task LoadBooks()
        {
            if (IsLoading) return;
            IsLoading = true;

            try
            {
                var books = await _bookService.GetAllBooks();
                Debug.WriteLine($"📚 Retrieved {books?.Count ?? 0} books");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    FavoriteBooks.Clear();
                    BestSellers.Clear();

                    if (books?.Count > 0)
                    {
                        foreach (var book in books.Take(3))
                        {
                            FavoriteBooks.Add(book);
                        }

                        foreach (var book in books.Skip(3).Take(3))
                        {
                            BestSellers.Add(book);
                        }
                    }

                    HasBooks = books?.Count > 0;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading books: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        async Task GoToDetails(Book book)
        {
            if (book == null)
                return;
            await Shell.Current.GoToAsync(nameof(BookDetailsPage), true, new Dictionary<string, object>
                    {
                        {"Book", book}
                    });

        }
    }
}
