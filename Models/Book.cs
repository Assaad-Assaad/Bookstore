namespace Bookstore.Models
{
    public class Book
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required, StringLength(100)]
        public string Author { get; set; }

        
        public string PhotoUrl { get; set; }

        [Required]
        public decimal Price { get; set; }
    }




    public class Root
    {
        public int TotalBooks { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<Book> Books { get; set; }
    }
}

