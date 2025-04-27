using Bookstore.Auth;
using Bookstore.ViewModels;

namespace Bookstore.Views;

public partial class AuthPage : ContentPage
{

	//private readonly AuthViewModel _authViewModel;
	
	public AuthPage(AuthViewModel authViewModel)
	{
		InitializeComponent();
		//_authViewModel = authViewModel;
		BindingContext = authViewModel;
	}

    
}