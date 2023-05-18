using System.ComponentModel.DataAnnotations;

namespace Watermark.Models.Dtos;
public class UserCreateDto
{
    public string Name { get; set; }
    public string LastName { get; set; }
    [Required(ErrorMessage = "��������� ����� � ����'�������")]
    [EmailAddress(ErrorMessage = "������� ��������� ����� ������������� �������")]
    public string Email { get; set; }
    [Phone]
    public string PhoneNumber { get; set; }
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "���� � ������� �� ���� ���� ������")]
    public string Password { get; set; }
    [Compare("Password", ErrorMessage = "����� �� ����������")]
    public string ConfirmPassword { get; set; }
}