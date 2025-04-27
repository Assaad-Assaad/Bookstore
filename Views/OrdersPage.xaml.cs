using Bookstore.ViewModels;

namespace Bookstore.Views;

public partial class OrdersPage : ContentPage
{
	public OrdersPage(OrdersViewModel ordersViewModel)
	{
		InitializeComponent();
		BindingContext = ordersViewModel;
	}
}