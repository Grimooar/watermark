using FluentValidation;
using Kirel.Repositories.Interfaces;
using Watermark.Models.Dtos;
using WatermarkApi.Models;

namespace WatermarkApi.Validators;

public class UserValidator: AbstractValidator<UserCreateDto>
{
    private readonly IKirelGenericEntityFrameworkRepository<int, User> _repository;
   
    public UserValidator(IKirelGenericEntityFrameworkRepository<int, User> repository)
    {
        _repository = repository;
       
        RuleFor(user => user.UserName).NotEmpty();
    }


   
}