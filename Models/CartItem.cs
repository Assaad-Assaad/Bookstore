namespace Bookstore.Models
{
    public class CartItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }


        public int UserId { get; set; }
        public User? User { get; set; }


        public int BookId { get; set; }
        public Book? Book { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
