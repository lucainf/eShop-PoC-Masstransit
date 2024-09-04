using System.ComponentModel.DataAnnotations;

namespace Login.Models.Request;

public class RegisterForm
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}