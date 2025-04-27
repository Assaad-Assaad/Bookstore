namespace Bookstore
{
    public static class ServiceExtensions
    {
        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {

            // Main & Info
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<OnBoardingPage>();
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<ProfileViewModel>();
            builder.Services.AddSingleton<ProfilePage>();


            // HTTP Client
            builder.Services.AddSingleton<HttpClient>();


            // Book
            builder.Services.AddSingleton<BookService>();
            builder.Services.AddSingleton<AllBooksViewModel>();
            builder.Services.AddTransient<BookDetailsViewModel>();
            builder.Services.AddSingleton<BookDetailsPage>();


            // Authorization 
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<AuthViewModel>();
            builder.Services.AddSingleton<AuthPage>();
            builder.Services.AddSingleton<CommonService>();


            // Order
            builder.Services.AddSingleton<OrderService>();
            builder.Services.AddSingleton<CartService>();
            builder.Services.AddSingleton<OrdersViewModel>();
            builder.Services.AddSingleton<OrdersPage>();
            builder.Services.AddSingleton<CartPage>();
            builder.Services.AddSingleton<CartViewModel>();


            // Database
            builder.Services.AddSingleton<BookstoreDatabase>();






            return builder;
        }
    }
}
