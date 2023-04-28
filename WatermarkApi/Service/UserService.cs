using WatermarkApi.Models;
using AutoMapper;
using Kirel.Repositories.Interfaces;
using Kirel.Repositories.Sorts;
using Microsoft.AspNetCore.Identity;
using Watermark.Models.Dtos;

namespace WebApi.Service;

public class UserService
{
    private readonly IKirelGenericEntityFrameworkRepository<int, User> _repository;
    

    private readonly IMapper _mapper;

    public UserService(IKirelGenericEntityFrameworkRepository<int, User> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
      
    }

    public async Task<IEnumerable<UserDto>> GetAll()
    {
        // DTO <--> Entity
        var allCount = await _repository.Count();
        var User = await _repository.GetList(null, "Created", SortDirection.Asc, 1, allCount);
        var userDtos = _mapper.Map<List<UserDto>>(User);
        return userDtos;
    }

    public async Task<UserDto> GetById(int id)
    {
        var bookAbonement = await _repository.GetById(id);
        var bookAbonementDto = _mapper.Map<UserDto>(bookAbonement);
        return bookAbonementDto;
    }

    public void Insert(UserCreateDto userDto)
    {
        var user = new User
        {
            UserName = userDto.UserName,
            Name = userDto.Name,
            LastName = userDto.LastName,
            Email = userDto.Email,
            Password = userDto.Password
        };
        _repository.Insert(user);
    }


   
  
    public async Task<IdentityUser> GetUserByEmailAsync(string email)
    {
        var users = await _repository.GetList(u => u.Email == email);
        if (users == null || !users.Any())
        {
            return null;
        }

        var user = users.FirstOrDefault();
        return new IdentityUser
        {
            Id = user.Id.ToString(),
            UserName = user.UserName,

            Email = user.Email,
            PasswordHash = user.Password
        };
    }
}