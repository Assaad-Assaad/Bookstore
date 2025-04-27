namespace Bookstore.Models
{
    public class Order
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;


        public int UserId { get; set; }
        public User? User { get; set; }

        public string Status { get; set; } = "Pending";

        public List<CartItem> CartItems { get; set; } = new();
    }
}
