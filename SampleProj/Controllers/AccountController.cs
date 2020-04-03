using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SampleProj.Entities.Dtos;
using SampleProj.Entities.Models;
using SampleProj.Helpers;
using SampleProj.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SampleProj.Controllers
{
    [Authorize(Roles = "Administrator")]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<User> _logger;
        private readonly AppSettings _appSettings;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AccountController(
            IOptions<AppSettings> appSettings,
            IUserService userService,
            IMapper mapper,
            ILogger<User> logger)
        {
            _appSettings = appSettings.Value;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]UserAuthenticateDto userDto)
        {
            var user = await _userService.AuthenticateAsync(userDto.Email, userDto.Password);

            if (user == null)
            {
                return BadRequest(new { message = "К сожалению, логин или пароль неверны" });
            }
            else if (user.Status == Entities.Enums.State.NOT_ACTIVE)
            {
                return BadRequest(new { message = "К сожалению, ваш аккаунт заблокирован. Пожалуйста, обратитесь в службу поддержки" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.Name)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                RoleId = user.RoleId,
                Token = tokenString
            });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]UserRegisterDto userDto)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(userDto.Email) && !string.IsNullOrEmpty(userDto.Password))
            {
                var user = _mapper.Map<User>(userDto);
                var newUser = await _userService.CreateAsync(user, userDto.Password);
                return Ok();
            }
            else
            {
                return BadRequest(new { message = "Enter Email and  Password or write all required data for user" });
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody]UserChangePasdwordDto user)
        {
            if (ModelState.IsValid && user.Id != 0 && user.NewPassword.Equals(user.NewPasswordConfirm))
            {
                bool isPasswordChanged = await _userService.ChangePassword(user.Id, user.NewPassword, user.OldPassword);
                if (isPasswordChanged)
                    return Ok();
                else
                    return BadRequest(new { message = "User do not find in DB or old password is incorrect" });
            }
            else
            {
                return BadRequest(new { message = "Model state is not valid" });
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("NewPassword")]
        public async Task<IActionResult> NewPassword([FromBody]UserNewPasdwordDto user)
        {
            if (ModelState.IsValid && user.Id != 0 && user.NewPassword.Equals(user.NewPasswordConfirm))
            {
                bool isPasswordChanged = await _userService.ChangePassword(user.Id, user.NewPassword);
                if (isPasswordChanged)
                    return Ok();
                else
                    return BadRequest(new { message = "User do not find in DB" });
            }
            else
            {
                return BadRequest(new { message = "Model state is not valid" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            var userView = _mapper.Map<UserViewDto>(user);
            return Ok(userView);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            var usersView = _mapper.Map<IEnumerable<UserViewDto>>(users);
            return Ok(usersView);
        }

        [HttpGet]
        [Route("GetAllForPageFilter")]
        public async Task<IActionResult> GetAllForPageFilter(int page = 1, int pageSize = 5)
        {
            var usersPerPage = await _userService.GetAllPerPageAsync(page, pageSize);
            var usersView = _mapper.Map<IEnumerable<UserViewDto>>(usersPerPage);
            return Ok(usersView);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto userDto)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(userDto);
                user.Id = id;
                await _userService.UpdateAsync(user);
                return Ok();
            }
            else
            {
                return BadRequest(new { message = "Enter all required data for updated user" });
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var status = await _userService.DeleteAsync(id);
            if (status)
                return Ok();
            else
                return BadRequest();
        }
    }
}