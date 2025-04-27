using Bookstore.ViewModels;

namespace Bookstore.Views;

public partial class CartPage : ContentPage
{
	public CartPage(CartViewModel cartViewModel)
	{
        var userRole = SecureStorage.GetAsync("user_role").Result;
        if (string.IsNullOrEmpty(userRole)) 
        {
            Shell.Current.GoToAsync("//HomePage");
            return;
        }
        InitializeComponent();
		BindingContext = cartViewModel;
	}
}