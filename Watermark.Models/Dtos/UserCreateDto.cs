using System.ComponentModel.DataAnnotations;

namespace Watermark.Models.Dtos;
public class UserCreateDto
{
    public string Name { get; set; }
    public string LastName { get; set; }
    [Required(ErrorMessage = "Електрона пошта є обов'язковою")]
    [EmailAddress(ErrorMessage = "Введена електрона пошта неправильного формату")]
    public string Email { get; set; }
    [Phone]
    public string PhoneNumber { get; set; }
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Поле з паролем не може бути пустим")]
    public string Password { get; set; }
    [Compare("Password", ErrorMessage = "Паролі не співпадають")]
    public string ConfirmPassword { get; set; }
}