using Bookstore.ViewModels;

namespace Bookstore.Views;

public partial class BookDetailsPage : ContentPage
{
	public BookDetailsPage(BookDetailsViewModel bookDetailsViewModel)
	{
		InitializeComponent();
		BindingContext = bookDetailsViewModel;
	}
}