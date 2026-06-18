using Application.DTOs;
using Application.Interfaces.IRepo;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validator
{
    public class CreateRequestValidator : AbstractValidator<CreateUserDto>
    {

        private readonly IUserRepository _userRepository;

        public CreateRequestValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username too short")
                .MustAsync(BeUniqueUsername).WithMessage("Dublicat user name");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(4).WithMessage("Password too short")
                .Must(BeStrongPassword);
        }

        private async Task<bool> BeUniqueUsername(string username, CancellationToken cancellationToken)
        {
            var details = await _userRepository.GetByUsernameAsync(username);
            return details == null;
        }
        private bool BeStrongPassword(string password)
        {
            return password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit);
        }
    }
}
