using Bookstore.ViewModels;

namespace Bookstore.Views;

public partial class AllBooksPage : ContentPage
{
    

    public AllBooksPage(AllBooksViewModel allBooksViewModel)
    {
        InitializeComponent();
        
        BindingContext = allBooksViewModel;

        
    }
}
