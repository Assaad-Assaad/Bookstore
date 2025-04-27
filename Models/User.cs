namespace Bookstore.Models
{
    public class User
    {


        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [JsonIgnore] 
        public string PasswordHash { get; set; }

        public bool NeedsSync { get; set; } = true;

        public string Token { get; set; }
        public string Role { get; set; } = "User";

        [JsonIgnore]
        public bool IsAdmin => (Role ?? "User") == "Admin";
    }
}
