using Application.DTOs;
using Application.Interfaces.IRepo;
using Application.Services.UserServices;
using Application.Validator;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    //[Authorize]
    public class UserController : ApiControllerBase
    {
        private readonly UserService _userService;


        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            return ProcessResult(user);

        }
        [HttpGet]
        [Route("get-by-email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetUserByemailAsync(email);
            return ProcessResult(user);
        }
        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetUserList(string email)
        {
            var users = await _userService.GetAllUsersAsync();
            return ProcessResult(users);
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var users = await _userService.CreateUser(createUserDto);
            return ProcessResult(users);
        }

    }
}
