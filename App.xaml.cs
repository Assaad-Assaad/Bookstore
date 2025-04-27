namespace Bookstore
{
    public partial class App : Application
    {
        private readonly AuthService _authService;
        public static BookstoreDatabase Database { get; private set; }

        public App(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            Database = new BookstoreDatabase();

            Task.Run(async () => await Database.InitAsync());

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell(_authService));
        }

        
    }
}