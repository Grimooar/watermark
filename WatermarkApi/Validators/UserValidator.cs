using FluentValidation;
using Kirel.Repositories.Interfaces;
using Watermark.Models.Dtos;
using WatermarkApi.Models;

namespace WatermarkApi.Validators;

public class UserValidator: AbstractValidator<UserCreateDto>
{
    private readonly IKirelGenericEntityFrameworkRepository<int, User> repository;
   
    public UserValidator(IKirelGenericEntityFrameworkRepository<int, User> repository)
    {
        this.repository = repository;
        RuleFor(user => user.Email).NotEmpty();
        RuleFor(user => user.Email).EmailAddress();
    }


   
}