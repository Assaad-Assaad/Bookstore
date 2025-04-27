

namespace Bookstore.Services
{
    public class CommonService
    {
        public string? Token { get; private set; }

        public void SetToken(string? token) => Token = token;

        public EventHandler LoginStatusChanged;

        public void ToggleLoginStatus() => LoginStatusChanged?.Invoke(this, EventArgs.Empty);
    }
}
