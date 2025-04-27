namespace Bookstore
{
    public static class Constants
    {
        public const string OnboardingShown = "onboarding-shown";



        public const string DatabaseFilename = "bookstore.db";

        public const SQLite.SQLiteOpenFlags Flags =
            
            SQLite.SQLiteOpenFlags.ReadWrite |
            
            SQLite.SQLiteOpenFlags.Create |
            
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
}
