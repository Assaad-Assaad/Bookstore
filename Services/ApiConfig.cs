

namespace Bookstore.Services
{
    public static class ApiConfig
    {
        public static string BaseAddress { get; } =
            DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5021" : "http://localhost:5021";
    }
}
