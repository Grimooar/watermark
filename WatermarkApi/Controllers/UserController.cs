using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Watermark.Models.Dtos;
using WatermarkApi.Validators;
using WebApi.Service;

namespace WatermarkApi.Controllers;

[Route("api/[controller]")]
public class UserController : Controller
{

    private readonly UserService _userService;
    private readonly IValidator<UserCreateDto> _validator;
    private readonly UserValidator _userValidator;
    private readonly IMapper _mapper;

    public UserController(UserService userService, IValidator<UserCreateDto> validator, UserValidator userValidator,
        IMapper mapper)
    {
        _userService = userService;
        _validator = validator;
        _userValidator = userValidator;
        _mapper = mapper;

    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var user = await _userService.GetAll();


        return Ok(user);
    }

    [HttpGet("something")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAl()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _userService.GetById(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] UserCreateDto user)
    {
        var validationResult = await _validator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        _userService.Insert(user);
        return Ok();
    }
}




// [HttpPost("register")]
    // public async Task<IActionResult> Register([FromBody] UserCreateDto userDto)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         var existingUser = await _userService.GetUserByEmailAsync(userDto.Email);
    //         if (existingUser != null)
    //         {
    //             return BadRequest("A user with this email already exists");
    //         }
    //
    //         _userService.Insert(userDto);
    //
    //         return Ok();
    //     }
    //     return BadRequest(ModelState);
    // }

    // [HttpPost("login")]
    // public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    // {
    //     var user = await _userService.GetUserByEmailAsync(userLoginDto.Email);
    //
    //     if (user == null || user.Password != userLoginDto.Password)
    //     {
    //         return Unauthorized();
    //     }
    //
    //     var claims = new[]
    //     {
    //         new Claim(ClaimTypes.Name, user.UserName),
    //         new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    //     };
    //
    //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_secret_key_12345"));
    //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    //
    //     var token = new JwtSecurityToken(
    //         issuer: "my_issuer",
    //         audience: "my_audience",
    //         claims: claims,
    //         expires: DateTime.UtcNow.AddDays(1),
    //         signingCredentials: creds);
    //
    //     return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    // }


