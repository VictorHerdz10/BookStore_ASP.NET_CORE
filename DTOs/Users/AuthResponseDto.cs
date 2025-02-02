
namespace BookStoreApi.DTOs.Users
{
    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public string Username { get; set; } = null!;
    }
}