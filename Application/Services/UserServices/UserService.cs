using Application.DTOs;
using Application.Interfaces.IRepo;
using Domain.comman;
using Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.UserServices
{
    public class UserService
    {
        private readonly IValidator<CreateUserDto> _validator;

        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository, IValidator<CreateUserDto> validator)
        {
            _userRepository = userRepository;
            _validator = validator;
        }

        public async Task<Result<User?>> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return Result<User?>.Failure(Error.NotFound("User", "User is not found"));
            return Result<User?>.Success(user);
        }

        public async Task<Result<User?>> GetUserByemailAsync(string email)
        {
            var user = await _userRepository.GetByUsernameAsync(email);
            if (user == null)
                return Result<User?>.Failure(Error.NotFound("User", "User is not found"));
            return Result<User?>.Success(user);
        }

        public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetListUser();
            return Result<IEnumerable<UserDto>>.Success(users);
        }

        public async Task<Result<UserDto>> CreateUser(CreateUserDto createUserDto)
        {
            var validation = await _validator.ValidateAsync(createUserDto);

            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    );

                return Result<UserDto>.Failure(ValidationError.Create(errors));
            }


            var user = new User
            {
                Username = createUserDto.Username,
                PasswordHash = createUserDto.Password
            };
            var userId = await _userRepository.CreateUser(user);

            UserDto userDto = new ()
            {
                Id = userId,
                Username = createUserDto.Username
            };

            return Result<UserDto>.Success(userDto);
        }
    }
}
