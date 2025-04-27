namespace Bookstore.Data
{
    public class BookstoreDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public BookstoreDatabase()
        {
            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            Task.Run(async () => await InitAsync());

        }


        public async Task InitAsync()
        {
            if (File.Exists(Constants.DatabasePath))
            {
                Console.WriteLine("✅ Database already exists. Skipping initialization.");
                return; 
            }

            Console.WriteLine("🆕 Database does not exist. Creating tables...");
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<Book>();
            await _database.CreateTableAsync<Order>();
            await _database.CreateTableAsync<CartItem>();

            Console.WriteLine("✅ SQLite Database Initialized Successfully");
        }



        // Generic CRUD Operations

        // Create or Update an Item
        public async Task SaveItemAsync<T>(T item) where T : new()
        {
            await _database.InsertOrReplaceAsync(item);
        }

        // Create or Update Multiple Items
        public async Task SaveItemsAsync<T>(List<T> items) where T : new()
        {
            Debug.WriteLine($"💾 Saving {items.Count} items to SQLite...");
            foreach (var item in items)
            {
                await _database.InsertOrReplaceAsync(item);
            }
            Debug.WriteLine("✅ Data saved successfully.");
        }


        // Read All Items of a Type
        public async Task<List<T>> GetAllItemsAsync<T>() where T : new()
        {
            return await _database.Table<T>().ToListAsync();
        }

        // Read Single Item by Primary Key
        public async Task<T> GetItemAsync<T>(int id) where T : new()
        {
            return await _database.FindAsync<T>(id);
        }

        // Delete an Item
        public async Task DeleteItemAsync<T>(T item) where T : new()
        {
            await _database.DeleteAsync(item);
        }

        // Delete an Item by ID
        public async Task DeleteItemByIdAsync<T>(int id) where T : new()
        {
            await _database.DeleteAsync<T>(id);
        }


        // Delete All Items of a Type
        public async Task DeleteAllItemsAsync<T>() where T : new()
        {
            await _database.DeleteAllAsync<T>();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _database.Table<User>()
                                  .Where(u => u.Email == email)
                                  .FirstOrDefaultAsync();
        }
    }
}
