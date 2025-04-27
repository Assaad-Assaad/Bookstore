using Bookstore.ViewModels;

namespace Bookstore.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel homeViewModel)
    {
        InitializeComponent();
        BindingContext = homeViewModel;

    }

    


}