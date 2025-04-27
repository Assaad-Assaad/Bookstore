namespace Bookstore.ViewModels
{
    [QueryProperty("Book", "Book")]
    public partial class BookDetailsViewModel : ObservableObject
    {

        private readonly CartService _cartService;
        public BookDetailsViewModel(CartService cartService) 
        {
            _cartService = cartService;
        }

        [ObservableProperty]
        private Book book;

        // Command to go back
        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync(".."); // Goes back to the previous page
        }

        // Command to go to cart
        [RelayCommand]
        private async Task GoToCart()
        {
            await Shell.Current.GoToAsync(nameof(CartPage)); // Navigates to Cart Page
        }

        [RelayCommand]
        private async Task AddToCart() 
        {
            var cartitem = new CartItem
            {
                BookId = Book.Id,
                Book = Book,
                Quantity = 1
            };

          await _cartService.AddToCart(cartitem);


            
        }

    }
}




