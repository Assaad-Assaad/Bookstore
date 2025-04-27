namespace Bookstore
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .RegisterServices()
                .ConfigureMauiHandlers(handlers =>
                {
#if WINDOWS
                    ImageHandler.Mapper.AppendToMapping("WindowsFix", (handler, view) =>
                    {
                        if (view.Source is UriImageSource uriImageSource)
                        {
                            var bitmapImage = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(uriImageSource.Uri.AbsoluteUri));
                            handler.PlatformView.Source = bitmapImage;
                        }
                    });
#endif
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif


            return builder.Build();
        }
    }
}
