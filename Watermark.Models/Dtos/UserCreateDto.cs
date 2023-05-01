using System.ComponentModel.DataAnnotations;

namespace Watermark.Models.Dtos;
public class UserCreateDto
{
    public string Name { get; set; }
    public string LastName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Phone]
    public string PhoneNumber { get; set; }
    [DataType(DataType.Password)]
    [Required]
    public string Password { get; set; }
    [Compare("Password", ErrorMessage = "Password != Confirm password")]
    public string ConfirmPassword { get; set; }
}