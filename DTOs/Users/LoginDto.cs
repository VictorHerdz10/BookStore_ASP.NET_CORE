using System.ComponentModel.DataAnnotations;

 namespace BookStoreApi.DTOs.Users
{

public class LoginDto
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}

}