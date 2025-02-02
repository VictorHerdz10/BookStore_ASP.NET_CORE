using System.ComponentModel.DataAnnotations;

 namespace BookStoreApi.DTOs.Users
{
public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
}
}